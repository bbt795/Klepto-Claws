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

                tmpObject.text = "Money Stolen: " + MoneyCollected.ToString();
                SendUpdate("MONEY", MoneyCollected.ToString());
            }
            if(IsClient)
            {
                tmpObject.text = "Money Stolen: " + MoneyCollected.ToString();
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

                } else
                {

                    humanCount--;

                }

                SendUpdate("HTEAM", humanCount.ToString());

            }

            if (IsClient)
            {

                humanCount = int.Parse(value);

            }

        }

        if (flag == "LTEAM")
        {
            
            if (IsServer)
            {
                IsLobster = bool.Parse(value);

                bool toggleValue = bool.Parse(value);
                if (toggleValue)
                {

                    lobsterCount++;

                }
                else
                {

                    lobsterCount--;

                }

                SendUpdate("LTEAM", lobsterCount.ToString());
            }

            if (IsClient)
            {

                lobsterCount = int.Parse(value);

            }

        }

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
            lobsterImg.gameObject.SetActive(false);
        }
    }

    public void UI_LobsterTeam(bool team)
    {

        humanToggle.interactable = !team;
        SendCommand("LTEAM", team.ToString());
        if(IsLocalPlayer)
        {
            lobsterImg.gameObject.SetActive(true);
            humanImg.gameObject.SetActive(false);
        }
    }

    public override void NetworkedStart()
    {
        if(!IsLocalPlayer)
        {
            this.transform.GetChild(0).gameObject.SetActive(false);
        }

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

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
