using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Looker_Old : MonoBehaviour
{
    //Vector3 seenPosition = Vector3.zero;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    RaycastFrom raycastFrom;
    NpcBrain myBrain;
    Vector3 debugStart, debugEnd;
    BoxCollider[] FOV;

    public LayerMask lookLayerMask;
    private float m_thickness = 0.025f;

    [Header("DEBUG")]
    public bool debug = true;
    private float debugShowTime = 5f;
    private float debugCounter = 10f;
    private bool debug_sawTarget = false;

    private float _lastColliderCollectionResetTime = -1f;
    private List<Collider> _collidersInTrigger = new();

    void Start()
    {
        myBrain = GetComponentInParent<NpcBrain>();
        raycastFrom = GetComponentInChildren<RaycastFrom>();
        FOV = GetComponentsInChildren<BoxCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        if(debugCounter < debugShowTime)
        {
            debugCounter += Time.deltaTime;
        }
        else
        {
            debug_sawTarget = false;
        }
    }

    public bool TryGetCharactersInSight(out List<CharacterInfo> charactersInSight)
    {
        charactersInSight = _collidersInTrigger
            .Select(x => x.transform.GetComponent<CharacterInfo>())
            .Where(x => x != null && x != myBrain.GetComponent<CharacterInfo>())
            .ToList();

        return charactersInSight.Any();
    }

    
    private void OnTriggerStay(Collider other)
    {
        if (Time.time - _lastColliderCollectionResetTime > Time.deltaTime)
            _collidersInTrigger.Clear();

        _collidersInTrigger.Add(other);
    }

    public bool CanSeeTarget(GameObject target)
    {
        //Boxcast using child BoxColliders as position and size reference
        bool canSee = false;
        foreach(BoxCollider fov in FOV)
        {
            //Vector3 center = fov.transform.position;
            //Vector3 halfExtents = fov.size / 2;
            //Vector3 direction = target.transform.position - center; 
            //float maxDistance = Vector3.Distance(center, target.transform.position);
            //RaycastHit boxHit;
            //if (Physics.BoxCast(center, halfExtents, direction, out boxHit, Quaternion.identity, maxDistance, lookLayerMask))
            //{
            //    if (boxHit.collider.gameObject == target)
            //    {
            //        canSee = true;
            //        break;
            //    }
            //}

            Transform boxTransform = fov.transform;
            Vector3 _scaledSize = new Vector3(
                fov.size.x * boxTransform.lossyScale.x,
                fov.size.y * boxTransform.lossyScale.y,
                fov.size.z * boxTransform.lossyScale.z);

            float _distance = _scaledSize.y - m_thickness;
            Vector3 _direction = boxTransform.up;
            Vector3 _center = boxTransform.TransformPoint(fov.center);
            Vector3 _start = _center - _direction * (_distance / 2);
            Vector3 _halfExtents = new Vector3(_scaledSize.x, m_thickness, _scaledSize.z) / 2;
            Quaternion _orientation = boxTransform.rotation;


            Collider[] overlapHits = Physics.OverlapBox(_center, _halfExtents, _orientation, lookLayerMask);
            foreach (Collider hit in overlapHits)
            {
                //Debug.Log(myBrain.name + " sees: " + hit.gameObject.name + " in overlap box");
                if (hit.gameObject == target)
                {
                    canSee = true;
                    break;
                }
            }

        }

        // If its within the FOV, do a raycast to check if there are any obstacles
        if (canSee)
        {
            debugCounter = 0;
            //Debug.Log("can i see: " + target.name);
            RaycastHit hit;
            Vector3 start = raycastFrom.transform.position;
            Vector3 end = target.transform.position;
            end = new Vector3(end.x, start.y, end.z);

            debugStart = start;
            debugEnd = end;
            if (Physics.Raycast(start, end - start, out hit, Mathf.Infinity, lookLayerMask))
            {
                //Debug.Log(myBrain.name + " sees: " + hit.collider.gameObject.name);
                if (hit.collider.gameObject == target)
                {
                    Debug.Log(myBrain.name + " sees you: " + target.name);
                    debug_sawTarget = true;
                    return true;
                }
            }
        }
        return false;

    }
    private void OnTriggerEnter(Collider other)
    {
        //NpcBrain brain = other.GetComponent<NpcBrain>();
        //if(brain != null)
        //{
        //    //Debug.Log("I see you: " + other.name);
        //    if (brain.dragged)
        //    {
        //        // saw someone dragging a body
        //        if (CanSeeTarget(brain.gameObject))
        //            myBrain.SawCorpseDragging(brain.gameObject);
        //    }
        //    else if (brain.dead)
        //    {
        //        // saw a dead body
        //        if (CanSeeTarget(brain.gameObject))
        //            myBrain.SawCorpse(brain.gameObject);
        //        //Debug.Log(myBrain.name + " saw " + brain.name + " dead");
        //    }
        //    else if (brain.strangled)
        //    {
        //        // saw someone being strangled.
        //        if (CanSeeTarget(brain.gameObject))
        //            myBrain.SawStrangling(brain.gameObject);
        //            //Debug.Log(myBrain.name + " saw " + brain.name + " being strangled");
        //    }


        //}
    }
    private void OnDrawGizmos()
    {
        if(debug && debugCounter < debugShowTime)
        {
            Gizmos.color = Color.white;
            if(debug_sawTarget)
            {
                Gizmos.color = Color.green;
            }
            else
            {
                Gizmos.color = Color.red;
            }
            Gizmos.DrawRay(debugStart,debugEnd - debugStart);
            Gizmos.DrawWireSphere(debugEnd, 0.1f);
            Gizmos.DrawWireSphere(debugStart, 0.1f);

            foreach (BoxCollider col in FOV)
            {
                if (col == null) return;

                // Set the color to a semi-transparent version of the current color
                Gizmos.color = new Color(Gizmos.color.r, Gizmos.color.g, Gizmos.color.b, 0.2f);

                Transform boxTransform = col.transform;
                // Calculate the same parameters as in your CheckHit method
                Vector3 scaledSize = new Vector3(
                    col.size.x * boxTransform.lossyScale.x,
                    col.size.y * boxTransform.lossyScale.y,
                    col.size.z * boxTransform.lossyScale.z);

                float distance = scaledSize.y - m_thickness;
                Vector3 direction = boxTransform.up;
                Vector3 center = boxTransform.TransformPoint(col.center);
                Vector3 start = center - direction * (distance / 2);
                Vector3 halfExtents = new Vector3(scaledSize.x, m_thickness, scaledSize.z) / 2;
                Quaternion orientation = boxTransform.rotation;

                // Draw the starting box
                Matrix4x4 rotationMatrixStart = Matrix4x4.TRS(start, orientation, boxTransform.lossyScale);
                Gizmos.matrix = rotationMatrixStart;
                //Gizmos.color = new Color(1, 0, 0, 0.5f); // Semi-transparent red
                Gizmos.DrawCube(Vector3.zero, new Vector3(halfExtents.x * 2, halfExtents.y * 2, halfExtents.z * 2));

                // Calculate the end position of the box cast
                Vector3 end = start + direction * distance;

                // Draw the ending box
                Matrix4x4 rotationMatrixEnd = Matrix4x4.TRS(end, orientation, boxTransform.lossyScale);
                Gizmos.matrix = rotationMatrixEnd;
                //Gizmos.color = new Color(0, 1, 0, 0.5f); // Semi-transparent green
                Gizmos.DrawCube(Vector3.zero, new Vector3(halfExtents.x * 2, halfExtents.y * 2, halfExtents.z * 2));

                // Reset Gizmos matrix to default
                Gizmos.matrix = Matrix4x4.identity;

                // Optionally, draw a line between the start and end points for clarity
                //Gizmos.color = Color.yellow; // Yellow color for the line
                Gizmos.DrawLine(start, end);
            }

        }
    }
}
