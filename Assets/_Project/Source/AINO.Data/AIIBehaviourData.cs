using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AINO.AI;

namespace AINO.Data
{
    [CreateAssetMenu(fileName = ("AI Behaviour"), menuName = ("Create AI Behaviour"))]
    public class AIIBehaviourData : ScriptableObject
    {
        [SerializeField]
        private AiPropertiesInfo _info;

        public AiPropertiesInfo Info => _info;
    }
}