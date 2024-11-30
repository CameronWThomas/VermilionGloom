using UnityEngine;

public class TurnOnYeMeshes : MonoBehaviour
{
    public MeshRenderer mesh;

    // Start is called before the first frame update
    void Start()
    {
        mesh = GetComponent<MeshRenderer>();
        mesh.enabled = true;
    }
}   