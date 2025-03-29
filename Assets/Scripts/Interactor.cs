using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactor : MonoBehaviour
{
    [Header("Config")]
    public float interactionRange;
    public bool isPlayer;

    void Start()
    {
        
    }

    void Update()
    {
        bool interactCondition = true; // true by default
        if (isPlayer) // for player E must be pressed
            interactCondition = Input.GetKeyDown(KeyCode.E);

        Collider2D[] collidersInRange = Physics2D.OverlapCircleAll(transform.position, interactionRange);
        foreach (Collider2D collider in collidersInRange)
        {
            Interactable interactable = collider.GetComponent<Interactable>();
            if (interactable != null && interactCondition)
            {
                interactable.Interact(this);
                break;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}
