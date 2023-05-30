using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIScreenManager : MonoBehaviour
{
    public TextMeshProUGUI levelText;
    private Queue<UIScreen> screenQueue = new Queue<UIScreen>();
    private UIScreen currentScreen;

    public void EnqueueScreen(UIScreen screen)
    {
        screenQueue.Enqueue(screen);

        if (currentScreen == null)
        {
            ShowNextScreen();
        }
    }

    public void DequeueScreen()
    {
        screenQueue.Dequeue();
        
        if (currentScreen != null)
        {
            HideCurrentScreen();
            ShowNextScreen();
        }
    }

    private void ShowNextScreen()
    {
        if (screenQueue.Count > 0)
        {
            currentScreen = screenQueue.Peek();
            currentScreen.Show();
        }
    }

    private void HideCurrentScreen()
    {
        if (currentScreen != null)
        {
            currentScreen.Hide();
            currentScreen = null;
        }
    }
    
    public void SetLevelText(int level)
    {
        levelText.text = "Level " + level;
    }
}