using UnityEngine;
using System;

[CreateAssetMenu(fileName = "Block Config", menuName = "Tools/Configs/Block Config", order = 3)]
public class BlockConfig : ScriptableObjectSingleton<BlockConfig> {

    [SerializeField] private float moveDuration = 0.2f;
    public static float MoveDuration { get { return Instance.moveDuration; } }

    [SerializeField] private float fallDirectionalForce = 50f;
    public static float FallDirectionalForce { get { return Instance.fallDirectionalForce; } }

    [SerializeField] private float fallDownwardForce = 50f;
    public static float FallDownwardForce { get { return Instance.fallDownwardForce; } }

    [SerializeField] private float fallRotationalForce = 20f;
    public static float FallRotationalForce { get { return Instance.fallRotationalForce; } }

    [SerializeField] private FloatMinMax introAnimationDuration = new FloatMinMax(1f, 2f);
    public static FloatMinMax IntroAnimationDuration { get { return Instance.introAnimationDuration; } }

    [SerializeField] private float introAnimationStartingHeight = 10f;
    public static float IntroAnimationStartingHeight { get { return Instance.introAnimationStartingHeight; } }

    [SerializeField] private FloatMinMax introAnimationDelay = new FloatMinMax(0.5f, 0.6f);
    public static FloatMinMax IntroAnimationDelay { get { return Instance.introAnimationDelay; } }

    [SerializeField] private float destroyDelayOnFall = 1f;
    public static float DestroyDelayOnFall { get { return Instance.destroyDelayOnFall; } }

    [SerializeField] private float fallCutOff = 0.5f;
    public static float FallCutOff { get { return Instance.fallCutOff; } }

    [SerializeField] private float fallForce = 100;
    public static float FallForce { get { return Instance.fallForce; } }
}
