using UnityEngine;

public class CutoutObject : MonoBehaviour
{
    [SerializeField]
    private Transform targetObject;

    [SerializeField]
    private LayerMask cutoutMatLayerMask;

    [SerializeField]
    private float cutoutSize = 0.1f;
    [SerializeField]
    private float falloffSize = 0.05f;

    Camera mainCam;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        mainCam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 cutoutPos = mainCam.WorldToScreenPoint(targetObject.position);
        cutoutPos.y /= (Screen.width / Screen.height);

        Vector3 offset = targetObject.position - transform.position;
        RaycastHit[] hitObjects = Physics.RaycastAll(transform.position, offset, offset.magnitude, cutoutMatLayerMask);

        for (int i = 0; i < hitObjects.Length; i++)
        {
            Material[] materials = hitObjects[i].collider.gameObject.GetComponent<MeshRenderer>().materials;

            for(int m = 0; m < materials.Length; ++m)
            {
                
                string material_name = materials[m].name;
                if (material_name.Contains("RoomHidden"))
                {
                    materials[m].SetVector("_CutoutPos", cutoutPos);
                    materials[m].SetFloat("_CutoutSize", cutoutSize);
                    materials[m].SetFloat("_FalloffSize", falloffSize);
                }

            }
        }
    }
}
