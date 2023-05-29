using System;
using UnityEngine;

public enum StickmanColor
{
    Red,
    Green,
    Blue,
    LightBlue,
    Yellow,
    Orange,
    Black,
    White,
    None
}

[Serializable]
public class StickmanMaterial
{
    public StickmanColor color;
    public Material material;
}