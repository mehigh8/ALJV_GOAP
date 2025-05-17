using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Helpers;
using UnityEngine.AI;
using static SwordPickUp;
using static UnityEngine.RuleTile.TilingRuleOutput;

public interface IActionStrategy
{
    bool CanPerform { get; }
    bool Complete { get; }

    void Start() {}

    void Update(float deltaTime) {}

    void Stop() {}
}

public class AttackStrategy : IActionStrategy
{
    public bool CanPerform => true; // Agent can always attack
    public bool Complete { get; private set; }

    private CountdownTimer timer;
    private EnemyController enemyController;

    public AttackStrategy(float attackCooldown, EnemyController enemyController)
    {
        timer = new CountdownTimer(attackCooldown);
        timer.OnTimerStart += () => Complete = false;
        timer.OnTimerStop += () => Complete = true;
        this.enemyController = enemyController;
    }

    public void Start()
    {
        timer.Start();
        Collider2D[] colliders = Physics2D.OverlapCircleAll(enemyController.transform.position, enemyController.sword.range);
        foreach (Collider2D collider in colliders)
        {
            if (collider.isTrigger)
                continue;

            PlayerController player = collider.GetComponent<PlayerController>();
            if (player != null)
            {
                player.TakeDamage(enemyController.sword.damage);
                GameManager.GetInstance().goapAgent.playerHP = player.healthBar.currentHealth;
                enemyController.sword.durability--;

                if (enemyController.sword.durability <= 0)
                {
                    enemyController.sword = null;
                    GameManager.GetInstance().enemyHasSword = false;
                    enemyController.swordIndicator.SetActive(false);
                    GameManager.GetInstance().goapAgent.attackSensor.UpdateRange(GameManager.GetInstance().goapAgent.attackSensor.initialRadius);
                }
            }
        }
    }

    public void Update(float deltaTime) => timer.Update(deltaTime);
}

public class MoveStrategy : IActionStrategy
{
    private Pathfinder pathfinder;
    private Func<Vector3> destination;

    public bool CanPerform => !Complete;
    public bool Complete => !pathfinder.hasPath && !pathfinder.characterMovement.isMoving;

    public MoveStrategy(Pathfinder pathfinder, Func<Vector3> destination)
    {
        this.pathfinder = pathfinder;
        this.destination = destination;
    }

    public void Start() => pathfinder.FindPath(destination());
    public void Stop() => pathfinder.ResetPath();
}

public class MoveToPickupStrategy : IActionStrategy
{
    private Pathfinder pathfinder;
    private List<Vector3> pickupList;
    private Func<Vector3> destination;

    public bool CanPerform => !Complete;
    public bool Complete => !pathfinder.hasPath && !pathfinder.characterMovement.isMoving;

    public MoveToPickupStrategy(Pathfinder pathfinder, Func<Vector3> destination, List<Vector3> pickupList)
    {
        this.pathfinder = pathfinder;
        this.destination = destination;
        this.pickupList = pickupList;
    }

    public void Start() => pathfinder.FindPath(destination());
    public void Stop()
    {
        pathfinder.ResetPath();
        if (pickupList.Contains(destination()))
            pickupList.Remove(destination());
    }
}

public class WanderStrategy : IActionStrategy
{
    private Pathfinder pathfinder;
    private float wanderRadius;

    public bool CanPerform => !Complete;
    public bool Complete => !pathfinder.hasPath && !pathfinder.characterMovement.isMoving;

    public WanderStrategy(Pathfinder pathfinder, float wanderRadius)
    {
        this.pathfinder = pathfinder;
        this.wanderRadius = wanderRadius;
    }

    public void Start()
    {
        Vector3 point;
        do
        {
            Vector3 randomDirection = UnityEngine.Random.insideUnitSphere;
            randomDirection.z = 0;
            randomDirection = randomDirection.normalized * wanderRadius;
            point = pathfinder.transform.position + randomDirection;
        } while (!pathfinder.IsPointInMap(point));

        pathfinder.FindPath(point);
    }
}

public class IdleStrategy : IActionStrategy
{
    public bool CanPerform => true;
    public bool Complete { get; private set; }

    private CountdownTimer timer;

    public IdleStrategy(float duration)
    {
        timer = new CountdownTimer(duration);
        timer.OnTimerStart += () => Complete = false;
        timer.OnTimerStop += () => Complete = true;
    }

    public void Start() => timer.Start();
    public void Update(float deltaTime) => timer.Update(deltaTime);
}

public class DiscardWeaponStrategy : IActionStrategy
{
    public bool CanPerform => true;
    public bool Complete { get; private set; }

    private EnemyController enemyController;

    public DiscardWeaponStrategy(EnemyController enemyController)
    {
        this.enemyController = enemyController;
    }

    public void Start()
    {
        GameManager.GetInstance().enemyHasSword = false;
        enemyController.sword = null;
        Complete = true;
    }
}