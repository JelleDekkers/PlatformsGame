using UnityEngine;
using System;

[CreateAssetMenu(fileName = "Player Config", menuName = "Tools/Configs/Player Config", order = 1)]
public class PlayerConfig : ScriptableObjectSingleton<PlayerConfig> {

    [SerializeField] private float introAnimationStartingDelay = 1f;
    public static float IntroAnimationStartingDelay { get { return Instance.introAnimationStartingDelay; } }

    [SerializeField] private float introAnimationDuration = 1f;
    public static float IntroAnimationDuration { get { return Instance.introAnimationDuration; } }

    [SerializeField] private float introAnimationStartingHeight = 10f;
    public static float IntroAnimationStartingHeight { get { return Instance.introAnimationStartingHeight; } }
}
