using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement config")]
    public float speed;
    public float speedUp = 1f;
    private Vector3 direction = Vector3.zero;
    [Header("Stats")]
    public float health = 100f;
    [HideInInspector] public SwordPickUp.Sword sword = null;

    private float maxHealth;
    

    void Awake()
    {
        maxHealth = health;
    }

    void Update()
    {
        CalculateMovementDirection();
        if (Input.GetMouseButtonDown(0))
            Attack();
    }

    void FixedUpdate()
    {
        MovePlayer();
    }

    private void CalculateMovementDirection()
    {
        float x = Helpers.Bool2Int(Input.GetKey(KeyCode.D)) - Helpers.Bool2Int(Input.GetKey(KeyCode.A));
        float y = Helpers.Bool2Int(Input.GetKey(KeyCode.W)) - Helpers.Bool2Int(Input.GetKey(KeyCode.S));

        direction = new Vector3(x, y, 0).normalized;
    }

    private void MovePlayer()
    {
        transform.position += direction * speed * speedUp;
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

    private void Attack()
    {
        if (sword != null)
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, sword.range);
            foreach (Collider2D collider in colliders)
            {
                EnemyController enemy = collider.GetComponent<EnemyController>();
                if (enemy != null)
                {
                    enemy.TakeDamage(sword.damage);
                    sword.durability--;

                    if (sword.durability <= 0)
                    {
                        sword = null;
                        GameManager.GetInstance().playerHasSword = false;
                    }
                }
            }
        }
    }
}
