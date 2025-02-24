using UnityEngine;
using System.Collections.Generic;

public class PID : MonoBehaviour
{
    public Rigidbody rb;
    public float xSetpoint = 0f;
    public float ySetpoint = 0f;
    public float zSetpoint = 0f;
    public float yawSetpoint = 0f;
    public float rollSetpoint = 0f;
    public float pitchSetpoint = 0f;
    public List<float> kValues = new List<float>() {0.5f, 0, 0.1f, 0.5f, 0, 0.1f, 0.5f, 0, 0.1f, 0.05f, 0, 0.1f, 0.05f, 0, 0.1f, 0.05f, 0, 0.1f};

    private List<float> location;
    private PIDHandler pidHandler = new PIDHandler();


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        location = new List<float>() {rb.position.x, rb.position.y, rb.position.z, rb.rotation.eulerAngles.x, rb.rotation.eulerAngles.y, rb.rotation.eulerAngles.z};
        pidHandler.getLocation(location);
        pidHandler.UpdateKValues(kValues);
        pidHandler.UpdateSetpoint(xSetpoint, ySetpoint, zSetpoint, yawSetpoint, rollSetpoint, pitchSetpoint);
    }

    public List<Vector3> getVectors()
    {
        location = new List<float>() {rb.position.x, rb.position.y, rb.position.z, rb.rotation.eulerAngles.x, rb.rotation.eulerAngles.y, rb.rotation.eulerAngles.z};
        pidHandler.getLocation(location);
        pidHandler.UpdateKValues(kValues);
        pidHandler.UpdateSetpoint(xSetpoint, ySetpoint, zSetpoint, yawSetpoint, rollSetpoint, pitchSetpoint);
        return pidHandler.Update();
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

    //this is for angles, as PIDs get confused when you want to go from 15 to 345, as it thinks it has to go all the way around
    public float UpdateAngle(float dt, float currentAngle, float targetAngle)
    {
        float result;

        float error = angleDifference(targetAngle, currentAngle);

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
            float valueRateOfChange = angleDifference(currentAngle, lastValue) / dt;
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

    float angleDifference(float a, float b)
    {
        return (a - b + 540) % 360 - 180;
    }
}

public class PIDHandler
{
    public List<float> location = new List<float>();
    public PIDController xController = new PIDController();
    public PIDController yController = new PIDController();
    public PIDController zController = new PIDController();
    public PIDController yawController = new PIDController();
    public PIDController rollController = new PIDController();
    public PIDController pitchController = new PIDController();

    //z is forward, x is right, y is up
    private float xSetpoint;
    private float ySetpoint;
    private float zSetpoint;
    private float yawSetpoint;
    private float rollSetpoint;
    private float pitchSetpoint;

    private float xValue;
    private float yValue;
    private float zValue;
    private float yawValue;
    private float rollValue;
    private float pitchValue;

    private float divideCounter = 0f;

    private List<Vector3> forces = new List<Vector3>() {Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero};
    public void getLocation(List<float> location)
    {
        this.location = location;
    }

    public void Reset()
    {
        xController.Reset();
        yController.Reset();
        zController.Reset();
        yawController.Reset();
        rollController.Reset();
        pitchController.Reset();
    }

    public List<Vector3> Update()
    {
        forces = new List<Vector3>() {Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero};

        xValue = xController.Update(Time.fixedDeltaTime, location[0], xSetpoint);
        yValue = yController.Update(Time.fixedDeltaTime, location[1], ySetpoint);
        zValue = zController.Update(Time.fixedDeltaTime, location[2], zSetpoint);
        yawValue = yawController.UpdateAngle(Time.fixedDeltaTime, location[3], yawSetpoint);
        rollValue = rollController.UpdateAngle(Time.fixedDeltaTime, location[4], rollSetpoint);
        pitchValue = pitchController.UpdateAngle(Time.fixedDeltaTime, location[5], pitchSetpoint);

        if(zValue != 0)
        {
            divideCounter++;
        }
        if(xValue != 0)
        {
            divideCounter++;
        }
        if(yawValue != 0)
        {
            divideCounter++;
        }

        if(divideCounter == 0)
        {
            forces[0] = Vector3.zero;
            forces[1] = Vector3.zero;
            forces[2] = Vector3.zero;
            forces[3] = Vector3.zero;
        }
        else 
        {
            forces[0] += (Vector3.forward + Vector3.right) * (zValue / divideCounter) + (Vector3.forward + Vector3.right) * (xValue / divideCounter) + (Vector3.back + Vector3.left) * (yawValue / divideCounter);
            forces[1] += (Vector3.forward + Vector3.left) * (zValue / divideCounter) + (Vector3.back + Vector3.right) * (xValue / divideCounter) + (Vector3.forward + Vector3.left) * (yawValue / divideCounter);
            forces[2] += (Vector3.forward + Vector3.left) * (zValue / divideCounter) + (Vector3.back + Vector3.right) * (xValue / divideCounter) + (Vector3.back + Vector3.right) * (yawValue / divideCounter);
            forces[3] += (Vector3.forward + Vector3.right) * (zValue / divideCounter) + (Vector3.forward + Vector3.right) * (xValue / divideCounter) + (Vector3.forward + Vector3.right) * (yawValue / divideCounter);
        }

        divideCounter = 0f;

        if(yValue != 0)
        {
            divideCounter++;
        }
        if(rollValue != 0)
        {
            divideCounter++;
        }
        if(pitchValue != 0)
        {
            divideCounter++;
        }

        if(divideCounter == 0)
        {
            forces[4] = Vector3.zero;
            forces[5] = Vector3.zero;
            forces[6] = Vector3.zero;
            forces[7] = Vector3.zero;
        }
        else
        {
            forces[4] += Vector3.up * (yValue / divideCounter) + Vector3.up * (rollValue / divideCounter) + Vector3.down * (pitchValue / divideCounter);
            forces[5] += Vector3.up * (yValue / divideCounter) + Vector3.up * (rollValue / divideCounter) + Vector3.up * (pitchValue / divideCounter);
            forces[6] += Vector3.up * (yValue / divideCounter) + Vector3.down * (rollValue / divideCounter) + Vector3.down * (pitchValue / divideCounter);
            forces[7] += Vector3.up * (yValue / divideCounter) + Vector3.down * (rollValue / divideCounter) + Vector3.up * (pitchValue / divideCounter);
        }

        // if we ever change it to be more realistic, the 40f should be changed to be 40/51.4 so it conforms to the actual motor limits if backwards
        forces[0] *= 40f;
        forces[1] *= 40f;
        forces[2] *= 40f;
        forces[3] *= 40f;
        forces[4] *= 40f;
        forces[5] *= 40f;
        forces[6] *= 40f;
        forces[7] *= 40f;

        divideCounter = 0f;
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
        yawController.Kp = kValues[9];
        yawController.Ki = kValues[10];
        yawController.Kd = kValues[11];
        rollController.Kp = kValues[12];
        rollController.Ki = kValues[13];
        rollController.Kd = kValues[14];
        pitchController.Kp = kValues[15];
        pitchController.Ki = kValues[16];
        pitchController.Kd = kValues[17];
    }

    public void UpdateSetpoint(float x, float y, float z, float yaw, float roll, float pitch)
    {
        xSetpoint = x;
        ySetpoint = y;
        zSetpoint = z;
        yawSetpoint = yaw;
        rollSetpoint = roll;
        pitchSetpoint = pitch;
    }
}