using UnityEngine;

public class StairSupport
{
    public float compareDistance { get; set; }

    public StairSupport()
    {
        compareDistance = 1.0f;
    }

    public bool FallEvaluation(Vector3 myPos, Vector3 colPos) {
        bool status = false;
        if (HeightEvaluation(myPos, colPos))
        {
            status = true;
        }
        return status;
    }

    public bool StairEvaluation(Vector3 myPos, Vector3 colPos, string facing)
    {
        bool status = false;
        if (RightClimbEvaluation(facing, myPos, colPos) || LeftClimbEvaluation(facing, myPos, colPos) || HeightEvaluation(myPos, colPos))
        {
            status = true;
        }
        return status;
    }

    public bool OnGroundEvaluation(Vector3 myPos, Vector3 colPos, string facing, bool upKey, bool fade)
    {
        bool status = false;
        if (RightEvaluation(facing, myPos, colPos) || LeftEvaluation(facing, myPos, colPos) ||
            IgnoreEvaluation(upKey, myPos, colPos) || FadeEvaluation(fade, myPos, colPos))
        {
            status = true;
        }
        return status;

    }

    public string CheckStairDirectionInName(string name)
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

    bool RightEvaluation(string facing, Vector3 myPos, Vector3 colPos)
    {
        bool value = false;
/*        if (facing == "right" && myPos.x > colPos.x && Mathf.Abs(myPos.y - colPos.y) < compareDistance)
*/        if (facing == "right" && myPos.x > colPos.x && Mathf.Abs(myPos.y - 0.474f - colPos.y + 0.5f) < compareDistance)
        {
            value = true;
        }
        Debug.Log("Right: " + value);
        return value;
    }

    bool LeftEvaluation(string facing, Vector3 myPos, Vector3 colPos)
    {
        bool value = false;
        // abs compare val of 0.25f
/*        if (facing == "left" && myPos.x < colPos.x && Mathf.Abs(myPos.y - colPos.y) < compareDistance)
*/        if (facing == "left" && myPos.x < colPos.x && Mathf.Abs(myPos.y - 0.474f - colPos.y + 0.5f) < compareDistance)
        {
            value = true;
        }
        Debug.Log("Left: " + value);
        return value;
    }

    bool RightClimbEvaluation(string facing, Vector3 myPos, Vector3 colPos)
    {
        bool value = false;
        if (facing == "right" && myPos.y < colPos.y)
        {
            value = true;
        }
        return value;
    }

    bool LeftClimbEvaluation(string facing, Vector3 myPos, Vector3 colPos)
    {
        bool value = false;
        // abs compare val of 0.25f
        if (facing == "left" && myPos.y < colPos.y)
        {
            value = true;
        }
        return value;
    }

    bool IgnoreEvaluation(bool upKey, Vector3 myPos, Vector3 colPos)
    {
        // Mathf.Abs(transform.position.y - colPos.y) < height/2.0f : used to keep stairs from being ignored when above.
        // Fails when falling onto the stairs
        bool value = false;
        //		if (!isUpPressed && transform.position.y - height / 2.0f < colPos.y - 0.45f) {
        //		Debug.Log(transform.position.y - colPos.y < height/2.0f);
        /*if (!upKey && Mathf.Abs(myPos.y - colPos.y) < compareDistance &&
            myPos.y - compareDistance < colPos.y - 0.45f)*/
        /*if (!upKey && Mathf.Abs(myPos.y - colPos.y) < compareDistance)*/
        if (!upKey && Mathf.Abs(myPos.y - 0.474f - colPos.y + 0.5f) < compareDistance)
        {
            value = true;
        }
        Debug.Log("Ignore: " + value);
        return value;
    }

    bool FadeEvaluation(bool fade, Vector3 myPos, Vector3 colPos)
    {
        bool value = false;
        if (fade && Mathf.Abs(myPos.y - colPos.y) < compareDistance)
        {
            value = true;
        }
        return value;
    }

    bool HeightEvaluation(Vector3 myPos,Vector3 colPos) {
        bool value = false;
        if(myPos.y < colPos.y) {
            value = true;
        }
        return value;
    }

}