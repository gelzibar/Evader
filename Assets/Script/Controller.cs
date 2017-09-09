using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller
{

    public float horizontal;
    public bool isUpKey;
    public bool isDownKey;

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

        if (Input.GetKey(KeyCode.S))
        {
            isDownKey = true;
        }
        else
        {
            isDownKey = false;
        }
    }

    public void Initialize() {
        horizontal = 0.0f;
        isUpKey = false;
        isDownKey = false;
    }

    public void Reset() {
        Initialize();
    }

}