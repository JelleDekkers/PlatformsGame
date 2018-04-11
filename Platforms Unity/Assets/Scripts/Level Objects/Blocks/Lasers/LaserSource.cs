using System;
using Serializing;
using UnityEngine;

public class LaserSource : Block, IActivatable {

    public bool IsActive { get; private set; }

    public Action<bool> onLethalStateChanged;

    [SerializeField] private bool isLethal;
    public bool IsLethal { get { return isLethal; } }

    [SerializeField] private bool isActiveOnStart = true;
    public bool IsActiveOnStart { get { return isActiveOnStart; } }

    private Laser laser;

    public override void Deserialize(BlockData data) {
        base.Deserialize(data);
        isActiveOnStart = (data as LaserSourceBlockData).isActiveOnStart;
        isLethal = (data as LaserSourceBlockData).isLethal;
    }

    protected override void Awake() {
        base.Awake();
        laser = transform.GetChild(0).GetComponent<Laser>();
        laser.Init(this);
        laser.ChangeLethalState(isLethal);

        if (!isActiveOnStart)
            Deactivate();
    }

    public void SetLethal(bool lethal) {
        SetLaserLethality(lethal);
    }

    public void ToggleLethal() {
        SetLaserLethality(!isLethal);
    }

    private void SetLaserLethality(bool lethal) {
        isLethal = lethal;
        laser.ChangeLethalState(lethal);

        if (onLethalStateChanged != null)
            onLethalStateChanged(isLethal);
    }

    public Laser CreateNewLaser(Transform diverter) {
        return CreateNewLaser(diverter, Quaternion.Euler(transform.eulerAngles.x, diverter.transform.eulerAngles.y, diverter.transform.eulerAngles.z));
    }

    public Laser CreateNewLaser(Transform diverter, Quaternion direction) {
        Laser laser = Instantiate(this.laser, diverter.transform.position, Quaternion.identity) as Laser;
        laser.transform.rotation = direction;
        laser.transform.SetParent(diverter);
        return laser;
    }

    public void ToggleActivateState() {
        if (IsActive)
            Deactivate();
        else
            Activate();
    }

    public void Activate() {
        SetActiveState(true);
    }

    public void Deactivate() {
        SetActiveState(false);
    }

    private void SetActiveState(bool state) {
        IsActive = state;
        laser.SetActive(state);
        enabled = state;
    }

    #region Events
    protected override void OnIntroComplete() {
        IsActive = isActiveOnStart;
        if (IsActive)
            Activate();

    }

    protected override void OnTileStandingOnMoveUpEnd() {
        base.OnTileStandingOnMoveUpEnd();
        Activate();
    }

    protected override void OnTileStandingOnMoveDownStart() {
        base.OnTileStandingOnMoveDownStart();
        Deactivate();
    }
    #endregion

    private void OnDrawGizmos() {
        GizmosExtension.DrawArrow(transform.position, transform.forward, Color.red);
    }
}