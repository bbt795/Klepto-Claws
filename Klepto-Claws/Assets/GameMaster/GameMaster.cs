using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NETWORK_ENGINE;

public class GameMaster : NetworkComponent
{
    public bool GameStarted = false;
    public override void HandleMessage(string flag, string value)
    {   
        if(flag == "GAMESTART")
        {
            //   Want to disable PlayerInfo
            GameStarted = true;
            foreach(NPM np in GameObject.FindObjectsOfType<NPM>())
            {
                np.transform.GetChild(0).gameObject.SetActive(false);
            }
        }
    }
    public override void NetworkedStart()
    {

    }
    public override IEnumerator SlowUpdate()
    {
        while(!GameStarted && IsServer)
        {
            bool readyGo = true;
            int count = 0;
            foreach(NPM np in GameObject.FindObjectsOfType<NPM>())
            {
                if(!np.IsReady)
                {
                    readyGo = false;
                    break;
                }
                count++;
            }
            if(count <1)
            {
                readyGo = false;
            }
            GameStarted = readyGo;
            yield return new WaitForSeconds(2);
        }
        if (IsServer)
        {
            //while(all players have not hit ready)
            //Wait

            SendUpdate("GAMESTART", GameStarted.ToString()); 

            //Go to each NetworkPlayerManager and look at their options
            //Create the appropriate character for their options
            //GameObject temp = MyCore.NetCreateObject(1,Owner,new Vector3);
            //temp.GetComponent<MyCharacterScript>().team = //set the team;

            MyId.NotifyDirty();
        }
        while(IsServer)
        {
            if(IsDirty)
            {
                SendUpdate("GAMESTART", GameStarted.ToString());
                IsDirty = false;
            }
            yield return new WaitForSeconds(1);
        }
        yield return new WaitForSeconds(.1f);
    }

    // Start is called before the first frame update
    void Start()
    {
        GameStarted = false;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
