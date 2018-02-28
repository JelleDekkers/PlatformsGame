public interface IActivatable {
    bool IsActive { get; }
    bool IsActiveOnStart { get; }
    void ToggleActivateState();
    void Activate();
    void Deactivate();
    void SetIsActiveOnStart(bool active);
}

public interface IInputSystem {
    float GetAxisRawHorizontal();
    float GetAxisRawVertical();
}

public interface ISerializableEventTarget {
    string[] GetEventArgsForDeserialization();
}


public interface ILaserHittable {
    void OnLaserHitStart(LaserSource source);
    void OnLaserHitEnd();
}

public interface ILaserDiverter {
    Laser Laser { get; }
    void OnLaserHitStart(LaserSource source);
    void OnLaserHitEnd();
    void FireLaser();
}