using UnityEngine;

public class FollowCam : MonoBehaviour
{
    /*
     * 
     * This implementation is really simple, feel free to modify
     * 
     * 
     * In the future it would be good to lerp to its position, 
     * and potentially move outwards a little towards the cursor
     * 
     */

    public Transform followTarget;
    private Vector3 zoomDirection;

    //TODO: implement
    [Header("Zoom Settings")]
    private float targetDistance;
    public Vector2 zoomLimits = new Vector2(5, 20);

    public Vector3 InitialOffset { get; private set; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InitialOffset = transform.position - followTarget.position;
        zoomDirection = followTarget.position - transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = followTarget.position + InitialOffset;
    }
}
