using System.Collections;
using System.Collections.Generic;
using Serializing;
using UnityEngine;

public class LaserSource : Block, IActivatable {

    public List<ILaserDiverter> Diverters { get; private set; }
    public bool IsActive { get; private set; }

    private Laser laser;
    private bool canFire;

    [SerializeField] private bool isActiveOnStart = true;
    public bool IsActiveOnStart { get { return isActiveOnStart; } }

    protected override void Awake() {
        base.Awake();
        Diverters = new List<ILaserDiverter>();
        laser = transform.GetChild(0).GetComponent<Laser>();
        laser.Init(this);
        Deactivate();    
    }

    public void SetIsActiveOnStart(bool active) {
        isActiveOnStart = active;
    }

    private void Update() {
        if (IsActive && canFire)
            Fire();
    }

    private void Fire() {
        laser.Fire();
        for (int i = 0; i < Diverters.Count; i++) {
            Diverters[i].DivertLaser();
        }
    }

    public void AddDiverter(ILaserDiverter diverter) {
        Diverters.Add(diverter);
        diverter.OnDivertLaserStart(this);
    }

    public void RemoveDiverter(ILaserDiverter diverter) {
        for (int i = Diverters.Count - 1; i >= 0; i--) {
            ILaserDiverter last = Diverters[i];
            last.OnDivertLaserEnd();
            Diverters.RemoveAt(i);
            if (last == diverter) 
                return;
        }
    }

    private void ClearAllDiverters() {
        for (int i = Diverters.Count - 1; i >= 0; i--) {
            ILaserDiverter last = Diverters[i];
            last.OnDivertLaserEnd();
            Diverters.RemoveAt(i);
        }
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

    public void Toggle() {
        if (IsActive)
            Deactivate();
        else
            Activate();
    }

    public void Activate() {
        IsActive = true;
        laser.SetActive(true);
    }

    public void Deactivate() {
        IsActive = false;
        laser.SetActive(false);
        ClearAllDiverters();
    }

    #region Events
    protected override void OnIntroComplete() {
        canFire = isActiveOnStart;
        IsActive = isActiveOnStart;
        if (IsActive)
            Activate();
    }

    protected override void OnTileStandingOnMoveUpEnd() {
        base.OnTileStandingOnMoveUpEnd();
        canFire = true;
        Activate();
    }

    protected override void OnTileStandingOnMoveDownStart() {
        base.OnTileStandingOnMoveDownStart();
        canFire = false;
        Deactivate();
    }
    #endregion

    private void OnDrawGizmos() {
        GizmosExtension.DrawArrow(transform.position, transform.forward, Color.red);
    }
}