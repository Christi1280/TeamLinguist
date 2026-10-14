using UnityEngine;

public class DecisionPoint : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        CarMovement car = other.GetComponent<CarMovement>();

        if (car == null)
            return;

        car.StopDriving();

        Debug.Log("Intersection reached. Waiting for directions!");
    }
}
