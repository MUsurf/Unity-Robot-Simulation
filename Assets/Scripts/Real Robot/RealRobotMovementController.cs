using System.Collections.Generic;
using NUnit.Framework.Constraints;
using UnityEngine;

public class RealRobotMovementController : MonoBehaviour
{
    public float keyboardForceMultiplier = 10;
    public float mouseForceMultiplier = 20f;
    public float yawForceMultiplier = 4f;
    public float shiftMultiplier = 4f;
    public bool invertMouse = false;
    private int invertMouseMultiplier = 1;
    public bool calledFor = false;
    private float timeSinceLastToggle = 0f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    // void Start()
    // {
        
    // }

    // Update is called once per frame
    void Update()
    {
        if(calledFor)
        {
            timeSinceLastToggle += Time.deltaTime;

            if(Input.GetKey(KeyCode.M) && timeSinceLastToggle > 0.25f)
            {
                if(Cursor.lockState == CursorLockMode.Locked)
                {
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                }
                else
                {
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                }

                timeSinceLastToggle = 0f;
            }
        }
    }

    public void invertMouseToggle(bool invertMouseBool)
    {
        invertMouse = invertMouseBool;
    }

    public List<Vector3> MovementOverride()
    {
        Vector3 force1 = Vector3.zero;
        Vector3 force2 = Vector3.zero;
        Vector3 force3 = Vector3.zero;
        Vector3 force4 = Vector3.zero;
        Vector3 force5 = Vector3.zero;
        Vector3 force6 = Vector3.zero;
        Vector3 force7 = Vector3.zero;
        Vector3 force8 = Vector3.zero;

        Vector3 mouseforce1 = Vector3.zero;
        Vector3 mouseforce2 = Vector3.zero;
        Vector3 mouseforce3 = Vector3.zero;
        Vector3 mouseforce4 = Vector3.zero;
        Vector3 mouseforce5 = Vector3.zero;
        Vector3 mouseforce6 = Vector3.zero;
        Vector3 mouseforce7 = Vector3.zero;
        Vector3 mouseforce8 = Vector3.zero;
        
        if(invertMouse)
        {
            invertMouseMultiplier = -1;
        }
        else
        {
            invertMouseMultiplier = 1;
        }

        //Debug.Log(invertMouseMultiplier);

        if(Input.GetKey(KeyCode.Escape))
        {
            if(Cursor.lockState == CursorLockMode.Locked)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }

        if(Input.GetKey(KeyCode.W))
        {
            force1 += Vector3.forward + Vector3.right;
            force2 += Vector3.forward + Vector3.left;
            force3 += Vector3.forward + Vector3.left;
            force4 += Vector3.forward + Vector3.right;
        }
        if
        (Input.GetKey(KeyCode.S))
        {

            force1 += Vector3.back + Vector3.left;
            force2 += Vector3.back + Vector3.right;
            force3 += Vector3.back + Vector3.right;
            force4 += Vector3.back + Vector3.left;

        }
        if(Input.GetKey(KeyCode.A))
        {
            force1 += Vector3.back + Vector3.left;
            force2 += Vector3.forward + Vector3.left;
            force3 += Vector3.forward + Vector3.left;
            force4 += Vector3.back + Vector3.left;
        }
        if(Input.GetKey(KeyCode.D))
        {

            force1 += Vector3.forward + Vector3.right;
            force2 += Vector3.back + Vector3.right;
            force3 += Vector3.back + Vector3.right;
            force4 += Vector3.forward + Vector3.right;

        }

        force1 = force1.normalized;
        force2 = force2.normalized;
        force3 = force3.normalized;
        force4 = force4.normalized;

        if(Input.GetKey(KeyCode.Space))
        {
            force5 += Vector3.up;
            force6 += Vector3.up;
            force7 += Vector3.up;
            force8 += Vector3.up;
        }
        if(Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.C))
        {
            force5 += Vector3.down;
            force6 += Vector3.down;
            force7 += Vector3.down;
            force8 += Vector3.down;
        }
        
        // TODO - make this go at half speed?
        
        // if(Input.GetKey(KeyCode.LeftShift))
        // {
        //     force5 *= shiftMultiplier;
        //     force6 *= shiftMultiplier;
        //     force7 *= shiftMultiplier;
        //     force8 *= shiftMultiplier;
        // }

        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        Vector3 mouseDelta = new Vector3(mouseX, mouseY, 0);

        if(Input.GetKey(KeyCode.Q))
        {
            mouseforce1 += (Vector3.back + Vector3.left) * yawForceMultiplier;
            mouseforce2 += (Vector3.forward + Vector3.left) * yawForceMultiplier;
            mouseforce3 += (Vector3.back + Vector3.right) * yawForceMultiplier;
            mouseforce4 += (Vector3.forward + Vector3.right) * yawForceMultiplier;
        }
        if(Input.GetKey(KeyCode.E))
        {
            mouseforce1 += (Vector3.forward + Vector3.right) * yawForceMultiplier;
            mouseforce2 += (Vector3.back + Vector3.right) * yawForceMultiplier;
            mouseforce3 += (Vector3.forward + Vector3.left) * yawForceMultiplier;
            mouseforce4 += (Vector3.back + Vector3.left) * yawForceMultiplier;
        }

        mouseforce1 = mouseforce1.normalized;
        mouseforce2 = mouseforce2.normalized;
        mouseforce3 = mouseforce3.normalized;
        mouseforce4 = mouseforce4.normalized;
        mouseforce5 = mouseforce5.normalized;
        mouseforce6 = mouseforce6.normalized;
        mouseforce7 = mouseforce7.normalized;
        mouseforce8 = mouseforce8.normalized;
        
        mouseDelta.y = mouseDelta.y * mouseForceMultiplier * invertMouseMultiplier / 40.00f;
        mouseDelta.x = mouseDelta.x * mouseForceMultiplier / 40.00f;

        if(mouseDelta.y > 1)
        {
            mouseDelta.y = 1;
        }
        if(mouseDelta.x > 1)
        {
            mouseDelta.x = 1;
        }
        if(mouseDelta.y < -1)
        {
            mouseDelta.y = -1;
        }
        if(mouseDelta.x < -1)
        {
            mouseDelta.x = -1;
        }

        mouseforce5 += new Vector3(0, mouseDelta.y, 0);
        mouseforce6 += new Vector3(0, mouseDelta.y, 0);
        mouseforce7 -= new Vector3(0, mouseDelta.y, 0);
        mouseforce8 -= new Vector3(0, mouseDelta.y, 0);

        mouseforce5 -= new Vector3(0, mouseDelta.x, 0);
        mouseforce6 += new Vector3(0, mouseDelta.x, 0);
        mouseforce7 -= new Vector3(0, mouseDelta.x, 0);
        mouseforce8 += new Vector3(0, mouseDelta.x, 0);

        greaterCheck(ref mouseforce5, ref mouseforce6, ref mouseforce7, ref mouseforce8);

        force1 += mouseforce1;
        force2 += mouseforce2;
        force3 += mouseforce3;
        force4 += mouseforce4;
        force5 += mouseforce5;
        force6 += mouseforce6;
        force7 += mouseforce7;
        force8 += mouseforce8;

        force1 = force1.normalized;
        force2 = force2.normalized;
        force3 = force3.normalized;
        force4 = force4.normalized;

        checkNegativeAndApply(ref force1, ref force2, ref force3, ref force4, ref force5, ref force6, ref force7, ref force8);

        Debug.Log($"force1: {force1}, force2: {force2}, force3: {force3}, force4: {force4}, force5: {force5}, force6: {force6}, force7: {force7}, force8: {force8}");

        return new List<Vector3> {force1, force2, force3, force4, force5, force6, force7, force8};
    }

    private void checkNegativeAndApply(ref Vector3 force1, ref Vector3 force2, ref Vector3 force3, ref Vector3 force4, ref Vector3 force5, ref Vector3 force6, ref Vector3 force7, ref Vector3 force8)
    {
        // TODO - maybe possible, for now just 40
        // TODO - max thrust is - 51.4 N and 40 N

        force1 = force1 * 40;
        force2 = force2 * 40;
        force3 = force3 * 40;
        force4 = force4 * 40;
        force5 = force5 * 40;
        force6 = force6 * 40;
        force7 = force7 * 40;
        force8 = force8 * 40;

        // if(force1.z < 0)
        // {
        //     force1 *= 40;
        // }
        // else
        // {
        //     force1 *= 51.4f;
        // }

        // if(force2.z < 0)
        // {
        //     force2 *= 40;
        // }
        // else
        // {
        //     force2 *= 51.4f;
        // }

        // if(force3.z > 0)
        // {
        //     force3 *= 40;
        // }
        // else
        // {
        //     force3 *= 51.4f;
        // }

        // if(force4.z > 0)
        // {
        //     force4 *= 40;
        // }
        // else
        // {
        //     force4 *= 51.4f;
        // }
        
        // if(force5.y < 0)
        // {
        //     force5 *= 40;
        // }
        // else
        // {
        //     force5 *= 51.4f;
        // }

        // if(force6.y < 0)
        // {
        //     force6 *= 40;
        // }
        // else
        // {
        //     force6 *= 51.4f;
        // }

        // if(force7.y < 0)
        // {
        //     force7 *= 40;
        // }
        // else
        // {
        //     force7 *= 51.4f;
        // }

        // if(force8.y < 0)
        // {
        //     force8 *= 40;
        // }
        // else
        // {
        //     force8 *= 51.4f;
        // }
    }

    private void greaterCheck(ref Vector3 mouseforce5, ref Vector3 mouseforce6, ref Vector3 mouseforce7, ref Vector3 mouseforce8)
    {
        if(mouseforce5.y > 1)
        {
            mouseforce5.y = 1;
        }
        else if(mouseforce5.y < -1)
        {
            mouseforce5.y = -1;
        }
        
        if(mouseforce6.y > 1)
        {
            mouseforce6.y = 1;
        }
        else if(mouseforce6.y < -1)
        {
            mouseforce6.y = -1;
        }

        if(mouseforce7.y > 1)
        {
            mouseforce7.y = 1;
        }
        else if(mouseforce7.y < -1)
        {
            mouseforce7.y = -1;
        }

        if(mouseforce8.y > 1)
        {
            mouseforce8.y = 1;
        }
        else if(mouseforce8.y < -1)
        {
            mouseforce8.y = -1;
        }
    }
}
