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

    public static int lobsterCount;
    public static int humanCount;

    public bool IsLobster;
    public bool IsHuman;

    public TextMeshProUGUI tmpObject;
    public Toggle humanToggle;
    public Toggle lobsterToggle;

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

        if(flag == "HTEAM")
        {

            IsHuman = bool.Parse(value);

            if (IsServer)
            {
                SendUpdate("HTEAM", value);

                bool toggleValue = bool.Parse(value);
                if (toggleValue)
                {

                    humanCount++;

                    if(humanCount == 1)
                    {

                        humanToggle.interactable = false;

                    }

                } else
                {

                    humanCount--;
                    //small logic error here when one player has lobster chosen, but then another player unchecks human, it would turn it back on for the lobster player
                    //will deal with later
                    if(humanCount < 1)
                    {

                        humanToggle.interactable = true;

                    }

                }

                SendUpdate("HTEAM", humanCount.ToString());
                Debug.Log(humanCount);
                //I give up for tonight
                //it's counting correctly, but not sync'ing the interactable?

            }

            if (IsClient)
            {

                humanCount = int.Parse(value);
                if (humanCount == 1)
                {
                    if (!humanToggle.isOn)
                    {
                        humanToggle.interactable = false;
                    }

                } else
                {

                    humanToggle.interactable = true;

                }

            }

        }

        if (flag == "LTEAM")
        {
            IsLobster = bool.Parse(value);
            if (IsServer)
            {
                SendUpdate("LTEAM", value);

                bool toggleValue = bool.Parse(value);
                if (toggleValue)
                {

                    lobsterCount++;

                    if(lobsterCount == 3)
                    {

                        lobsterToggle.interactable = false;

                    }

                }
                else
                {

                    lobsterCount--;

                    //same potential logic issue as above
                    if(lobsterCount < 3)
                    {

                        lobsterToggle.interactable = true;

                    }

                }

                SendUpdate("LTEAM", lobsterCount.ToString());
                Debug.Log(lobsterCount);
            }

            if (IsClient)
            {

                lobsterCount = int.Parse(value);

                if(lobsterCount == 3)
                {

                    lobsterToggle.interactable = false;

                } else
                {

                    lobsterToggle.interactable = true;

                }


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

    public void UI_HumanTeam(bool team)
    {

        lobsterToggle.interactable = !team;
        SendCommand("HTEAM",team.ToString());

    }

    public void UI_LobsterTeam(bool team)
    {

        humanToggle.interactable = !team;
        SendCommand("LTEAM", team.ToString());

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
                    SendUpdate("HTEAM", humanCount.ToString());
                    SendUpdate("LTEAM", lobsterCount.ToString());



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
