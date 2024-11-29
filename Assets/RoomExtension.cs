using UnityEngine;

public class RoomExtension : MonoBehaviour
{
    public Room extensionOf;


    private void OnTriggerEnter(Collider other)
    {
        extensionOf.TriggerEntered(other);
    }
}