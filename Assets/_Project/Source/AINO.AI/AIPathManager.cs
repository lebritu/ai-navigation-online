using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace AINO.AI
{
    public class AIPathManager : NetworkBehaviour
    {
        [SerializeField]
        private List<AiSpawnInfo> _spawnInfo;
        [SerializeField]
        private AICommonController _aiAgentPrefab;

        private AIPathPoints[] _points;

        private void Awake()
        {
            _points = GetComponentsInChildren<AIPathPoints>();
        }

        public override void OnStartServer()
        {
            foreach (AiSpawnInfo info in _spawnInfo)
            {
                for (int i = 0; i < info.SpawnCount; i++)
                {
                    CreateSpawnAgent(info.Data.Info);
                }
            }
        }

        public Vector3 GetPoint(AIPathPoints currentPoint)
        {
            int randomNumber = Random.Range(0, _points.Length - 1);
            List<AIPathPoints> paths = new List<AIPathPoints>();
            paths.AddRange(_points);
            paths.Remove(currentPoint);

            return paths[randomNumber].transform.position;
        }

        public AIPathPoints GetRandomPath()
        {
            int randomNumber = Random.Range(0, _points.Length - 1);

            return _points[randomNumber];
        }

        private void CreateSpawnAgent(AiPropertiesInfo info)
        {
            Transform initialPosition = GetRandomPath().transform;

            AICommonController agent = Instantiate(_aiAgentPrefab, initialPosition.position, initialPosition.rotation);
            NetworkServer.Spawn(agent.gameObject);
            agent.Initialize(this, info);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(transform.position, 0.5f);
        }
    }
}