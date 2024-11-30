using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class RoomVisibilityManager : GlobalSingleInstanceMonoBehaviour<RoomVisibilityManager>   
{

    public Room[] rooms;
    public Room lastVisibleRoom;

    private float visDispPct = 7.91f;
    private float visAlpha = 1;
    private float hidDispPct = 1f;
    private float hidAlpha = 0.1f;
    public float showHideSpeed = 0.1f;
    public List<MeshRenderer> meshesToHide = new List<MeshRenderer>();
    public List<MeshRenderer> lastHiddenMeshes = new List<MeshRenderer>();

    protected override void Start()
    {
        base.Start();
        rooms = FindObjectsByType<Room>(FindObjectsSortMode.InstanceID);
    }

    private void Update()
    {
        foreach(MeshRenderer show in lastHiddenMeshes)
        {
            if (show.material.HasProperty("_DisplayPct"))
            {
                var newDisPct = Mathf.Lerp(show.material.GetFloat("_DisplayPct"), visDispPct, showHideSpeed);    
                var newAlpha = Mathf.Lerp(show.material.GetFloat("_Alpha"), visAlpha, showHideSpeed);
                show.material.SetFloat("_DisplayPct", newDisPct);
                show.material.SetFloat("_Alpha", newAlpha);
            }
        }
        foreach(MeshRenderer hide in meshesToHide)
        {

            if (hide.material.HasProperty("_DisplayPct"))
            {
                var newDisPct = Mathf.Lerp(hide.material.GetFloat("_DisplayPct"), hidDispPct, showHideSpeed);
                var newAlpha = Mathf.Lerp(hide.material.GetFloat("_Alpha"), hidAlpha, showHideSpeed);
                hide.material.SetFloat("_DisplayPct", newDisPct);
                hide.material.SetFloat("_Alpha", newAlpha);
            }
        }
    }


    public void UpdateRoomVisibility(Room visibleRoom)
    {
        foreach (Room room in rooms)
        {
            if(room == visibleRoom)
            {
                //room.SetVisible(true);
            }
            else
            {
                room.SetVisible(false);
            }
        }
        lastVisibleRoom = visibleRoom;
        visibleRoom.SetVisible(true);
        lastHiddenMeshes = meshesToHide;
        meshesToHide = visibleRoom.meshesToHide;
        foreach(MeshRenderer mesh in meshesToHide)
        {
            if(lastHiddenMeshes.Contains(mesh))
            {
                lastHiddenMeshes.Remove(mesh);
            }
        }

    }
}
