using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    [Header("Config")]
    public float speed;
    public float speedUp = 1f;
    [Tooltip("How close to get to destination")]
    public float epsilon;
    [Space]
    public bool isMoving = false;

    [SerializeField] private Vector3 destination;
    [SerializeField] private Vector3 direction = Vector3.zero;
    private float distanceLastFrame;

    void Update()
    {
        if (isMoving)
        {
            float distance = Vector3.Distance(destination, transform.position);
            if (distance < epsilon || distance > distanceLastFrame) // Second condition is in case of overshooting
                StopMoving();

            transform.position += direction * speed * Time.deltaTime * speedUp;
        }
    }

    public void MoveTowards(Vector3 destination)
    {
        isMoving = true;
        this.destination = destination;
        direction = (destination - transform.position).normalized;
        direction.z = 0;
        distanceLastFrame = Vector3.Distance(destination, transform.position);
    }

    public void StopMoving()
    {
        isMoving = false;
        direction = Vector3.zero;
    }
}
