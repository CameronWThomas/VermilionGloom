using System.Collections.Generic;
using UnityEngine;

public class LookerCollider : MonoBehaviour
{
    float _lastColliderCollectionResetTime = -1f;

    public List<Transform> CharactersInSight { get; } = new();

    private void OnTriggerStay(Collider other)
    {
        if (Time.time - _lastColliderCollectionResetTime > Time.deltaTime)
        {
            CharactersInSight.Clear();
            _lastColliderCollectionResetTime = Time.time;
        }

        if (other.transform.TryGetComponent<CharacterInfo>(out _) && !CharactersInSight.Contains(other.transform))
            CharactersInSight.Add(other.transform);
    }
}