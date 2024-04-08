using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NETWORK_ENGINE;
using UnityEngine.InputSystem;

//COPIED DIRECTLY FROM SKELETON ASSIGNMENT, MAY NEED/WANT CHANGES!!!

public class NetworkPlayerController : NetworkComponent
{
    public Material[] SkeletonMatArray;
    public Renderer SkeletonRenderer;
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

            if (IsServer)
            {

                CanFire = false;
                StartCoroutine(Reload());
                SendUpdate("FIRE", "1");

            }

            MyAnime.SetTrigger("Attack1h1");

        }

    }

    public override void NetworkedStart()
    {
        SkeletonRenderer.materials[0] = SkeletonMatArray[this.Owner % 3];
        SkeletonRenderer.material = SkeletonMatArray[this.Owner % 3];
        if (IsServer)
        {
            int tstart = (this.Owner % 3) + 1;
            GameObject temp = GameObject.Find("SpawnPoint" + tstart);
            MyRig.position = temp.transform.position;
            MyRig.useGravity = true;
        }

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
            SendCommand("FIRE", "1");
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
            MyAnime.SetFloat("speedv", speed);

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

                MyAnime.SetFloat("speedv", MyRig.velocity.magnitude);

            }
            else
            {

                MyAnime.SetFloat("speedv", MyRig.angularVelocity.y);

            }

        }

    }

    public IEnumerator Reload()
    {

        yield return new WaitForSeconds(0.5f);
        CanFire = true;

    }

}