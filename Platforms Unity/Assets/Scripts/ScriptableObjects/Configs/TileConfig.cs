using UnityEngine;
using System;

[CreateAssetMenu(fileName = "Tile Config", menuName = "Tools/Configs/Tile Config", order = 2)]
public class TileConfig : ScriptableObjectSingleton<TileConfig> {

    [SerializeField] private float disabledPositionHeight = -10;
    public static float DisabledPositionHeight { get { return Instance.disabledPositionHeight; } }

    [SerializeField] private float tempColliderEnabledDuration = 0.1f;
    public static float TempColliderEnabledDuration { get { return Instance.tempColliderEnabledDuration; } }

    // Move Up:
    [SerializeField] private float moveUpStandardAnimationDuration = 0.3f;
    public static float MoveUpStandardAnimationDuration { get { return Instance.moveUpStandardAnimationDuration; } }

    [SerializeField] private FloatMinMax moveUpIntroAnimationDuration = new FloatMinMax(0.3f, 0.5f);
    public static FloatMinMax MoveUpAnimationDuration { get { return Instance.moveUpIntroAnimationDuration; } }

    [SerializeField] private FloatMinMax moveUpIntroAnimationDelay = new FloatMinMax(0f, 0.2f);
    public static FloatMinMax MoveUpAnimationDelay { get { return Instance.moveUpIntroAnimationDelay; } }

    // Move Down:
    [SerializeField] private float moveDownStandardAnimationDuration = 0.75f;
    public static float MoveDownAnimationDuration { get { return Instance.moveDownStandardAnimationDuration; } }

    [SerializeField] private FloatMinMax moveDownIntroAnimationDuration = new FloatMinMax(0.75f, 1f);
    public static FloatMinMax MoveDownIntroAnimationDuration { get { return Instance.moveDownIntroAnimationDuration; } }

    [SerializeField] private FloatMinMax moveDownAnimationDelay = new FloatMinMax(0f, 0.2f);
    public static FloatMinMax MoveDownAnimationDelay { get { return Instance.moveDownAnimationDelay; } }
}