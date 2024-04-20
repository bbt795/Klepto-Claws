using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NETWORK_ENGINE;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Lobster : NetworkComponent, IPlayer
{
    public float Speed { get; set; }
    public float Strength { get; set; }

    public Text PlayerName;

    public int TreasureCollected;
    public Material[] MaterialArray;
    
    public bool canCollect;
    public bool isCaptured = false;
    public Vector3 capturedPosition = new Vector3(-36f, 1.5f, 11.5f);
    public GameObject currentcolliding;

    public GameMaster gameMaster;

    public Text Value;

    public bool isInsideTrigger = false;
    public AudioClip pickupSound;

    public List<Vector3> SpawnPoints;
    public Vector3 CurrentSpawn;

    public override void HandleMessage(string flag, string value)
    {

        if(flag == "PICKUP" && canCollect)
        {
            Treasure treasure = currentcolliding.GetComponent<Treasure>();
            TreasureCollected += treasure.treasureValue;
            MyCore.NetDestroyObject(currentcolliding.GetComponent<NetworkID>().NetId);
            gameMaster.RemoveItemFromList(currentcolliding);
            SendUpdate("MONEY", TreasureCollected.ToString());
        }

        if (flag == "NAME")
        {
            foreach (NPM lp in GameObject.FindObjectsOfType<NPM>())
            {
                if (IsServer)
                {
                    lp.PName = value;
                    PlayerName.text = lp.PName;
                    SendUpdate("NAME", value);
                }
                if (IsClient)
                {
                    lp.PName = value;
                    PlayerName.text = lp.PName;
                }
            }
        }

        if (flag == "MONEY")
        {
            TreasureCollected = int.Parse(value.ToString());
            if (IsServer)
            {
                Value.text = TreasureCollected.ToString();
                SendUpdate("MONEY", value.ToString());
            }
            if (IsClient)
            {
                Value.text = TreasureCollected.ToString();
            }
        }
        if(flag == "CAUGHT")
        {
            isCaptured = bool.Parse(value);
            if(IsServer)
            {
                SendUpdate("CAUGHT", value.ToString());
            }
            if(IsClient)
            {
                isCaptured = bool.Parse(value);
            }
        }
        if(flag == "CAPTURED")
        {
            isCaptured = bool.Parse(value);
            if(IsServer)
            {
                isCaptured = false;
                this.transform.position = capturedPosition;
                StartCoroutine(TankTime());
                SendUpdate("CAPTURED", value);
            }
            if(IsClient)
            {
                isCaptured = bool.Parse(value);
                this.transform.position = capturedPosition;
            }
            
        }
        if(flag == "FREE")
        {
            if (IsServer)
            {
                isCaptured = false;
                Vector3 CurrentSpawn = SpawnPoints[int.Parse(value)];
                this.transform.position = CurrentSpawn;

                SendUpdate("FREE", value);
            }
            if(IsClient)
            {
                isCaptured = false;
                Vector3 CurrentSpawn = SpawnPoints[int.Parse(value)];
                this.transform.position = CurrentSpawn;
            }
        }
    }

    public IEnumerator TankTime()
    {
        while(IsServer)
        {
            yield return new WaitForSeconds(10f);
            isCaptured = false;
            int spawnIndex = Random.Range(0, SpawnPoints.Count);
            Vector3 CurrentSpawn = SpawnPoints[spawnIndex];
            this.transform.position = CurrentSpawn;
            TreasureCollected = 0;
            SendUpdate("MONEY", TreasureCollected.ToString());
            SendUpdate("FREE", spawnIndex.ToString());
            yield break; 
        }
    }

    public void CapturedTrue()
    {
        SendCommand("CAUGHT", "true");
    }

    public void LobsterCaptured()
    {
        SendCommand("CAPTURED", "true"); 
    }

    public override void NetworkedStart()
    {

    }

    public override IEnumerator SlowUpdate()
    {
        while (IsConnected)
        {
            if (IsServer)
            {
                if (IsDirty)
                {
                    SendUpdate("CAUGHT", isCaptured.ToString());
                }
            }
            if(isCaptured)
            {
                LobsterCaptured();
            }

            foreach (NPM lp in GameObject.FindObjectsOfType<NPM>())
            {
                if (lp.Owner == this.Owner)
                {

                    PlayerName.text = lp.PName;
                }
            }

            yield return new WaitForSeconds(.1f);
        }

        yield return new WaitForSeconds(0.1f);

    }

    // Start is called before the first frame update
    void Start()
    {

        gameMaster = FindObjectOfType<GameMaster>();

        GameObject[] temp = GameObject.FindGameObjectsWithTag("SpawnPoint");
        SpawnPoints = new List<Vector3>();
        foreach (GameObject g in temp)
        {
            SpawnPoints.Add(g.transform.position);
        }

    }

    public void OnPickUp(InputAction.CallbackContext context)
    {
        if (context.started && isInsideTrigger)
        {
            if (IsLocalPlayer)
            {
                AudioSource.PlayClipAtPoint(pickupSound, transform.position);
            }
            SendCommand("PICKUP", "");
        }
        if(context.canceled)
        {
            isInsideTrigger = false;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider c)
    {

        if (IsServer || IsLocalPlayer)
        {

            if (c.gameObject.GetComponent<Treasure>() != null)
            {
                isInsideTrigger = true;

                c.gameObject.transform.GetChild(0).gameObject.SetActive(true);
                currentcolliding = c.gameObject;
                canCollect = true;

            }

        }

    }

    private void OnTriggerExit(Collider c)
    {

        if (IsServer || IsLocalPlayer)
        {

            if (c.gameObject.GetComponent<Treasure>() != null)
            {
                isInsideTrigger = false;

                c.gameObject.transform.GetChild(0).gameObject.SetActive(false);
                currentcolliding = null;
                canCollect = false;

            }

        }

    }

    public IEnumerator Reload()
    {

        yield return new WaitForSeconds(0.1f);

    }

}
