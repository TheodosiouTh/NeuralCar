using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    Transform transform;
    Rigidbody rigidbody;

    Vector3 currentPosition;
    Quaternion currentRotation;

    float speed = 50f;
    float rotation = 25f;
    float[] distacneArray = new float[5];
    Vector3[] RayAngles = {
        new Vector3(0, 0, 1),
        new Vector3(0.5f, 0, 0.5f),
        new Vector3(1, 0, 0),
        new Vector3(0.5f, 0, -0.5f),
        new Vector3(0, 0, -1),
    };
    Vector3 forward;


    bool collided = false;



    // Start is called before the first frame update
    void Start()
    {
        transform = GetComponent<Transform>();
        rigidbody = GetComponent<Rigidbody>();

        // get weights
        // Initialize NN
    }

    void Update()
    {
        if (!collided)
        {
            // cast rays
            currentPosition = transform.position;
            currentRotation = transform.rotation;
            forward = transform.forward;
            for (int i = 0; i < RayAngles.Length; i++)
            {
                Vector3 angle = new Vector3(forward.x * RayAngles[i].x, forward.y * RayAngles[i].y, forward.z * RayAngles[i].z);
                distacneArray[i] = getDistance(angle);
                Debug.Log(distacneArray[i]);
            }

            // get values (x, z) from NN

            //rigidbody.velocity = transform.forward * (speed * Time.deltaTime);
            transform.Rotate(Vector3.up * (rotation * Time.deltaTime));
        }

    }

    float getDistance(Vector3 angle)
    {
        RaycastHit hit;
        Physics.Raycast(currentPosition, angle, out hit, 100);
        return hit.distance;

    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.other.name != "Ground")
        {
            collided = true;
            Debug.Log(collision.other.name);
            // send signal to manager
        }
    }
}
