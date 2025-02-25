using UnityEngine;

public class ArrowCreator : MonoBehaviour
{
    // TODO - for every scale of 1, add 0.5 to x positon on arrow
    // TODO - every fixed frame set the position of the arrow to the position of the arrowposition thingy
    public GameObject cube;
    public GameObject AccelerationArrow;
    public PIDSim pidSimScript;

    Arrow accelRay;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        accelRay = new Arrow(AccelerationArrow, cube, new Vector3(0, 0, 0));
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        accelRay.updatePosition();
    }
}

public class Arrow
{
    public GameObject arrow;
    public GameObject cube;
    public Vector3 position;
    public Vector3 scale;
    public Vector3 rotation;
    public Arrow(GameObject arrow, GameObject cube, Vector3 position)
    {
        this.arrow = arrow;
        this.cube = cube;
        this.position = position;
    }

    public void updatePosition()
    {
        arrow.transform.position = cube.transform.position + position;
    }
}