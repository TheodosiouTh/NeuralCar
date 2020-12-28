using System;
using UnityEngine;

public class Character : MonoBehaviour
{
    public NeuralNetwork brain;

    public float movementSpeed;
    public int rotationSpeed;

    Rigidbody rigidBody;
    Transform transform;

    Vector3 currentPosition;

    Vector3[] angles;
    float[] distances = new float[8];

    bool dead = false;

    int lastCheckpoint = 0;
    float lastCheckpointHitTime;


    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        transform = GetComponent<Transform>();

        angles = new Vector3[8] {
            -transform.right,
            transform.forward - transform.right,
            transform.forward,
            transform.forward + transform.right,
            transform.right,
            -transform.forward + transform.right,
            -transform.forward,
            -transform.forward - transform.right,
        };

        lastCheckpointHitTime = Time.time;
        lastCheckpoint = 0;
    }

    // Update is called once per frame
    void Update()
    {
        currentPosition = transform.position;
        if( Mathf.Abs(currentPosition.z) > 8.5f || Mathf.Abs(currentPosition.x) > 14)
        {
            Die();
        }

        if (!dead) {
            distances = calculateDistances();
            float[] output = brain.FeedForward(distances);
            transform.Translate(transform.forward * (output[0] * Time.deltaTime));
            transform.Rotate(Vector3.up * (output[1] * rotationSpeed * Time.deltaTime));

            if (Time.time - lastCheckpointHitTime > Manager.timeToDeath)
            {
                Die();
                brain.fitness -= 5;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name.Contains("Wall") && !dead)
        {
            Die();
        }

        if (other.gameObject.name.Contains("Checkpoint"))
        {
            int checkpointHit = Int32.Parse(other.gameObject.name.Split(' ')[1]);
            if(checkpointHit == (lastCheckpoint + 1) || (lastCheckpoint == 17 && checkpointHit == 1))
            {
                brain.fitness += 5;
                lastCheckpoint = checkpointHit;
                lastCheckpointHitTime = Time.time;
            }
            else
            {
                brain.fitness -= 10;
            }
        }
    }

    private float[] calculateDistances()
    {
        float[] distancesFromWalls = new float[8];
        for (int i = 0; i < angles.Length; i++)
        { 
            distancesFromWalls[i] = castRay(angles[i]);
        }
        return distancesFromWalls;
    }

    private float castRay(Vector3 angle)
    {
        RaycastHit hit;
        Physics.Raycast(transform.position, transform.TransformDirection(angle), out hit, Mathf.Infinity);
        Debug.DrawRay(transform.position, transform.TransformDirection(angle) * hit.distance, Color.yellow);
        return hit.distance;
    }

    private void Die()
    {
        if (brain.fitness == 0) brain.fitness -= 2;
        Manager.dead += 1;
        dead = true;
    }
}
