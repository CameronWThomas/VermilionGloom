using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

//TODO these should probably utilize a navmesh link eventually if want the NPCs to be able to utilize secret passages

public class SecretPassage : MonoBehaviour
{
    [SerializeField] private bool _reverseDirection = false;
    [SerializeField, Range(0f, 10f)] private float _destinationDistance = 1f;    

    public Vector3 DestinationPoint => GetDestinationPoint();   

    public SecretPassage EndPoint { get; set; }

    public void UsePassage(Transform transportedTransform)
    {
        if (EndPoint == null)
            return;

        StartCoroutine(Transport(transportedTransform));
    }

    private IEnumerator Transport(Transform transportedTransform)
    {
        DeactivateNavigation(transportedTransform);

        try
        {
            yield return UsePassage(this, transportedTransform, true);
            yield return UsePassage(EndPoint, transportedTransform, false);
        }
        finally
        {
            ReactivateNavigation(transportedTransform);
        }
    }

    private void DeactivateNavigation(Transform transportedTransform)
    {
        if (transportedTransform.CompareTag(GlobalConstants.PLAYER_TAG_NAME))
            MouseReceiver.Instance.Deactivate();

        //TODO I don't like activating and reactivating other gameobject components here, but for now...
        if (transportedTransform.TryGetComponent<MvmntController>(out var mvmntController))
            mvmntController.enabled = false;
        if (transportedTransform.TryGetComponent<NavMeshAgent>(out var navMeshAgent))
            navMeshAgent.enabled = false;

    }

    private void ReactivateNavigation(Transform transportedTransform)
    {
        if (transportedTransform.CompareTag(GlobalConstants.PLAYER_TAG_NAME))
            MouseReceiver.Instance.Activate();

        if (transportedTransform.TryGetComponent<MvmntController>(out var mvmntController))
            mvmntController.enabled = true;
        if (transportedTransform.TryGetComponent<NavMeshAgent>(out var navMeshAgent))
            navMeshAgent.enabled = true;
    }

    private static IEnumerator UsePassage(SecretPassage passage, Transform transportedTransform, bool isEntering)
    {
        var destinationFromPassage = passage.GetDestinationFromPassage();
        var doorAtTransformHeight = passage.transform.position;
        doorAtTransformHeight.y = transportedTransform.position.y;
        
        Vector3 startingPosition, endingPosition;
        if (isEntering)
        {
            startingPosition = transportedTransform.position;
            endingPosition = doorAtTransformHeight - destinationFromPassage;
        }
        else
        {
            startingPosition = doorAtTransformHeight - destinationFromPassage;
            endingPosition = doorAtTransformHeight + destinationFromPassage;
        }

        var totalDuration = SecretPassageManager.Instance.EnterSecretPassageTime;
        var stepTime = Time.deltaTime;
        var distance = (startingPosition - endingPosition).magnitude;

        var totalSteps = totalDuration / stepTime;
        var changePerStep = distance / totalSteps;
        var direction = (endingPosition - startingPosition).normalized;

        for (var step = 0f; step < totalSteps; step++)
        {
            transportedTransform.position = Vector3.Lerp(startingPosition, endingPosition, step / totalSteps);

            yield return new WaitForSeconds(stepTime);
        }
    }

    private Vector3 GetDestinationPoint()
    {
        var ground = transform.position - new Vector3(0f, transform.position.y, 0f);
        return ground + GetDestinationFromPassage();
    }

    private Vector3 GetDestinationFromPassage()
    {
        var modifier = _destinationDistance * (_reverseDirection ? -1f : 1f);
        return transform.forward * modifier;
    }

    private void OnDrawGizmos()
    {
        var destination = GetDestinationPoint();

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(destination, .1f);
    }
}