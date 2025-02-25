using UnityEngine;
using UnityEngine.UI;

public class PIDSim : MonoBehaviour
{
    // TODO - add button for dont get rid of integral windup or derivative kick?
    public Rigidbody rb;
    public GameObject Target;
    public bool onYAxis = false;
    public Toggle IToggle;
    PIDSimController controller = new PIDSimController();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        controller.Kp = 0f;
        controller.Ki = 0f;
        controller.Kd = 0f;
        IToggle.isOn = false;
        IToggle.gameObject.SetActive(false);
    }

    //Update is called once per frame
    void FixedUpdate()
    {
        if((controller.Kp != 0f || controller.Ki != 0f || controller.Kd != 0f) && !onYAxis)
        {
            Vector3 force = new Vector3(controller.Update(Time.fixedDeltaTime, rb.transform.position.x, 0f), 0f, 0f);
            rb.AddForce(force, ForceMode.Force);
        }
        else if((controller.Kp != 0f || controller.Ki != 0f || controller.Kd != 0f) && onYAxis)
        {
            Vector3 force = new Vector3(0f, controller.Update(Time.fixedDeltaTime, rb.transform.position.y, 5.5f), 0f);
            rb.AddForce(force, ForceMode.Force);
        }
    }

    public void enableP(bool enable)
    {
        if(enable)
        {
            controller.Kp = 1f;
        }
        else
        {
            controller.Kp = 0f;
        }
    }

    public void enableI(bool enable)
    {
        if(enable)
        {
            controller.Ki = 1f;
        }
        else
        {
            controller.Ki = 0f;
        }
    }

    public void enableD(bool enable)
    {
        if(enable)
        {
            controller.Kd = 1f;
        }
        else
        {
            controller.Kd = 0f;
        }
    }

    public void enableYAxis()
    {
        onYAxis = !onYAxis;
        if(onYAxis)
        {
            rb.transform.position = new Vector3(0f, 0.5f, 0f);
            Target.transform.position = new Vector3(2f, 5.5f, 0f);
            IToggle.isOn = false;
            IToggle.gameObject.SetActive(true);
            controller.Reset();
        }
        else
        {
            rb.transform.position = new Vector3(-8f, 2.5f, 0f);
            Target.transform.position = new Vector3(0f, 0.5f, 0f);
            IToggle.isOn = false;
            IToggle.gameObject.SetActive(false);
            controller.Reset();
        }
    }
}

// a class for initializing the PID controller
public class PIDSimController
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

    // reset the controller when the pid is not in use
    public void Reset()
    {
        lastValue = 0;
        notFirstUpdate = false;
    }
}