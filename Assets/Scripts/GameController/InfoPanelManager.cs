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

    public List<UIStatSlot> stats = new List<UIStatSlot>();
    public Transform abilityContainer;
}
