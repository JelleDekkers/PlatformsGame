using UnityEngine;

public class LaserDiverterMoveable : BlockMoveable, ILaserDiverter {
    public Laser Laser { get; private set; }

    public void RotateRight() {
        transform.rotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y + 90, transform.eulerAngles.z);
    }
    public void RotateLeft() {
        transform.rotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y - 90, transform.eulerAngles.z);
    }

    public void DivertLaser() {
        Laser.Fire();
    }

    public void OnDivertLaserStart(LaserSource src) {
        if (Laser == null) {
            Laser = transform.GetChild(0).GetComponent<Laser>();
            if (Laser == null)
                Laser = src.CreateNewLaser(transform);
        }

        Laser.Init(src);
        Laser.SetActive(true);
    }

    public void OnDivertLaserEnd() {
        Laser.SetActive(false);
    }

    private void OnDrawGizmos() {
        GizmosExtension.DrawArrow(transform.position, transform.forward, Color.red);
    }
}
