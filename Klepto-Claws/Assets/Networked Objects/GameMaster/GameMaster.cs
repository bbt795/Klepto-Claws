using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NETWORK_ENGINE;

public class GameMaster : NetworkComponent
{
    public bool GameStarted = false;
    public bool GameEnd = false;

    public float MoneyStolen;
    public int humanCount;
    public int lobsterCount;

    public List<Vector3> SpawnPoints;
    public Vector3 CurrentSpawn;

    public List<Vector3> ItemPoints;
    public Vector3 CurrentItemSpawn;

    List<GameObject> spawnedItems = new List<GameObject>();

    public override void HandleMessage(string flag, string value)
    {
        if (flag == "GAMESTART")
        {
            //   Want to disable PlayerInfo
            GameStarted = true;
            foreach (NPM np in GameObject.FindObjectsOfType<NPM>())
            {
                np.transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
                //np.transform.GetChild(0).gameObject.SetActive(false);
            }
        }
        if (flag == "GAMEEND")
        {
            GameEnd = true;
            //StartCoroutine(StartGameEnd());
        }
    }
    public override void NetworkedStart()
    {
        MoneyStolen = 5.88f;
    }

    public IEnumerator StartGameEnd()
    {
        yield return new WaitForSeconds(5);
        foreach (NPM np in GameObject.FindObjectsOfType<NPM>())
        {
            //np.transform.GetChild(0).gameObject.SetActive(true);
            np.UI_Money(MoneyStolen);
            np.transform.GetChild(0).GetChild(1).gameObject.SetActive(true);
        }

    }
    public override IEnumerator SlowUpdate()
    {
        while (!GameStarted && IsServer)
        {
            bool readyGo = true;
            int count = 0;
            foreach (NPM np in GameObject.FindObjectsOfType<NPM>())
            {
                if (!np.IsReady)
                {
                    readyGo = false;
                    break;
                }
                count++;
            }
            if (count < 1)
            {
                readyGo = false;
            }
            if (NPM.humanCount != 1 && NPM.lobsterCount != 3)
            {
                readyGo = false;
            }
            GameStarted = readyGo;
            yield return new WaitForSeconds(2);
        }
        if (IsServer)
        {
            //while(all players have not hit ready)
            //Wait

            SendUpdate("GAMESTART", GameStarted.ToString());
            SendUpdate("GAMEEND", GameEnd.ToString());

            //Go to each NetworkPlayerManager and look at their options
            //Create the appropriate character for their options
            //GameObject temp = MyCore.NetCreateObject(1,Owner,new Vector3);
            //temp.GetComponent<MyCharacterScript>().team = //set the team;

            MyId.NotifyDirty();

            int spawnIndex = Random.Range(0, SpawnPoints.Count); //randomly select initial spawn index

            foreach (NPM np in GameObject.FindObjectsOfType<NPM>())
            {
                //get the current spawn point
                Vector3 currentSpawn = SpawnPoints[spawnIndex];

                if (np.IsHuman == true)
                {
                    MyCore.NetCreateObject(
                            1, np.Owner, currentSpawn, Quaternion.identity
                        );
                }

                if (np.IsLobster == true)
                {
                    MyCore.NetCreateObject(
                            2, np.Owner, currentSpawn, Quaternion.identity
                        );
                }

                //move to the next spawn point index, and wrap around if necessary
                spawnIndex = (spawnIndex + 1) % SpawnPoints.Count;
            }

            //shuffle the ItemPoints list to randomize the order
            for (int i = 0; i < ItemPoints.Count; i++)
            {
                Vector3 temp = ItemPoints[i];
                int randomIndex = Random.Range(i, ItemPoints.Count);
                ItemPoints[i] = ItemPoints[randomIndex];
                ItemPoints[randomIndex] = temp;
            }

            //loop through each point and spawn a treasure item
            for (int i = 0; i < ItemPoints.Count; i++)
            {
                Vector3 currentItemSpawn = ItemPoints[i];

                //randomize type
                int randomTreasure = Random.Range(3, 22);
                GameObject newItem = MyCore.NetCreateObject(randomTreasure, randomTreasure, currentItemSpawn, Quaternion.identity);

                spawnedItems.Add(newItem);
            }
        }
        while (IsServer)
        {
            if (IsDirty)
            {
                SendUpdate("GAMESTART", GameStarted.ToString());
                SendUpdate("GAMEEND", GameEnd.ToString());
                IsDirty = false;
            }

            //check for empty spawn points and respawn items after a small delay
            foreach (Vector3 currentItemSpawn in ItemPoints)
            {
                if (IsSpawnPointEmpty(currentItemSpawn))
                {
                    //wait for a short delay before respawning the item
                    yield return new WaitForSeconds(5f);

                    //randomize type
                    int randomTreasure = Random.Range(3, 22);
                    GameObject newItem = MyCore.NetCreateObject(randomTreasure, randomTreasure, currentItemSpawn, Quaternion.identity);

                    spawnedItems.Add(newItem);
                }
            }

            yield return new WaitForSeconds(1);
        }
        yield return new WaitForSeconds(.1f);
    }

    // Start is called before the first frame update
    void Start()
    {
        GameStarted = false;

        GameObject[] temp = GameObject.FindGameObjectsWithTag("SpawnPoint");
        SpawnPoints = new List<Vector3>();
        foreach (GameObject g in temp)
        {
            SpawnPoints.Add(g.transform.position);
        }

        GameObject[] temp2 = GameObject.FindGameObjectsWithTag("ItemPoint");
        ItemPoints = new List<Vector3>();
        foreach (GameObject g in temp2)
        {
            ItemPoints.Add(g.transform.position);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    bool IsSpawnPointEmpty(Vector3 spawnPoint)
    {
        foreach (GameObject item in spawnedItems)
        {
            if (Vector3.Distance(spawnPoint, item.transform.position) < 0.01f)
            {
                return false;
            }
        }
        return true;
    }

    public void RemoveItemFromList(GameObject item)
    {
        if (spawnedItems.Contains(item))
        {
            spawnedItems.Remove(item);
        }
    }

}
