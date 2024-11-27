using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HostilityMarkerController : MonoBehaviour
{
    [SerializeField] Material _willBeHostileToPlayer;
    [SerializeField] Material _generalHostileMat;
    [SerializeField] Material _hostileTowardsPlayerMat;

    [Header("debug")]
    [SerializeField] State _state = State.General;

    NpcBrain _brain;
    List<MeshRenderer> _meshRenderers = new();

    private void Start()
    {
        _brain = transform.parent.GetComponent<NpcBrain>();
        _meshRenderers = GetComponentsInChildren<MeshRenderer>(true).ToList();

        OnStateChange();
    }    

    private void Update()
    {
        var newState = _state;
        if (_brain.IsHostile)
        {
            if (_brain.HostileTowardsTarget == null)
                newState = _state;
            else if (_brain.HostileTowardsTarget.IsPlayer())
                newState = State.HostileTowardsPlayer;
            else
                newState = State.GeneralHostile;
        }
        else if (_brain.RelationshipWithPlayerIsHostile)
            newState = State.WillBeHostileToPlayer;
        else
            newState = State.General;

        if (newState != _state)
        {
            _state = newState;
            OnStateChange();
        }
    }

    private void OnStateChange()
    {
        var meshRenderersEnabled = false;
        var meshRendererMaterial = _hostileTowardsPlayerMat;

        switch (_state)
        {
            case State.General:
                meshRenderersEnabled = false;
                break;

            case State.WillBeHostileToPlayer:
                meshRenderersEnabled = true;
                meshRendererMaterial = _willBeHostileToPlayer;
                break;

            case State.GeneralHostile:
                meshRenderersEnabled = true;
                meshRendererMaterial = _generalHostileMat;
                break;

            case State.HostileTowardsPlayer:
                meshRenderersEnabled = true;
                meshRendererMaterial = _hostileTowardsPlayerMat;
                break;
        }

        foreach (var meshRenderer in _meshRenderers)
        {
            meshRenderer.enabled = meshRenderersEnabled;
            meshRenderer.material = meshRendererMaterial;
        }
    }


    private enum State
    {
        General,
        WillBeHostileToPlayer,
        GeneralHostile,
        HostileTowardsPlayer,
    }
}