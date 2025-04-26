using System;
using System.Collections;
using System.Collections.Generic;
using static Helpers;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class PlayerSensor : MonoBehaviour
{
    [SerializeField] float detectionRadius = 5f;
    [SerializeField] float timerInterval = 1f;

    CircleCollider2D detectionRange;

    public event Action OnTargetChanged = delegate { };
    [HideInInspector] public float initialRadius;

    public Vector3 targetPosition => target ? target.transform.position : Vector3.zero;
    public bool isTargetInRange => targetPosition != Vector3.zero;

    GameObject target;
    Vector3 lastKnownPosition;
    CountdownTimer timer;

    bool wasInside = false;

    void Awake()
    {
        initialRadius = detectionRadius;
        detectionRange = GetComponent<CircleCollider2D>();
        detectionRange.isTrigger = true;
        detectionRange.radius = detectionRadius;
    }

    void Start()
    {
        timer = new CountdownTimer(timerInterval);
        timer.OnTimerStop += () => {
            object targetObject = target;
            UpdateTargetPosition(targetObject != null ? target : null);
            timer.Start();
        };
        timer.Start();
    }

    void Update()
    {
        timer.Update(Time.deltaTime);

        bool found = false;
        Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, detectionRadius);
        foreach (Collider2D col in cols)
        {
            if (col.CompareTag("Player"))
            {
                found = true;
                if (wasInside == false)
                {
                    wasInside = true;
                    UpdateTargetPosition(col.gameObject);
                }
            }
        }

        if (found == false && wasInside == true)
        {
            wasInside = false;
            UpdateTargetPosition();
        }
    }

    void UpdateTargetPosition(GameObject target = null)
    {
        this.target = target;
        if (isTargetInRange && (lastKnownPosition != targetPosition || lastKnownPosition != Vector3.zero))
        {
            lastKnownPosition = targetPosition;
            OnTargetChanged.Invoke();
        }
    }

    //void OnTriggerEnter2D(Collider2D other)
    //{
    //    if (!other.CompareTag("Player")) return;
    //    UpdateTargetPosition(other.gameObject);
    //}

    //void OnTriggerExit2D(Collider2D other)
    //{
    //    if (!other.CompareTag("Player")) return;
    //    UpdateTargetPosition();
    //}

    public void UpdateRange(float range)
    {
        detectionRadius = range;
        detectionRange.radius = range;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = isTargetInRange ? Color.red : Color.green;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
