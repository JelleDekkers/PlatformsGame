using System;
using UnityEngine;

public class LaserReciever : Block, ILaserHittable {

    public Action OnRecieve;

    public void OnLaserHitEnd() {
        Debug.Log("OnLaserHitEnd()");
    }

    public void OnLaserHitStart(LaserSource source) {
        Debug.Log("OnLaserHitStart() " + source.name);
    }
}
