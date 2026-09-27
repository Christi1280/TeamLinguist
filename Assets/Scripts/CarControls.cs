using UnityEngine;
using UnityEngine.UIElements;

public class CarControls : MonoBehaviour
{
    float speed = 3f;
    float acceleration = 20f;

    float currentSpeed = 0f;
    float rotateSpeed = 200f;

    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        

        float input = 0f;

        if((Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)))
        {
            input = -1f;
        }
        if((Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)))
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
        //rb.MovePosition( rb.position + (Vector2)(Vector3.down * currentSpeed * Time.deltaTime));

        

    }

    void FixedUpdate()
    {
        Vector2 movement = transform.TransformDirection(Vector3.down);

        rb.MovePosition( rb.position + movement * currentSpeed * Time.fixedDeltaTime);

        //rotate car
        float horizontalInput = Input.GetAxis("Horizontal");
        transform.Rotate(Vector3.forward, -horizontalInput * rotateSpeed * Time.deltaTime);

        
    }


}
