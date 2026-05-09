using UnityEngine;
using UnityEngine.InputSystem;

public class Node : MonoBehaviour
{
    [SerializeField]
    private Cubemap skybox;

    [SerializeField]
    private Vector3 defaultCameraRotation;

    [SerializeField]
    private NodePath[] pathArray;

    public Cubemap Skybox => skybox;
    public Quaternion DefaultCameraRotation => Quaternion.Euler(defaultCameraRotation);
    public NodePath ClosestPath { get; private set; }
    public bool HasClosestPath => ClosestPath != null;

    public void UpdateNode(Camera camera)
    {
        ClosestPath = null;

        if (pathArray == null)
            return;

        Ray ray = camera.ScreenPointToRay(Mouse.current.position.ReadValue());
        float minDistance = Mathf.Infinity;
        NodePath closestPath = null;

        for (int i = 0; i < pathArray.Length; i++)
        {
            NodePath path = pathArray[i];

            if (path == null)
                continue;

            path.DisableMarker();
            path.UpdatePoint(ray);

            if (path.IsPointValid && path.PointMinDistance < minDistance)
            {
                minDistance = path.PointMinDistance;
                closestPath = path;
            }
        }

        if (closestPath != null)
        {
            closestPath.UpdateMarker();
        }

        ClosestPath = closestPath;
    }
}
