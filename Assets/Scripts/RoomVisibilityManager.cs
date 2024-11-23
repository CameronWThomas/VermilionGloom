using NUnit.Framework;
using UnityEngine;

public class RoomVisibilityManager : GlobalSingleInstanceMonoBehaviour<RoomVisibilityManager>   
{

    public Room[] rooms;

    private void Start()
    {
        rooms = FindObjectsOfType<Room>();
    }

    public void UpdateRoomVisibility(Room visibleRoom)
    {
        foreach (Room room in rooms)
        {
            if(room == visibleRoom)
            {
                room.SetVisible(true);
            }
            else
            {
                room.SetVisible(false);
            }
        }
    }
}
