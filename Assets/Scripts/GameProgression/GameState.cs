using UnityEngine;

public class GameState : GlobalSingleInstanceMonoBehaviour<GameState>
{
    private const int MAX_BODIES = 10;

    [Header("Progression")]
    public bool VampireLordVisited = false;
    [SerializeField, Range(0, MAX_BODIES)] int _bodyDeliverCount = 0;

    [Header("Progression Conditions")]
    [SerializeField, Range(0, MAX_BODIES)] int _winGameBodyCount = 5;

    [Header("Needed to help run sequences")]
    public VampireController Vampire;
    public CoffinController CoffinController;

    protected override void Start()
    {
        base.Start();
     
        PutVampireLordInDefaultPosition();        
    }

    public void PutVampireLordInDefaultPosition()
    {
        GetVampireLordDefaultPositionAndRotation(out var position, out var rotation);
        Vampire.transform.SetPositionAndRotation(position, rotation);
    }

    public void GetVampireLordDefaultPositionAndRotation(out Vector3 position, out Quaternion rotation)
    {
        var defaultVampireTransform = UsefulTransforms.Instance.V_Default;
        position = defaultVampireTransform.position;
        rotation = defaultVampireTransform.rotation;
    }
}