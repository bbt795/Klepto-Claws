using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NETWORK_ENGINE;
using UnityEngine.InputSystem;

//COPIED DIRECTLY FROM SKELETON ASSIGNMENT, MAY NEED/WANT CHANGES!!!

public class NetworkPlayerController : NetworkComponent
{
    public Material[] PlayerMatArray;
    //public Renderer PlayerRenderer;
    public Rigidbody MyRig;
    public Animator MyAnime;

    public PlayerInput MyInput;
    public InputActionAsset MyMap;
    public InputAction MoveA;
    public InputAction FireA;

    public Vector2 LastMove;
    public bool IsFiring;
    public bool CanFire = true;
    public bool GameOver = false;

    public override void HandleMessage(string flag, string value)
    {

        if (flag == "MOVE" && IsServer)
        {

            string[] args = value.Split(',');
            LastMove = new Vector2(float.Parse(args[0]), float.Parse(args[1]));

        }

        if (flag == "FIRE" && CanFire)
        {
            MyAnime.SetInteger("DIR", 2);

            if (IsServer)
            {

                CanFire = false;
                StartCoroutine(Reload());
                SendUpdate("FIRE", "2");

            }

        }

    }

    public override void NetworkedStart()
    {
        
        /*if(this.gameObject.tag == "Lobster")
        {

            //PlayerRenderer.materials[0] = PlayerMatArray[this.Owner % 3];
            //PlayerRenderer.material = PlayerMatArray[this.Owner % 3];

        }

        if (IsServer)
        {
            //3 Human Spawn points (HSpawn#)
            //4 Lobster Spawn points (LSpawn#)

            if(this.gameObject.tag == "Lobster")
            {

                GameObject temp = GameObject.Find("LSpawn" + Random.Range(1, 5));
                MyRig.position = temp.transform.position;
                MyRig.useGravity = true;

            } else if (this.gameObject.tag == "Human")
            {

                GameObject temp = GameObject.Find("HSpawn" + Random.Range(1, 4));
                MyRig.position = temp.transform.position;
                MyRig.useGravity = true;

            }

        }*/

        if (IsLocalPlayer)
        {

            MyInput.actions = MyMap;
            MoveA = MyInput.currentActionMap.FindAction("Move", true);
            MoveA.started += this.OnDirectionChanged;
            MoveA.performed += this.OnDirectionChanged;
            MoveA.canceled += this.OnDirectionChanged;
            FireA = MyInput.currentActionMap.FindAction("Fire", true);
            FireA.started += this.OnFire;

        }

    }

    public void OnDestroy()
    {

        MoveA.started -= this.OnDirectionChanged;
        MoveA.performed -= this.OnDirectionChanged;
        MoveA.canceled -= this.OnDirectionChanged;
        FireA.started -= this.OnFire;

    }

    public void OnDirectionChanged(InputAction.CallbackContext context)
    {
        if (context.action.phase == InputActionPhase.Started || context.action.phase == InputActionPhase.Performed)
        {

            LastMove = context.ReadValue<Vector2>();
            SendCommand("MOVE", LastMove.x + "," + LastMove.y);

        }
        if (context.action.phase == InputActionPhase.Canceled)
        {

            LastMove = Vector2.zero;
            SendCommand("MOVE", LastMove.x + "," + LastMove.y);

        }
    }

    public void OnFire(InputAction.CallbackContext context)
    {
        if (context.action.phase == InputActionPhase.Started)
        {
            SendCommand("FIRE", "2");
        }
    }

    public override IEnumerator SlowUpdate()
    {
        yield return new WaitForSeconds(.1f);
    }

    // Start is called before the first frame update
    void Start()
    {

        MyAnime = GetComponent<Animator>();
        MyInput = GetComponent<PlayerInput>();

    }

    // Update is called once per frame
    void Update()
    {

        if (IsServer)
        {

            MyRig.velocity = this.transform.forward * LastMove.y * 3 + new Vector3(0, MyRig.velocity.y, 0);
            MyRig.angularVelocity = new Vector3(0, LastMove.x, 0) * Mathf.PI / 3.0f;
            var speed = Mathf.Max(Mathf.Abs(MyRig.velocity.x), Mathf.Max(MyRig.angularVelocity.y));
            if(this.gameObject.tag == "Human")
            {
                bool caught = MyAnime.GetBool("Captured");
               if(!caught)
                {
                    MyAnime.SetInteger("DIR", 1);
                }
                else if(caught)
                {
                    MyAnime.SetInteger("DIR", 2);
                } 
            }
            else
            {
                MyAnime.SetInteger("DIR", 1);
            }
            

        }

        if (IsLocalPlayer)
        {

            Camera.main.transform.position = this.transform.position + this.transform.forward * -3 + this.transform.up;
            Camera.main.transform.LookAt(this.transform.position);

        }

        if (IsClient)
        {

            if (Mathf.Abs(MyRig.velocity.magnitude) > Mathf.Abs(MyRig.angularVelocity.y))
            {

                MyAnime.SetInteger("DIR", 1);

            }
            else
            {

                MyAnime.SetInteger("DIR", 0);

            }

        }

    }

    public IEnumerator Reload()
    {

        yield return new WaitForSeconds(0.5f);
        MyAnime.SetInteger("DIR", 0);
        CanFire = true;

    }

}
