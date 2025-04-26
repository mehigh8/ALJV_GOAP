using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectPowerupSensor : MonoBehaviour
{
    public float detectionRadius;

    private void Update()
    {
        GameManager gm = GameManager.GetInstance();
        Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, detectionRadius);
        foreach (Collider2D col in cols)
        {
            Interactable interactable = col.GetComponent<Interactable>();
            if (interactable != null)
            {
                if (interactable is HealPickUp && !gm.goapAgent.heals.Contains(col.transform.position))
                    gm.goapAgent.heals.Add(col.transform.position);
                if (interactable is SpeedPickUp && !gm.goapAgent.speedUps.Contains(col.transform.position))
                    gm.goapAgent.speedUps.Add(col.transform.position);
                if (interactable is SwordPickUp && !gm.goapAgent.swords.Contains(col.transform.position))
                    gm.goapAgent.swords.Add(col.transform.position);
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
