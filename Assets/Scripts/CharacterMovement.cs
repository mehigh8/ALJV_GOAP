using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    [Header("Config")]
    public float speed;
    [Tooltip("How close to get to destination")]
    public float epsilon;
    [Space]
    public bool isMoving = false;

    private Vector3 destination;
    private Vector3 direction = Vector3.zero;

    void Update()
    {
        if (isMoving)
        {
            if (Vector3.Distance(destination, transform.position) < epsilon)
                StopMoving();

            transform.position += direction * speed * Time.deltaTime;
        }
    }

    public void MoveTowards(Vector3 destination)
    {
        isMoving = true;
        this.destination = destination;
        direction = (destination - transform.position).normalized;
        direction.z = 0;
    }

    public void StopMoving()
    {
        isMoving = false;
        direction = Vector3.zero;
    }
}
