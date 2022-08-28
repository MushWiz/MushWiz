using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UITypeController : MonoBehaviour
{
    public UIType uiType;

    public void EnableUI()
    {
        gameObject.SetActive(true);
    }

    public void DisableUI()
    {
        gameObject.SetActive(false);
    }

    public void UpdateUI()
    {
        if (gameObject.activeSelf)
        {
            return;
        }
        EnableUI();
    }

    public void ToggleUI()
    {
        if (gameObject.activeSelf)
        {
            DisableUI();
        }
        else
        {
            EnableUI();
        }
    }
}

public enum UIType
{
    NULL,
    MainMenu,
    InGame,
    LevelUp,
    CharacterInfo,
    LoadingScreen,
}