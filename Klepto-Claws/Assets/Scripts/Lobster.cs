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
    public GameObject currentcolliding;

    public GameMaster gameMaster;

    public Text Value;

    public override void HandleMessage(string flag, string value)
    {
        //throw new System.NotImplementedException();
        if(flag == "PICKUP" && canCollect)
        {
            Treasure treasure = currentcolliding.GetComponent<Treasure>();
            TreasureCollected += treasure.treasureValue;
            //Debug.Log("Treasure collected: " + TreasureCollected);
            MyCore.NetDestroyObject(currentcolliding.GetComponent<NetworkID>().NetId);
            gameMaster.RemoveItemFromList(currentcolliding);
            //Debug.Log("Object destroyed on server");
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
    }

    public override void NetworkedStart()
    {
        //throw new System.NotImplementedException();
    }

    public override IEnumerator SlowUpdate()
    {
        while (IsConnected)
        {
            if (IsServer)
            {
                if (IsDirty)
                {

                }
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

    }

    // public void OnClick(InputAction.CallbackContext context)
    // {
    //     if (canCollect)
    //     {

    //         Debug.Log("Zoinks");

    //         if (context.GetKey(KeyCode.E))
    //         {

    //             Debug.Log("Press E");

    //             Treasure treasure = currentcolliding.GetComponent<Treasure>();
    //             TreasureCollected += treasure.treasureValue;
    //             Debug.Log("Treasure collected: " + TreasureCollected);
    //             MyCore.NetDestroyObject(currentcolliding.GetComponent<NetworkID>().NetId);
    //             Debug.Log("Object destroyed on server");

    //         }

    //         //Debug.Log("E key pressed");
    //         //isPressed = true;
    //     }
    // }
    public void OnPickUp(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            Debug.Log("E was pushed");
            SendCommand("PICKUP", "");
        }
    }

    // Update is called once per frame
    void Update()
    {

        // if (canCollect)
        // {

        //     Debug.Log("Zoinks");

        //     if (Input.GetKey(KeyCode.E))
        //     {

        //         Debug.Log("Press E");

        //         Treasure treasure = currentcolliding.GetComponent<Treasure>();
        //         TreasureCollected += treasure.treasureValue;
        //         Debug.Log("Treasure collected: " + TreasureCollected);
        //         MyCore.NetDestroyObject(currentcolliding.GetComponent<NetworkID>().NetId);
        //         Debug.Log("Object destroyed on server");

        //     }

        //     //Debug.Log("E key pressed");
        //     //isPressed = true;
        // }

    }

    private void OnTriggerEnter(Collider c)
    {
        //MyCore.NetDestroyObject(MyId.NetId);
        //shouldnt be MyId.NetId but idk how to access other objects' net IDs :(

        if (IsServer || IsLocalPlayer)
        {

            if (c.gameObject.GetComponent<Treasure>() != null)
            {

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
