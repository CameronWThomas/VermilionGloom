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


    [SerializeField]
    RaycastFrom castFrom;
    [SerializeField]
    LayerMask lookLayerMask;

    [Header("Debug")]
    private bool debug = false;
    Vector3 debugRayStart;
    Vector3 debugRayEnd;

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

    private List<Transform> FilterCharactersByRaycastViewable(List<Transform> charactersInSight)
    {
        Vector3 start = castFrom.transform.position;
        debugRayStart = start;
        List<Transform> filteredCharsInSight = new List<Transform>();
        foreach(Transform ch in charactersInSight)
        {
            Vector3 end = ch.transform.position;
            debugRayEnd = end;
            end = new Vector3(end.x, start.y , end.z);
            RaycastHit hit;
            if (Physics.Raycast(start, end - start, out hit, Mathf.Infinity, lookLayerMask))
            {
                if (hit.transform == ch.transform)
                {
                    filteredCharsInSight.Add(ch);
                }
                debugRayEnd = hit.point;
            }
        }
        return filteredCharsInSight;
    }
    public bool TryGetCharactersInSight(out List<CharacterInfo> charactersInSight)
    {
        var filteredChars = FilterCharactersByRaycastViewable(_charactersInSight);
        charactersInSight = filteredChars
            .Select(x => x.transform.GetComponent<CharacterInfo>())
            .Where(x => x != null && x != _myBrain.GetComponent<CharacterInfo>())
            .ToList();

        //charactersInSight = FilterCharactersByRaycastViewable(charactersInSight);

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

    //private void OnDrawGizmos()
    //{
    //    if (!debug)
    //        return;
    //    Gizmos.color = Color.blue;
    //    Gizmos.DrawLine(debugRayStart, debugRayEnd);
    //    Gizmos.DrawSphere(debugRayEnd, 0.1f);
    //    Gizmos.DrawSphere(debugRayStart, 0.1f);
    //}
}