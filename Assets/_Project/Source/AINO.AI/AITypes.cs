using System;
using AINO.Data;

namespace AINO.AI
{
    [Serializable]
    public struct AiSpawnInfo
    {
        public int SpawnCount;
        public AIIBehaviourData Data;
    }

    [Serializable]
    public struct AiPropertiesInfo
    {
        public float WalkSpeed;
        public float RunSpeed;
        public float LookRange;
        public float AgentRadius;
        public float AgentAngularSpeed;
        public float AgentStopDistance;
        public AiBehaviours Behaviour;
    }

    public enum PathTypes
    {
        None,
        Random,
        Stop
    }

    public enum AiBehaviours
    {
        Neutral,
        Agressive,
        Coward,
    }
}
