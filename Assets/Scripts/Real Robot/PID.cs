using UnityEngine;

public class PID : MonoBehaviour
{
    public Rigidbody rb;
    public float xSetpoint = 0f;
    public float ySetpoint = 0f;
    public float zSetpoint = 0f;
    public float yawSetpoint = 0f;
    public float rollSetpoint = 0f;
    public float pitchSetpoint = 0f;

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

public class PIDAngleController
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
            float valueRateOfChange = (error - lastValue) / dt;
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

    // reset the controller when the pid is not in use
    public void Reset()
    {
        lastValue = 0;
        notFirstUpdate = false;
    }


}