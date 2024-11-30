using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class CarriageBounce : MonoBehaviour
{
    [Header("Bouncing")]
    [SerializeField] bool _enablePositionBounce = true;
    [SerializeField, Range(0f, 10f)] float _bounceRange = .05f;
    [SerializeField, Range(0f, 20f)] float _maxBounceAcceleration = 2f;

    [Header("Rocking")]
    [SerializeField] bool _enableRocking = true;
    [SerializeField, Range(0f, 90f)] float _rockingRange = .5f;
    [SerializeField, Range(0f, 90f)] float _maxRockingAcceleration = 20f;    


    bool _isTraveling = true;

    Vector3 _framePositionDiff = Vector3.zero;
    Vector3 _frameRotationEulerDiff = Vector3.zero;

    private void Start()
    {
        StartCoroutine(BouncePositionRoutine());
        StartCoroutine(RockingRoutine());
    }

    private void Update()
    {
        transform.position += _framePositionDiff;
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + _frameRotationEulerDiff);

        _framePositionDiff = Vector3.zero;
        _frameRotationEulerDiff = Vector3.zero;
    }

    public void StopMoving()
    {
        _isTraveling = false;
    }

    private IEnumerator RockingRoutine()
    {
        var startingRotationX = transform.rotation.eulerAngles.x;

        var velocity = 0f;
        float? desiredPosition = null;
        while (true)
        {
            if (!_isTraveling && !_enableRocking)
                break;

            var currentXAngle = transform.rotation.eulerAngles.x;
            var correctedCurrentXAngle = currentXAngle > 180f ? currentXAngle - 360f : currentXAngle;

            if (!_isTraveling)
            {
                if (Mathf.Abs(correctedCurrentXAngle - startingRotationX) < 0.001)
                    break;

                desiredPosition = startingRotationX;
            }

            var minXAngle = startingRotationX - (_rockingRange / 2f);
            var maxXAngle = startingRotationX + (_rockingRange / 2f);

            if (_enableRocking)
            {
                var xAngleDiff = GetAccelerationLimitedValueDiff(
                    minXAngle, maxXAngle,
                    correctedCurrentXAngle,
                    ref velocity,
                    _maxRockingAcceleration,
                    desiredPosition);

                _frameRotationEulerDiff += new Vector3(xAngleDiff, 0f, 0f);
            }
            else
                velocity = 0f;

            yield return new WaitForNextFrameUnit();
        }


    }   

    private IEnumerator BouncePositionRoutine()
    {
        var startingPosition = transform.position;
        var minYPosition = transform.position.y;

        float? desiredPosition = null;
        var velocity = 0f;
        while (true)
        {
            if (!_isTraveling && !_enablePositionBounce)
                break;

            if (!_isTraveling)
            {
                if (Mathf.Abs(transform.position.y - minYPosition) < 0.001)
                    break;

                desiredPosition = minYPosition;
            }

            var maxYPosition = minYPosition + _bounceRange;

            if (_enablePositionBounce)
            {
                var positionDiff = GetAccelerationLimitedValueDiff(
                    minYPosition, maxYPosition,
                    transform.position.y,
                    ref velocity,
                    _maxBounceAcceleration,
                    desiredPosition);

                _framePositionDiff += positionDiff * Vector3.up;
            }
            else
                velocity = 0f;

            yield return new WaitForNextFrameUnit();
        }
    }

    private static float GetAccelerationLimitedValueDiff(float minValue, float maxValue,
        float currentValue,
        ref float velocity,
        float maxAcceleration, 
        float? desiredPosition)
    {
        desiredPosition ??= UnityEngine.Random.Range(minValue, maxValue);
        var newVelocity = (desiredPosition.Value - currentValue) / Time.deltaTime;

        var acceleration = (newVelocity - velocity) / Time.deltaTime;

        if (Mathf.Abs(acceleration) > maxAcceleration)
        {
            var sign = acceleration > 0 ? 1 : -1;
            newVelocity = sign * maxAcceleration * Time.deltaTime + velocity;
            desiredPosition = newVelocity * Time.deltaTime + currentValue;
        }

        velocity = newVelocity;

        return desiredPosition.Value - currentValue;
    }    
}
