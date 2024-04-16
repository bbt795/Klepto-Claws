using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using NETWORK_ENGINE;

public class NPM : NetworkComponent
{
    public string PName;

    public bool IsReady;
    public int MoneyCollected;

    public static int lobsterCount;
    public static int humanCount;

    public bool IsLobster;
    public bool IsHuman;

    public TextMeshProUGUI tmpObject;
    public Toggle humanToggle;
    public Toggle lobsterToggle;

    public Image humanImg;
    public Image lobsterImg;

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

        if (flag == "NAME")
        {
            PName = value;
            if (IsServer)
            {
                SendUpdate("NAME", value);
            }
        }

        if (flag == "MONEY")
        {
            MoneyCollected = int.Parse(value);

            if(IsServer)
            {

                // foreach (Lobster pl in GameObject.FindObjectsOfType<Lobster>())
                // {
                //     MoneyCollected += pl.TreasureCollected;
                // }

                //tmpObject.text = "Money Collected: " + value;
                tmpObject.text = "Money Stolen: " + MoneyCollected.ToString();
                Debug.Log("Money on NPM: " + MoneyCollected);
                SendUpdate("MONEY", MoneyCollected.ToString());
            }
            if(IsClient)
            {
                tmpObject.text = "Money Stolen: " + MoneyCollected.ToString();
                // foreach (Lobster pl in GameObject.FindObjectsOfType<Lobster>())
                // {
                //     MoneyCollected += pl.TreasureCollected;
                // }
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

    public void UI_NameInput(string s)
    {
        if (IsLocalPlayer)
        {
            SendCommand("NAME", s);
        }

    }

    public void UI_Ready(bool r)
    {
        if(IsLocalPlayer)
        {
            SendCommand("READY", r.ToString());
        }
    }

    public void UI_Money(int money)
    {
        SendCommand("MONEY", money.ToString());
    }

    public void UI_HumanTeam(bool team)
    {

        lobsterToggle.interactable = !team;
        SendCommand("HTEAM",team.ToString());
        if(IsLocalPlayer)
        {
            humanImg.gameObject.SetActive(true);
        }

    }

    public void UI_LobsterTeam(bool team)
    {

        humanToggle.interactable = !team;
        SendCommand("LTEAM", team.ToString());
        if(IsLocalPlayer)
        {
            lobsterImg.gameObject.SetActive(true);
        }

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
                    SendUpdate("NAME", PName);
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
        humanImg = transform.Find("Human").GetComponent<Image>();
        lobsterImg = transform.Find("Lobster").GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
