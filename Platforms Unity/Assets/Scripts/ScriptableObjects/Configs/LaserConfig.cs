using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Laser Config", menuName = "Tools/Configs/Laser Config", order = 4)]
public class LaserConfig : ScriptableObjectSingleton<LaserConfig> {

    [SerializeField] private Color normalLaserColor = Color.blue;
    public static Color NormalLaserColor { get { return Instance.normalLaserColor; } }

    [SerializeField] private Color lethalLaserColor = Color.red;
    public static Color LethalLaserColor { get { return Instance.lethalLaserColor; } }

    [SerializeField] private float materialOffsetSpeed = 0.4f;
    public static float MaterialOffsetSpeed { get { return Instance.materialOffsetSpeed; } }
}
