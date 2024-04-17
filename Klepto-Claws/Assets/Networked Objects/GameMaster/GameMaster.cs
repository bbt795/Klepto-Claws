using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NETWORK_ENGINE;
using TMPro;
//using System;

public class GameMaster : NetworkComponent
{
    public bool GameStarted = false;
    public bool GameRunning = false;
    public bool GameEnding = false;

    private float elapsedTime = 0f;
    private float timeout = 300f; //300f is 5 minutes
    public float TimeRemaining = 300f;
    public int minutes;
    public int seconds;

    public int StartingMoney = 10000;
    public int MoneyStolen;
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
            GameRunning = true;
            foreach (NPM np in GameObject.FindObjectsOfType<NPM>())
            {
                np.transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
                np.transform.GetChild(0).GetChild(2).gameObject.SetActive(true);

                //np.transform.GetChild(0).gameObject.SetActive(false);
            }
            StartCoroutine(UpdateTimer());
        }

        if (flag == "GAMEEND")
        {
            GameRunning = false;
            GameStarted = false;
            GameEnding = true;
            Debug.Log("Game Ending Here");
            foreach (NPM np in GameObject.FindObjectsOfType<NPM>())
            {
                //np.transform.GetChild(0).gameObject.SetActive(true);
                //np.UI_Money(MoneyStolen);
                np.transform.GetChild(0).GetChild(2).gameObject.SetActive(false);
                np.transform.GetChild(0).GetChild(1).gameObject.SetActive(true);
                np.transform.GetChild(0).GetChild(1).GetChild(0).GetComponentInChildren<TextMeshProUGUI>().text = "Money Stolen: " + MoneyStolen;
                if ((StartingMoney * 0.75) < MoneyStolen)
                {
                    np.transform.GetChild(0).GetChild(1).GetChild(2).GetComponentInChildren<TextMeshProUGUI>().text = "Lobsters!";
                }
                else
                {
                    np.transform.GetChild(0).GetChild(1).GetChild(2).GetComponentInChildren<TextMeshProUGUI>().text = "Human!";
                }
                
            }
            if(IsServer)
            {
                StartCoroutine(DisconnectGameServer());
            }
            
            //Debug.Log("Money on GameMaster: " + MoneyStolen);
        }

        if (flag == "NAME")
        {
            foreach (NPM lp in GameObject.FindObjectsOfType<NPM>())
            {
                if (IsServer)
                {
                    lp.PName = value;
                    SendUpdate("NAME", value);
                }
                if (IsClient)
                {
                    lp.PName = value;
                }
            }
        }

        if (flag == "MONEY")
        {
            MoneyStolen = int.Parse(value);

            if (IsServer)
            {
                Debug.Log("Money on GM: " + MoneyStolen);
            }
            if(IsClient)
            {
                MoneyStolen = int.Parse(value);

                foreach (NPM np in GameObject.FindObjectsOfType<NPM>())
                {

                    np.transform.GetChild(0).GetChild(2).GetChild(0).GetChild(3).GetComponent<TextMeshProUGUI>().text = MoneyStolen.ToString();

                }
            }
        }

        if(flag == "TIME")
        {
            TimeRemaining = float.Parse(value);

            if (IsServer)
            {
                SendUpdate("TIME", TimeRemaining.ToString());
            }
            if(IsClient)
            {
                TimeRemaining = float.Parse(value);

                foreach (NPM np in GameObject.FindObjectsOfType<NPM>())
                {
                    np.transform.GetChild(0).GetChild(2).gameObject.SetActive(true);

                    np.transform.GetChild(0).GetChild(2).GetComponentInChildren<TextMeshProUGUI>().text = TimeRemaining.ToString();

                    if(TimeRemaining <= 10)
                    {
                        np.transform.GetChild(0).GetChild(2).GetComponentInChildren<TextMeshProUGUI>().color = new Color(1.0f, 0.5f, 0.0f);

                        if (TimeRemaining <= 5)
                        {
                            np.transform.GetChild(0).GetChild(2).GetComponentInChildren<TextMeshProUGUI>().color = Color.red;
                        }
                    }

                }
            }
        }
    }
    public override void NetworkedStart()
    {
        MoneyStolen = 0;
    }

    public IEnumerator DisconnectGameServer()
    {
        yield return new WaitForSeconds(10f);
        Debug.LogError("In the Coroutine");
        if(IsServer)
        {
            Debug.LogError("IsServer");
            if(MyCore.IsConnected)
            {
                Debug.LogError("IsConnected");
                StartCoroutine(MyCore.DisconnectServer());
            }
        }
    }

    public IEnumerator UpdateTimer()
    {
        while(GameRunning && IsServer)
        {
            yield return new WaitForSeconds(1f);
            elapsedTime += 1f;
            //Debug.Log(elapsedTime);

            /*if (TimeRemaining > 0)
            {
                TimeRemaining -= 1f;
            }
            else if (TimeRemaining < 0)
            {
                TimeRemaining = 0;
            }
            minutes = Mathf.FloorToInt(TimeRemaining / 60);
            seconds = Mathf.FloorToInt(TimeRemaining % 60);

            Debug.Log(minutes + ":" + seconds);*/

            if (TimeRemaining > 0)
            {
                TimeRemaining -= 1f;
            }
            else if (TimeRemaining < 0)
            {
                TimeRemaining = 0;
            }
            SendUpdate("TIME", TimeRemaining.ToString());

            if (elapsedTime >= timeout)
            {
                //SendCommand("GAMEEND", "true");
                GameEnd();
                yield break;
            }
        }

        /*foreach (NPM np in GameObject.FindObjectsOfType<NPM>())
        {
            np.transform.GetChild(0).GetChild(2).gameObject.SetActive(true);

            np.transform.GetChild(0).GetChild(2).GetComponentInChildren<TextMeshProUGUI>().text = minutes + ":" + seconds;
        }*/
    }

    public void GameEnd()
    {
        //yield return new WaitForSeconds(30);
        GameRunning = false;
        GameStarted = false;
        GameEnding = true;
        SendUpdate("GAMEEND", "true");
        MoneyStolen = 0;
        SendUpdate("MONEY", MoneyStolen.ToString());
        int finalStolen = MoneyStolen;
        foreach (NPM np in GameObject.FindObjectsOfType<NPM>())
        {
            //np.transform.GetChild(0).gameObject.SetActive(true);
            //np.UI_Money(MoneyStolen);
            np.transform.GetChild(0).GetChild(1).gameObject.SetActive(true);

            //np.transform.GetChild(0).GetChild(1).GetChild(0).GetComponentInChildren<TextMeshProUGUI>().text = "Money Stolen: " + finalStolen;
            np.UI_Money(finalStolen);
          
        }
        SendUpdate("MONEY", MoneyStolen.ToString());
        Debug.Log("Money on GameMaster: " + MoneyStolen);
        if(IsServer)
        {
            StartCoroutine(DisconnectGameServer());
        }

        //MyCore.DisconnectServer();

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
            if (count < 4)
            {
                readyGo = false;
            }
            if (NPM.humanCount != 1 && NPM.lobsterCount != 3)
            {
                readyGo = false;
            }
            GameStarted = readyGo;
            GameRunning = readyGo;
            yield return new WaitForSeconds(2);
        }

        if (IsServer && GameRunning)
        {
            //while(all players have not hit ready)
            //Wait

            SendUpdate("GAMESTART", GameStarted.ToString());
            StartCoroutine(UpdateTimer());
            
            SendUpdate("MONEY", MoneyStolen.ToString());
            Debug.Log("Game Master Money: " + MoneyStolen);
            //SendUpdate("GAMEEND", GameEnding.ToString());

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
        while (IsServer && GameRunning)
        {

            //counts every NPM and adds it to the MoneyStolen
            MoneyStolen = 0;
            foreach (Lobster pl in GameObject.FindObjectsOfType<Lobster>())
            {
                MoneyStolen += pl.TreasureCollected;
            }
            SendUpdate("MONEY", MoneyStolen.ToString());
            // if((StartingMoney * 0.05f) < MoneyStolen)
            // {
            //     GameEnd();
            // }
            //Debug.Log("Money on GameMaster: " + MoneyStolen); //always ends up at 0 :(

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

            if (IsDirty)
            {
                SendUpdate("GAMESTART", GameStarted.ToString());
                //SendUpdate("GAMEEND", GameEnding.ToString());
                SendUpdate("MONEY", MoneyStolen.ToString());
                SendUpdate("TIME", TimeRemaining.ToString());
                IsDirty = false;
            }
            yield return new WaitForSeconds(.1f);
        }
        //yield return new WaitForSeconds(.1f);
    }

    // Start is called before the first frame update
    void Start()
    {
        GameStarted = false;
        GameEnding = false;

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
