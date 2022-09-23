using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIStatSlot : MonoBehaviour
{
    public TextMeshProUGUI statName;
    public Stats mushStat;
    public Button increaseButton;

    public StatType GetStatType()
    {
        return mushStat.statType;
    }

    public bool CompareStatType(StatType statType)
    {
        return GetStatType() == statType;
    }

    public void UpdateStatSlot(MushController mushController)
    {
        statName.text = mushStat.GetStatType().ToString() + ": " + mushController.GetStatValueByType(mushStat.GetStatType()).ToString();


        if (mushController.availablePoints > 0)
        {
            increaseButton.gameObject.SetActive(true);
        }
        else
        {
            increaseButton.gameObject.SetActive(false);
        }

    }

    public void IncreaseStatValue(Stats stat, MushController mushController)
    {
        if (mushController.availablePoints > 0)
        {
            stat.IncreaseValue();
            mushController.availablePoints--;
        }
    }
}
