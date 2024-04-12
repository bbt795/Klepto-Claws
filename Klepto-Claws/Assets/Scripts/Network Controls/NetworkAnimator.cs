using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NETWORK_ENGINE;

[RequireComponent(typeof(Animator))]
public class NetworkAnimator : NetworkComponent
{
    Animator anim;

    public override void HandleMessage(string flag, string value)
    {
        if (flag == "SETBOOL")
        {
            if (IsServer)
                SendUpdate(flag, value);
            if (IsClient)
            {
                string[] parameters = value.Split(',');
                anim.SetBool(parameters[0], bool.Parse(parameters[1]));
            }
        }
        if (flag == "SETFLOAT")
        {
            if (IsServer)
                SendUpdate(flag, value);
            if (IsClient)
            {
                string[] parameters = value.Split(',');
                anim.SetFloat(parameters[0], float.Parse(parameters[1]));
            }
        }

        if (flag == "SETTRIGGER")
        {
            if (IsServer)
                SendUpdate(flag, value);
            if (IsClient)
            {
                anim.SetTrigger(value);
            }
        }
    }

    public override void NetworkedStart()
    {
        
    }

    public override IEnumerator SlowUpdate()
    {
        yield return new WaitForSeconds(MyId.UpdateFrequency);
    }


    void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void SetBool(string parameterName, bool value)
    {
        if (IsServer)
            SendUpdate("SETBOOL", parameterName + "," + value.ToString());
        if (IsClient)
            SendCommand("SETBOOL", parameterName + "," + value.ToString());
    }

    public void SetInteger(string parameterName, int value)
    {
        if (IsServer)
            SendUpdate("SETINT", parameterName + "," + value.ToString());
        if (IsClient)
            SendCommand("SETINT", parameterName + "," + value.ToString());
    }

    public void SetFloat(string parameterName, float value)
    {
        if (IsServer)
            SendUpdate("SETFLOAT", parameterName + "," + value.ToString());
        if (IsClient)
            SendCommand("SETFLOAT", parameterName + "," + value.ToString());
    }

    public void SetTrigger(string parameterName)
    {
        if (IsServer)
            SendUpdate("SETTRIGGER", parameterName);
        if (IsClient)
            SendCommand("SETTRIGGER", parameterName);
    }
}
