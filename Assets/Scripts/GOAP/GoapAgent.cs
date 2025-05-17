using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Helpers;
using UnityEngine.AI;

[RequireComponent(typeof(Pathfinder))]
[RequireComponent(typeof(EnemyController))]
public class GoapAgent : MonoBehaviour
{
    [Header("Sensors")]
    public PlayerSensor chaseSensor;
    public PlayerSensor attackSensor;
    public DetectPowerupSensor detectPowerupSensor;

    [Header("Known Locations")]
    public List<Vector3> heals;
    public List<Vector3> speedUps;
    public List<Vector3> swords;

    [Header("Known Infos")]
    public float playerHP = 100f;
    public float playerDamage = 0f;

    Pathfinder pathfinder;

    [Header("Stats")]
    EnemyController enemyController;

    GameObject target;
    Vector3 destination;

    AgentGoal lastGoal;
    public AgentGoal currentGoal;
    public ActionPlan actionPlan;
    public AgentAction currentAction;

    public Dictionary<string, AgentBelief> beliefs;
    public HashSet<AgentAction> actions;
    public HashSet<AgentGoal> goals;

    void Awake()
    {
        pathfinder = GetComponent<Pathfinder>();
        enemyController = GetComponent<EnemyController>();
    }

    void Start()
    {
        SetupBeliefs();
        SetupActions();
        SetupGoals();
    }

    void SetupBeliefs()
    {
        beliefs = new Dictionary<string, AgentBelief>();
        BeliefFactory factory = new BeliefFactory(this, beliefs);

        factory.AddBelief("Nothing", () => false);

        factory.AddBelief("AgentIdle", () => !pathfinder.hasPath);
        factory.AddBelief("AgentMoving", () => pathfinder.hasPath);
        factory.AddBelief("AgentHealthLow", () => enemyController.healthBar.currentHealth <= 20);
        factory.AddBelief("AgentIsHealthy", () => enemyController.healthBar.currentHealth > 20);
        factory.AddBelief("AgentIsFullHealth", () => enemyController.healthBar.currentHealth == enemyController.healthBar.maxHealth);
        factory.AddBelief("AgentKnowsHeals", () => heals.Count > 0);
        factory.AddBelief("AgentKnowsSpeeds", () => speedUps.Count > 0);
        factory.AddBelief("AgentKnowsSwords", () => swords.Count > 0);
        factory.AddBelief("AgentFastSpeed", () => enemyController.characterController.speedUp > 1f);
        factory.AddBelief("AgentHasNoWeapon", () => !GameManager.GetInstance().enemyHasSword);
        factory.AddBelief("AgentHasWeapon", () => GameManager.GetInstance().enemyHasSword);
        factory.AddBelief("AgentCanKillPlayer", () => GameManager.GetInstance().enemyHasSword && (enemyController.sword.durability * enemyController.sword.damage) >= playerHP);
        factory.AddBelief("AgentCannotKillPlayer", () => GameManager.GetInstance().enemyHasSword && (enemyController.sword.durability * enemyController.sword.damage) < playerHP);
        factory.AddBelief("AgentCanKillPlayerOneHit", () => GameManager.GetInstance().enemyHasSword && enemyController.sword.damage >= playerHP);

        factory.AddPlayerSensorBelief("PlayerInChaseRange", chaseSensor);
        factory.AddPlayerSensorBelief("PlayerInAttackRange", attackSensor);

        factory.AddBelief("AttackingPlayer", () => false);
    }

    void SetupActions()
    {
        actions = new HashSet<AgentAction>();

        actions.Add(new AgentAction.Builder("Idle")
            .WithStrategy(new IdleStrategy(5))
            .AddEffect(beliefs["Nothing"])
            .Build());

        actions.Add(new AgentAction.Builder("Wander Around")
            .WithStrategy(new WanderStrategy(pathfinder, 10))
            .AddEffect(beliefs["AgentMoving"])
            .Build());

        actions.Add(new AgentAction.Builder("MoveToHeal")
            .WithStrategy(new MoveToPickupStrategy(pathfinder, () => GetRandomElement(heals), heals))
            .AddPrecondition(beliefs["AgentKnowsHeals"])
            .AddPrecondition(beliefs["AgentHealthLow"])
            .AddEffect(beliefs["AgentIsHealthy"])
            .Build());

        actions.Add(new AgentAction.Builder("MoveToSpeed")
            .WithStrategy(new MoveToPickupStrategy(pathfinder, () => GetRandomElement(speedUps), speedUps))
            .AddPrecondition(beliefs["AgentKnowsSpeeds"])
            .AddEffect(beliefs["AgentFastSpeed"])
            .Build());

        actions.Add(new AgentAction.Builder("MoveToWeapon")
            .WithStrategy(new MoveToPickupStrategy(pathfinder, () => GetRandomElement(swords), swords))
            .AddPrecondition(beliefs["AgentHasNoWeapon"])
            .AddPrecondition(beliefs["AgentKnowsSwords"])
            .AddEffect(beliefs["AgentHasWeapon"])
            .AddEffect(beliefs["AgentCanKillPlayer"])
            .Build());

        actions.Add(new AgentAction.Builder("DiscardWeapon")
            .WithStrategy(new DiscardWeaponStrategy(enemyController))
            .AddPrecondition(beliefs["AgentCannotKillPlayer"])
            .AddEffect(beliefs["AgentHasNoWeapon"])
            .Build());

        actions.Add(new AgentAction.Builder("ChasePlayerWhenHealthy")
            .WithStrategy(new MoveStrategy(pathfinder, () => beliefs["PlayerInChaseRange"].location))
            .AddPrecondition(beliefs["PlayerInChaseRange"])
            .AddPrecondition(beliefs["AgentHasWeapon"])
            .AddPrecondition(beliefs["AgentIsHealthy"])
            .AddPrecondition(beliefs["AgentCanKillPlayer"])
            .AddEffect(beliefs["PlayerInAttackRange"])
            .Build());

        actions.Add(new AgentAction.Builder("ChasePlayerWhenLowHP")
            .WithStrategy(new MoveStrategy(pathfinder, () => beliefs["PlayerInChaseRange"].location))
            .AddPrecondition(beliefs["PlayerInChaseRange"])
            .AddPrecondition(beliefs["AgentHasWeapon"])
            .AddPrecondition(beliefs["AgentHealthLow"])
            .AddPrecondition(beliefs["AgentCanKillPlayerOneHit"])
            .AddEffect(beliefs["PlayerInAttackRange"])
            .Build());

        actions.Add(new AgentAction.Builder("AttackPlayer")
            .WithStrategy(new AttackStrategy(1f, enemyController))
            .AddPrecondition(beliefs["PlayerInAttackRange"])
            .AddPrecondition(beliefs["AgentHasWeapon"])
            .AddEffect(beliefs["AttackingPlayer"])
            .Build());
    }

    void SetupGoals()
    {
        goals = new HashSet<AgentGoal>();

        goals.Add(new AgentGoal.Builder("Wander")
            .WithPriority(1)
            .WithDesiredEffect(beliefs["AgentMoving"])
            .Build());

        goals.Add(new AgentGoal.Builder("GetSpeedUp")
            .WithPriority(1)
            .WithDesiredEffect(beliefs["AgentFastSpeed"])
            .Build());

        goals.Add(new AgentGoal.Builder("GetToFullHealth")
            .WithPriority(1)
            .WithDesiredEffect(beliefs["AgentIsFullHealth"])
            .Build());

        goals.Add(new AgentGoal.Builder("GetNewWeapon")
            .WithPriority(1.5f)
            .WithDesiredEffect(beliefs["AgentHasWeapon"])
            .WithDesiredEffect(beliefs["AgentCanKillPlayer"])
            .Build());

        goals.Add(new AgentGoal.Builder("HealUp")
            .WithPriority(2)
            .WithDesiredEffect(beliefs["AgentIsHealthy"])
            .Build());

        goals.Add(new AgentGoal.Builder("AttackPlayer")
            .WithPriority(2)
            .WithDesiredEffect(beliefs["AttackingPlayer"])
            .Build());
    }

    bool InRangeOf(Vector3 pos, float range) => Vector3.Distance(transform.position, pos) < range;

    void OnEnable() => chaseSensor.OnTargetChanged += HandleTargetChanged;
    void OnDisable() => chaseSensor.OnTargetChanged -= HandleTargetChanged;

    void HandleTargetChanged()
    {
        Debug.Log("Target changed, clearing current action and goal");
        // Force the planner to re-evaluate the plan
        currentAction = null;
        currentGoal = null;
    }

    void Update()
    {
        // Update the plan and current action if there is one
        if (currentAction == null)
        {
            Debug.Log("Calculating any potential new plan");
            CalculatePlan();

            if (actionPlan != null && actionPlan.Actions.Count > 0)
            {
                pathfinder.ResetPath();

                currentGoal = actionPlan.AgentGoal;
                Debug.Log($"Goal: {currentGoal.name} with {actionPlan.Actions.Count} actions in plan");
                currentAction = actionPlan.Actions.Pop();
                Debug.Log($"Popped action: {currentAction.name}");
                // Verify all precondition effects are true
                if (currentAction.preconditions.All(b => b.Evaluate()))
                {
                    currentAction.Start();
                }
                else
                {
                    Debug.Log("Preconditions not met, clearing current action and goal");
                    currentAction = null;
                    currentGoal = null;
                }
            }
        }

        // If we have a current action, execute it
        if (actionPlan != null && currentAction != null)
        {
            currentAction.Update(Time.deltaTime);

            if (currentAction.complete)
            {
                Debug.Log($"{currentAction.name} complete");
                currentAction.Stop();
                currentAction = null;

                if (actionPlan.Actions.Count == 0)
                {
                    Debug.Log("Plan complete");
                    lastGoal = currentGoal;
                    currentGoal = null;
                }
            }
        }
    }

    void CalculatePlan()
    {
        var priorityLevel = currentGoal?.priority ?? 0;

        HashSet<AgentGoal> goalsToCheck = goals;

        // If we have a current goal, we only want to check goals with higher priority
        if (currentGoal != null)
        {
            Debug.Log("Current goal exists, checking goals with higher priority");
            goalsToCheck = new HashSet<AgentGoal>(goals.Where(g => g.priority > priorityLevel));
        }

        var potentialPlan = GoapPlanner.Plan(this, goalsToCheck, lastGoal);
        if (potentialPlan != null)
        {
            actionPlan = potentialPlan;
        }
    }
}
