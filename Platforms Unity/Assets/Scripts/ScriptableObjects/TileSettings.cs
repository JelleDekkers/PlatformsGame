using UnityEngine;
using System;

[CreateAssetMenu(fileName = "Tile Settings", menuName = "Tools/Tile Settings", order = 2)]
public class TileSettings : ScriptableObjectSingleton<TileSettings> {

    [SerializeField] private bool useTransitions = true;
    public static bool UseStartingTransitions { get { return Instance.useTransitions; } }

    [SerializeField] private float moveUpStandardDuration = 0.3f;
    public static float MoveUpStandardDuration { get { return Instance.moveUpStandardDuration; } }

    [SerializeField] private float moveUpDurationRandomMax = 0.3f;
    public static float MoveUpDurationMax { get { return Instance.moveUpDurationRandomMax; } }

    [SerializeField] private float moveUpDelayRandomMax = 0.3f;
    public static  float MoveUpDelayRandomMax { get { return Instance.moveUpDelayRandomMax; } }

    [SerializeField] private float moveDownStandardDuration = 0.75f;
    public static float MoveDownStandardDuration { get { return Instance.moveDownStandardDuration; } }

    [SerializeField] private float transitionOnGameOverDelay = 0.5f;
    public static float TransitionOnGameOverDelay { get { return Instance.transitionOnGameOverDelay; } }

    [SerializeField] private float transitionOnGameOverDurationRandomMax = 0.4f;
    public static float TransitionOnGameOverDurationRandomMax { get { return Instance.transitionOnGameOverDurationRandomMax; } }
}