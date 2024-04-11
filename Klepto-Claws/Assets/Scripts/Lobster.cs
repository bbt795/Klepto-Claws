using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NETWORK_ENGINE;

public class Lobster: NetworkComponent, IPlayer
{
    public float Speed { get; set; }
    public float Strength { get; set; }
    public int TreasureCollected;
    public Material[] MaterialArray;
    private bool isPressed;

    public override void HandleMessage(string flag, string value)
    {
        //throw new System.NotImplementedException();
    }

    public override void NetworkedStart()
    {
        //throw new System.NotImplementedException();
    }

    public override IEnumerator SlowUpdate()
    {
        yield return new WaitForSeconds(0.1f);
    }

    // Start is called before the first frame update
    void Start()
    {
        


    }

    // Update is called once per frame
    void Update()
    {
        
        if(Input.GetKeyDown(KeyCode.E))
        {

            isPressed = true;

        }

    }

    private void OnTriggerEnter(Collider c)
    {
        //MyCore.NetDestroyObject(MyId.NetId);
        //shouldnt be MyId.NetId but idk how to access other objects' net IDs :(

        if (IsServer || IsClient)
        {

            if (c.gameObject.GetComponent<Treasure>() != null)
            {

                c.gameObject.transform.GetChild(0).gameObject.SetActive(true);

            }

        }

    }

    private void OnTriggerStay(Collider c)
    {

        if (IsServer || IsClient)
        {

            if(c.gameObject.GetComponent<Treasure>() != null)
            {

                Debug.Log("i'm in");

                if (isPressed)
                {

                    Debug.Log("E down");
                    TreasureCollected += c.gameObject.GetComponent<Treasure>().treasureValue;
                    MyCore.NetDestroyObject(c.gameObject.GetComponent<NetworkID>().NetId);
                    isPressed = false;

                }

            }

        }
        
    }

    private void OnTriggerExit(Collider c)
    {

        if (IsServer || IsClient)
        {

            if (c.gameObject.GetComponent<Treasure>() != null)
            {

                c.gameObject.transform.GetChild(0).gameObject.SetActive(false);

            }

        }

    }

    public IEnumerator Reload()
    {

        yield return new WaitForSeconds(0.1f);

    }

}
