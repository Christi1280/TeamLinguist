using UnityEngine;
using UnityEngine.UIElements;

public class CarControls : MonoBehaviour
{
    float speed = 7f;
    float acceleration = 20f;

    float currentSpeed = 0f;

    // Update is called once per frame
    void Update()
    {
        Vector3 position = transform.position;
        position.x = Mathf.Clamp(position.x, -5.8f, 5.8f);
        transform.position = position;

        float input = 0f;

        if((Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)))
        {
            input = -1f;
        }
        if((Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)))
        {
            input = 1f;
        }

        //acceleration and deceleration
        if(input != 0f)
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, input * speed, acceleration * Time.deltaTime);
        }
        else
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0f, acceleration * Time.deltaTime);
        }

        //move car
        transform.Translate(Vector3.right * currentSpeed * Time.deltaTime);
    }
}
