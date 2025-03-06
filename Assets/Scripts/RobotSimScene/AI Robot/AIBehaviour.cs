using UnityEngine;

public class AIBehaviour : MonoBehaviour
{
    public GameObject PlayerBot;
    public GameObject AIBot;
    public int distance = 10;
    public bool aiEnabled = true;
    public bool followPlayer = true;
    public AIPID pidScript;
    public GameObject movePoint;

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

            Physics.Raycast(Vector3 origin, Vector3 direction, out RaycastHit hitInfo, float maxDistance, int layerMask, QueryTriggerInteraction queryTriggerInteraction);






            pidScript.xSetpoint = wantedPosition.x;
            pidScript.ySetpoint = wantedPosition.y;
            pidScript.zSetpoint = wantedPosition.z;

            movePoint.transform.position = wantedPosition;
        }
    }
}
