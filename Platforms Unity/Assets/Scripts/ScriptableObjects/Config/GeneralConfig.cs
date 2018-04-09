using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "General Config", menuName = "Tools/Configs/General Config", order = 0)]
public class GeneralConfig : ScriptableObjectSingleton<GeneralConfig> {

    [SerializeField] private bool useTransitionAnimations = true;
    public static bool UseTransitionAnimations { get { return Instance.useTransitionAnimations; } }

    [SerializeField] private float gameOverCounterDuration = 1f;
    public static float GameOverCounterDuration { get { return Instance.gameOverCounterDuration; } }

    [SerializeField] private float introCounterExtraDurationOffset = 0.5f;
    public static float IntroCounterExtraDurationOffset { get { return Instance.introCounterExtraDurationOffset; } }
}
