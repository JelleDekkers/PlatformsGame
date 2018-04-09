﻿using System;
using Serializing;
using UnityEngine;

public class LaserSource : Block, IActivatable {

    public bool IsActive { get; private set; }

    public Action<Color> OnLaserColorChanged;

    [SerializeField] private bool isLethal;
    public bool IsLethal { get { return isLethal; } }

    [SerializeField] private bool isActiveOnStart = true;
    public bool IsActiveOnStart { get { return isActiveOnStart; } }

    public Color CurrentLaserColor { get; private set; }
    [SerializeField] private Color lethalColor, nonLethalColor;

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

        if(!isActiveOnStart)
            Deactivate();

        if (isLethal)
            CurrentLaserColor = lethalColor;
        else
            CurrentLaserColor = nonLethalColor;

        laser.ChangeColor(CurrentLaserColor);
    }

    public void SetLethal(bool lethal) {
        isLethal = lethal;
        if (isLethal)
            CurrentLaserColor = lethalColor;
        else
            CurrentLaserColor = nonLethalColor;
        laser.ChangeColor(CurrentLaserColor);
        if(OnLaserColorChanged != null)
            OnLaserColorChanged(CurrentLaserColor);
    }

    public void ToggleLethal() {
        isLethal = !isLethal;
        if (isLethal)
            CurrentLaserColor = lethalColor;
        else
            CurrentLaserColor = nonLethalColor;
        laser.ChangeColor(CurrentLaserColor);
        if(OnLaserColorChanged != null)
            OnLaserColorChanged(CurrentLaserColor);
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
        IsActive = true;
        laser.SetActive(true);
        enabled = true;
    }

    public void Deactivate() {
        IsActive = false;
        laser.SetActive(false);
        enabled = false;
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