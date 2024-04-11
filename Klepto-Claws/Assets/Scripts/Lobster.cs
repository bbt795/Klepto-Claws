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
        


    }

    public void OnTrigger()
    {
        


    }

    private void OnTriggerEnter(Collider c)
    {
        //MyCore.NetDestroyObject(MyId.NetId);
        //shouldnt be MyId.NetId but idk how to access other objects' net IDs :(

        if (IsServer || IsClient)
        {
            switch (c.gameObject.tag)
            {

                case "Treasure1":

                    TreasureCollected += 250;
                    MyCore.NetDestroyObject(c.gameObject.GetComponent<NetworkID>().NetId);
                    Debug.Log(TreasureCollected);

                    break;

                case "Treasure2":

                    TreasureCollected += 150;
                    MyCore.NetDestroyObject(c.gameObject.GetComponent<NetworkID>().NetId);
                    Debug.Log(TreasureCollected);

                    break;

                case "Treasure3":

                    TreasureCollected += 120;
                    MyCore.NetDestroyObject(c.gameObject.GetComponent<NetworkID>().NetId);
                    Debug.Log(TreasureCollected);

                    break;

                case "Treasure4":

                    TreasureCollected += 100;
                    MyCore.NetDestroyObject(c.gameObject.GetComponent<NetworkID>().NetId);
                    Debug.Log(TreasureCollected);

                    break;

                case "Treasure5":

                    TreasureCollected += 95;
                    MyCore.NetDestroyObject(c.gameObject.GetComponent<NetworkID>().NetId);
                    Debug.Log(TreasureCollected);

                    break;

                case "Treasure6":

                    TreasureCollected += 75;
                    MyCore.NetDestroyObject(c.gameObject.GetComponent<NetworkID>().NetId);
                    Debug.Log(TreasureCollected);

                    break;
            }   
        }

    }

    public IEnumerator Reload()
    {

        yield return new WaitForSeconds(0.1f);

    }

}
