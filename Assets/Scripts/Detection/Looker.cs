using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[ExecuteInEditMode]
public class Looker : MonoBehaviour
{
    [SerializeField, Range(1f, 20f)] float _distance = 5f;
    [SerializeField, Range(0, 160)] int _horizontalAngle = 70;
    [SerializeField, Range(0, 160)] int _verticalAngle = 70;
    [SerializeField] List<Transform> _charactersInSight = new();

    NpcBrain _myBrain;

    float _lastDistance = -1f;
    int _lastAngle = -1;

    LookerCollider _lookerCollider;

    private void Start()
    {
        _myBrain = GetComponentInParent<NpcBrain>();
        _lookerCollider = transform.GetComponentInChildren<LookerCollider>(true);
    }

    private void Update()
    {
        UpdateSize();
        _charactersInSight = _lookerCollider.CharactersInSight;
    }

    public bool TryGetCharactersInSight(out List<CharacterInfo> charactersInSight)
    {
        charactersInSight = _charactersInSight
            .Select(x => x.transform.GetComponent<CharacterInfo>())
            .Where(x => x != null && x != _myBrain.GetComponent<CharacterInfo>())
            .ToList();

        return charactersInSight.Any();
    }


    private void UpdateSize()
    {
        if (Mathf.Abs(_lastAngle - _distance) < Mathf.Epsilon && _lastAngle == _horizontalAngle)
            return;

        _lastAngle = _horizontalAngle;
        _lastDistance = _distance;

        var yScale = _distance;
        var zDistance = _distance / 2f;
        var xScale = Mathf.Tan(_horizontalAngle / 2f * Mathf.Deg2Rad) * 2 * yScale;
        var zScale = Mathf.Tan(_verticalAngle / 2f * Mathf.Deg2Rad) * 2 * yScale;

        _lookerCollider.transform.localPosition = new Vector3(0f, 0f, zDistance);
        _lookerCollider.transform.localScale = new Vector3(xScale, yScale, zScale);

    }
}