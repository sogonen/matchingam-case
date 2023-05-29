using System.Collections.Generic;
using UnityEngine;

public class ColorManager : MonoBehaviour
{
    public StickmanMaterial[] stickmanMaterials;
    public static ColorManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public List<StickmanColor> GetStickmanColors(int stickmanSetNumbers)
    {
        var colors = new List<StickmanColor>();

        for (var i = 0; i < stickmanSetNumbers; i++)
        {
            var index = i % stickmanMaterials.Length;
            colors.Add(stickmanMaterials[index].color);
        }

        return colors;
    }

    public Material GetMaterialForColor(StickmanColor color)
    {
        foreach (var stickmanMaterial in stickmanMaterials)
            if (stickmanMaterial.color == color)
                return stickmanMaterial.material;
        return null;
    }
}