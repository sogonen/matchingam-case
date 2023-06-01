using System;
using UnityEngine;

public enum StickmanColor
{
    Red = 0,
    Green = 1,
    Blue = 2,
    LightBlue = 3,
    Yellow = 4,
    Orange = 5,
    Black = 6,
    White = 7
}

[Serializable]
public class StickmanMaterial
{
    public StickmanColor color;
    public Material material;
}