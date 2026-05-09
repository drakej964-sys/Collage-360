using UnityEngine;
using UnityEngine.InputSystem;

public class WorldInteractor : MonoBehaviour
{
    [SerializeField]
    private Node startNode;

    [SerializeField]
    private Material skyboxMaterial;

    [SerializeField]
    private CameraController cameraController;

    private Camera mainCamera;
    private Node currentNode;
    private Vector2 downMousePosition;
    private bool isLMBDown;

    private void Awake()
    {
        isLMBDown = false;
        mainCamera = Camera.main;
        currentNode = null;
    }

    private void OnEnable()
    {
        cameraController.OnAutoRotationCompleted += OnAutoRotationCompleted;
    }

    private void OnDisable()
    {
        cameraController.OnAutoRotationCompleted -= OnAutoRotationCompleted;
    }

    private void Start()
    {
        SpawnNode(startNode);
    }

    private void Update()
    {
        if (CameraController.IsUserRotatingCamera || CameraController.IsCameraAutoRotating)
        {
            if (currentNode != null && currentNode.ClosestPath != null)
                currentNode.ClosestPath.DisableMarker();

            isLMBDown = false;
            return;
        }

        if (currentNode != null)
            currentNode.UpdateNode(mainCamera);

        UpdatePathClicking();
    }

    private void UpdatePathClicking()
    {
        if (currentNode == null || currentNode.ClosestPath == null)
            return;

        var mouseButton = Mouse.current.leftButton;

        if (mouseButton.wasPressedThisFrame)
        {
            isLMBDown = true;
            downMousePosition = Mouse.current.position.ReadValue();
        }

        if (mouseButton.isPressed)
        {
            if (isLMBDown && Vector2.Distance(downMousePosition, Mouse.current.position.ReadValue()) > 10.0f)
                isLMBDown = false;
        }

        if (mouseButton.wasReleasedThisFrame)
        {
            if (isLMBDown)
                MoveToNextNode(currentNode.ClosestPath.ConnectedNode, currentNode.ClosestPath);

            isLMBDown = false;
        }
    }

    private void MoveToNextNode(Node nextNode, NodePath path)
    {
        if (nextNode == null)
            return;

        SpawnNode(nextNode);

        if (path != null)
            cameraController.SetRotation(path.CameraRotation);
    }

    private void SetSkybox(Cubemap skybox)
    {
        skyboxMaterial.SetTexture("_Tex", skybox);
    }

    private void SpawnNode(Node node)
    {
        if (node == null)
            return;

        Node newNode = Instantiate(node, Vector3.zero, Quaternion.identity);

        newNode.gameObject.SetActive(true);
        SetSkybox(newNode.Skybox);
        cameraController.SetRotation(newNode.DefaultCameraRotation);

        if (currentNode != null)
            Destroy(currentNode.gameObject);

        currentNode = newNode;
    }

    private void OnAutoRotationCompleted()
    {

    }
}
