using UnityEngine;

public class Road : MonoBehaviour
{
    float road_speed = 7f;

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.down * road_speed * Time.deltaTime);

        if (transform.position.y < -10f)
        {
            transform.position = new Vector3(0f, 10f, 0f);
        }
    }
}
