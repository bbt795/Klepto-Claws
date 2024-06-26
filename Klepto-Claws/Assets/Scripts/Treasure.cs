using NETWORK_ENGINE;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Treasure : NetworkComponent
{

    public int type;
    public int treasureValue;

    public override void HandleMessage(string flag, string value)
    {
        
    }

    public override void NetworkedStart()
    {

        //Generally how we want to assign value
        switch (this.gameObject.tag)
        {

            case "Treasure1":

                treasureValue = 250;

                break;

            case "Treasure2":

                treasureValue = 150;

                break;

            case "Treasure3":

                treasureValue = 120;
                
                break;

            case "Treasure4":

                treasureValue = 100;

                break;

            case "Treasure5":

                treasureValue = 95;

                break;

            case "Treasure6":

                treasureValue = 75;

                break;
        }


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
}
