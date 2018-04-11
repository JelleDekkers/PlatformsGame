using UnityEngine;

public class LaserDiverter : Block, ILaserHittable, ILaserDiverter {

    public Laser Laser { get; private set; }

    private LaserSource laserSource;

    public void OnLaserHitStart(LaserSource source) {
        if (Laser == null) {
            Laser = transform.GetChild(0).GetComponent<Laser>();
            if (Laser == null)
                Laser = source.CreateNewLaser(transform);
        }

        Laser.Init(source);
        Laser.SetActive(true);
        laserSource = source;
        laserSource.onLethalStateChanged += Laser.ChangeLethalState;
    }

    public void OnLaserHitEnd() {
        if(Laser != null)
            Laser.SetActive(false);
        laserSource.onLethalStateChanged -= Laser.ChangeLethalState;
    }

    public void FireLaser() {
        Laser.Fire();
    }

    private void OnDrawGizmos() {
        GizmosExtension.DrawArrow(transform.position, transform.forward, Color.red);
    }
}
