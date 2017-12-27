public interface IActivatable {
    bool IsActive { get; }
    bool IsActiveOnStart { get; }
    void Toggle();
    void Activate();
    void Deactivate();
    void SetIsActiveOnStart(bool active);
}

public interface ILaserDiverter {
    Laser Laser { get; }
    void OnDivertLaserStart(LaserSource src);
    void DivertLaser();
    void OnDivertLaserEnd();
}

public interface IInputSystem {
    float GetAxisRawHorizontal();
    float GetAxisRawVertical();
}