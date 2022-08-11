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

    public List<TextMeshProUGUI> stats = new List<TextMeshProUGUI>();
    public Transform abilityContainer;
}
