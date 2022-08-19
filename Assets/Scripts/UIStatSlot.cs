using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIStatSlot : MonoBehaviour
{
    public TextMeshProUGUI statName;
    public MushStats mushStat;
    public Button increaseValueButton;

    public void UpdateStatSlot(MushController mushController)
    {
        statName.text = mushStat.GetStatType().ToString() + ": " + mushController.GetStatValueByType(mushStat.GetStatType()).ToString();
    }
}
