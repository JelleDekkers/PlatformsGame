﻿using UnityEngine;

public class LaserDiverter : Block, ILaserHittable, ILaserDiverter {

    public Laser Laser { get; private set; }

    public void RotateRight() {
        transform.rotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y + 90, transform.eulerAngles.z);
    }
    public void RotateLeft() {
        transform.rotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y - 90, transform.eulerAngles.z);
    }

    public void OnLaserHitStart(LaserSource source) {
        if (Laser == null) {
            Laser = transform.GetChild(0).GetComponent<Laser>();
            if (Laser == null)
                Laser = source.CreateNewLaser(transform);
        }

        Laser.Init(source);
        Laser.SetActive(true);
    }

    public void OnLaserHitEnd() {
        if(Laser != null)
            Laser.SetActive(false);
    }

    public void FireLaser() {
        Laser.Fire();
    }

    private void OnDrawGizmos() {
        GizmosExtension.DrawArrow(transform.position, transform.forward, Color.red);
    }
}
