using UnityEngine;

public class GameState : GlobalSingleInstanceMonoBehaviour<GameState>
{
    [Header("Protression")]
    public bool VampireLordVisited = false;

    [Header("Needed to help run sequences")]
    public VampireController Vampire;
    public CoffinController CoffinController;

    private VampireDiscoverySequence _vampireDiscoverySequence;

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