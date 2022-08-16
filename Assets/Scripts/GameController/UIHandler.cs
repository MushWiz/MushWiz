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

    public void EnableUIByType(UIType type)
    {
        foreach (UITypeController uITypeController in uiTypeControllers)
        {
            if (uITypeController.uiType == type)
            {
                uITypeController.gameObject.SetActive(true);
            }
        }
    }

    public void DisableUIByType(UIType type)
    {
        foreach (UITypeController uITypeController in uiTypeControllers)
        {
            if (uITypeController.uiType == type)
            {
                uITypeController.gameObject.SetActive(false);
            }
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
        infoPanelManager.infoSprite.sprite = mushController.GetComponent<SpriteRenderer>().sprite;
        for (int i = 0; i < infoPanelManager.stats.Count; i++)
        {
            foreach (MushStats mushStats in mushController.stats)
            {
                if (mushStats.name == infoPanelManager.stats[i].gameObject.name)
                {
                    infoPanelManager.stats[i].text = mushStats.name + ": " + mushStats.value.ToString();
                }
            }
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

}