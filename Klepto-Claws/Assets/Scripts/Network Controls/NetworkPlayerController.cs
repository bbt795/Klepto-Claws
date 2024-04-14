using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NETWORK_ENGINE;
using UnityEngine.InputSystem;

public class NetworkPlayerController : NetworkComponent
{
    public Material[] PlayerMatArray;
    public Renderer PlayerRenderer;
    public Rigidbody MyRig;
    public Animator MyAnime;

    public PlayerInput MyInput;
    public InputActionAsset MyMap;
    public InputAction MoveA;
    public InputAction FireA;

    public LayerMask collisionMask;
    private float distance = 3f;

    public Vector2 LastMove;
    public bool IsFiring;
    public bool CanFire = true;
    public bool GameOver = false;

    public bool Captured;

    public override void HandleMessage(string flag, string value)
    {

        if (flag == "MOVE" && IsServer)
        {

            string[] args = value.Split(',');
            LastMove = new Vector2(float.Parse(args[0]), float.Parse(args[1]));

        }

        if (flag == "FIRE" && CanFire)
        {
            

            if (IsServer)
            {

                CanFire = false;
                StartCoroutine(Reload());
                SendUpdate("FIRE", value);

            }
            MyAnime.SetTrigger("Attack");
        }
        
        if(flag == "TAUNT")
        {
            if(IsServer)
            {
                SendUpdate("TAUNT", value);
            }
            MyAnime.SetTrigger("Taunt");
        }

    }

    public override void NetworkedStart()
    {
        
        if(this.gameObject.tag == "Lobster")
        {

            PlayerRenderer.materials[0] = PlayerMatArray[this.Owner % 3];
            PlayerRenderer.material = PlayerMatArray[this.Owner % 3];

        }

        /*if (IsServer)
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
        if (IsLocalPlayer && (context.started || context.performed))
        {

            LastMove = context.ReadValue<Vector2>();
            SendCommand("MOVE", LastMove.x + "," + LastMove.y);

        }
        if (context.canceled)
        {

            LastMove = Vector2.zero;
            SendCommand("MOVE", LastMove.x + "," + LastMove.y);

        }
    }

    public void OnFire(InputAction.CallbackContext context)
    {
        if (IsLocalPlayer && context.started)
        {
            Debug.Log("Fire button was pushed");
            SendCommand("FIRE", "2");
        }
    }

    public void OnTaunt(InputAction.CallbackContext context)
    {
        if(IsLocalPlayer && context.started)
        {
            Debug.Log("Taunt button was pushed");
            SendCommand("TAUNT", "3");
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
        Captured = MyAnime.GetBool("Caught");

        if (IsServer)
        {

            MyRig.velocity = this.transform.forward * LastMove.y * 3 + new Vector3(0, MyRig.velocity.y, 0);
            MyRig.angularVelocity = new Vector3(0, LastMove.x, 0) * Mathf.PI / 3.0f;
            var speed = Mathf.Max(Mathf.Abs(MyRig.velocity.x), Mathf.Max(MyRig.angularVelocity.y));
            MyAnime.SetFloat("Move", speed);
        }

        if (IsLocalPlayer)
        {
            if(this.gameObject.tag == "Lobster")
            {
                Vector3 desiredPosition = this.transform.position - this.transform.forward * distance + transform.up;

                RaycastHit hit;
                if (Physics.Raycast(desiredPosition, this.transform.forward, out hit, distance, collisionMask))
                {
                    Camera.main.transform.position = hit.point;
                }
                else
                {
                    Camera.main.transform.position = this.transform.position + this.transform.forward * -3 + this.transform.up;
                }

                Camera.main.transform.LookAt(this.transform.position);
            }
            else if(this.gameObject.tag == "Human")
            {
                Vector3 desiredPosition = this.transform.position - this.transform.forward * distance + transform.up;

                RaycastHit hit;
                if (Physics.Raycast(desiredPosition, this.transform.forward, out hit, distance, collisionMask))
                {
                    Camera.main.transform.position = hit.point;
                }
                else
                {
                    Camera.main.transform.position = this.transform.position + this.transform.forward * -3 + this.transform.up;
                }

                //Camera.main.transform.position = this.transform.position + this.transform.forward * -3 + this.transform.up;
                Camera.main.transform.LookAt(this.transform.position);
            }
            

        }

        if (IsClient)
        {

            if (Mathf.Abs(MyRig.velocity.magnitude) > Mathf.Abs(MyRig.angularVelocity.y))
            {

                MyAnime.SetFloat("Move", 1f);

            }
            else
            {

                MyAnime.SetFloat("Move", 0f);

            }

        }

    }

    public IEnumerator Reload()
    {

        yield return new WaitForSeconds(0.5f);
        //MyAnime.SetInteger("DIR", 0);
        CanFire = true;

    }

}