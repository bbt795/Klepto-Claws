using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using NETWORK_ENGINE;

public class NPM : NetworkComponent
{
    public bool IsReady;
    public float MoneyCollected;

    public TextMeshProUGUI tmpObject;

    public override void HandleMessage(string flag, string value)
    {
        if(flag == "READY")
        {
            IsReady = bool.Parse(value);
            if(IsServer)
            {
                SendUpdate("READY", value);
            }
        }
        if(flag == "MONEY")
        {
            MoneyCollected = float.Parse(value);
            if(IsServer)
            {
                //tmpObject.text = "Money Collected: " + value;
                tmpObject.text = "Money Collected: " + MoneyCollected;
                SendUpdate("MONEY", value.ToString());
            }
            if(IsClient)
            {
                tmpObject.text = "Money Collected: " + float.Parse(value);
            }
        }
        //throw new System.NotImplementedException();
    }

    public void UI_Ready(bool r)
    {
        if(IsLocalPlayer)
        {
            SendCommand("READY", r.ToString());
        }
    }

    public void UI_Money(float money)
    {
        SendCommand("MONEY", money.ToString());
    }

    public override void NetworkedStart()
    {
        if(!IsLocalPlayer)
        {
            this.transform.GetChild(0).gameObject.SetActive(false);
        }
        //throw new System.NotImplementedException();
    }

    public override IEnumerator SlowUpdate()
    {
        while(IsConnected)
        {
            if(IsServer)
            {
                if(IsDirty)
                {
                    SendUpdate("MONEY", MoneyCollected.ToString());
                    IsDirty = false;
                }
            }
            yield return new WaitForSeconds(.1f);
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
}
