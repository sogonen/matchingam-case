using UnityEngine;

public class Stickman : MonoBehaviour
{
    public SkinnedMeshRenderer _renderer; // Cache reference to renderer

    private StickmanColor stickmanColor; // Private backing field

    public StickmanColor Color
    {
        get => stickmanColor;
        set
        {
            stickmanColor = value;
            var material = ColorManager.Instance.GetMaterialForColor(value);
            if (material != null) _renderer.material = material;
        }
    }

    private void Start()
    {
        _renderer = GetComponent<SkinnedMeshRenderer>();
    }
}