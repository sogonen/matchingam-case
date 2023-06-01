using UnityEngine;

public class UIScreen : MonoBehaviour
{
    public GameObject screenObject;
    public bool isActive;

    public UIScreen(GameObject screenObject)
    {
        this.screenObject = screenObject;
        isActive = false;
    }

    public void Show()
    {
        screenObject.SetActive(true);
        isActive = true;
    }

    public void Hide()
    {
        screenObject.SetActive(false);
        isActive = false;
    }
}