using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    public Transform target;

    [Header("Follow")]
    public float followSpeed = 8f;
    public float rotationSpeed = 8f;

    [Header("Camera")]
    public float cameraSize = 5f;

    [Header("Camera Bounds")]
    public float minX = -20f;
    public float maxX = 20f;
    public float minY = -20f;
    public float maxY = 20f;

    private float rotationVelocity = 0f;

    public float rotationSmoothTime = 0.5f;

    void Start()
    {
        Camera.main.orthographicSize = cameraSize;
    }

    void LateUpdate()
    {
        if (target == null)
            return;

        // -------------------------
        // FOLLOW CAR
        // -------------------------

        Vector3 targetPosition = target.position;
        targetPosition.z = transform.position.z;

        Vector3 newPosition = Vector3.Lerp(
            transform.position,
            targetPosition,
            followSpeed * Time.deltaTime
        );

        // -------------------------
        // CAMERA BOUNDS
        // -------------------------

        float verticalSize = Camera.main.orthographicSize;
        float horizontalSize = verticalSize * Camera.main.aspect;

        newPosition.x = Mathf.Clamp(
            newPosition.x,
            minX + horizontalSize,
            maxX - horizontalSize
        );

        newPosition.y = Mathf.Clamp(
            newPosition.y,
            minY + verticalSize,
            maxY - verticalSize
        );

        transform.position = newPosition;

        // -------------------------
        // ROTATE CAMERA
        // -------------------------

        float carRotation = target.eulerAngles.z;

        // add 180 degrees if car is down
        float desiredRotation = -carRotation + 180f;

        float smoothRotation = Mathf.SmoothDampAngle(
            transform.eulerAngles.z,
            desiredRotation,
            ref rotationVelocity,
            rotationSmoothTime
        );

        transform.rotation = Quaternion.Euler(
            0f,
            0f,
            smoothRotation
        );
    }
}