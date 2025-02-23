using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;

//TODO - this needs to be redone

public class RealRobotPID : MonoBehaviour
{
    public Rigidbody rb;
    public float xSetpoint = 0f;
    public float ySetpoint = 0f;
    public float zSetpoint = 0f;
    public float yawSetpoint = 0f;
    public float rollSetpoint = 0f;
    public float pitchSetpoint = 0f;
}

public class RealRobotPIDController
{
    //proportional gain
    public float Kp;
    
    //integral gain
    public float Ki;

    //derivative gain
    public float Kd;


    public float Update(float dt, float currentValue, float targetValue)
    {
        float error = targetValue - currentValue;

        //proportional term
        float P = Kp * error;

        


        return 0;
    }
}