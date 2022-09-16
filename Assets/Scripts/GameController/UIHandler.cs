using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIHandler : MonoBehaviour
{
    public List<UITypeController> uiTypeControllers = new List<UITypeController>();
    public List<GameObject> levelUpStatButtons = new List<GameObject>();
    public TextMeshProUGUI killCountText;
    public TextMeshProUGUI killCountHighscoreText;
    public TextMeshProUGUI enemyWaveText;
    public TextMeshProUGUI currentWaveInitialization;

    public InfoPanelManager infoPanelManager;
    public ActionBarManager actionBarManager;
    public MushInventory mushInventory;
    public GameObject loadingScreen;

    public void ToggleUIType(UIType uiType)
    {
        foreach (UITypeController uiTypeController in uiTypeControllers)
        {
            if (uiTypeController.uiType == uiType)
            {
                uiTypeController.ToggleUI();
            }
        }
    }

    public void EnableUIByType(UIType type)
    {
        foreach (UITypeController uITypeController in uiTypeControllers)
        {
            if (uITypeController.uiType == type)
            {
                uITypeController.EnableUI();
            }
        }
    }

    public void EnableUIByTypeList(List<UIType> types)
    {
        foreach (UIType type in types)
        {
            EnableUIByType(type);
        }
    }

    public void DisableUIByType(UIType type)
    {
        foreach (UITypeController uITypeController in uiTypeControllers)
        {
            if (uITypeController.uiType == type)
            {
                uITypeController.DisableUI();
            }
        }
    }

    public void DisableUIByTypeList(List<UIType> types)
    {
        foreach (UIType type in types)
        {
            DisableUIByType(type);
        }
    }

    public void DisableAllUI()
    {
        foreach (UITypeController uITypeController in uiTypeControllers)
        {
            uITypeController.DisableUI();
        }
    }

    public void UpdateUIByType(UIType type)
    {
        foreach (UITypeController uITypeController in uiTypeControllers)
        {
            if (uITypeController.uiType == type)
            {
                uITypeController.UpdateUI();
            }
        }
    }

    public void UpdateUIByTypeList(List<UIType> types)
    {
        foreach (UIType type in types)
        {
            UpdateUIByType(type);
        }
    }

    public UITypeController GetUITypeControllerByType(UIType type)
    {
        foreach (UITypeController uITypeController in uiTypeControllers)
        {
            if (uITypeController.uiType == type)
            {
                return uITypeController;
            }
        }
        return null;
    }

    public void SetTextToFullAlpha(TextMeshProUGUI text)
    {
        text.alpha = 1;
    }

    public void SetTextToZeroAlpha(TextMeshProUGUI text)
    {
        text.alpha = 0;
    }

    public IEnumerator FadeTextToFullAlpha(float timeFrame, TextMeshProUGUI text)
    {
        text.alpha = 0;
        while (text.alpha < 1)
        {
            text.alpha = Mathf.MoveTowards(text.alpha, 1, Time.deltaTime / timeFrame);
            yield return null;
        }
    }

    public IEnumerator FadeTextToZeroAlpha(float timeFrame, TextMeshProUGUI text)
    {
        text.alpha = 1;
        while (text.alpha > 0)
        {
            text.alpha = Mathf.MoveTowards(text.alpha, 0, Time.deltaTime / timeFrame);
            yield return null;
        }
    }

    public void UpdateInfoPanel(MushController mushController)
    {
        infoPanelManager.level.text = "Level: " + mushController.level.ToString();
        infoPanelManager.playerName.text = "Mush";
        infoPanelManager.availablePoints.text = "Available Points: " + mushController.availablePoints.ToString();
        foreach (UIStatSlot statSlot in infoPanelManager.stats)
        {
            statSlot.UpdateStatSlot(mushController);
        }
    }

    public void UpdateActionBar(MushController mushController)
    {
        actionBarManager.mushController = mushController;
    }

    public void UpdateInventory(MushController mushController)
    {
        mushInventory.mushController = mushController;
    }

    public void UpdateLoadingScreen(float progress)
    {
        Image loadingBar = loadingScreen.transform.GetChild(1).GetComponent<Image>();
        loadingBar.fillAmount = progress;
    }

}