using UnityEngine;

public class MouseReceiver : MonoBehaviour
{
    //public LayerMask movementLayerMask;
    public MvmntController playerMvmnt;
    private void Update()
    {

        // TODO: use input actions instead
        // could also maybe use OnMouseDown?
        if (Input.GetMouseButtonDown(0))
        {
            MouseInteraction();
        }
    }


    private void MouseInteraction()
    {
        Debug.Log("Mouse Clicked");
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        // Debug draw the ray
        //Debug.DrawRay(ray.origin, ray.direction, Color.red);

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            Debug.Log("Mouse Hit:");
            Debug.Log("     Transform Name: " + hit.transform.name);
            Debug.Log("     Point: " + hit.point);

            if(hit.transform.tag == "Walkable")
            {
                playerMvmnt.SetTarget(hit.point);
            }
        }
    }

    private void OnDrawGizmos()
    {
        
    }
}
