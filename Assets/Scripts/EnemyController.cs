using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private Helpers.HealthBar healthBar;
    [HideInInspector] public SwordPickUp.Sword sword = null;
    [Header("References")]
    public CharacterMovement characterController;
    public GameObject swordIndicator;


    void Awake()
    {

    }

    void Update()
    {
        // For testing
        if (sword != null && Input.GetMouseButtonDown(1))
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, sword.range);
            foreach (Collider2D collider in colliders)
            {
                PlayerController player = collider.GetComponent<PlayerController>();
                if (player != null)
                {
                    player.TakeDamage(sword.damage);
                    sword.durability--;

                    if (sword.durability <= 0)
                    {
                        sword = null;
                        GameManager.GetInstance().enemyHasSword = false;
                        swordIndicator.SetActive(false);
                    }
                }
            }
        }
    }

    public void TakeDamage(float damage)
    {
        healthBar.TakeDamage(damage);
    }

    public void Heal(float health)
    {
        healthBar.Heal(health);
    }

    public SwordPickUp.Sword GainSword(SwordPickUp.Sword sword)
    {
        GameManager.GetInstance().enemyHasSword = true;
        swordIndicator.SetActive(true);
        SwordPickUp.Sword old = this.sword;

        this.sword = sword;
        return old;
    }
}
