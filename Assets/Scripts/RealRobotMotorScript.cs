using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

// TODO - reorganize the motor positions (so that it is like polar cords)
// TODO - also, do the motor vectors need to be accurate?
// TODO - lower tolerance of STL export, this is way to expensive
// TODO - max thrust is - 51.4 N and 40 N
// TODO - invert mouse button

public class RealRobotMotorScript : MonoBehaviour
{
    public Rigidbody rb;
    private Vector3 force1;
    private Vector3 force2;
    private Vector3 force3;
    private Vector3 force4;
    private Vector3 force5;
    private Vector3 force6;
    private Vector3 force7;
    private Vector3 force8;
    //front left, front right, back left, back right XY motors
    //then front left, front right, back left, back right Z motors
    private Vector3 position1 = new Vector3(4.9f, 0f, 1.95f);
    private Vector3 position2 = new Vector3(4.9f, 0f, -1.95f);
    private Vector3 position3 = new Vector3(-4.9f, 0f, 1.95f);
    private Vector3 position4 = new Vector3(-4.9f, 0f, -1.95f);
    private Vector3 position5 = new Vector3(5.66f, 0f, 2.72f);
    private Vector3 position6 = new Vector3(5.66f, 0f, -2.72f);
    private Vector3 position7 = new Vector3(-5.66f, 0f, 2.72f);
    private Vector3 position8 = new Vector3(-5.66f, 0f, -2.72f);
    private List<Vector3> MovementOverrideList = new List<Vector3>();
    public RealRobotMovementController RealRobotMovementControllerScript;
    
    // uncomment this to use PID when pushed
    // public PID PIDScript;
    public bool overrideMovement = false;
    public GameObject InvertMouseButton;
    private float timeSinceLastIToggle = 0f;

    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        InvertMouseButton.SetActive(false);
        rb.useGravity = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {   
        Vector3 localposition1 = transform.TransformPoint(position1);
        Vector3 localposition2 = transform.TransformPoint(position2);
        Vector3 localposition3 = transform.TransformPoint(position3);
        Vector3 localposition4 = transform.TransformPoint(position4);
        Vector3 localposition5 = transform.TransformPoint(position5);
        Vector3 localposition6 = transform.TransformPoint(position6);
        Vector3 localposition7 = transform.TransformPoint(position7);
        Vector3 localposition8 = transform.TransformPoint(position8);

        if(overrideMovement)
        {
            MovementOverrideList = RealRobotMovementControllerScript.MovementOverride();
            force1 = MovementOverrideList[0];
            force2 = MovementOverrideList[1];
            force3 = MovementOverrideList[2];
            force4 = MovementOverrideList[3];
            force5 = MovementOverrideList[4];
            force6 = MovementOverrideList[5];
            force7 = MovementOverrideList[6];
            force8 = MovementOverrideList[7];
        }

        Vector3 localforce1 = transform.TransformDirection(force1);
        Vector3 localforce2 = transform.TransformDirection(force2);
        Vector3 localforce3 = transform.TransformDirection(force3);
        Vector3 localforce4 = transform.TransformDirection(force4);
        Vector3 localforce5 = transform.TransformDirection(force5);
        Vector3 localforce6 = transform.TransformDirection(force6);
        Vector3 localforce7 = transform.TransformDirection(force7);
        Vector3 localforce8 = transform.TransformDirection(force8);
        rb.AddForceAtPosition(localforce1, localposition1, ForceMode.Force);
        rb.AddForceAtPosition(localforce2, localposition2, ForceMode.Force);
        rb.AddForceAtPosition(localforce3, localposition3, ForceMode.Force);
        rb.AddForceAtPosition(localforce4, localposition4, ForceMode.Force);
        rb.AddForceAtPosition(localforce5, localposition5, ForceMode.Force);
        rb.AddForceAtPosition(localforce6, localposition6, ForceMode.Force);
        rb.AddForceAtPosition(localforce7, localposition7, ForceMode.Force);
        rb.AddForceAtPosition(localforce8, localposition8, ForceMode.Force);           
    }

    public void enableOverrideMovement(bool enable)
    {
        overrideMovement = enable;
        if(enable)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        RealRobotMovementControllerScript.invertMouse = false;
        RealRobotMovementControllerScript.calledFor = enable;
        InvertMouseButton.SetActive(enable);
    }

    private string targetWord = "rainbow";
    private int currentIndex = 0;
    private bool rainbowAchieved = false;

    private float duration = 1f; // Duration to transition between each color
    private List<Color> rainbowColors = new List<Color>
        {
            Color.red,
            new Color(1f, 0.5f, 0f), // Orange
            Color.yellow,
            Color.green,
            Color.blue,
            new Color(0.29f, 0f, 0.51f), // Indigo
            new Color(0.56f, 0f, 1f) // Violet
        };
    private int currentColorIndex = 0;
    private float timeElapsed = 0f;
    public Renderer objectRenderer1;
    public Renderer objectRenderer2;
    public Renderer objectRenderer3;
    public Renderer objectRenderer4;
    public Renderer objectRenderer5;

    void Update()
    {

        if(Input.GetKeyDown(KeyCode.R))
        {
            rb.position = new Vector3(0, 1, 0);
        }

        timeSinceLastIToggle += Time.deltaTime;

        if(Input.GetKeyDown(KeyCode.I) && overrideMovement && timeSinceLastIToggle > 0.2f)
        {
            timeSinceLastIToggle = 0f;
            //RealRobotMovementControllerScript.invertMouse = !RealRobotMovementControllerScript.invertMouse;
            InvertMouseButton.GetComponent<Toggle>().isOn = !InvertMouseButton.GetComponent<Toggle>().isOn;
        }

        if (currentIndex < targetWord.Length)
        {
            foreach (char c in Input.inputString)
            {
                if (c == targetWord[currentIndex])
                {
                    currentIndex++;
                    
                    if (currentIndex == targetWord.Length)
                    {
                        rainbowAchieved = !rainbowAchieved;
                        currentIndex = 0;
                        if (!rainbowAchieved)
                        {
                            objectRenderer1.material.color = Color.gray;
                            objectRenderer2.material.color = Color.gray;
                            objectRenderer3.material.color = Color.gray;
                            objectRenderer4.material.color = Color.gray;
                            objectRenderer5.material.color = Color.gray;

                            currentColorIndex = 0;
                            timeElapsed = 0f;
                        }
                    }
                }
                else
                {
                    currentIndex = 0;
                }
            }
        }
        
        if (rainbowAchieved)
        {
            timeElapsed += Time.deltaTime;

            // lerp value is a value between 0 and 1 that represents the progress of the interpolation
            float lerpValue = timeElapsed / duration;

            // Interpolate the color
            Color startColor = rainbowColors[currentColorIndex];
            Color endColor = rainbowColors[(currentColorIndex + 1) % rainbowColors.Count];
            objectRenderer1.material.color = Color.Lerp(startColor, endColor, lerpValue);
            objectRenderer2.material.color = Color.Lerp(startColor, endColor, lerpValue);
            objectRenderer3.material.color = Color.Lerp(startColor, endColor, lerpValue);
            objectRenderer4.material.color = Color.Lerp(startColor, endColor, lerpValue);
            objectRenderer5.material.color = Color.Lerp(startColor, endColor, lerpValue);

            // Move to the next color if the duration is exceeded
            if (timeElapsed >= duration)
            {
                timeElapsed = 0f;
                currentColorIndex = (currentColorIndex + 1) % rainbowColors.Count;
            }
        }
    }
}