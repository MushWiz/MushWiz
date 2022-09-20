using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public static class SaveSystem
{
    public static void SaveGame(GameController controller)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/player.fun";
        FileStream stream = new FileStream(path, FileMode.Create);

        GameData data = new GameData(controller);

        formatter.Serialize(stream, data);
        stream.Close();
        Debug.Log("Game saved to " + path);
    }
    public static GameData LoadGame()
    {
        string path = Application.persistentDataPath + "/player.fun";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            GameData data = formatter.Deserialize(stream) as GameData;
            stream.Close();

            return data;
        }
        else
        {
            Debug.LogError("Save file not found in " + path);
            return null;
        }
    }
}

[System.Serializable]
public class GameData
{
    public PlayerData player;


    public GameData(GameController controller)
    {
        player = GatherPlayerData(controller);
    }

    public PlayerData GatherPlayerData(GameController controller)
    {
        return new PlayerData(controller.mushController);
    }
}

[System.Serializable]
public class PlayerData
{
    public List<int> stats = new List<int>();
    public List<string> abilities = new List<string>();
    public List<float> position = new List<float>();
    public List<int> items = new List<int>();
    public List<int> equipments = new List<int>();
    public bool isActive;
    public bool isDead;
    public bool isInvincible;
    public float experience;
    public float experienceToNextLevel;
    public float experienceToNextLevelMultiplier;
    public int level;

    public PlayerData(MushController player)
    {
        foreach (Stats stat in player.stats)
        {
            stats.Add(stat.value);
        }

        foreach (MushAbilities ability in player.abilities)
        {
            abilities.Add(ability.abilityName);
        }

        MushInventory inventory = player.controller.uIHandler.mushInventory;
        ItemDatabase database = inventory.database;

        foreach (MushInventorySlot equipment in inventory.inventorySlots)
        {
            if (equipment != null && equipment.itemEquipment.item != null)
            {
                items.Add(database.itemsIDs[equipment.itemEquipment.item]);
                Debug.Log(equipment.itemEquipment.item.itemName + " " + database.itemsIDs[equipment.itemEquipment.item]);
            }
        }

        MushInventorySlot currentWeaponSlot = inventory.currentWeaponSlot;
        if (currentWeaponSlot != null && currentWeaponSlot.itemEquipment.item != null)
        {
            equipments.Add(database.itemsIDs[currentWeaponSlot.itemEquipment.item]);
            Debug.Log(currentWeaponSlot.itemEquipment.item.itemName + " " + database.itemsIDs[currentWeaponSlot.itemEquipment.item]);
        }

        isActive = player.isActive;
        isDead = player.isDead;
        isInvincible = player.isInvincible;
        experience = player.experience;
        experienceToNextLevel = player.experienceToNextLevel;
        experienceToNextLevelMultiplier = player.experienceToNextLevelMultiplier;
        level = player.level;

        position.Add(player.transform.position.x);
        position.Add(player.transform.position.y);
        position.Add(player.transform.position.z);
    }
}
