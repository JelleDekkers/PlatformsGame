using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "General Settings", menuName = "Tools/Settings/General Settings", order = 0)]
public class GeneralSettings : ScriptableObjectSingleton<GeneralSettings> {

    [SerializeField] private bool useTransitions = true;
    public static bool UseTransitions { get { return Instance.useTransitions; } }

    [SerializeField] private float gameOverCounter = 1f;
    public static float GameOverCounter { get { return Instance.gameOverCounter; } }

    [SerializeField] private float introCounterOffset = 0.5f;
    public static float IntroCounterOffset { get { return Instance.introCounterOffset; } }
}
