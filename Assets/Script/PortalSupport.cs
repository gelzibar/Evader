using UnityEngine;

public class PortalSupport
{
    public float portMax { get; set; }
    public float portTime { get; set; }
    public Vector3 portTarget { get; set; }
    public bool isPorting { get; set; }

    public LineRenderer guidebg, guidefg, referencebg, referencefg;

    public PortalSupport() {
        portMax = 0.5f;
        portTime = 0.0f;
        portTarget = Vector3.zero;
        isPorting = false;
        referencebg = GameObject.Find("GuideBG").GetComponent<LineRenderer>();
        referencefg = GameObject.Find("GuideFG").GetComponent<LineRenderer>();
    }

    public void ResolvePort(Actor player)
    {
        if (isPorting && portTime == 0.0f)
        {
            Vector3 portOrigin = player.transform.position;
            Quaternion portRotation = player.transform.rotation;
            player.myRigidbody.isKinematic = true;
            player.myCapsuleCollider.isTrigger = true;
            /*player.transform.position = portTarget;*/
            player.transform.position = new Vector3(400.0f,400.0f,400.0f);
            /*player.myRigidbody.isKinematic = false;*/
            guidebg = GameObject.Instantiate(referencebg, portOrigin, portRotation);
            guidebg.positionCount = 2;
            guidebg.SetPosition(0, portOrigin);
            guidebg.SetPosition(1, portTarget);
            guidefg = GameObject.Instantiate(referencefg, portOrigin, portRotation);
            guidefg.SetPosition(0, portOrigin);
            guidefg.SetPosition(1, portOrigin);
        }

        if (isPorting == true && portTime < portMax)
        {
            portTime += Time.deltaTime;
            Vector3 midpoint = Vector3.LerpUnclamped(guidefg.GetPosition(0), portTarget, portTime/portMax);
            guidefg.SetPosition(1, midpoint);
        }
        else if(isPorting && portTime >= portMax)
        {
            FieldPlayer(player);
            isPorting = false;
            portTime = 0.0f;
            portTarget = Vector3.zero;
            GameObject.Destroy(guidefg.gameObject, 0.1f);
            GameObject.Destroy(guidebg.gameObject, 0.1f);
        }
    }

    public void FieldPlayer(Actor player) {
        player.myRigidbody.isKinematic = false;
        player.myCapsuleCollider.isTrigger = false;
        player.transform.position = portTarget;
    }
}