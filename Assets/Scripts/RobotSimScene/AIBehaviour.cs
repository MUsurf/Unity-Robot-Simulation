using UnityEngine;

public class AIBehaviour : MonoBehaviour
{
    public GameObject PlayerBot;
    public GameObject AIBot;
    public int distance = 10;
    public bool aiEnabled = false;
    public PID pidScript;

    // Update is called once per frame
    void Update()
    {
        if(aiEnabled)
        {
            Vector3 directionVector = AIBot.transform.position - PlayerBot.transform.position;
            Vector3 wantedPosition;

            wantedPosition = PlayerBot.transform.TransformPoint(directionVector.normalized * distance);
            
            pidScript.xSetpoint = wantedPosition.x;
            pidScript.ySetpoint = wantedPosition.y;
            pidScript.zSetpoint = wantedPosition.z;
        }
    }
}
