using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeliefFactory
{
    private GoapAgent agent;
    private Dictionary<string, AgentBelief> beliefs;

    public BeliefFactory(GoapAgent agent, Dictionary<string, AgentBelief> beliefs)
    {
        this.agent = agent;
        this.beliefs = beliefs;
    }

    public void AddBelief(string key, Func<bool> condition)
    {
        beliefs.Add(key, new AgentBelief.Builder(key)
            .WithCondition(condition)
            .Build());
    }

    public void AddPlayerSensorBelief(string key, PlayerSensor sensor)
    {
        beliefs.Add(key, new AgentBelief.Builder(key)
            .WithCondition(() => sensor.isTargetInRange)
            .WithLocation(() => sensor.targetPosition)
            .Build());
    }

    public void AddLocationBelief(string key, float distance, Transform locationCondition)
    {
        AddLocationBelief(key, distance, locationCondition.position);
    }

    public void AddLocationBelief(string key, float distance, Vector3 locationCondition)
    {
        beliefs.Add(key, new AgentBelief.Builder(key)
            .WithCondition(() => InRangeOf(locationCondition, distance))
            .WithLocation(() => locationCondition)
            .Build());
    }

    bool InRangeOf(Vector3 pos, float range) => Vector3.Distance(agent.transform.position, pos) < range;
}

public class AgentBelief
{
    public string name;

    Func<bool> condition = () => false;
    Func<Vector3> observedLocation = () => Vector3.zero;

    public Vector3 location => observedLocation();

    AgentBelief(string name)
    {
        this.name = name;
    }

    public bool Evaluate() => condition();

    public class Builder
    {
        private AgentBelief belief;

        public Builder(string name)
        {
            belief = new AgentBelief(name);
        }

        public Builder WithCondition(Func<bool> condition)
        {
            belief.condition = condition;
            return this;
        }

        public Builder WithLocation(Func<Vector3> observedLocation)
        {
            belief.observedLocation = observedLocation;
            return this;
        }

        public AgentBelief Build()
        {
            return belief;
        }
    }
}
