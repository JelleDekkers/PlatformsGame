using System;
using UnityEngine;
using UnityEngine.Events;
using Serialization;

public class LaserReciever : Block, ILaserHittable {

    public UnityEvent onActivated;
    public UnityEvent onDisabled;

    public void OnLaserHitEnd() {
        if (onDisabled != null)
            onDisabled.Invoke();
    }

    public void OnLaserHitStart(LaserSource source) {
        if (onActivated != null)
            onActivated.Invoke();
    }

    #region Serialization
    public override DataContainer Serialize() {
        return new LaserRecieverData(this);
    }

    public override object Deserialize(DataContainer data) {
        BlockData baseData = base.Deserialize(data) as BlockData;
        LaserRecieverData parsedData = baseData as LaserRecieverData;
        parsedData.onActivated.Deserialize(ref onActivated);
        parsedData.onDisabled.Deserialize(ref onDisabled);
        return parsedData;
    }
    #endregion
}
