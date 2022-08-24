using System;
using UnityEngine.AI;
using UnityEngine;
using FMT.Player;
using Mirror;

namespace AINO.AI
{
    public class AICommonController : NetworkBehaviour
    {
        public event Action OnSetPath;

        private const string MotorAnimTrigger = "Z";
        private const string RunAnimTrigger = "Run";

        [SerializeField]
        private NavMeshAgent _agent;
        [SerializeField]
        private Animator _animator;
        [SerializeField]
        private AICheckTrigger _checkTrigger;
        [SerializeField]
        private Transform _rayPosition;
        [SerializeField]
        private float _normalSpeed;
        [SerializeField]
        private float _runSpeed;
        [SerializeField]
        private float _targetRange;
        [SerializeField]
        private float _lookRange = 90;
        [SerializeField]
        private LayerMask _layerMask;
        [SerializeField]
        private AiBehaviours _behaviour;

        private bool _chasing;
        private bool _Flee;
        private bool _targetLock;
        private float _currentSpeed;
        private float _targetCurrentSpeed = 4;
        private float _lookAngle = 60;
        private Transform _target;
        private AIPathManager _pathManager;
        private AIPathPoints _currentPath;

        private void Awake()
        {
            _currentSpeed = _normalSpeed;
            _agent.speed = _currentSpeed;
        }

        public override void OnStartClient()
        {
            if (!isServer)
            {
                _checkTrigger.gameObject.SetActive(false);
            }
            else
            {
                _checkTrigger.OnCatchPlayer += HandleCheckEnemy;
                _checkTrigger.OnMissingPlayer += HandleMissingPlayer;
            }
        }

        public void Initialize(AIPathManager pathManager, AiPropertiesInfo info)
        {
            _pathManager = pathManager;
            _normalSpeed = info.WalkSpeed;
            _runSpeed = info.RunSpeed;
            _lookRange = info.LookRange;
            _agent.radius = info.AgentRadius;
            _agent.angularSpeed = info.AgentAngularSpeed;
            _agent.stoppingDistance = info.AgentStopDistance;
            _behaviour = info.Behaviour;
            _currentSpeed = _normalSpeed;
            _agent.speed = _currentSpeed;
            _currentPath = _pathManager.GetRandomPath();

            SetPathPoint();
        }

        private void Update()
        {
            _animator.SetFloat(MotorAnimTrigger, _agent.velocity.magnitude, 0.1f, Time.deltaTime);

            if (!isServer) { return; }

            CheckEnemy();
            Pursue();
            Evade();
        }

        private void Evade()
        {
            if (!_Flee) { return; }

            Vector3 targetDirection = _target.position - transform.position;
            float lookAhead = targetDirection.magnitude / (_agent.speed + _targetCurrentSpeed);
            Flee(_target.position + _target.forward * lookAhead);
        }

        private void Pursue()
        {
            if (!_chasing) { return; }

            Vector3 targetDirection = _target.position - transform.position;

            float relativeHeading = Vector3.Angle(transform.forward, transform.TransformVector(_target.forward));
            float toTarget = Vector3.Angle(transform.forward, transform.TransformVector(targetDirection));

            if ((toTarget > 90 && relativeHeading < 20) || _targetCurrentSpeed < 0.01f)
            {
                RpcSetPoint(_target.position);
                return;
            }

            float lookAhead = targetDirection.magnitude / (_agent.speed + _targetCurrentSpeed);
            RpcSetPoint(_target.position + _target.forward * lookAhead);
        }

        private void StartFlee()
        {
            _currentSpeed = _runSpeed;
            _agent.speed = _currentSpeed;
            RpcSetAnimationBool(RunAnimTrigger, true);
            _Flee = true;
        }

        private void CheckEnemy()
        {
            if (!_targetLock) { return; }

            if (CanSeeTarget() && TargetInRange())
            {
                switch (_behaviour)
                {
                    case AiBehaviours.Neutral:
                        break;

                    case AiBehaviours.Agressive:
                        StartChasing();
                        break;

                    case AiBehaviours.Coward:
                        StartFlee();
                        break;
                }             
            }
        }

        private void SetPathPoint()
        {
            if (_chasing) { return; }

            Vector3 point = _pathManager.GetPoint(_currentPath);
            RpcSetPoint(point);
        }

        private void StartChasing()
        {
            _currentSpeed = _runSpeed;
            _agent.speed = _currentSpeed;
            RpcSetAnimationBool(RunAnimTrigger, true);
            _chasing = true;
        }

        private void TakeDecision()
        {
            SetPathPoint();
        }

        private void HandleMissingPlayer()
        {
            _targetLock = false;
            _chasing = false;
            _Flee = false;
            _currentSpeed = _normalSpeed;
            _agent.speed = _currentSpeed;
            RpcSetAnimationBool("Run", false);
            SetPathPoint();
        }

        private void Flee(Vector3 vector)
        {
            Vector3 fleeVector = vector - transform.position;
            RpcSetPoint(transform.position - fleeVector);
        }

        private void HandleCheckEnemy(Transform target)
        {
            _targetLock = true;
            _target = target;
        }

        [ClientRpc]
        private void RpcSetPoint(Vector3 point)
        {
            _agent.SetDestination(point);
        }

        [ClientRpc]
        private void RpcSetAnimationBool(string animId, bool active)
        {
            _animator.SetBool(animId, active);
        }

        private bool TargetInRange()
        {
            if (Vector3.Distance(transform.position, _target.position) < _targetRange)
            {
                return true;
            }

            return false;
        }

        private bool CanSeeTarget()
        {
            RaycastHit hit;
            Vector3 rayToTarget = _target.position - _rayPosition.position;

            float lookingAngle = Vector3.Angle(transform.forward, rayToTarget);

            if (Physics.Linecast(_rayPosition.position, _target.position, out hit, _layerMask))
            {
                if (hit.collider.GetComponent<PlayerMotor>())
                {
                    if (lookingAngle < _lookRange)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private bool TargetCanSeeMe()
        {
            Vector3 toAgent = transform.position - _target.position;
            float lookingAngle = Vector3.Angle(_target.forward, toAgent);

            if (lookingAngle < _lookAngle)
            {
                return true;
            }

            return false;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!isServer) { return; }

            if (other.GetComponent<AIPathPoints>())
            {
                _currentPath = other.GetComponent<AIPathPoints>();
                TakeDecision();
            }
        }
    }
}
