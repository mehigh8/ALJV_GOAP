using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [Header("Movement config")]
    public float speed;
    public float speedUp = 1f;
    private Vector3 direction = Vector3.zero;
    [Header("Stats")]
    public Helpers.HealthBar healthBar;
    [HideInInspector] public SwordPickUp.Sword sword = null;
    [Header("References")]
    public GameObject swordIndicator;


    void Awake()
    {

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
        healthBar.TakeDamage(damage);
    }

    public void Heal(float health)
    {
        healthBar.Heal(health);
    }

    private void Attack()
    {
        if (sword != null)
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, sword.range);
            foreach (Collider2D collider in colliders)
            {
                if (collider.isTrigger)
                    continue;

                EnemyController enemy = collider.GetComponent<EnemyController>();
                if (enemy != null)
                {
                    enemy.TakeDamage(sword.damage);
                    GameManager.GetInstance().goapAgent.playerDamage = sword.damage;
                    sword.durability--;

                    if (sword.durability <= 0)
                    {
                        sword = null;
                        GameManager.GetInstance().playerHasSword = false;
                        swordIndicator.SetActive(false);
                    }
                }
            }
        }
    }

    public SwordPickUp.Sword GainSword(SwordPickUp.Sword sword)
    {
        GameManager.GetInstance().playerHasSword = true;
        swordIndicator.SetActive(true);
        SwordPickUp.Sword old = this.sword;

        this.sword = sword;
        return old;
    }
}
