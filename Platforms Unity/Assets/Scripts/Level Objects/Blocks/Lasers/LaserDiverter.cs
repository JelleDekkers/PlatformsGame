using UnityEngine;

public class LaserDiverter : Block, ILaserHittable, ILaserDiverter {

    public Laser Laser { get; private set; }

    [SerializeField] private Laser laserPrefab;

    private LaserSource laserSource;

    public void OnLaserHitStart(LaserSource source) {
        if (Laser == null) 
            Laser = Instantiate(laserPrefab, transform);

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

    protected override void OnTileStandingOnMoveDownStart() {
        base.OnTileStandingOnMoveDownStart();
        Laser.SetActive(false);
    }

    private void OnDrawGizmos() {
        GizmosExtension.DrawArrow(transform.position, transform.forward, Color.red);
    }
}
