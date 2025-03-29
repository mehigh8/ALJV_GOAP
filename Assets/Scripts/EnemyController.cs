using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("Stats")]
    public float health = 100f;
    [HideInInspector] public SwordPickUp.Sword sword = null;
    [Header("References")]
    public CharacterMovement characterController;

    private float maxHealth;

    void Awake()
    {
        maxHealth = health;
    }

    void Update()
    {
        
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health < 0) 
            health = 0;
    }

    public void Heal(float health)
    {
        this.health += health;
        if (this.health > maxHealth)
            this.health = maxHealth;
    }
}
