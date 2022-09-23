using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InfoPanelManager : MonoBehaviour
{
    public Image infoSprite;
    public TextMeshProUGUI playerName;
    public TextMeshProUGUI level;
    public TextMeshProUGUI availablePoints;

    public Transform statsHolder;
    public GameObject statIncreaseButton;
    public List<UIStatSlot> stats = new List<UIStatSlot>();
    public Transform abilityContainer;

    public bool StatExist(StatType statType)
    {
        foreach (UIStatSlot statUI in stats)
        {
            if (statUI.CompareStatType(statType))
            {
                return true;
            }
        }
        return false;
    }

    public UIStatSlot GetStatSlot(StatType statType)
    {
        UIStatSlot returnSlot = null;
        foreach (UIStatSlot checkedSlot in stats)
        {
            if (checkedSlot.CompareStatType(statType))
            {
                returnSlot = checkedSlot;
            }
        }
        return returnSlot;
    }

}
