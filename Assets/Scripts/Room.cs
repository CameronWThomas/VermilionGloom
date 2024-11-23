
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class Room : MonoBehaviour
{
    public bool npcHidden = false;
    public bool isVisible = false;
    [UnityEngine.Range(0, 1)]
    public float socialScore;  // score from 0 to 1 of how much of a "hang" the room is. Storage closet: 0, living room: 1

    BoxCollider boxCollider;
    MeshRenderer meshRenderer;
    public Material blackedOut;

    public List<MeshRenderer> meshesToHide;
    public List<MeshRenderer> meshesToShow;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {

        boxCollider = GetComponent<BoxCollider>();
    }
    private void OnEnable()
    {
        meshRenderer = gameObject.GetComponent<MeshRenderer>();
        meshRenderer.material = blackedOut;
        meshRenderer.enabled = false;
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
            foreach(MeshRenderer wall in meshesToHide)
            {
                wall.enabled = false;
            }
            foreach (MeshRenderer wall in meshesToShow)
            {
                wall.enabled = true;
            }

            meshRenderer.enabled = false;
        }
        else
        {
            foreach (MeshRenderer wall in meshesToHide)
            {
                wall.enabled = true;
            }
            foreach (MeshRenderer wall in meshesToShow)
            {
                wall.enabled = false;
            }

            meshRenderer.enabled = true;
            
        }

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //meshRenderer.enabled = true;
            PlayerController player = other.GetComponent<PlayerController>();
            player.EnteredRoom(this);
        }
    }
}
