
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Follow Settings")]
    [SerializeField] private Transform target;
    [SerializeField] private Vector2 offset = Vector2.zero;

    [Header("Rotation Settings")]
    [SerializeField] private float rotationSmoothSpeed = 180f;

    private void LateUpdate()
    {
        if (target == null)
            return;

        // Follow position directly to avoid camera jitter.
        transform.position = new Vector3(
            target.position.x + offset.x,
            target.position.y + offset.y,
            -10f
        );

        // Smoothly rotate toward the car's direction.
        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            Quaternion.Euler(0f, 0f, target.eulerAngles.z),
            rotationSmoothSpeed * Time.deltaTime
        );
    }
}
