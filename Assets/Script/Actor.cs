using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class Actor : MonoBehaviour
{

    public Rigidbody myRigidbody { get; set; }
    protected CapsuleCollider myCapsuleCollider;
    protected Collider myCollider;

    protected List<Collider> IgnoredCollisions;

    protected float speed { get; set; }
    protected float maxSpeed { get; set; }

    /*private float height = 1.95f;*/

    // Use this for initialization
    void Start()
    {
        OnStart();

    }

    // Update is called once per frame
    void Update()
    {
    }

    public virtual void OnStart()
    {
        myRigidbody = GetComponent<Rigidbody>();
        myRigidbody.velocity = Vector3.zero;
        myRigidbody.angularVelocity = Vector3.zero;

        myCapsuleCollider = transform.GetComponent<CapsuleCollider>();
        myCollider = myCapsuleCollider.GetComponent<Collider>();

        IgnoredCollisions = new List<Collider>();
    }

    public virtual void OnFixedUpdate()
    {
    }

    public virtual void OnUpdate() { }
}
