using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIScreenManager : MonoBehaviour
{
    public TextMeshProUGUI levelText;
    private UIScreen currentScreen;
    private readonly Queue<UIScreen> screenQueue = new();

    public void EnqueueScreen(UIScreen screen)
    {
        screenQueue.Enqueue(screen);

        if (currentScreen == null) ShowNextScreen();
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

    public bool IsScreenQueueEmpty()
    {
        return screenQueue.Count == 0;
    }
}