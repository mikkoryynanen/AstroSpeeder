using UnityEngine;

public enum drops { _, Gold, CommonDrop, UncommonDrop, RareDrop }
public enum type { _, Gold, Material, Ship, Particle}

//Custom serializable class
[System.Serializable]
public class Container
{
    public string name;
    public drops drop;
    [Range(0f,100f)]
    public float dropRate = 1f;
}

[System.Serializable]
public class Loot
{
    public string name;
    public type type;
    public GameObject go;//
    public Material mat;//
}

public class DropRateManager : MonoBehaviour
{
    [Header("Loot tables for each kind of runs (Desc/type/drop rate)")]
    public Container[] lootDroppedFromLegendaryRun;
    public Container[] lootDroppedFromExcellentRun;
    public Container[] lootDroppedFromAverageRun;
    public Container[] lootDroppedFromPoorRun;

    [Header("What is dropped (desc/type/go/mat)")]
    public Loot[] rareDrops;
    public Loot[] uncommonDrops;
    public Loot[] commonDrops;

    void Start ()
    {
        //LootFromLegendaryRun(); // TEMP for testing purposes
    }
	
    void LootFromLegendaryRun()// Call this if player does legendary run
    {
        foreach (Container item in lootDroppedFromLegendaryRun)
        {
            float randomNmbr = Random.Range(1,100);

            if(randomNmbr <= item.dropRate)
            {
                SortLoot(item);
            }
        }
    }

    void LootFromExcellentRun()// Call this if player does excellent run
    {
        foreach (Container item in lootDroppedFromExcellentRun)
        {
            float randomNmbr = Random.Range(1,100);

            if (randomNmbr <= item.dropRate)
            {
                SortLoot(item);
            }
        }
    }

    void LootFromAverageRun()// Call this if player does average run
    {
        foreach (Container item in lootDroppedFromAverageRun)
        {
            float randomNmbr = Random.Range(1,100);

            if (randomNmbr <= item.dropRate)
            {
                SortLoot(item);
            }
        }
    }

    void LootFromPoorRun()// Call this if player does poor run
    {
        foreach (Container item in lootDroppedFromPoorRun)
        {
            float randomNmbr = Random.Range(1,100);

            if (randomNmbr <= item.dropRate)
            {
                SortLoot(item);
            }
        }
    }

    void SortLoot(Container item)
    {
        if (item.drop == drops.Gold)
        {
            int goldGained = 10;// SET GOLD TO BE ADJUSTED WITH THE TRAVEL DONE
            SendMessage("SetCash", goldGained);
        }

        if (item.drop == drops.CommonDrop)
        {
            int randomNmbr = Random.Range(0, commonDrops.Length);
            Loot receivedItem = commonDrops[randomNmbr];
            //Debug.Log("Received common drop of " + receivedItem.name);
            HandOutTheLoot(receivedItem);
        }

        if (item.drop == drops.UncommonDrop)
        {
            int randomNmbr = Random.Range(0, uncommonDrops.Length);
            Loot receivedItem = uncommonDrops[randomNmbr];
            //Debug.Log("Received uncommon drop of " + receivedItem.name);
            HandOutTheLoot(receivedItem);
        }

        if (item.drop == drops.RareDrop)
        {
            int randomNmbr = Random.Range(0, rareDrops.Length);
            Loot receivedItem = rareDrops[randomNmbr];
           // Debug.Log("Received rare drop of " + receivedItem.name);
            HandOutTheLoot(receivedItem);
        }
    }

    void HandOutTheLoot (Loot item)
//Sends message in this GO so it'll add the items to the temp player inventory script
    {
        if (item.type == type.Material)
        {
            SendMessage("AddMaterial", item.mat);
        }

        if (item.type == type.Particle)
        {
            SendMessage("AddParticle", item.go);
        }

        if (item.type == type.Ship)
        {
            SendMessage("AddShip", item.go);
        }
    }
}
