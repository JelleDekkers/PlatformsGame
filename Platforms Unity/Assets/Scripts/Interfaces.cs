public interface IActivatable {
    bool IsActive { get; }
    bool IsActiveOnStart { get; }
    void ToggleActiveState();
    void Activate();
    void Deactivate();
}

public interface IInputSystem {
    float GetAxisRawHorizontal();
    float GetAxisRawVertical();
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

namespace Serialization {
    public interface ISerializableGameObject {
        GUID Guid { get; set; }
        DataContainer Serialize();
        object Deserialize(DataContainer data);
    }
}

// TODO: remove
public interface ITestSerializable {
    GUID Guid { get; }
    TestDataContainer Serialize();
    object Deserialize(TestDataContainer data);
}