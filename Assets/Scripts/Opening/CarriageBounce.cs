using System;
using System.Collections;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class CarriageBounce : MonoBehaviour
{
    [SerializeField, Range(0f, 10f)] float _bounceTime = 2.5f;

    [Header("Position Bouncing")]
    [SerializeField] bool _enablePositionBounce = true;
    [SerializeField, Range(0f, 20f)] float _maxBounceAcceleration = 1f;
    [SerializeField, Range(0f, 10f)] float _bounceRange = .25f;

    [Header("Rotation Bouncing")]
    [SerializeField] bool _enableRotationBounce = true;
    [SerializeField, Range(0f, 45f)] float _bounceZRotate = 4f;

    //TODO x rotation

    bool _keepBouncing = true;

    Vector3 _framePositionDiff = Vector3.zero;
    Vector3 _frameRotationEulerDiff = Vector3.zero;

    private void Start()
    {
        StartCoroutine(BouncePositionRoutine());
    }

    private void Update()
    {
        transform.position += _framePositionDiff;
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + _frameRotationEulerDiff);

        _framePositionDiff = Vector3.zero;
        _frameRotationEulerDiff = Vector3.zero;
    }

    private IEnumerator BouncePositionRoutine()
    {
        var startingPosition = transform.position;
        var minYPosition = transform.position.y;

        var lastVelocity = 0f;
        while (_keepBouncing)
        {
            if (!_enablePositionBounce)
            {
                yield return new WaitForNextFrameUnit();
                continue;
            }

            var maxYPosition = minYPosition + _bounceRange;

            var desiredYPosition = UnityEngine.Random.Range(minYPosition, maxYPosition);
            var newVelocity = (desiredYPosition - transform.position.y) / Time.deltaTime;

            var acceleration = (newVelocity - lastVelocity) / Time.deltaTime;

            if (Mathf.Abs(acceleration) > _maxBounceAcceleration)
            {
                var sign = acceleration > 0 ? 1 : -1;
                newVelocity = sign * _maxBounceAcceleration * Time.deltaTime + lastVelocity;
                desiredYPosition = newVelocity * Time.deltaTime + transform.position.y;
            }

            lastVelocity = newVelocity;

            var positionDiff = desiredYPosition - transform.position.y;
            _framePositionDiff += positionDiff * Vector3.up;

            yield return new WaitForNextFrameUnit();
        }
    }

    private IEnumerator BounceRoutine_Old()
    {
        while (_keepBouncing)
        {
            var startTime = Time.time;
            var duration = _bounceTime;

            var enablePositionBounce = _enablePositionBounce;
            var enableRotationBounce = _enableRotationBounce;

            var positionAcceleration = GetAcceleration(_bounceTime, _bounceRange);
            var rotationAcceleration = GetAcceleration(_bounceTime, _bounceZRotate);

            var positionSpeed = 0f;
            var rotationSpeed = 0f;

            while (Time.time - startTime <= duration)
            {
                var elapsedTime = Time.time - startTime;
                var t = elapsedTime / duration;
                
                if (enablePositionBounce)
                    _framePositionDiff += GetBouncePositionDiff(ref positionSpeed, positionAcceleration, t);
                if (enableRotationBounce)
                    _frameRotationEulerDiff += GetBounceRotationEulerDiff(ref rotationSpeed, rotationAcceleration, t);

                yield return new WaitForNextFrameUnit();
            }
        }
    }

    private Vector3 GetBouncePositionDiff(ref float speed, float acceleration, float t)
    {
        var diff = GetStepDiff(ref speed, acceleration, t);
        if (float.IsNaN(diff))
            return Vector3.zero;

        var positionDiff = diff * Vector3.down;
        //return transform.position + positionDiff;
        return positionDiff;
    }

    private Vector3 GetBounceRotationEulerDiff(ref float speed, float acceleration, float t)
    {
        var diff = GetStepDiff(ref speed, acceleration, t);
        if (float.IsNaN(diff))
            return Vector3.zero;

        var rotationEuler = transform.rotation.eulerAngles;

        //return Quaternion.Euler(rotationEuler + new Vector3(0f, 0f, -diff));
        return new Vector3(0f, 0f, -diff);
    }

    private float GetStepDiff(ref float speed, float acceleration, float t)
    {
        if (t >= 1f)
            return float.NaN;

        var modifier = 1f;
        if (t < .5f)
            speed += acceleration * Time.deltaTime;
        else
        {
            modifier = -1f;
            speed += -acceleration * Time.deltaTime;
        }

        return modifier * speed * Time.deltaTime;
    }

    private static float GetAcceleration(float bounceTime, float range)
    {
        var halfBounceTime = bounceTime / 2f;
        var maxSpeed = (range * 2f) / halfBounceTime;
        return maxSpeed / halfBounceTime;
    }
}
