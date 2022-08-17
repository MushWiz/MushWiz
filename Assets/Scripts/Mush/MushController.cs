using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MushController : MonoBehaviour
{
    [HideInInspector] public GameController controller;
    public float lifePoints = 5f;
    public int maxLifePoints = 10;

    private float lastDamageTime = 0f;

    private MushAttack mushAttack;
    private MushMovementController mushMovementController;

    public bool isActive = true;
    public bool isDead = false;
    public bool isInvincible = false;

    public Image lifeBar;
    public Image expBar;

    public List<MushAbilities> abilities = new List<MushAbilities>();
    public List<MushAbilities> activableAbilities = new List<MushAbilities>();
    private List<MushAbilities> unlockedOrUnreachableAbilities = new List<MushAbilities>();

    public float experience = 0f;
    public float experienceToNextLevel = 10f;
    public float experienceToNextLevelMultiplier = 1.5f;
    public int level = 1;

    public List<MushStats> stats = new List<MushStats>(){
        new MushStats(10f, "Health"),
        new MushStats(5f, "Magic"),
        new MushStats(5f, "Speed"),
        new MushStats(0f, "Armor"),
        new MushStats(0f, "Dodge"),
        new MushStats(0f, "Block")
    };

    public MushWeaponHolder weaponHolder;

    private void Start()
    {
        mushAttack = GetComponent<MushAttack>();
        mushMovementController = GetComponent<MushMovementController>();

        weaponHolder = GetComponentInChildren<MushWeaponHolder>();

        //Reset abilities cooldowns
        foreach (MushAbilities ability in activableAbilities)
        {
            ability.cooldownTimer = 0f;
        }

        Heal(maxLifePoints * GetStatValueByName("Health"));
    }

    private void Awake()
    {
        GameStateManager.Instance.OnGameStateChanged += OnGameStateChanged;
    }

    private void OnDestroy()
    {
        GameStateManager.Instance.OnGameStateChanged -= OnGameStateChanged;
    }

    //On collision stay, if the object is an enemy, take damage once every 1.5 seconds
    private void OnCollisionStay2D(Collision2D other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            if (Time.time - lastDamageTime > 1.5f && !isInvincible)
            {
                float damageAmount = TakeDamage(other.gameObject.GetComponent<MonsterController>().damageDealer);
                lastDamageTime = Time.time;
                if (!isDead)
                {
                    StartCoroutine(TookDamage(damageAmount));
                }
            }
        }
    }

    //on collisioin enter, if the object is an enemy projectile, take damage
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "EnemyProjectile")
        {
            if (Time.time - lastDamageTime > 1.5f && !isInvincible)
            {
                float damageAmount = TakeDamage(other.gameObject.GetComponent<ProjectileController>().projectileDamage);
                lastDamageTime = Time.time;
                if (!isDead)
                {
                    StartCoroutine(TookDamage(damageAmount));
                }
            }

            Destroy(other.gameObject);
        }
    }

    public float GetStatValueByName(string statName)
    {
        foreach (MushStats stat in stats)
        {
            if (stat.name == statName)
            {
                return stat.GetValue();
            }
        }
        Debug.LogError("Stat not found: " + statName);
        return 0f;
    }

    public float GetStatValueIncreaseByName(string statName)
    {
        foreach (MushStats stat in stats)
        {
            if (stat.name == statName)
            {
                return stat.GetValueIncrease();
            }
        }
        Debug.LogError("Stat not found: " + statName);
        return 0f;
    }

    public float TakeDamage(float damage)
    {
        lifePoints -= damage;
        if (lifePoints <= 0)
        {
            isDead = true;
            isActive = false;
        }
        lifeBar.fillAmount = lifePoints / (maxLifePoints * GetStatValueByName("Health"));
        return damage;
    }

    public void Heal(float heal)
    {
        lifePoints = Mathf.Min((maxLifePoints * GetStatValueByName("Health")), lifePoints + heal);
        lifeBar.fillAmount = lifePoints / (maxLifePoints * GetStatValueByName("Health"));
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            IncreaseExperience(15f);
        }

        if (!isActive || isDead)
        {
            return;
        }
        mushAttack.AttackControl(this);
        mushMovementController.MovementControl(this);
    }

    IEnumerator TookDamage(float damageAmount)
    {
        isInvincible = true;
        StartCoroutine(Flash());
        yield return new WaitForSeconds(1.5f);
        isInvincible = false;
    }

    IEnumerator Flash()
    {
        while (isInvincible)
        {
            GetComponent<SpriteRenderer>().enabled = false;
            yield return new WaitForSeconds(0.1f);
            GetComponent<SpriteRenderer>().enabled = true;
            yield return new WaitForSeconds(0.2f);
        }
    }

    //Add experience to the player
    public void IncreaseExperience(float experience)
    {
        this.experience += experience;
        if (this.experience >= experienceToNextLevel)
        {
            this.experience -= experienceToNextLevel;
            level++;
            experienceToNextLevel = experienceToNextLevel * experienceToNextLevelMultiplier;
            LevelUp();
        }
        expBar.fillAmount = this.experience / experienceToNextLevel;
    }

    //Add an ability to the player
    public void AddAbility(MushAbilities ability)
    {
        if (abilities.Contains(ability))
        {
            return;
        }
        abilities.Add(ability);
        controller.uIHandler.actionBarManager.AddAbility(ability);
    }

    //Remove an ability from the player
    public void RemoveAbility(MushAbilities ability)
    {
        abilities.Remove(ability);
        controller.uIHandler.actionBarManager.RemoveAbility(ability);
    }

    public void RemoveAllAbilities()
    {
        foreach (MushAbilities ability in abilities)
        {
            controller.uIHandler.actionBarManager.RemoveAbility(ability);
        }
        abilities.Clear();
    }

    //Get the player's current level
    public int GetLevel()
    {
        return level;
    }

    //Get the player's current experience
    public float GetExperience()
    {
        return experience;
    }

    //Get the player's current experience to the next level
    public float GetExperienceToNextLevel()
    {
        return experienceToNextLevel;
    }

    //Check if the player has the given ability
    public bool HasAbility(MushAbilities ability)
    {
        return abilities.Contains(ability);
    }

    //Check if the player level can unlock the given ability
    public bool CanUnlockAbility(MushAbilities ability)
    {
        return level >= ability.unlockLevel;
    }

    public void CheckActivableAbilities()
    {
        foreach (MushAbilities ability in activableAbilities)
        {
            //Add the ability to the player's abilities if the player can unlock it
            if (CanUnlockAbility(ability))
            {
                AddAbility(ability);
            }
        }
    }

    public void LevelUp()
    {
        Heal(maxLifePoints * GetStatValueByName("Health"));
        GameStateManager.Instance.SendEvent(GameEvent.LevelUp);
    }

    private void OnGameStateChanged(GameState newState)
    {
        if (newState != GameState.Paused)
        {
            enabled = true;
            return;
        }

        enabled = false;

    }

    public void CreateLevelUpButtons()
    {
        UITypeController levelUpUI = controller.uIHandler.GetUITypeControllerByType(UIType.LevelUp);
        Transform uiLevelUp = levelUpUI.transform;

        //Get the child of the level up
        Transform levelUpChild = uiLevelUp.GetChild(0);
        //remove all the children of the level up child
        foreach (Transform child in levelUpChild)
        {
            Destroy(child.gameObject);
        }

        Transform statUpChild = uiLevelUp.GetChild(1);
        //remove all the children of the level up child
        foreach (Transform child in statUpChild)
        {
            Destroy(child.gameObject);
        }


        List<MushAbilities> abilitiesCopy = new List<MushAbilities>();

        foreach (MushAbilities ability in activableAbilities)
        {
            //Continue if the ability is already unlocked
            if (HasAbility(ability) || !CanUnlockAbility(ability))
            {
                continue;
            }
            abilitiesCopy.Add(ability);
        }

        List<MushAbilities> randomAbilities = new List<MushAbilities>();
        while (randomAbilities.Count < 4 && abilitiesCopy.Count > 0)
        {
            int randomIndex = Random.Range(0, abilitiesCopy.Count);
            if (!randomAbilities.Contains(abilitiesCopy[randomIndex]))
            {
                randomAbilities.Add(abilitiesCopy[randomIndex]);
                abilitiesCopy.RemoveAt(randomIndex);
            }
        }

        if (randomAbilities.Count > 0)
        {
            foreach (MushAbilities ability in randomAbilities)
            {
                GameObject button = Instantiate(ability.abilityButtonPrefab, uiLevelUp);
                button.transform.SetParent(uiLevelUp.GetChild(0));
                Button buttonComponent = button.GetComponent<Button>();
                buttonComponent.onClick.AddListener(() =>
                {
                    AddAbility(ability);
                    AddAbilityToUI(button);
                    controller.paused = false;
                });
            }
        }

        foreach (MushStats stat in stats)
        {
            GameObject buttonObject = Instantiate(stat.statButtonPrefab, uiLevelUp) as GameObject;
            buttonObject.transform.SetParent(uiLevelUp.GetChild(1));
            buttonObject.name = stat.name;

            GameObject buttonName = buttonObject.transform.GetChild(0).gameObject;
            if (buttonName)
            {
                buttonName.GetComponent<TextMeshProUGUI>().text = stat.GetName() + " up " + stat.GetValueIncrease();
            }
            GameObject buttonValue = buttonObject.transform.GetChild(1).gameObject;
            if (buttonValue)
            {
                buttonValue.GetComponent<TextMeshProUGUI>().text = stat.GetValue() + " points";
            }

            Button buttonComponent = buttonObject.GetComponent<Button>();
            buttonComponent.onClick.AddListener(() =>
            {
                stat.IncreaseValue(stat.GetValueIncrease());
                controller.paused = false;
            });
        }

    }

    public void OnLevelUpButtonPressed(string statName)
    {
        foreach (MushStats stat in stats)
        {
            if (stat.name == statName)
            {
                stat.IncreaseValue();
            }
        }
    }

    public void Reset()
    {
        isActive = false;
        isDead = false;
        expBar.fillAmount = 0f;
        experience = 0;
        level = 1;
        experienceToNextLevel = 10;
        foreach (MushStats stat in stats)
        {
            stat.ResetValueToInitial();
        }
        Heal(maxLifePoints * GetStatValueByName("Health"));

        RemoveAllAbilities();

        transform.position = new Vector3(0, 0, 0);
    }

    public void AddAbilityToUI(GameObject button)
    {
        button.GetComponent<Button>().interactable = false;
        button.transform.SetParent(controller.uIHandler.infoPanelManager.abilityContainer.transform);
        button.transform.localScale = new Vector3(1, 1, 1);
    }

    public MushAbilities GetAbilityByName(string name)
    {
        foreach (MushAbilities ability in abilities)
        {
            if (ability.name == name)
            {
                return ability;
            }
        }
        return null;
    }

    public MushAbilities GetAvailableAbilityByName(string name)
    {
        foreach (MushAbilities ability in activableAbilities)
        {
            if (ability.name == name)
            {
                return ability;
            }
        }
        return null;
    }

    public void LoadPlayer(PlayerData playerData)
    {
        Reset();

        foreach (string abilityName in playerData.abilities)
        {
            MushAbilities ability = GetAvailableAbilityByName(abilityName);
            if (ability != null)
            {
                AddAbility(ability);
            }
        }

        foreach (MushStats stat in stats)
        {
            stat.SetValue(playerData.stats[stats.IndexOf(stat)]);
        }
        experience = playerData.experience;
        level = playerData.level;
        experienceToNextLevel = playerData.experienceToNextLevel;
        isActive = true;
        isDead = false;
        controller.paused = false;

        transform.position = new Vector3(playerData.position[0], playerData.position[1], playerData.position[2]);
    }

}
