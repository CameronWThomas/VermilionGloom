using UnityEngine;

[ExecuteInEditMode]
public class TriggerVolume : MonoBehaviour
{
    [Header("Trigger Volume")]
    public bool IsPlayerPresent = false;
    public bool IsNpcBodyPresent = false;



    [Header("Debug")]
    [SerializeField] bool _debug = false;
    [SerializeField] Material _triggerVolumeMaterial;

    private void Start()
    {
        IsPlayerPresent = false;
    }

    private void Update()
    {
        var meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.enabled = _debug;

        if (_debug)
            meshRenderer.material = _triggerVolumeMaterial;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.IsPlayer())
            IsPlayerPresent = true;
        else if (other.transform.IsNpc() && other.transform.GetComponent<CharacterInfo>().IsDead)
            IsNpcBodyPresent = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.IsPlayer())
            IsPlayerPresent = false;
        else if (other.transform.IsNpc())
            IsNpcBodyPresent = false;
    }
}