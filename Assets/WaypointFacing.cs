using UnityEngine;

public class WaypointFacing : MonoBehaviour
{
    public enum FacingDirection
    {
        Up,
        Down,
        Left,
        Right
    }

    [SerializeField]
    private FacingDirection direction = FacingDirection.Down;

    public Vector2 GetDirection()
    {
        switch (direction)
        {
            case FacingDirection.Up:
                return Vector2.up;

            case FacingDirection.Down:
                return Vector2.down;

            case FacingDirection.Left:
                return Vector2.left;

            case FacingDirection.Right:
                return Vector2.right;

            default:
                return Vector2.down;
        }
    }
}
