using UnityEngine;
using UnityEngine.AI;

public enum RoomID
{
    Unknown,
    Foyer,
    Den,
    Living,
    Sun,
    Dining,
    Library,
    Kitchen,
    Storage
}

/// <summary>
/// I was wathching the half life 2 doc that came out recently and they mentioned that 
/// mass was hard to exactly state, so they created a logarithmic series of objects of 
/// different mass and they just selected what it was closest to. Seems good for this too
/// (:
/// </summary>
public enum Occupancy
{
    Closet = 2,
    Bedroom = 8,
    LivingRoom = 16,
    Ballroom = 1000
}

public class Room : MonoBehaviour
{
    public RoomID ID;

    public bool hidden = false;
    [Range(0, 1)]
    public float socialScore;  // score from 0 to 1 of how much of a "hang" the room is. Storage closet: 0, living room: 1
    public Occupancy occupancy = Occupancy.LivingRoom;

    public int MaxOccupancy => (int)occupancy;

    BoxCollider boxCollider;
    MeshRenderer meshRenderer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {

        boxCollider = GetComponent<BoxCollider>();
    }
    void Start()
    {
        RoomBB.Instance.Register(this);

        meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.enabled = false;
    }
    
    public bool PointIsInRoom(Vector3 point)
    {

        return boxCollider.bounds.Contains(point);
    }

    public bool RandomRoomChance(int currentOccupancy = 0)
    {
        if (UnityEngine.Random.Range(0f, 1f) > socialScore)
            return false;

        // Good room, but lets make sure its not super occupied
        if (MaxOccupancy <= currentOccupancy)
            return false;

        return true;
    }

    /// <summary>
    /// Returns a random point in the room
    /// </summary>
    public Vector3 GetRandomPointInRoom()
    {
        // Use box collider to get random point in room
        var randomPoint = new Vector3(
            Random.Range(boxCollider.bounds.min.x, boxCollider.bounds.max.x),
            0f,
            Random.Range(boxCollider.bounds.min.z, boxCollider.bounds.max.z));

        return randomPoint;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.TryGetComponent<CharacterInfo>(out var characterInfo))
            RoomBB.Instance.UpdateCharacterLocation(characterInfo.ID, ID);
    }
}
