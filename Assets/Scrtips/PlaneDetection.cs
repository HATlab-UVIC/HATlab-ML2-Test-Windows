using UnityEngine;
using UnityEngine.XR.MagicLeap;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using Unity.VisualScripting;

public class PlaneDetection : MonoBehaviour
{
    public GameObject planeSpawner;

    private ARPlaneManager planeManager;

    [SerializeField, Tooltip("Maximum number of planes to return each query")]
    private uint maxResults = 100;

    [SerializeField, Tooltip("Minimum plane area to treat as a valid plane")]
    private float minPlaneArea = 0.25f;

    private const int GRAY_PLANE_QUEUE = 3001;
    private const int DEFAULT_PLANE_QUEUE = 3000;

    private readonly MLPermissions.Callbacks permissionCallbacks = new MLPermissions.Callbacks();

    private void Awake()
    {
        // subscribe to permission events
        permissionCallbacks.OnPermissionGranted += PermissionCallbacks_OnPermissionGranted;
        permissionCallbacks.OnPermissionDenied += PermissionCallbacks_OnPermissionDenied;
        permissionCallbacks.OnPermissionDeniedAndDontAskAgain += PermissionCallbacks_OnPermissionDenied;

    }

    private void OnDestroy()
    {
        // unsubscribe from permission events
        permissionCallbacks.OnPermissionGranted -= PermissionCallbacks_OnPermissionGranted;
        permissionCallbacks.OnPermissionDenied -= PermissionCallbacks_OnPermissionDenied;
        permissionCallbacks.OnPermissionDeniedAndDontAskAgain -= PermissionCallbacks_OnPermissionDenied;
    }

    private void Start()
    {
        // make sure the plane manager is disabled at the start of the scene before permissions are granted
        planeManager = FindObjectOfType<ARPlaneManager>();
        if (planeManager == null)
        {
            Debug.LogError("Failed to find ARPlaneManager in scene. Disabling Script");
            enabled = false;
        }
        else
        {
            planeManager.enabled = false;
        }

        // request spatial mapping permission for plane detection
        MLPermissions.RequestPermission(MLPermission.SpatialMapping, permissionCallbacks);

    }

    // if permission denied, disable plane manager
    private void PermissionCallbacks_OnPermissionDenied(string permission)
    {
        Debug.LogError($"Failed to create Planes Subsystem due to missing or denied {MLPermission.SpatialMapping} permission. Please add to manifest. Disabling script.");
        planeManager.enabled = false;
    }

    // if permission granted, enable plane manager
    private void PermissionCallbacks_OnPermissionGranted(string permission)
    {
        if (permission == MLPermission.SpatialMapping)
        {
            planeManager.enabled = true;
            Debug.Log("Plane manager is active");
        }
    }

    private void Update()
    {
        if (planeManager.enabled)
        {
            PlanesSubsystem.Extensions.Query = new PlanesSubsystem.Extensions.PlanesQuery
            {
                // Detects Ceiling and Floor. For more information of flags: https://developer-docs.magicleap.cloud/docs/api-ref/api/Modules/group___planes#enums-mlplanesqueryflags
                Flags = planeManager.requestedDetectionMode.ToMLQueryFlags() | PlanesSubsystem.Extensions.MLPlanesQueryFlags.Polygons | PlanesSubsystem.Extensions.MLPlanesQueryFlags.Semantic_Ceiling | PlanesSubsystem.Extensions.MLPlanesQueryFlags.Semantic_Floor,
                BoundsCenter = Camera.main.transform.position,
                BoundsRotation = Camera.main.transform.rotation,
                BoundsExtents = Vector3.one * 20f,
                MaxResults = maxResults,
                MinPlaneArea = minPlaneArea
            };
        }
    }

    public void Lock()
    {
        foreach (var plane in planeManager.trackables)
        {
            var temp = Instantiate(plane);
            var planeClassification = plane.GetComponent<ARPlane>();

            Debug.Log(planeClassification.classification);

            Color color = Color.gray;
            switch (planeClassification.classification)
            {
                case PlaneClassification.Floor:
                    color = Color.yellow;
                    break;
                case PlaneClassification.Ceiling:
                    color = Color.white;
                    break;
                case PlaneClassification.Wall:
                    color = Color.red;
                    break;
            }

            temp.GetComponent<ARPlaneMeshVisualizer>().enabled = false;

            temp.GetComponent<MeshRenderer>().enabled = true;
            temp.GetComponent<PlanePrefabExample>().enabled = false;

            var mat = temp.GetComponent<MeshRenderer>().material;
            mat.color = color;
            mat.renderQueue = color == Color.gray ? GRAY_PLANE_QUEUE : DEFAULT_PLANE_QUEUE;


            temp.transform.parent = planeSpawner.transform;
            plane.gameObject.SetActive(false);
        }
        planeManager.enabled = false;
    }

    public void Unlock()
    {
        planeManager.enabled = true;
        foreach (var plane in planeManager.trackables)
        {
            plane.gameObject.SetActive(true);
        }
        foreach( Transform child in planeSpawner.transform)
        {
            Destroy(child.gameObject);
        }
    }

    public void OpenWindow()
    {

    }
    public void CloseWindow()
    {

    }
}