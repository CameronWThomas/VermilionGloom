
using System.Collections.Generic;
using System.Linq;
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

    public bool npcHidden = false;
    public bool isVisible = false;
    [UnityEngine.Range(0, 1)]
    public float socialScore;  // score from 0 to 1 of how much of a "hang" the room is. Storage closet: 0, living room: 1


    public Occupancy occupancy = Occupancy.LivingRoom;
    public int MaxOccupancy => (int)occupancy;


    BoxCollider boxCollider;
    MeshRenderer meshRenderer;
    MeshRenderer[] childMeshRenderers;


    public Material blackedOut;
    public Material mirrorTexture;

    public List<MeshRenderer> meshesToHide = new List<MeshRenderer>();
    public List<SkinnedMeshRenderer> skinnedMeshesToShow = new List<SkinnedMeshRenderer>();
    public List<MeshRenderer> meshesToShow = new List<MeshRenderer>();

    public Camera mirrorCam;
    public GameObject mirror;
    public Vector3 mirrorCamPos = new Vector3(0, 0.00139999995f, 0.00591999991f);
    public Vector3 mirrorCamRot = new Vector3(90, 0, 0);

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {

        boxCollider = GetComponent<BoxCollider>();
    }
    private void Start()
    {
        RoomBB.Instance.Register(this);

        meshRenderer = gameObject.GetComponent<MeshRenderer>();
        childMeshRenderers = gameObject.GetComponentsInChildren<MeshRenderer>();
        meshRenderer.material = blackedOut;
        meshRenderer.enabled = false;
        foreach (MeshRenderer child in childMeshRenderers)
        {
            child.material = blackedOut;
            child.enabled = false;
        }
        SetVisible(false);
    }


    // Update is called once per frame
    void Update()
    {

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
    // Returns a random point in the room
    public Vector3 GetRandomPointInRoom()
    {
        // Use box collider to get random point in room
        var randomPoint = new Vector3(
            Random.Range(boxCollider.bounds.min.x, boxCollider.bounds.max.x),
            0f,
            Random.Range(boxCollider.bounds.min.z, boxCollider.bounds.max.z));

        return randomPoint;
    }

    // TODO: will return a random point of interest in the room
    public Vector3 GetRandomPoiInRoom()
    {
        return GetRandomPointInRoom();
    }

    public void SetVisible(bool visible)
    {
        isVisible = visible;
        if(isVisible)
        {
            //if (meshesToHide != null)
            //{
            //    foreach (MeshRenderer wall in meshesToHide)
            //    {
            //        wall.enabled = false;
            //    }
            //}
            if(meshesToShow != null)
            {
                foreach (MeshRenderer wall in meshesToShow)
                {
                    wall.enabled = true;
                }
            }
            if(skinnedMeshesToShow != null)
            {
                foreach (SkinnedMeshRenderer wall in skinnedMeshesToShow)
                {
                    wall.enabled = true;
                }
            }
            meshRenderer.enabled = false;
            foreach (MeshRenderer child in childMeshRenderers)
            {
                child.enabled = false;
            }

            // camera
            if(mirror != null && mirrorCam != null)
            {
                mirrorCam.transform.parent = mirror.transform;
                mirrorCam.transform.localPosition = mirrorCamPos;
                mirrorCam.transform.localEulerAngles = mirrorCamRot;
                MeshRenderer mirrorMesh = mirror.GetComponent<MeshRenderer>();
                if (mirrorMesh != null)
                {
                    mirrorMesh.materials[0] = mirrorTexture;
                }
            }
        }
        else
        {
            //if(meshesToHide != null)
            //{
            //    foreach (MeshRenderer wall in meshesToHide)
            //    {
            //        wall.enabled = true;
            //    }
            //}
            if (meshesToShow != null)
            {
                foreach (MeshRenderer wall in meshesToShow)
                {
                    wall.enabled = false;
                }
            }
            if (skinnedMeshesToShow != null)
            {
                foreach (SkinnedMeshRenderer wall in skinnedMeshesToShow)
                {
                    wall.enabled = false;
                }
            }

            meshRenderer.enabled = true;
            foreach (MeshRenderer child in childMeshRenderers)
            {
                child.enabled = true;
            }
            if(mirror != null)
            {
                MeshRenderer mirrorMesh = mirror.GetComponent<MeshRenderer>();
                if (mirrorMesh != null)
                {
                    mirrorMesh.materials[0] = blackedOut;
                }
            }
        }

    }

    public void TriggerEntered(Collider other)
    {

        //Debug.Log(other.name + "... Entered room: " + ID);
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered room: " + ID);
            //meshRenderer.enabled = true;
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
                player.EnteredRoom(this);
        }

        if (other.transform.TryGetComponent<CharacterInfo>(out var characterInfo))
            RoomBB.Instance.UpdateCharacterLocation(characterInfo.ID, ID);
    }
    private void OnTriggerEnter(Collider other)
    {
        TriggerEntered(other);
    }


    private void OnTriggerExit(Collider other)
    {
        if (other.transform.TryGetComponent<CharacterInfo>(out var characterInfo))
            RoomBB.Instance.CharacterLeftRoom(characterInfo.ID, ID);
    }
}
