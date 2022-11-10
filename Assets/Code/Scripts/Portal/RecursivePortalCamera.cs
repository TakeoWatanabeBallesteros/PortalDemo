using System;
using JetBrains.Annotations;
using UnityEngine;

public class RecursivePortalCamera : MonoBehaviour
{
    [SerializeField]
    private PortalBehaviour portal;
    [SerializeField] 
    private PortalBehaviour mirrorPortal;

    [SerializeField]
    private Camera portalCamera;
    private Camera mainCamera;

    private const int iterations = 7;

    private RenderTexture tempTexture;

    private void Awake()
    {
        mainCamera = GetComponent<Camera>();
        
        tempTexture = new RenderTexture(Screen.width, Screen.height, 24, RenderTextureFormat.ARGB32);
    }

    private void Start()
    {
        // portal.SetTexture(tempTexture);
    }

    private void OnPreRender()
    {
        if (!portal.IsPlaced)
        {
            return;
        }

        if (portal.IsRendererVisible())
        {
            // portalCamera.targetTexture = tempTexture;
            for (int i = iterations - 1; i >= 0; --i)
            {
                RenderCamera(portal, mirrorPortal, i);
            }
        }
    }

    private void RenderCamera(PortalBehaviour inPortal, PortalBehaviour outPortal, int iterationID)
    {
        var inTransform = inPortal.PortalTransform;
        var outTransform = outPortal.PortalTransform;
        
        Transform cameraTransform = portalCamera.transform;
        cameraTransform.position = transform.position;
        cameraTransform.rotation = transform.rotation;

        for(int i = 0; i <= iterationID; ++i)
        {
            // Position the camera behind the other portal.
            Vector3 relativePos = inTransform.InverseTransformPoint(cameraTransform.position);
            relativePos = Quaternion.Euler(0.0f, 180.0f, 0.0f) * relativePos;
            cameraTransform.position = outTransform.TransformPoint(relativePos);

            // Rotate the camera to look through the other portal.
            Quaternion relativeRot = Quaternion.Inverse(inTransform.rotation) * cameraTransform.rotation;
            relativeRot = Quaternion.Euler(0.0f, 180.0f, 0.0f) * relativeRot;
            cameraTransform.rotation = outTransform.rotation * relativeRot;
        }

        // Set the camera's oblique view frustum.
        Plane p = new Plane(-outTransform.forward, outTransform.position);
        Vector4 clipPlane = new Vector4(p.normal.x, p.normal.y, p.normal.z, p.distance);
        Vector4 clipPlaneCameraSpace =
            Matrix4x4.Transpose(Matrix4x4.Inverse(portalCamera.worldToCameraMatrix)) * clipPlane;

        var newMatrix = mainCamera.CalculateObliqueMatrix(clipPlaneCameraSpace);
        portalCamera.projectionMatrix = newMatrix;

        // Render the camera to its render target.
        portalCamera.Render();
    }
}