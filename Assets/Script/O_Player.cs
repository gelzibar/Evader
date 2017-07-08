using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class O_Player : Actor
{

    private bool isUpPressed, isDownPressed, isOnStairs, isOnGround, isPorting;
    private bool isFacingRight;
    private int stairCount;
    private bool isRightPressed, isLeftPressed;
    private bool fadeState;
    private bool dropState;
    private float dropTime, dropDistTotal, dropDistStart;
    private const float dropMax = 0.3f;
    private const float portMax = 0.5f;
    private float portTime;
    private Vector3 portTarget;

    private float height = 1.95f;

    public bool isDead;

    void Start()
    {
        base.OnStart();

        Initialize();
    }

    void FixedUpdate()
    {
        OnFixedUpdate();
    }

    void Update()
    {
        OnUpdate();
    }

    public override void OnFixedUpdate()
    {
        Vector3 curPos = myRigidbody.position;
        Vector3 curVel = myRigidbody.velocity;
        float speed = 0.0f;
        float maxSpeed = 2.5f;
        if (isLeftPressed == true)
        {
            speed = maxSpeed * -1;
        }
        else if (isRightPressed == true)
        {
            speed = maxSpeed;
        }

        if (isLeftPressed || isRightPressed)
        {
            myRigidbody.velocity = new Vector3(speed, curVel.y, curVel.z);
        }
    }

    public override void OnUpdate()
    {
        PlayerInput();
        isFacingRight = DetermineFacing();
        isOnGround = CheckIfOnGround();

        ResolveDropThrough();
        ResolvePort();

        if (isDead)
        {
            Respawn();
        }

        CleanIgnoredCollisions();

        if (stairCount < 0)
        {
            stairCount = 0;
        }
    }

    void CleanIgnoredCollisions()
    {
        foreach (Collider col in IgnoredCollisions.ToArray())
        {
            if (Mathf.Abs(transform.position.x - col.transform.position.x) > 1.2f)
            {
                Physics.IgnoreCollision(myCollider, col, false);
                col.transform.parent.gameObject.GetComponent<Stair>().Unfade();
                IgnoredCollisions.Remove(col);
            }
        }
        if (IgnoredCollisions.Count == 0)
        {
            fadeState = false;
        }
    }

    void ResolveDropThrough()
    {
        if (isDownPressed == true && dropState == false || isOnStairs == true &&
        dropState == false && !isUpPressed)
        {
            SetDropThrough(true);
            dropTime = 0.0f;
            dropDistStart = transform.position.y;
        }

        if (dropState == true && (dropDistStart - transform.position.y) > 2.0f ||
            dropState == true && isOnGround == true && dropTime > dropMax)
        {
            //			dropTime += Time.deltaTime;
            //			dropDistTotal = dropDistCur - transform.position.y;
            SetDropThrough(false);
        }
        else
        {
            dropTime += Time.deltaTime;
        }
    }

    void ResolvePort()
    {
        if (isPorting == true && portTime == 0.0f)
        {
            myRigidbody.isKinematic = true;
            transform.position = portTarget;
            myRigidbody.isKinematic = false;
        }

        if (isPorting == true && portTime < portMax)
        {
            portTime += Time.deltaTime;
        }
        else
        {
            isPorting = false;
            portTime = 0.0f;
            portTarget = Vector3.zero;
        }
    }

    void SetDropThrough(bool setting)
    {
        Physics.IgnoreLayerCollision(8, 9, setting);
        dropState = setting;
    }

    void PlayerInput()
    {
        Vector3 curPos = transform.position;
        if (Input.GetKey(KeyCode.A))
        {
            isLeftPressed = true;
        }
        else
        {
            isLeftPressed = false;
        }
        if (Input.GetKey(KeyCode.D))
        {
            isRightPressed = true;
        }
        else
        {
            isRightPressed = false;
        }

        if (Input.GetKey(KeyCode.W))
        {
            isUpPressed = true;
        }
        else
        {
            isUpPressed = false;
        }
        if (Input.GetKey(KeyCode.S))
        {
            isDownPressed = true;
        }
        else
        {
            isDownPressed = false;
        }
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "Stairs")
        {
            string facing = CheckStairDirectionInName(col.transform.parent.name);
            bool fullStairs = CheckStairTypeByName(col.transform.parent.name);
            Vector3 colPos = col.transform.position;

            if (!isOnStairs && isOnGround && !fullStairs)
            {
                if (RightEvaluation(facing, colPos) ||
                    LeftEvaluation(facing, colPos) ||
                    IgnoreEvaluation(facing, colPos) ||
                    FadeEvaluation(colPos))
                {

                    Collider childCollider = col.gameObject.GetComponent<CapsuleCollider>().GetComponent<Collider>();
                    Physics.IgnoreCollision(myCollider, childCollider, true);
                    childCollider.transform.parent.gameObject.GetComponent<Stair>().Fade();
                    IgnoredCollisions.Add(childCollider);
                    fadeState = true;
                }
                else
                {
                    isOnStairs = true;
                    stairCount++;
                }
            }
            else if (isOnStairs)
            {
                //				if (transform.position.y < col.transform.position.y) {
                //					Physics.IgnoreCollision (myCollider, col.collider, true);
                //					col.transform.parent.gameObject.GetComponent<Stair> ().Fade ();
                //					IgnoredCollisions.Add (col);
                //					fadeState = true;
                //
                //				} else if (isFacingRight && facing == "Left") {
                if (isFacingRight && facing == "left")
                {
                    Physics.IgnoreCollision(myCollider, col.collider, true);
                    col.transform.parent.gameObject.GetComponent<Stair>().Fade();
                    IgnoredCollisions.Add(col.collider);
                    fadeState = true;
                }
                else if (!isFacingRight && facing == "right")
                {
                    Physics.IgnoreCollision(myCollider, col.collider, true);
                    col.transform.parent.gameObject.GetComponent<Stair>().Fade();
                    IgnoredCollisions.Add(col.collider);
                    fadeState = true;
                }
                else
                {
                    isOnStairs = true;
                    stairCount++;
                }
            }
            else
            {
                isOnStairs = true;
                stairCount++;
            }
        }
    }

    bool RightEvaluation(string facing, Vector3 colPos)
    {
        bool value = false;
        if (facing == "right" && transform.position.x > colPos.x && Mathf.Abs(transform.position.y - colPos.y) < height / 2.0f)
        {
            value = true;
        }
        return value;
    }

    bool LeftEvaluation(string facing, Vector3 colPos)
    {
        bool value = false;
        // abs compare val of 0.25f
        if (facing == "left" && transform.position.x < colPos.x && Mathf.Abs(transform.position.y - colPos.y) < height / 2.0f)
        {
            value = true;
        }
        return value;
    }

    bool IgnoreEvaluation(string facing, Vector3 colPos)
    {
        // Mathf.Abs(transform.position.y - colPos.y) < height/2.0f : used to keep stairs from being ignored when above.
        // Fails when falling onto the stairs
        bool value = false;
        //		if (!isUpPressed && transform.position.y - height / 2.0f < colPos.y - 0.45f) {
        //		Debug.Log(transform.position.y - colPos.y < height/2.0f);
        if (!isUpPressed && Mathf.Abs(transform.position.y - colPos.y) < height / 2.0f &&
            transform.position.y - height / 2.0f < colPos.y - 0.45f)
        {
            value = true;
        }
        return value;
    }

    bool FadeEvaluation(Vector3 colPos)
    {
        bool value = false;
        if (fadeState && Mathf.Abs(transform.position.y - colPos.y) < height / 2.0f)
        {
            value = true;
        }
        return value;
    }

    string CheckStairDirectionInName(string name)
    {
        string facing = "blank";
        if (name.Contains("Right"))
        {
            facing = "right";
        }
        else if (name.Contains("Left"))
        {
            facing = "left";
        }

        return facing;
    }

    bool CheckStairTypeByName(string name)
    {
        bool fullStairs = false;
        if (name.Contains("Full"))
        {
            fullStairs = true;
        }
        return fullStairs;
    }

    void OnCollisionStay(Collision col)
    {
        if (col.gameObject.tag == "Stairs")
        {
            //				isOnStairs = true;
        }
    }

    void OnCollisionExit(Collision col)
    {
        if (col.gameObject.tag == "Stairs")
        {
            stairCount--;
            if (stairCount < 1)
            {
                isOnStairs = false;
            }
        }
    }

    void OnTriggerStay(Collider col)
    {
        if (col.transform.name == "Trigger" && col.transform.parent.tag == "Stairwell" && isUpPressed && !isPorting)
        {
            GameObject target = col.transform.parent.GetComponent<Stairwell>().exit;
            //			myRigidbody.isKinematic = true;
            //			Depth.SetActive (false);
            //			transform.position = target.transform.FindChild ("Spawn").position;
            //			myRigidbody.isKinematic = false;
            isPorting = true;
            portTarget = target.transform.Find("Trigger").position;
        }
    }

    void StairEntryCheck()
    {

        // Concern: Depth activation doesn't care about what direction the character is coming from. If coming from the  "back" of the stairs
        // then weird collision things may occur. Additionally, when those don't occur, the character "hops" over the first stair-- not game breaking, but distracting

        // Conditions:
        // If depth sensor is on
        // And depth sensor is colliding with a stair
        // Get Depth Sensor Position and Stair Position
    }

    bool CheckIfOnGround()
    {
        bool hitReturn = false;
        RaycastHit ray;
        float radius = 0.49f;
        Vector3 position = new Vector3(transform.position.x, transform.position.y - height / 4.0f, transform.position.z);
        Vector3 tempPos1 = new Vector3(transform.position.x, transform.position.y - radius - height / 4.0f, transform.position.z);
        Vector3 tempPos2 = new Vector3(transform.position.x, transform.position.y - radius - height / 4.0f - height / 3.0f, transform.position.z);
        Debug.DrawLine(tempPos1, tempPos2);

        int layerMask = 1 << 8;
        layerMask = ~layerMask;

        hitReturn = Physics.SphereCast(position, radius, Vector3.down, out ray, height / 3.0f, layerMask);
        return hitReturn;
    }

    void Respawn()
    {
        Transform respawnZone = GameObject.Find("Spawn").transform;
        myRigidbody.isKinematic = true;
        transform.position = respawnZone.transform.position;
        myRigidbody.isKinematic = false;
        Initialize();
    }

    bool DetermineFacing()
    {
        bool faceRight = false;
        if (myRigidbody.velocity.x > 0.0f)
        {
            faceRight = true;
        }
        return faceRight;
    }

    /// <summary>
    /// Resets all basic class members to default settings. </summary>
    void Initialize()
    {
        isUpPressed = false;
        isOnStairs = false;
        isOnGround = false;
        isDownPressed = false;
        isRightPressed = false;
        isLeftPressed = false;
        isPorting = false;
        fadeState = false;
        isDead = false;
        isFacingRight = true;
        stairCount = 0;
        portTime = 0.0f;
    }

}





