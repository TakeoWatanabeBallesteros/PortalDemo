using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CreatePortal : MonoBehaviour
{
    [SerializeField] 
    private GameObject portal;
    [SerializeField] 
    private GameObject portalImage;
    [SerializeField]
    private Transform[] validPoints;
    [SerializeField] 
    private LayerMask portalLayerMask;
    [SerializeField] 
    private float minValidDistance;
    [SerializeField] 
    private float maxValidDistance;
    [SerializeField] 
    private float minDotAngle;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 position;
        Vector3 normal; 
        
        portalImage.SetActive(IsValidPosition(Camera.main.transform.position, Camera.main.transform.forward, 100, out position, out normal));
    }
    
    public bool IsValidPosition(Vector3 startPosition, Vector3 forward, float maxDistance, out Vector3 position, out Vector3 normal)
    {
        Ray ray = new Ray(startPosition, forward);
        position = Vector3.zero;
        normal = Vector3.forward;
        portal.transform.position = position;
        portal.transform.rotation = Quaternion.LookRotation(normal);
        
        if (Physics.Raycast(ray, out var hitInfo, maxDistance, portalLayerMask.value))
        {
            if (hitInfo.collider.CompareTag("DrawableWall"))
            {
                normal = hitInfo.normal;
                position = hitInfo.point;
                portal.transform.position = position;
                portal.transform.rotation = Quaternion.LookRotation(normal);
                foreach (var direction in validPoints.Select(point => point.position - startPosition))
                {
                    direction.Normalize();
                    ray = new Ray(startPosition, direction);
                    if (Physics.Raycast(ray, out var pointHit, maxDistance, portalLayerMask.value))
                    {
                        if (pointHit.collider.CompareTag("DrawableWall"))
                        {
                            float distance = Vector3.Distance(position, pointHit.point);
                            float dotAngle = Vector3.Dot(normal, pointHit.normal);
                            // if one of the conditions is false the portal cant be there
                            if (!(distance >= minValidDistance && distance <= maxValidDistance && 
                                  dotAngle > minDotAngle))
                                return false;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
                return true;
            }
        }
        return false;
    }
}
