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

    [SerializeField] private Laser laserPrefab;

    private Laser laser;

    public override void Deserialize(BlockData data) {
        base.Deserialize(data);
        isActiveOnStart = (data as LaserSourceBlockData).isActiveOnStart;
        isLethal = (data as LaserSourceBlockData).isLethal;
    }

    protected override void Awake() {
        base.Awake();
        laser = Instantiate(laserPrefab, transform);
        laser.Init(this);
    }

    private void SetLaserLethality(bool lethal) {
        isLethal = lethal;
        laser.ChangeLethalState(lethal);

        if (onLethalStateChanged != null)
            onLethalStateChanged(isLethal);
    }

    public Laser CreateNewLaser(Transform diverter, Quaternion direction) {
        Laser laser = Instantiate(laserPrefab, diverter.transform.position, Quaternion.identity) as Laser;
        laser.transform.rotation = direction;
        laser.transform.SetParent(diverter);
        return laser;
    }

    private void SetActiveState(bool state) {
        IsActive = state;
        laser.SetActive(state);
        enabled = state;
    }
    
    #region Events
    protected override void OnIntroComplete() {
        base.OnIntroComplete();
        IsActive = isActiveOnStart;
        if (IsActive)
            Activate();
    }

    protected override void OnTileStandingOnMoveUpEnd() {
        base.OnTileStandingOnMoveUpEnd();
        if (IsActive)
            Activate();
    }

    protected override void OnTileStandingOnMoveDownStart() {
        base.OnTileStandingOnMoveDownStart();
        Deactivate();
    }
    #endregion

    #region Unity Actions
    public void SetLethal(bool lethal) {
        SetLaserLethality(lethal);
    }

    public void ToggleLethal() {
        SetLaserLethality(!isLethal);
    }

    public void ToggleActiveState() {
        SetActiveState(!IsActive);
    }

    public void Activate() {
        SetActiveState(true);
    }

    public void Deactivate() {
        SetActiveState(false);
    }
    #endregion

    private void OnDrawGizmos() {
        GizmosExtension.DrawArrow(transform.position, transform.forward, Color.red);
    }
}