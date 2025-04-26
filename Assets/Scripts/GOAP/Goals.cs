using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentGoal
{
    public string name;
    public float priority;
    public HashSet<AgentBelief> desiredEffects = new HashSet<AgentBelief>();

    AgentGoal(string name)
    {
        this.name = name;
    }

    public class Builder
    {
        private AgentGoal goal;

        public Builder(string name)
        {
            goal = new AgentGoal(name);
        }

        public Builder WithPriority(float priority)
        {
            goal.priority = priority;
            return this;
        }

        public Builder WithDesiredEffect(AgentBelief effect)
        {
            goal.desiredEffects.Add(effect);
            return this;
        }

        public AgentGoal Build()
        {
            return goal;
        }
    }
}
