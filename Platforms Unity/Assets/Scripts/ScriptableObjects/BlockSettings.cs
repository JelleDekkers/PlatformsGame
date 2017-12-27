using UnityEngine;
using System;

[CreateAssetMenu(fileName = "Block Settings", menuName = "Tools/Block Settings", order = 3)]
public class BlockSettings : ScriptableObjectSingleton<BlockSettings> {

    [SerializeField] private float moveDuration = 0.2f;
    public static float MoveDuration { get { return Instance.moveDuration; } }

    [SerializeField] private float fallDirectionalForce = 50f;
    public static float FallDirectionalForce { get { return Instance.fallDirectionalForce; } }

    [SerializeField] private float fallDownwardForce = 50f;
    public static float FallDownwardForce { get { return Instance.fallDownwardForce; } }

    [SerializeField] private float fallRotationalForce = 20f;
    public static float FallRotationalForce { get { return Instance.fallRotationalForce; } }

    [SerializeField] private float introDurationRandomMax = 1f;
    public static float IntroDurationMax { get { return Instance.introDurationRandomMax; } }

    [SerializeField] private float introStandardDuration = 1f;
    public static float IntroStandardDuration { get { return Instance.introStandardDuration; } }

    [SerializeField] private float introStartingHeight = 10f;
    public static float IntroStartingHeight { get { return Instance.introStartingHeight; } }

    [SerializeField] private float introDelay = 0.5f;
    public static float IntroDelay { get { return Instance.introDelay; } }

    [SerializeField] private float destroyDelayOnFall = 1f;
    public static float DestroyDelayOnFall { get { return Instance.destroyDelayOnFall; } }
}
