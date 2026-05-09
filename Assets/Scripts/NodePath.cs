using UnityEngine;
using UnityEngine.InputSystem;

[ExecuteInEditMode]
public class NodePath : MonoBehaviour
{
    public readonly struct PathPoint
    {
        public readonly Vector3 position;
        public readonly Vector3 forward;
        public readonly float minDistance;

        public PathPoint(Vector3 position, Vector3 forward, float minDistance)
        {
            this.position = position;
            this.forward = forward;
            this.minDistance = minDistance;
        }
    }

    [SerializeField]
    private Node connectedNode;

    [SerializeField]
    private Vector3 cameraRotation;

    [SerializeField]
    private Transform marker;

    [SerializeField]
    private float markerVisibleThreshold = 2.0f;

#if UNITY_EDITOR
    [SerializeField]
    private Color pathColor = Color.white;
#endif

    [Space]
    [SerializeField]
    private Transform[] waypointArray;

    public Node ConnectedNode => connectedNode;
    public Quaternion CameraRotation => Quaternion.Euler(cameraRotation);
    public Vector3 PointPosition { get; private set; }
    public Vector3 PointForward { get; private set; }
    public float PointMinDistance { get; private set; }
    public bool IsPointValid => PointMinDistance < markerVisibleThreshold;

#if UNITY_EDITOR

    private void Update()
    {
        if (Application.isPlaying)
            return;

        if (marker != null && waypointArray != null && waypointArray.Length > 1)
        {
            if (waypointArray[0] != null && waypointArray[1] != null)
            {
                Vector3 a = waypointArray[0].position;
                Vector3 b = waypointArray[1].position;

                marker.position = (a + b) * 0.5f;
                marker.forward = (b - a).normalized;
            }
        }
    }

    private void OnDrawGizmos()
    {
        DrawWaypoints();
    }

    private void DrawWaypoints()
    {
        if (waypointArray == null || waypointArray.Length < 2)
            return;

        for (int i = 0; i < waypointArray.Length - 1; i++)
        {
            if (waypointArray[i] != null && waypointArray[i + 1] != null)
            {
                Gizmos.color = pathColor;
                Gizmos.DrawLine(waypointArray[i].position, waypointArray[i + 1].position);
            }
        }
    }

#endif

    public void UpdatePoint(Camera camera)
    {
        Ray ray = camera.ScreenPointToRay(Mouse.current.position.ReadValue());
        UpdatePoint(ray);
    }

    public void UpdatePoint(Ray ray)
    {
        PathPoint pathPointInfo = GetMousePointOnPath(ray, waypointArray);

        PointPosition = pathPointInfo.position;
        PointForward = pathPointInfo.forward;
        PointMinDistance = pathPointInfo.minDistance;
    }

    public void UpdateMarker()
    {
        marker.position = PointPosition;
        marker.forward = PointForward;
        marker.gameObject.SetActive(IsPointValid);
    }

    public void DisableMarker()
    {
        marker.gameObject.SetActive(false);
    }

    private static Vector3 ClosestPointOnSegmentToRay(Vector3 a, Vector3 b, Ray ray)
    {
        Vector3 ab = b - a;
        Vector3 ao = a - ray.origin;

        float abDotAb = Vector3.Dot(ab, ab);
        float abDotDir = Vector3.Dot(ab, ray.direction);
        float dirDotDir = Vector3.Dot(ray.direction, ray.direction);
        float abDotAo = Vector3.Dot(ab, ao);
        float dirDotAo = Vector3.Dot(ray.direction, ao);

        float denom = abDotAb * dirDotDir - abDotDir * abDotDir;

        float t = (abDotDir * dirDotAo - dirDotDir * abDotAo) / denom;
        t = Mathf.Clamp01(t);

        return a + ab * t;
    }

    private static PathPoint GetMousePointOnPath(Ray ray, Transform[] waypointArray)
    {
        Vector3 bestPoint = Vector3.zero;
        Vector3 forward = Vector3.zero;
        float minDist = Mathf.Infinity;

        for (int i = 0; i < waypointArray.Length - 1; i++)
        {
            Vector3 a = waypointArray[i].position;
            Vector3 b = waypointArray[i + 1].position;

            Vector3 pointOnSegment = ClosestPointOnSegmentToRay(a, b, ray);

            // Distance from ray to that point
            float dist = Vector3.Cross(ray.direction, pointOnSegment - ray.origin).magnitude;

            if (dist < minDist)
            {
                minDist = dist;
                bestPoint = pointOnSegment;
                forward = (b - a).normalized;
            }
        }

        return new PathPoint(bestPoint, forward, minDist);
    }

}