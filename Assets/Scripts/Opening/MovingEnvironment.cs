using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class MovingEnvironment : MonoBehaviour
{
    [SerializeField, Range(0f, 10f)] float _speed = 1f;
    [SerializeField] bool _reverseMovingDirection = false;

    [Header("Prefabs")]
    [SerializeField] Transform _environmentPrefab;
    [SerializeField] float _environmentSize = 50f;

    Transform _additionalMoveTransform = null;

    float? _deceleration = null;

    private void Start()
    {
        StartCoroutine(MoveEnvironmentRoutine());
    }

    public void AttachTransform(Transform targetTransform)
    {
        _additionalMoveTransform = targetTransform;
        _deceleration = -(_speed * _speed) / (2f * targetTransform.position.x);
    }

    public void StopMoving()
    {
        StopAllCoroutines();
    }

    private IEnumerator MoveEnvironmentRoutine()
    {
        var environments = new List<Transform>()
        {
            CreateNextEnvironmentInstance(),
            CreateNextEnvironmentInstance()
        };

        var locationDiff = _environmentSize / 2f;
        environments[0].transform.position = GetMovingDirection() * locationDiff;
        environments[1].transform.position = -GetMovingDirection() * locationDiff;

        while (true)
        {
            if (_deceleration.HasValue)
            {
                _speed += _deceleration.Value * Time.deltaTime;
                _speed = Mathf.Clamp(_speed, 0.01f, 100f);
            }

            var distanceDiff = _speed * Time.deltaTime;

            Transform resetEnvironment = null;
            foreach (var environment in environments)
            {
                environment.position += GetMovingDirection() * distanceDiff;

                if (HasEnvironmentReachedEnd(environment))
                    resetEnvironment = environment;
            }

            if (_additionalMoveTransform != null)
                _additionalMoveTransform.position += GetMovingDirection() * distanceDiff;

            if (resetEnvironment != null)
            {
                var otherEnvironment = environments.First(x => x != resetEnvironment);
                resetEnvironment.position = otherEnvironment.position + _environmentSize * -GetMovingDirection();
            }

            yield return new WaitForNextFrameUnit();
        }
    }

    private bool HasEnvironmentReachedEnd(Transform environment)
    {
        var movingDirection = GetMovingDirection();
        var environmentResetPoint = _environmentSize * .6f;

        var vector = Vector3.Project(environment.transform.position - movingDirection * environmentResetPoint, movingDirection);

        return Vector3.Dot(vector.normalized, movingDirection.normalized) >= 1f - Mathf.Epsilon;

        //if (Vector3.Dot(vector.normalized, movingDirection.normalized) >= 1f - Mathf.Epsilon)
            //environment.transform.position = -movingDirection * halfEnvironmentSize;
    }

    private Transform CreateNextEnvironmentInstance()
    {
        var environmentTransform = Instantiate(_environmentPrefab, transform).transform;
        environmentTransform.position = transform.position;
        return environmentTransform;
    }

    private Vector3 GetMovingDirection()
    {
        var value = _reverseMovingDirection ? 1f : -1f;
        return new Vector3(value, 0f, 0f);
    }
}