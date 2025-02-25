using UnityEngine;

public class ArrowHeadMover : MonoBehaviour
{
    public GameObject arrowPosition;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transform.position = arrowPosition.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = arrowPosition.transform.position;
    }
}
