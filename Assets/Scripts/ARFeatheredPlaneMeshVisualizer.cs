using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

// Ensures the required components are present on the GameObject
[RequireComponent(typeof(ARPlaneMeshVisualizer), typeof(MeshRenderer), typeof(ARPlane))]
public class ARFeatheredPlaneMeshVisualizer : MonoBehaviour
{
    // Controls the width of the feathering effect on plane edges
    [Tooltip("The width of the texture feathering (in world units).")]
    [SerializeField]
    float m_FeatheringWidth = 0.2f;

    // Property to get/set the feathering width
    public float featheringWidth
    {
        get { return m_FeatheringWidth; }
        set { m_FeatheringWidth = value; }
    }

    // Initialize required components when the object is created
    void Awake()
    {
        m_PlaneMeshVisualizer = GetComponent<ARPlaneMeshVisualizer>();
        m_FeatheredPlaneMaterial = GetComponent<MeshRenderer>().material;
        m_Plane = GetComponent<ARPlane>();
    }

    // Subscribe to plane boundary change events when enabled
    void OnEnable()
    {
        m_Plane.boundaryChanged += ARPlane_boundaryUpdated;
    }

    // Unsubscribe from plane boundary change events when disabled
    void OnDisable()
    {
        m_Plane.boundaryChanged -= ARPlane_boundaryUpdated;
    }

    // Called when the AR plane's boundary changes
    void ARPlane_boundaryUpdated(ARPlaneBoundaryChangedEventArgs eventArgs)
    {
        GenerateBoundaryUVs(m_PlaneMeshVisualizer.mesh);
    }

    // Generates UV coordinates for the feathered edge effect
    void GenerateBoundaryUVs(Mesh mesh)
    {
        int vertexCount = mesh.vertexCount;

        // Clear and prepare the UV list
        s_FeatheringUVs.Clear();
        if (s_FeatheringUVs.Capacity < vertexCount) { s_FeatheringUVs.Capacity = vertexCount; }

        // Get vertices from the mesh
        mesh.GetVertices(s_Vertices);

        // Get the center vertex (last vertex in the list)
        Vector3 centerInPlaneSpace = s_Vertices[s_Vertices.Count - 1];
        Vector3 uv = new Vector3(0, 0, 0);
        float shortestUVMapping = float.MaxValue;

        // Calculate UV mapping for each vertex except the center
        for (int i = 0; i < vertexCount - 1; i++)
        {
            // Calculate distance from vertex to center
            float vertexDist = Vector3.Distance(s_Vertices[i], centerInPlaneSpace);

            // Calculate UV mapping based on distance and feathering width
            float uvMapping = vertexDist / Mathf.Max(vertexDist - featheringWidth, 0.001f);
            uv.x = uvMapping;

            // Keep track of shortest UV mapping
            if (shortestUVMapping > uvMapping) { shortestUVMapping = uvMapping; }

            s_FeatheringUVs.Add(uv);
        }

        // Set the shortest UV mapping in the material
        m_FeatheredPlaneMaterial.SetFloat("_ShortestUVMapping", shortestUVMapping);

        // Add UV for center vertex
        uv.Set(0, 0, 0);
        s_FeatheringUVs.Add(uv);

        // Apply UVs to the mesh
        mesh.SetUVs(1, s_FeatheringUVs);
        mesh.UploadMeshData(false);
    }

    // Static lists to reuse for UV and vertex calculations
    static List<Vector3> s_FeatheringUVs = new List<Vector3>();
    static List<Vector3> s_Vertices = new List<Vector3>();

    // Component references
    ARPlaneMeshVisualizer m_PlaneMeshVisualizer;
    ARPlane m_Plane;
    Material m_FeatheredPlaneMaterial;
}