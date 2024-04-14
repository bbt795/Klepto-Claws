using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NETWORK_ENGINE;

public class Human : NetworkComponent, IPlayer
{
    public float Speed { get; set; }
    public float Strength { get; set; }

    bool canCapture = false;

    public override void HandleMessage(string flag, string value)
    {
        throw new System.NotImplementedException();
    }

    public override void NetworkedStart()
    {
        throw new System.NotImplementedException();
    }

    public override IEnumerator SlowUpdate()
    {
        throw new System.NotImplementedException();
    }

    private void OnTriggerEnter(Collider c)
    {
        
        if(IsServer || IsClient)
        {

            if(c.gameObject.GetComponent<Lobster>() != null)
            {

                Debug.Log("Yeet");
                c.gameObject.transform.GetChild(3).gameObject.SetActive(true);
                canCapture = true;

            }

        }

    }

    private void OnTriggerExit(Collider c)
    {

        if (IsServer || IsClient)
        {

            if (c.gameObject.GetComponent<Lobster>() != null)
            {

                Debug.Log("Yuh");
                c.gameObject.transform.GetChild(3).gameObject.SetActive(false);
                canCapture = false;

            }

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
