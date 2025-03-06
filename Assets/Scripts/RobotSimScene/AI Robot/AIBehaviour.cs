using UnityEngine;

public class AIBehaviour : MonoBehaviour
{
    public GameObject PlayerBot;
    public GameObject AIBot;
    public int distance = 10;
    public int rayDistance = 5;
    public bool aiEnabled = true;
    public bool followPlayer = true;
    public AIPID pidScript;
    public GameObject movePoint;
    public bool avoidObstacles = true;
    private Vector3 frontVector = new Vector3(-3.4f, 1.5f, 6.2f);
    

    // Update is called once per frame
    void FixedUpdate()
    {
        if(aiEnabled)
        {
            Vector3 directionVector = AIBot.transform.position - PlayerBot.transform.position;

            if(!followPlayer && directionVector.magnitude > distance)
            {
                return;
            }

            Vector3 wantedPosition;

            wantedPosition = PlayerBot.transform.position + directionVector.normalized * distance;

            Debug.Log($"wantedPosition: {wantedPosition:F10}, directionVector: {directionVector.normalized:F10}");

            if(avoidObstacles)
            {
                Vector3 modifiedVector = frontVector;

                float closestHit = rayDistance + 1;

                bool hit;

                int flip = 1;

                // flip = 0;
                // while(flip != -1)
                // {
                //     if(flip == 0)
                //     {
                //         flip = 1;
                //     }
                //     else
                //     {
                //         flip = -1;
                //     }

                //     frontVector.y *= flip;
                // }

                for(int i = 0; i < 3; i++)
                {
                    modifiedVector.y = frontVector.y - i * 1.5f;
                    for(int j = 0; j < 3; j++)
                    {
                        modifiedVector.x = frontVector.x + j * 3.4f;
                        hit = Physics.Raycast(modifiedVector + AIBot.transform.position, Vector3.forward * flip, out RaycastHit hitInfo, rayDistance, 0);

                        Debug.DrawRay(modifiedVector + AIBot.transform.position, Vector3.forward * rayDistance * flip, Color.red, 0);

                        if(hit && hitInfo.distance < closestHit)
                        {
                            closestHit = hitInfo.distance;
                        }
                    }
                }

                if(closestHit != rayDistance + 1)
                {
                    wantedPosition.z = -(rayDistance - closestHit) + AIBot.transform.position.z;
                }
            

                //-1.5 to 1.5 y

                //-6.2 to 6.2 z

                //-3.4 to 3.4 x
            }



            pidScript.xSetpoint = wantedPosition.x;
            pidScript.ySetpoint = wantedPosition.y;
            pidScript.zSetpoint = wantedPosition.z;

            movePoint.transform.position = wantedPosition;
        }
    }
}
