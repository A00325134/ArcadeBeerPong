using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class OpenBook : MonoBehaviour
{
    public GameObject Cover;
    public HingeJoint myHinge;

    void Start()
    {
        myHinge = Cover.GetComponent<HingeJoint>();
        myHinge.useMotor = false;
    }

    public void Open()
    {
        myHinge.useMotor = true;
        Debug.Log("Using");
    }
}