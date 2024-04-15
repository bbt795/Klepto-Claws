using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NETWORK_ENGINE;
using UnityEngine.InputSystem;

public class Human : NetworkComponent, IPlayer
{
    public float Speed { get; set; }
    public float Strength { get; set; }

    public bool canCapture;
    public bool canTank;
    public GameObject currentColliding;
    public int capturedTreasure;

    public override void HandleMessage(string flag, string value)
    {
        if(flag == "CAPTURE" && canCapture)
        {

            Lobster player = currentColliding.GetComponent<Lobster>();

            //Insert however we want to deal with lobster here

        }

        if(flag == "TANK")
        {

            Lobster player = currentColliding.GetComponent<Lobster>();
            capturedTreasure = player.TreasureCollected;
            player.TreasureCollected = 0;

        }
    }

    public override void NetworkedStart()
    {
        //throw new System.NotImplementedException();
    }

    public override IEnumerator SlowUpdate()
    {

        yield return new WaitForSeconds(0.1f);
        //throw new System.NotImplementedException();

    }

    public void onCapture(InputAction.CallbackContext context)
    {

        if (context.started)
        {
            Debug.Log("Human E Push");
            if(currentColliding.tag == "Lobster")
            {

                SendCommand("CAPTURE", "");

            } else if (currentColliding.tag == "Tank")
            {

                SendCommand("TANK", "");

            }
            

        }

    }

    private void OnTriggerEnter(Collider c)
    {
        
        if(IsServer || IsClient)
        {

            if(c.gameObject.tag == "Lobster")
            {

                Debug.Log("Yeet");
                c.gameObject.transform.GetChild(3).gameObject.SetActive(true);
                canCapture = true;

            } else if(c.gameObject.tag == "Tank")
            {

                Debug.Log("Yoink");
                c.gameObject.transform.GetChild(2).gameObject.SetActive(false);
                canTank = true;

            }

            currentColliding = c.gameObject;

        }

    }

    private void OnTriggerExit(Collider c)
    {

        if (IsServer || IsClient)
        {

            if (c.gameObject.tag == "Lobster")
            {

                Debug.Log("Yuh");
                c.gameObject.transform.GetChild(3).gameObject.SetActive(false);
                canCapture = false;

            }else if (c.gameObject.tag == "Tank")
            {

                Debug.Log("Yaga");
                c.gameObject.transform.GetChild(2).gameObject.SetActive(false);
                canTank = false;

            }

            currentColliding = null;

        }

    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Capture()
    {



    }
}
