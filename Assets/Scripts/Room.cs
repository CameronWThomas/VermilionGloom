using UnityEngine;

public class Room : MonoBehaviour
{
    public bool hidden = false;
    [Range(0, 1)]
    public float socialScore;  // score from 0 to 1 of how much of a "hang" the room is. Storage closet: 0, living room: 1

    BoxCollider boxCollider;
    MeshRenderer meshRenderer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.enabled = false;
        boxCollider = GetComponent<BoxCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public bool PointIsInRoom(Vector3 point)
    {
        return boxCollider.bounds.Contains(point);
    }
    // Returns a random point in the room
    public Vector3 GetRandomPointInRoom()
    {
        // Use box collider to get random point in room
        Vector3 randomPoint = new Vector3(
            Random.Range(boxCollider.bounds.min.x, boxCollider.bounds.max.x),
            Random.Range(boxCollider.bounds.min.y, boxCollider.bounds.max.y),
            Random.Range(boxCollider.bounds.min.z, boxCollider.bounds.max.z)
                );
        return randomPoint;
    }

    // TODO: will return a random point of interest in the room
    public Vector3 GetRandomPoiInRoom()
    {
        return GetRandomPointInRoom();
    }
}
