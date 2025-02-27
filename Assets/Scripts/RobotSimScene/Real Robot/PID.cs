using UnityEngine;
using System.Collections.Generic;
public class PID : MonoBehaviour
{
    // TODO - does not work if yaw pitch and roll are not 0
    // TODO - make yaw track thing a button
    public Rigidbody rb;
    public float xSetpoint = 0f;
    public float ySetpoint = 0f;
    public float zSetpoint = 0f;
    public float rollSetpoint = 0f;
    public float pitchSetpoint = 0f;
    public float yawSetpoint = 0f;
    public float xSetpointRelative = 0f;
    public float ySetpointRelative = 0f;
    public float zSetpointRelative = 0f;
    public List<float> kValues = new List<float>() {0.5f, 0, 0.1f, 0.5f, 0, 0.1f, 0.5f, 0, 0.1f, 0.05f, 0, 0.1f, 0.05f, 0, 0.1f, 0.05f, 0, 0.1f};

    public RealRobotMotorScript motorScript;
    private List<float> location;
    private PIDHandler pidHandler = new PIDHandler();

    public bool yawTrack = false;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        location = new List<float>() {rb.transform.position.x, rb.transform.position.y, rb.transform.position.z, rb.rotation.eulerAngles.z, rb.rotation.eulerAngles.x, rb.rotation.eulerAngles.y};
        pidHandler.getMaxSpeed(motorScript.maxSpeed);
        pidHandler.getLocation(location, yawTrack);
        pidHandler.UpdateKValues(kValues);
        UpdateSetpoint(xSetpoint, ySetpoint, zSetpoint, rollSetpoint, pitchSetpoint, yawSetpoint);
    }

    public List<Vector3> getVectors()
    {
        location = new List<float>() {rb.transform.position.x, rb.transform.position.y, rb.transform.position.z, rb.rotation.eulerAngles.z, rb.rotation.eulerAngles.x, rb.rotation.eulerAngles.y};
        pidHandler.getMaxSpeed(motorScript.maxSpeed);
        pidHandler.getLocation(location, yawTrack);
        pidHandler.UpdateKValues(kValues);
        UpdateSetpoint(xSetpoint, ySetpoint, zSetpoint, rollSetpoint, pitchSetpoint, yawSetpoint);
        sendSetpoints();
        return pidHandler.Update();
    }

    public void resetAll()
    {
        pidHandler.resetAll();
    }

    public void UpdateSetpoint(float x, float y, float z, float roll, float pitch, float yaw)
    {
        if(yawTrack)
        {
            Vector3 vector = new Vector3(x, y, z);
            vector = transform.InverseTransformPoint(vector);
            xSetpointRelative = vector.x;
            ySetpointRelative = vector.y;
            zSetpointRelative = vector.z;
            rollSetpoint = roll;
            pitchSetpoint = pitch;
            yawSetpoint = yaw;
        }
        else
        {
            xSetpointRelative = x;
            ySetpointRelative = y;
            zSetpointRelative = z;
            rollSetpoint = roll;
            pitchSetpoint = pitch;
            yawSetpoint = yaw;
        }
    }

    public void sendSetpoints()
    {
        List<float> setpoints = new List<float>() {xSetpointRelative, ySetpointRelative, zSetpointRelative, rollSetpoint, pitchSetpoint, yawSetpoint};
        pidHandler.recieveSetpoints(setpoints);
    }
}

// a class for initializing the PID controllers
public class PIDController
{
    //note - the wording value just mean distance in this context

    //proportional gain
    public float Kp;
    
    // integral gain
    public float Ki;

    // derivative gain
    public float Kd;

    private float integrationStored;

    // the error from the last time step
    private float lastValue;
    private bool notFirstUpdate = false;

    // the maximum value the integral term can take, to prevent intergral windup
    private float integralSaturation = 1f;


    // dt - time step, in unity we use Time.fixedDeltaTime for this
    // currentValue - the current value of the system (current position)
    // targetValue - the desired value of the system (target position)
    public float Update(float dt, float currentValue, float targetValue)
    {
        float result;

        float error = targetValue - currentValue;

        // proportional term
        float P = Kp * error;

        // integral term
        integrationStored = Mathf.Clamp(integrationStored + (error * dt), -integralSaturation, integralSaturation);
        float I = Ki * integrationStored;

        // This check to make sure that the derivative term is not calculated on the first update, 
        // as it causes a large spike in the output since the error is very large and the lastValue is 0
        if(notFirstUpdate)
        {
            //derivative term
            float valueRateOfChange = (currentValue - lastValue) / dt;
            lastValue = currentValue;

            float D = Kd * (-valueRateOfChange);

            result = P + I + D;
        }
        else
        {
            notFirstUpdate = true;
            result = P + I;
        }

        // makes sure it cannot return an output greater than 1 or less than -1
        // if we ever change it to be more realistic, the -1f should be changed to be 40/51.4 so it conforms to the actual motor limits
        return Mathf.Clamp(result, -1f, 1f);
    }

    //this is for angles, as PIDs get confused when you want to go from 15 to 345, as it thinks it has to go all the way around
    public float UpdateAngle(float dt, float currentAngle, float targetAngle)
    {
        float result;

        float error = Mathf.DeltaAngle(currentAngle, targetAngle);

        // proportional term
        float P = -Kp * error;

        // integral term
        integrationStored = Mathf.Clamp(integrationStored + (error * dt), -integralSaturation, integralSaturation);
        float I = Ki * integrationStored;
        // This check to make sure that the derivative term is not calculated on the first update, 
        // as it causes a large spike in the output since the error is very large and the lastValue is 0
        if(notFirstUpdate)
        {
            //derivative term
            // float valueRateOfChange = Mathf.DeltaAngle(currentAngle, targetAngle) / dt;
            // lastValue = currentAngle;

            // float D = Kd * -valueRateOfChange;


            float valueRateOfChange = Mathf.DeltaAngle(lastValue, currentAngle) / dt;
            lastValue = currentAngle;

            float D = Kd * valueRateOfChange;


            result = P + I + D;
        }
        else
        {
            notFirstUpdate = true;
            result = P + I;
        }

        // makes sure it cannot return an output greater than 1 or less than -1
        // if we ever change it to be more realistic, the -1f should be changed to be 40/51.4 so it conforms to the actual motor limits
        return Mathf.Clamp(result, -1f, 1f);
    }

    // reset the controller when the pid is not in use
    public void Reset()
    {
        lastValue = 0;
        notFirstUpdate = false;
    }
}

public class PIDHandler
{
    public List<float> location = new List<float>();
    public PIDController xController = new PIDController();
    public PIDController yController = new PIDController();
    public PIDController zController = new PIDController();
    public PIDController rollController = new PIDController();
    public PIDController pitchController = new PIDController();
    public PIDController yawController = new PIDController();

    //z is forward, x is right, y is up
    private float xSetpoint;
    private float ySetpoint;
    private float zSetpoint;
    private float rollSetpoint;
    private float pitchSetpoint;
    private float yawSetpoint;

    private float xValue;
    private float yValue;
    private float zValue;
    private float rollValue;
    private float pitchValue;
    private float yawValue;

    // private float degreeFix = Mathf.Sqrt(2) / 2;

    private float maxSpeed;

    private List<Vector3> forces = new List<Vector3>() {Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero};
    public void getLocation(List<float> location, bool yawTrack)
    {
        this.location = location;

        if(yawTrack)
        {
            this.location[0] = 0;
            this.location[1] = 0;
            this.location[2] = 0;
        }
    }

    public void getMaxSpeed(float maxSpeed)
    {
        this.maxSpeed = maxSpeed;
    }

    public void Reset()
    {
        xController.Reset();
        yController.Reset();
        zController.Reset();
        rollController.Reset();
        pitchController.Reset();
        yawController.Reset();
    }

    public void recieveSetpoints(List<float> setpoints)
    {
        xSetpoint = setpoints[0];
        ySetpoint = setpoints[1];
        zSetpoint = setpoints[2];
        rollSetpoint = setpoints[3];
        pitchSetpoint = setpoints[4];
        yawSetpoint = setpoints[5];
    }

    public List<Vector3> Update()
    {
        forces = new List<Vector3>() {Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero};

        xValue = xController.Update(Time.fixedDeltaTime, location[0], xSetpoint);
        yValue = yController.Update(Time.fixedDeltaTime, location[1], ySetpoint);
        zValue = zController.Update(Time.fixedDeltaTime, location[2], zSetpoint);
        rollValue = rollController.UpdateAngle(Time.fixedDeltaTime, location[3], rollSetpoint);
        pitchValue = pitchController.UpdateAngle(Time.fixedDeltaTime, location[4], pitchSetpoint);
        yawValue = yawController.UpdateAngle(Time.fixedDeltaTime, location[5], yawSetpoint);

        forces[0] += (Vector3.forward + Vector3.right) * zValue + (Vector3.forward + Vector3.right) * xValue + (Vector3.back + Vector3.left) * yawValue;
        forces[1] += (Vector3.forward + Vector3.left) * zValue + (Vector3.back + Vector3.right) * xValue + (Vector3.forward + Vector3.left) * yawValue;
        forces[2] += (Vector3.forward + Vector3.left) * zValue + (Vector3.back + Vector3.right) * xValue + (Vector3.back + Vector3.right) * yawValue;
        forces[3] += (Vector3.forward + Vector3.right) * zValue + (Vector3.forward + Vector3.right) * xValue + (Vector3.forward + Vector3.right) * yawValue;

        forces[4] += Vector3.up * yValue + Vector3.up * pitchValue + Vector3.down * rollValue;
        forces[5] += Vector3.up * yValue + Vector3.up * pitchValue + Vector3.up * rollValue;
        forces[6] += Vector3.up * yValue + Vector3.down * pitchValue + Vector3.down * rollValue;
        forces[7] += Vector3.up * yValue + Vector3.down * pitchValue + Vector3.up * rollValue;

        //Debug.Log($"Values: zValue: {zValue}, xValue: {xValue}, yawValue: {yawValue}, yValue: {yValue}, pitchValue: {pitchValue}, rollValue: {rollValue}");

        float highestForce1 = 1f;
        float highestForce2 = 1f;

        for(int i = 0; i < 4; i++)
        {
            if(forces[i].magnitude > highestForce1)
            {
                highestForce1 = forces[i].magnitude;
            }
            if(forces[i+4].magnitude > highestForce2)
            {
                highestForce2 = forces[i+4].magnitude;
            }
        }

        forces[0] = forces[0] * (1 / highestForce1);
        forces[1] = forces[1] * (1 / highestForce1);
        forces[2] = forces[2] * (1 / highestForce1);
        forces[3] = forces[3] * (1 / highestForce1);
        forces[4] = forces[4] * (1 / highestForce2);
        forces[5] = forces[5] * (1 / highestForce2);
        forces[6] = forces[6] * (1 / highestForce2);
        forces[7] = forces[7] * (1 / highestForce2);

        //replace the bottom 4 lines divideCounter like they are in the top 4 lines divide counter but with the correct values for the bottom 4 lines
        // if we ever change it to be more realistic, the 40f should be changed to be 40/51.4 so it conforms to the actual motor limits if backwards
        forces[0] *= maxSpeed;
        forces[1] *= maxSpeed;
        forces[2] *= maxSpeed;
        forces[3] *= maxSpeed;
        forces[4] *= maxSpeed;
        forces[5] *= maxSpeed;
        forces[6] *= maxSpeed;
        forces[7] *= maxSpeed;

        return forces;
    }

    public void UpdateKValues(List<float> kValues)
    {
        xController.Kp = kValues[0];
        xController.Ki = kValues[1];
        xController.Kd = kValues[2];
        yController.Kp = kValues[3];
        yController.Ki = kValues[4];
        yController.Kd = kValues[5];
        zController.Kp = kValues[6];
        zController.Ki = kValues[7];
        zController.Kd = kValues[8];
        rollController.Kp = kValues[9];
        rollController.Ki = kValues[10];
        rollController.Kd = kValues[11];
        pitchController.Kp = kValues[12];
        pitchController.Ki = kValues[13];
        pitchController.Kd = kValues[14];
        yawController.Kp = kValues[15];
        yawController.Ki = kValues[16];
        yawController.Kd = kValues[17];
    }

    public void resetAll()
    {
        xController.Reset();
        yController.Reset();
        zController.Reset();
        rollController.Reset();
        pitchController.Reset();
        yawController.Reset();
    }
}