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
        //throw new System.NotImplementedException();
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

                    Debug.Log("Trigger entered");
                    //treasureValue = 250;

                    break;

                case "Treasure2":

                    Debug.Log("Trigger entered");
                    //treasureValue = 150;

                    break;

                case "Treasure3":

                    Debug.Log("Trigger entered");
                    //treasureValue = 120;

                    break;

                case "Treasure4":

                    Debug.Log("Trigger entered");
                    //treasureValue = 100;

                    break;

                case "Treasure5":

                    Debug.Log("Trigger entered");
                    //treasureValue = 95;

                    break;

                case "Treasure6":

                    Debug.Log("Trigger entered");
                    //treasureValue = 75;

                    break;
            }   
        }

    }

    public IEnumerator Reload()
    {

        yield return new WaitForSeconds(0.1f);

    }

}
