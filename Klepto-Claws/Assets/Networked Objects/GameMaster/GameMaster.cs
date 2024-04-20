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
    private float timeout = 180f; //300f is 5 minutes
    public float TimeRemaining = 180f;
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

    public string playerName;
    public int maxMoneyStolen = 0;

    public override void HandleMessage(string flag, string value)
    {
        if (flag == "GAMESTART")
        {

            GameStarted = true;
            GameRunning = true;
            MyCore.NotifyGameStart();
            foreach (NPM np in GameObject.FindObjectsOfType<NPM>())
            {
                np.transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
                np.transform.GetChild(0).GetChild(2).gameObject.SetActive(true);

            }
            StartCoroutine(UpdateTimer());
        }

        if (flag == "GAMEEND")
        {
            GameRunning = false;
            GameStarted = false;
            GameEnding = true;

            GetPlayerWithHighestMoneyStolen();
            foreach (NPM np in GameObject.FindObjectsOfType<NPM>())
            {

                np.transform.GetChild(0).GetChild(2).gameObject.SetActive(false);
                np.transform.GetChild(0).GetChild(1).gameObject.SetActive(true);
                np.transform.GetChild(0).GetChild(1).GetChild(0).GetComponentInChildren<TextMeshProUGUI>().text = "Money Stolen: " + MoneyStolen;

                if ((StartingMoney * 0.25) < MoneyStolen)
                {
                    np.transform.GetChild(0).GetChild(1).GetChild(2).GetComponentInChildren<TextMeshProUGUI>().text = "Lobsters!";
                }
                else
                {
                    np.transform.GetChild(0).GetChild(1).GetChild(2).GetComponentInChildren<TextMeshProUGUI>().text = "Human!";
                }

                np.transform.GetChild(0).GetChild(1).GetChild(4).GetComponentInChildren<TextMeshProUGUI>().text = playerName.ToString() + " with " + maxMoneyStolen;
            }
            

            if (IsServer)
            {
                StartCoroutine(DisconnectGameServer());
            }
            
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

        if(IsServer)
        {

            if(MyCore.IsConnected)
            {
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
                GameEnd();
                yield break;
            }
        }
    }

    public void GameEnd()
    {
        GameRunning = false;
        GameStarted = false;
        GameEnding = true;
        SendUpdate("GAMEEND", "true");
        MoneyStolen = 0;
        SendUpdate("MONEY", MoneyStolen.ToString());

        int finalStolen = MoneyStolen;
        
        foreach (NPM np in GameObject.FindObjectsOfType<NPM>())
        {

            np.transform.GetChild(0).GetChild(1).gameObject.SetActive(true);

            np.UI_Money(finalStolen);

            np.transform.GetChild(0).GetChild(1).GetChild(3).GetComponentInChildren<TextMeshProUGUI>().text = playerName.ToString() + " with " + maxMoneyStolen;
        }
        SendUpdate("MONEY", MoneyStolen.ToString());
        if(IsServer)
        {
            StartCoroutine(DisconnectGameServer());
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
            MyCore.NotifyGameStart();

            SendUpdate("GAMESTART", GameStarted.ToString());
            StartCoroutine(UpdateTimer());
            
            SendUpdate("MONEY", MoneyStolen.ToString());

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
            //Checks if server is empty
            if(GameObject.FindObjectsOfType<NPM>().Length == 0)
            {
                MyCore.UI_Quit();
            }

            //counts every NPM and adds it to the MoneyStolen
            MoneyStolen = 0;
            foreach (Lobster pl in GameObject.FindObjectsOfType<Lobster>())
            {
                MoneyStolen += pl.TreasureCollected;
            }
            SendUpdate("MONEY", MoneyStolen.ToString());

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
                SendUpdate("MONEY", MoneyStolen.ToString());
                SendUpdate("TIME", TimeRemaining.ToString());
                IsDirty = false;
            }
            yield return new WaitForSeconds(.1f);
        }
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

    public void GetPlayerWithHighestMoneyStolen()
    {

        foreach (Lobster lp in GameObject.FindObjectsOfType<Lobster>())
        {
            this.maxMoneyStolen = Mathf.Max(maxMoneyStolen, lp.TreasureCollected);

            if (lp.TreasureCollected == maxMoneyStolen)
            {
                playerName = lp.PlayerName.text;
                
            }
        }
    }

}
