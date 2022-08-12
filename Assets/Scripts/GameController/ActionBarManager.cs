using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ActionBarManager : MonoBehaviour
{

    [HideInInspector] public int actionBarSize = 10;

    public List<ActionBarSlot> actionBarSlots = new List<ActionBarSlot>();
    public MushController mushController;

    public bool dragging = false;

    public GameObject slotDrag;

    private void Start()
    {
        foreach (ActionBarSlot slot in actionBarSlots)
        {
            slot.abilityButton = slot.actionBarSlot.transform.GetChild(0).GetComponent<Image>();
            slot.abilityCooldownOverlay.fillAmount = 0;
            slot.actionBarSlot.GetComponent<ActionBarSlotDrag>().actionBarManager = this;
        }
    }

    private void Update()
    {

        UpdateAbilityCooldowns();

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            UseAbility(0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            UseAbility(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            UseAbility(2);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            UseAbility(3);
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            UseAbility(4);
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            UseAbility(5);
        }
        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            UseAbility(6);
        }
        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            UseAbility(7);
        }
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            UseAbility(8);
        }
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            UseAbility(9);
        }
    }

    public void AddAbility(MushAbilities ability)
    {
        if (ability == null)
        {
            return;
        }
        foreach (ActionBarSlot slot in actionBarSlots)
        {
            if (slot.ability == null)
            {
                slot.ability = ability;
                slot.abilityButton.sprite = ability.abilityIcon;
                slot.abilityIcon = ability.abilityIcon;

                if (slot.actionBarSlot.TryGetComponent<Button>(out Button component))
                {
                    component.onClick.AddListener(() => UseAbility(slot));
                }
                else
                {
                    Button actionButton = slot.actionBarSlot.AddComponent<Button>();
                    actionButton.onClick.AddListener(() => UseAbility(slot));
                }

                slot.abilityCooldown = ability.cooldown;
                slot.abilityOnCooldown = false;
                slot.abilityCooldownTimer = 0;
                break;
            }
        }
    }

    //Add ability to specified slot
    public void AddAbility(MushAbilities ability, int slot)
    {
        if (ability == null)
        {
            return;
        }
        actionBarSlots[slot].ability = ability;
        actionBarSlots[slot].abilityButton.sprite = ability.abilityIcon;
        actionBarSlots[slot].abilityIcon = ability.abilityIcon;
        actionBarSlots[slot].abilityCooldown = ability.cooldown;
        actionBarSlots[slot].abilityOnCooldown = false;
        actionBarSlots[slot].abilityCooldownTimer = 0;
        if (actionBarSlots[slot].actionBarSlot.TryGetComponent<Button>(out Button component))
        {
            component.onClick.AddListener(() => UseAbility(slot));
        }
        else
        {
            Button actionButton = actionBarSlots[slot].actionBarSlot.AddComponent<Button>();
            actionButton.onClick.AddListener(() => UseAbility(slot));
        }

    }

    public void UseAbility(int abilityIndex)
    {
        if (actionBarSlots[abilityIndex].abilityOnCooldown == false && !dragging)
        {
            if (actionBarSlots[abilityIndex].ability == null)
            {
                return;
            }

            actionBarSlots[abilityIndex].UseAbility(mushController);
        }
    }

    public void UseAbility(ActionBarSlot abilitySlot)
    {
        if (abilitySlot.abilityOnCooldown == false && !dragging)
        {
            if (abilitySlot.ability == null)
            {
                return;
            }
            abilitySlot.UseAbility(mushController);
        }
    }

    public void UpdateAbilityCooldowns()
    {
        foreach (ActionBarSlot slot in actionBarSlots)
        {
            if (slot.abilityOnCooldown)
            {
                slot.abilityCooldownOverlay.fillAmount = slot.abilityCooldownTimer / slot.abilityCooldown;
                slot.abilityCooldownTimer -= Time.deltaTime;
                if (slot.abilityCooldownTimer <= 0)
                {
                    slot.abilityOnCooldown = false;
                    slot.abilityCooldownTimer = 0;
                    slot.abilityCooldownOverlay.fillAmount = 0;
                }
            }
        }
    }

    public void RemoveAbility(MushAbilities ability)
    {
        if (ability == null)
        {
            return;
        }
        foreach (ActionBarSlot slot in actionBarSlots)
        {
            if (slot.ability == ability)
            {
                slot.ability = null;
                slot.abilityButton.sprite = null;
                slot.abilityIcon = null;
                if (slot.actionBarSlot.GetComponent<Button>() != null)
                {
                    slot.actionBarSlot.GetComponent<Button>().onClick.RemoveAllListeners();
                }
                slot.abilityCooldown = 0;
                slot.abilityCooldownTimer = 0;
                slot.abilityOnCooldown = false;
                slot.abilityCooldownOverlay.fillAmount = 0;
            }
        }
    }

    public void RemoveAbility(int abilityIndex)
    {
        actionBarSlots[abilityIndex].ability = null;
        actionBarSlots[abilityIndex].abilityButton.sprite = null;
        actionBarSlots[abilityIndex].abilityIcon = null;
        if (actionBarSlots[abilityIndex].actionBarSlot.GetComponent<Button>() != null)
        {
            actionBarSlots[abilityIndex].actionBarSlot.GetComponent<Button>().onClick.RemoveAllListeners();
        }
        actionBarSlots[abilityIndex].abilityCooldown = 0;
        actionBarSlots[abilityIndex].abilityCooldownTimer = 0;
        actionBarSlots[abilityIndex].abilityOnCooldown = false;
        actionBarSlots[abilityIndex].abilityCooldownOverlay.fillAmount = 0;
    }

    public void RemoveAbility(ActionBarSlot abilitySlot)
    {
        abilitySlot.ability = null;
        abilitySlot.abilityButton.sprite = null;
        abilitySlot.abilityIcon = null;
        if (abilitySlot.actionBarSlot.GetComponent<Button>() != null)
        {
            abilitySlot.actionBarSlot.GetComponent<Button>().onClick.RemoveAllListeners();
        }
        abilitySlot.abilityCooldown = 0;
        abilitySlot.abilityCooldownTimer = 0;
        abilitySlot.abilityOnCooldown = false;
        abilitySlot.abilityCooldownOverlay.fillAmount = 0;
    }

    public void RemoveAllAbilities()
    {
        foreach (ActionBarSlot slot in actionBarSlots)
        {
            slot.ability = null;
            slot.abilityButton.sprite = null;
            slot.abilityIcon = null;
            if (slot.actionBarSlot.GetComponent<Button>() != null)
            {
                slot.actionBarSlot.GetComponent<Button>().onClick.RemoveAllListeners();
            }
            slot.abilityCooldown = 0;
            slot.abilityCooldownTimer = 0;
            slot.abilityOnCooldown = false;
            slot.abilityCooldownOverlay.fillAmount = 0;
        }
    }

    public void SwapAbilities(int abilityIndex1, int abilityIndex2)
    {
        MushAbilities tempAbility1 = actionBarSlots[abilityIndex1].ability;
        MushAbilities tempAbility2 = actionBarSlots[abilityIndex2].ability;

        float tempCooldown1 = actionBarSlots[abilityIndex1].abilityCooldownTimer;
        float tempCooldown2 = actionBarSlots[abilityIndex2].abilityCooldownTimer;

        RemoveAbility(abilityIndex1);
        RemoveAbility(abilityIndex2);

        AddAbility(tempAbility1, abilityIndex2);
        AddAbility(tempAbility2, abilityIndex1);

        actionBarSlots[abilityIndex1].abilityCooldownTimer = tempCooldown2;
        actionBarSlots[abilityIndex2].abilityCooldownTimer = tempCooldown1;
        actionBarSlots[abilityIndex1].abilityOnCooldown = true;
        actionBarSlots[abilityIndex2].abilityOnCooldown = true;

    }

    public void SwapAbilities(GameObject abilitySlot1, GameObject abilitySlot2)
    {
        int firstSlot = -1;
        int secondSlot = -1;
        foreach (ActionBarSlot slot in actionBarSlots)
        {
            if (slot.actionBarSlot == abilitySlot1)
            {
                firstSlot = actionBarSlots.IndexOf(slot);
            }
            if (slot.actionBarSlot == abilitySlot2)
            {
                secondSlot = actionBarSlots.IndexOf(slot);
            }
        }
        SwapAbilities(firstSlot, secondSlot);
    }

}
