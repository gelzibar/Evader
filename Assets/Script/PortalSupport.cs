using UnityEngine;

public class PortalSupport
{
    public float portMax { get; set; }
    public float portTime { get; set; }
    public Vector3 portTarget { get; set; }
    public bool isPorting { get; set; }

    public PortalSupport() {
        portMax = 0.5f;
        portTime = 0.0f;
        portTarget = Vector3.zero;
        isPorting = false;
    }

    public void ResolvePort(Actor player)
    {
        if (isPorting && portTime == 0.0f)
        {
            player.myRigidbody.isKinematic = true;
            player.transform.position = portTarget;
            player.myRigidbody.isKinematic = false;
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
}