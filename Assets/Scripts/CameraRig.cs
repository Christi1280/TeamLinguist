using UnityEngine;

public class CameraRig : MonoBehaviour
{
    [Header("Target")]
    public Transform target;

    [Header("Follow")]
    public float followSpeed = 8f;

    [Header("Rotation")]
    public float rotationSmoothTime = 0.5f;

    [Header("Map Bounds")]
    public BoxCollider2D mapBounds;

    private float rotationVelocity = 0f;

    private bool initialized = false;

    void LateUpdate()
    {

        if (target == null)
            return;

        Camera cam = Camera.main;

        if (!initialized)
        {
            transform.position = new Vector3(
                target.position.x,
                target.position.y,
                transform.position.z
            );

            initialized = true;
        }

        // ==========================================
        // FOLLOW CAR
        // ==========================================

        Vector3 targetPosition = target.position;
        targetPosition.z = transform.position.z;

        Vector3 newPosition = Vector3.Lerp(
            transform.position,
            targetPosition,
            followSpeed * Time.deltaTime
        );

        // ==========================================
        // CAMERA ROTATION
        // ==========================================

        float carRotation = target.eulerAngles.z;

        // Snap car rotation to nearest 90 degrees
        float snappedRotation = Mathf.Round(carRotation / 90f) * 90f;

        // Your car faces DOWN
        float desiredRotation = -snappedRotation + 180f;

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

        // ==========================================
        // CAMERA BOUNDS
        // ==========================================

        if (mapBounds != null)
        {
            Bounds bounds = mapBounds.bounds;

            float halfHeight = cam.orthographicSize;
            float halfWidth = halfHeight * cam.aspect;

            // Camera rotation in radians
            float angle = transform.eulerAngles.z * Mathf.Deg2Rad;

            float cos = Mathf.Abs(Mathf.Cos(angle));
            float sin = Mathf.Abs(Mathf.Sin(angle));

            // Account for rotated camera
            float rotatedWidth =
                halfWidth * cos +
                halfHeight * sin;

            float rotatedHeight =
                halfWidth * sin +
                halfHeight * cos;

            float minX = bounds.min.x + rotatedWidth;
            float maxX = bounds.max.x - rotatedWidth;

            float minY = bounds.min.y + rotatedHeight;
            float maxY = bounds.max.y - rotatedHeight;

            newPosition.x = Mathf.Clamp(
                newPosition.x,
                minX,
                maxX
            );

            newPosition.y = Mathf.Clamp(
                newPosition.y,
                minY,
                maxY
            );
        }

        // ==========================================
        // APPLY POSITION
        // ==========================================

        transform.position = newPosition;
    }
}