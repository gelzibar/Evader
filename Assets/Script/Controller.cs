using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller
{

    public float horizontal { get; set; }
    public bool isUpKey { get; set; }

    public void StandardInput()
    {
        horizontal = Input.GetAxis("Horizontal");

        if (Input.GetKey(KeyCode.W))
        {
            isUpKey = true;
        }
        else
        {
            isUpKey = false;
        }

    }

}