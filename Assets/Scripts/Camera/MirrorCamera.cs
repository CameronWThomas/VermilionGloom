using UnityEngine;

public class MirrorCamera : MonoBehaviour
{
    public Camera mainCam;
    Camera me;
    public Transform mirror;

    public Vector3 positionMultiplier = new Vector3(1, 1, -1);
    public Vector3 lookMultiplier = new Vector3(-1, 1, 1);
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        me = GetComponent<Camera>();
        mainCam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 localCamera = mirror.InverseTransformPoint(mainCam.transform.position);
        Vector3 relativePos = new Vector3(localCamera.x * positionMultiplier.x, localCamera.y * positionMultiplier.y, localCamera.z * positionMultiplier.z);
        transform.position = mirror.TransformPoint(relativePos);

        Vector3 relativeLook = new Vector3(localCamera.x * lookMultiplier.x, localCamera.y * lookMultiplier.y, localCamera.z * lookMultiplier.z);
        Vector3 lookAtMirror = mirror.TransformPoint(relativeLook);
        transform.LookAt(lookAtMirror);
    }
}
