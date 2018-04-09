using UnityEngine;

public class LaserDiverterMoveable : BlockMoveable, ILaserDiverter {
    public Laser Laser { get; private set; }

    private LaserSource laserSource;

    public void RotateRight() {
        transform.rotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y + 90, transform.eulerAngles.z);
    }
    public void RotateLeft() {
        transform.rotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y - 90, transform.eulerAngles.z);
    }

    public void FireLaser() {
        Laser.Fire();
    }

    public override void OnLaserHitStart(LaserSource source) {
        if (Laser == null) {
            Laser = transform.GetChild(0).GetComponent<Laser>();
            if (Laser == null)
                Laser = source.CreateNewLaser(transform);
        }

        Laser.Init(source);
        Laser.SetActive(true);
        laserSource = source;
        laserSource.OnLaserColorChanged += Laser.ChangeColor;
    }

    public override void OnLaserHitEnd() {
        if(Laser != null)
            Laser.SetActive(false);
    }

    private void OnDrawGizmos() {
        GizmosExtension.DrawArrow(transform.position, transform.forward, Color.red);
    }
}
