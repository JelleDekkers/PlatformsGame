using UnityEngine;
using System;

[CreateAssetMenu(fileName = "Player Settings", menuName = "Tools/Player Settings", order = 1)]
public class PlayerSettings : ScriptableObjectSingleton<PlayerSettings> {

    [SerializeField] private float introDuration = 1f;
    public static float IntroDuration { get { return Instance.introDuration; } }

    [SerializeField] private float introStartingHeight = 10f;
    public static float IntroStartingHeight { get { return Instance.introStartingHeight; } }

    [SerializeField] private float introDelay = 1f;
    public static float IntroDelay { get { return Instance.introDelay; } }
}
