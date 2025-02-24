using UnityEditor.Callbacks;
using UnityEngine;

public class ObstacleCourse : MonoBehaviour
{
    public GameObject obstaclePrefab;
    public int obstacleDistance = 40;
    public int width = 20;
    public int obstacleAmount = 20;
    private Vector3 obstaclePosition;
    public GameObject hoopPrefab;
    public int hoopDistance = 120;
    public int hoopAmount = 5;
    private Vector3 hoopPosition;
    public int spaceApart = 50;
    public int spaceForward = 60;
    private int flipInt = 1;


    public PID PIDScript;
    public GameObject obstacleCourseSphere;
    public GameObject realRobot;
    private bool runCourse = false;
    private Vector3 spherePosition;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        obstacleCourseSphere.SetActive(false);
        obstaclePosition = new Vector3(spaceApart - width, 16, spaceForward);
        
        for(int i = 0; i < obstacleAmount; i++)
        {
            Instantiate(obstaclePrefab, obstaclePosition, new Quaternion(0, 0, 0, 0));
            obstaclePosition.z += obstacleDistance;
            obstaclePosition.x += flipInt * width;
            flipInt *= -1;
        }

        hoopPosition = new Vector3(-spaceApart, 0, spaceForward);

        for(int i = 0; i < hoopAmount; i++)
        {
            Instantiate(hoopPrefab, hoopPosition, Quaternion.Euler(0, 90, 0));
            hoopPosition.z += hoopDistance;
        }

        spherePosition = new Vector3(-spaceApart, 35, spaceForward);
    }

    void Update()
    {
        if(runCourse)
        {
            PIDScript.xSetpoint = spherePosition.x;
            PIDScript.ySetpoint = spherePosition.y;
            PIDScript.zSetpoint = spherePosition.z;

            if(Vector3.Distance(realRobot.transform.position, spherePosition) < 3)
            {
                spherePosition.z += hoopDistance;
                obstacleCourseSphere.transform.position = spherePosition;
            }
        }
    }

    public void RunCourse()
    {
        runCourse = !runCourse;
        obstacleCourseSphere.SetActive(runCourse);
        obstacleCourseSphere.transform.position = spherePosition;
    }
}
