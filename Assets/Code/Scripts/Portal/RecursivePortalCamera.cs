using UnityEngine;

public class RecursivePortalCamera : MonoBehaviour
{
    [SerializeField]
    private PortalBehaviour portal;
    [SerializeField] 
    private PortalBehaviour mirrorPortal;

    [SerializeField]
    private Camera portalCamera;

    private const int iterations = 7;


    private void OnPreRender()
    {
        if (!portal.IsPlaced)
        {
            return;
        }

        if (portal.IsRendererVisible())
        {
            for (int i = iterations - 1; i >= 0; --i)
            {
                RenderCamera(portal, mirrorPortal, i);
            }
        }
    }

    private void RenderCamera(PortalBehaviour inPortal, PortalBehaviour outPortal, int iterationID)
    {
        Transform inTransform = inPortal.transform;
        Transform outTransform = outPortal.transform;

        Transform mainCameraTransform = Camera.main.transform;
        
        Transform cameraTransform = portalCamera.transform;
        cameraTransform.position = mainCameraTransform.position;
        cameraTransform.rotation = mainCameraTransform.rotation;

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
        Plane p = new Plane(outTransform.forward, outTransform.position);
        Vector4 clipPlane = new Vector4(p.normal.x, p.normal.y, p.normal.z, p.distance);
        Vector4 clipPlaneCameraSpace =
            Matrix4x4.Transpose(Matrix4x4.Inverse(portalCamera.worldToCameraMatrix)) * clipPlane;

        var newMatrix = Camera.main.CalculateObliqueMatrix(clipPlaneCameraSpace);
        portalCamera.projectionMatrix = newMatrix;

        // Render the camera to its render target.
        portalCamera.Render();
    }
}