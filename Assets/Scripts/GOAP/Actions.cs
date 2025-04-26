using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentAction
{
    public string name;
    public float cost = 1;

    public HashSet<AgentBelief> preconditions = new HashSet<AgentBelief>();
    public HashSet<AgentBelief> effects = new HashSet<AgentBelief>();

    IActionStrategy strategy;
    public bool complete => strategy.Complete;

    AgentAction(string name)
    {
        this.name = name;
    }

    public void Start() => strategy.Start();

    public void Update(float deltaTime)
    {
        // Check if the action can be performed and update the strategy
        if (strategy.CanPerform)
        {
            strategy.Update(deltaTime);
        }

        // Bail out if the strategy is still executing
        if (!strategy.Complete) return;

        // Apply effects
        foreach (var effect in effects)
        {
            effect.Evaluate();
        }
    }

    public void Stop() => strategy.Stop();

    public class Builder
    {
        private AgentAction action;

        public Builder(string name)
        {
            action = new AgentAction(name);
        }

        public Builder WithCost(float cost)
        {
            action.cost = cost;
            return this;
        }

        public Builder WithStrategy(IActionStrategy strategy)
        {
            action.strategy = strategy;
            return this;
        }

        public Builder AddPrecondition(AgentBelief precondition)
        {
            action.preconditions.Add(precondition);
            return this;
        }

        public Builder AddEffect(AgentBelief effect)
        {
            action.effects.Add(effect);
            return this;
        }

        public AgentAction Build()
        {
            return action;
        }
    }
}
