using UnityEngine;

public class Billboard : MonoBehaviour
{
    [SerializeField]
    private bool showSprite = true;
    public MeshRenderer mr;
    private void Start()
    {
        mr = GetComponentInChildren<MeshRenderer>();
        mr.enabled = showSprite;
    }
    void Update()
    {
        transform.LookAt(Camera.main.transform.position, -Vector3.up);
    }

    public void SetShowSprite(bool showSprite)
    {
        this.showSprite = showSprite;
        mr.enabled = showSprite;
    }
}