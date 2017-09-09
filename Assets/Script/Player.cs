using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using portlysage;

public class Player : Actor, IControllable, IClimbable
{
    private Controller myCon;
    private StairSupport myClimb;
    private PortalSupport myPortal;
    private UIManager myUI;

    private bool isFaded;
    private bool isOnGround;
    private bool isFalling;
    public bool isOnStairs;
    public bool isDisabled;
    public int stairCount;

    public int coinsCollected;

    void Start()
    {
        OnStart();
    }

    void FixedUpdate()
    {
        OnFixedUpdate();
    }

    void Update()
    {
        OnUpdate();
    }

    public override void OnStart()
    {
        base.OnStart();
        myCon = new Controller();
        myClimb = new StairSupport();
        myPortal = new PortalSupport();
        myUI = GameObject.Find("UI Manager").GetComponent<UIManager>();

        Initialize();
    }

    public override void OnFixedUpdate()
    {
        Vector3 curVel = myRigidbody.velocity;

        myRigidbody.velocity = new Vector3(speed, curVel.y, curVel.z);
    }

    public override void OnUpdate()
    {
        ProcessInput();
        speed = myCon.horizontal * maxSpeed;
        isOnGround = CheckIfOnGround();
        isOnStairs = CheckIfOnStairs();
        isFalling = CheckFalling();

        myPortal.ResolvePort(this);
        CleanIgnoredCollisions();

        if (isDisabled)
        {
            myCon.Reset();
        }
    }

    public void ProcessInput()
    {
        if (!isDisabled)
        {
            myCon.StandardInput();
        }

    }

    private void Initialize()
    {
        maxSpeed = 2.5f;
        isFaded = false;
        isOnGround = false;
        isFalling = false;
        isOnStairs = false;
        isDisabled = false;
        stairCount = 0;
        coinsCollected = 0;
    }

    void OnCollisionEnter(Collision col)
    {
        // Stair conditions
        // 1) FULL STAIRS
        // Full stairs Always observe
        // - BUT May need to consider normal stairs about to ignore
        // 2) FLAT SURFACE
        // IGNORE when not pressing up
        // IGNORE when coming out from under the stairs
        // IGNORE when in fadeState
        // 3) ON STAIRS
        // IGNORE when player.y is lower than col.y
        // IGNORE when coming out from under the stairs
        // 4) FALLING
        // ALWAYS OBSERVE

        CheckStairCollision(col);

    }

    void OnCollisionStay(Collision col)
    {
        CheckPlatformCollision(col);
    }

    void OnTriggerStay(Collider col)
    {
        CheckPortalTrigger(col);
    }

    void CheckPortalTrigger(Collider col)
    {
        if (col.transform.name == "Trigger" && col.transform.parent.tag == "Stairwell" && myCon.isUpKey && !myPortal.isPorting)
        {
            GameObject target = col.transform.parent.GetComponent<Stairwell>().exit;
            myPortal.isPorting = true;
            myPortal.portTarget = target.transform.Find("Trigger").position;
        }
        else if (col.transform.name == "Trigger" && col.transform.parent.tag == "Goal" && myCon.isUpKey && !myPortal.isPorting)
        {
            Debug.Log("Boop");
            if (!col.transform.parent.GetComponent<Goal>().isLocked)
            {
                Debug.Log("Derp");
                isDisabled = true;
                myUI.celebrate = true;
            }

        }
    }

    void CheckPlatformCollision(Collision col)
    {
        if (col.gameObject.tag == "Platform")
        {
            bool willIgnore = false;

            if (this.IsAbove(col.transform.position.y) && myCon.isDownKey)
            {
                willIgnore = true;
            }
            else if (isOnStairs && !this.IsAbove(col.transform.position.y))
            {
                willIgnore = true;
            }
            else if (isFalling && isFaded)
            {
                willIgnore = true;
            }

            if (willIgnore)
            {
                IgnoreCollision(col);
            }
            else
            {
            }
        }
    }

    bool IsAbove(float target)
    {
        bool value = false;
        if (transform.position.y > target)
        {
            value = true;
        }
        return value;
    }

    void CheckStairCollision(Collision col)
    {
        if (col.gameObject.tag == "Stairs")
        {
            Vector3 colPos = col.transform.position;
            string facing = myClimb.CheckStairDirectionInName(col.transform.parent.name);
            bool willIgnore = false;

            if (isOnGround)
            {
                if (myClimb.OnGroundEvaluation(transform.position, colPos, facing, myCon.isUpKey, isFaded))
                {
                    willIgnore = true;
                }
            }
            else if (isFalling)
            {
                if (myClimb.FallEvaluation(transform.position, colPos))
                {
                    willIgnore = true;
                }

            }
            else if (isOnStairs)
            {
                if (myClimb.StairEvaluation(transform.position, colPos, facing))
                {
                    willIgnore = true;
                }
            }

            if (willIgnore)
            {
                IgnoreCollision(col);
            }
            else
            {
                IncrementStair();
            }
        }
    }

    void OnCollisionExit(Collision col)
    {
        if (col.gameObject.tag == "Stairs")
        {
            DecrementStair();
        }
    }

    void IgnoreCollision(Collision col)
    {
        Collider childCollider = col.gameObject.GetComponent<BoxCollider>().GetComponent<Collider>();
        Physics.IgnoreCollision(myCollider, childCollider, true);
        IgnoredCollisions.Add(childCollider);
        isFaded = true;
        childCollider.transform.parent.gameObject.GetComponent<AvoidObject>().Fade();
    }

    void CleanIgnoredCollisions()
    {
        foreach (Collider col in IgnoredCollisions.ToArray())
        {
            if (Mathf.Abs(transform.position.x - col.transform.position.x) > 1.2f ||
            Vector3.Distance(transform.position, col.transform.position) > 2.0f)
            {
                Physics.IgnoreCollision(myCollider, col, false);
                col.transform.parent.gameObject.GetComponent<AvoidObject>().Unfade();
                IgnoredCollisions.Remove(col);
            }
        }
        if (IgnoredCollisions.Count == 0)
        {
            isFaded = false;
        }
    }

    bool CheckIfOnGround()
    {
        bool hitReturn = false;
        RaycastHit ray;
        float radius = 0.49f;
        float offset = 0.025f;
        float height = 0.475f;
        Vector3 position = new Vector3(transform.position.x, transform.position.y - height, transform.position.z);

        int layerMask = 1 << 10;
        layerMask = ~layerMask;

        hitReturn = Physics.SphereCast(position, radius, Vector3.down, out ray, 0.015f, layerMask);

        /*        if (isOnStairs)
                {
                    hitReturn = false;
                }*/
        return hitReturn;

    }

    bool CheckIfOnStairs()
    {
        bool hitReturn = false;
        RaycastHit ray;
        float radius = 0.49f;
        float offset = 0.025f;
        float height = 0.475f;
        Vector3 position = new Vector3(transform.position.x, transform.position.y - height, transform.position.z);

        int layerMask = 1 << 10;
        hitReturn = Physics.SphereCast(position, radius, Vector3.down, out ray, 0.02f, layerMask);

        if (isOnGround)
        {
            hitReturn = false;
        }

        return hitReturn;

    }

    bool CheckFalling()
    {
        bool val = false;
        if (!isOnGround && !isOnStairs)
        {
            val = true;
        }
        return val;
    }

    public void IncrementStair()
    {
        //isOnStairs = true;
        stairCount++;
    }

    public void DecrementStair()
    {
        stairCount--;
        if (stairCount < 1)
        {
            stairCount = 0;
            //isOnStairs = false;
        }
    }
}