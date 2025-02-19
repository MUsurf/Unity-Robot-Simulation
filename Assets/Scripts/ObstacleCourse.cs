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
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        obstaclePosition = new Vector3(spaceForward, 16, width - spaceApart);
        
        for(int i = 0; i < obstacleAmount; i++)
        {
            Instantiate(obstaclePrefab, obstaclePosition, new Quaternion(0, 0, 0, 0));
            obstaclePosition.x += obstacleDistance;
            obstaclePosition.z += flipInt * width;
            flipInt *= -1;
        }

        hoopPosition = new Vector3(spaceForward, 0, spaceApart);

        for(int i = 0; i < hoopAmount; i++)
        {
            Instantiate(hoopPrefab, hoopPosition, new Quaternion(0, 0, 0, 0));
            hoopPosition.x += hoopDistance;
        }


    }
}
