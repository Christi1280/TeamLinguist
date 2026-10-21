using UnityEngine;
using UnityEngine.UIElements;

public class CarControls : MonoBehaviour
{
    float speed = 3f;
    //float acceleration = 20f;

    //float currentSpeed = 0f;
    float rotateSpeed = 500f;
    float arrivalDistance = 0.5f; //distance at which the car is considered to have reached the point
    bool atPoint = false; //flag to indicate if the car has reached the point

    public Vector3 nextPoint; //car has to point to and follow this point until it reaches it
    public GameObject point1;
    public GameObject point2;
    public GameObject point3;
    public GameObject point4;
    public GameObject point5;
    public GameObject point6;
    public GameObject point7;
    public GameObject point8;
    public GameObject point9;
    public GameObject point10;
    public GameObject point11;
    public GameObject point12;
    public GameObject point13;

    void Start()
    {
        nextPoint = point1.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        // Direction toward the target
        Vector3 direction = nextPoint - transform.position;
        direction.z = 0f;

        if (direction.sqrMagnitude > 0.01f)
        {
            // Rotate toward the target
            float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
            Quaternion targetRotation = Quaternion.Euler(0f, 0f, targetAngle);

            transform.rotation = Quaternion.RotateTowards
            (
                transform.rotation,
                targetRotation,
                rotateSpeed * Time.deltaTime
            );

            // Move forward toward the target
            transform.Translate(Vector3.up * speed * Time.deltaTime);
        }

        float distance = Vector3.Distance(
        transform.position,
        nextPoint);

        if (distance <= arrivalDistance)
        {
            Debug.Log("Car reached point " + nextPoint);
            atPoint = true;
        }

        //what the next turn is based on what point the car is currently at
        if (atPoint)
        {
            if(nextPoint == point1.transform.position)
            {
                if (Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    nextPoint = point2.transform.position;
                    atPoint = false;
                }
                if (Input.GetKeyDown(KeyCode.RightArrow))
                {
                    nextPoint = point3.transform.position;
                    atPoint = false;
                }
            }
            else if (nextPoint == point2.transform.position)
            {
                if (Input.GetKeyDown(KeyCode.RightArrow))
                {
                    nextPoint = point3.transform.position;
                    atPoint = false;
                }
            }
            else if (nextPoint == point3.transform.position)
            {
                if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    nextPoint = point4.transform.position;
                    Debug.Log("moving to point 4, position: " + point4.transform.position + "actual point: " + nextPoint);
                    atPoint = false;
                }
                if (Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    nextPoint = point1.transform.position;
                    atPoint = false;
                }
            }
            else if (nextPoint == point4.transform.position)
            {
                if (Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    nextPoint = point5.transform.position;
                    atPoint = false;
                }
                if(Input.GetKeyDown(KeyCode.UpArrow))
                {
                    nextPoint = point6.transform.position;
                    atPoint = false;
                }
                if (Input.GetKeyDown(KeyCode.RightArrow))
                {
                    nextPoint = point7.transform.position;
                    atPoint = false;
                }
                if (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    nextPoint = point3.transform.position;
                    atPoint = false;
                }
            }
            else if (nextPoint == point5.transform.position)
            {
                if (Input.GetKeyDown(KeyCode.RightArrow))
                {
                    nextPoint = point4.transform.position;
                    atPoint = false;
                }
            }
            else if (nextPoint == point6.transform.position)
            {
                if (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    nextPoint = point4.transform.position;
                    atPoint = false;
                }
            }
            else if (nextPoint == point7.transform.position)
            {
                if (Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    nextPoint = point4.transform.position;
                    atPoint = false;
                }
                if (Input.GetKeyDown(KeyCode.RightArrow))
                {
                    nextPoint = point8.transform.position;
                    atPoint = false;
                }
                if (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    nextPoint = point11.transform.position;
                    atPoint = false;
                }
            }
            else if (nextPoint == point8.transform.position)
            {
                if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    nextPoint = point9.transform.position;
                    atPoint = false;
                }
                if (Input.GetKeyDown(KeyCode.RightArrow))
                {
                    nextPoint = point10.transform.position;
                    atPoint = false;
                }
                if (Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    nextPoint = point7.transform.position;
                    atPoint = false;
                }
            }
            else if (nextPoint == point9.transform.position)
            {
                if (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    nextPoint = point9.transform.position;
                    atPoint = false;
                }
            }
            else if (nextPoint == point10.transform.position)
            {
                if (Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    nextPoint = point8.transform.position;
                    atPoint = false;
                }
            }
            else if (nextPoint == point11.transform.position)
            {
                if (Input.GetKeyDown(KeyCode.RightArrow))
                {
                    nextPoint = point12.transform.position;
                    atPoint = false;
                }
                if (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    nextPoint = point13.transform.position;
                    atPoint = false;
                }
                if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    nextPoint = point7.transform.position;
                    atPoint = false;
                }
            }
            else if (nextPoint == point12.transform.position)
            {
                if (Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    nextPoint = point11.transform.position;
                    atPoint = false;
                }

            }
            else if (nextPoint == point13.transform.position)
            {
                if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    nextPoint = point11.transform.position;
                    atPoint = false;
                }

            }
        }
    }



}
