using System.Collections;
using FMT.Network;
using FMT.Gameplay;
using UnityEngine;
using Mirror;

namespace FMT.Player
{
    public class PlayerVisualSync : NetworkBehaviour
    {
        private const string RunningAnimationTrigger = "Run";
        private const string SpineAnimationTrigger = "Spine";
        private const string MoveXAnimationTrigger = "X";
        private const string MoveZAnimationTrigger = "Z";

        [SerializeField] private Animator _animator;
        [SerializeField] private AudioSource _soundOutput;
        [SerializeField] private SkinnedMeshRenderer _headMesh;

        [SyncVar] private float AxisX;
        [SyncVar] private float AxisZ;
        [SyncVar] private float Spine;

        private MaterialEnum _floorMaterial;
        private bool _isMoving;
        private float _timeStep = 0.7f;
        private readonly float _timeStep_Walk = 0.7f;
        private readonly float _timeStep_Run = 0.3f;

        public override void OnStartAuthority()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            PlayerMotor motor = GetComponent<PlayerMotor>();
            motor.OnMovementCallBack += CmdUpdateVisualMovement;
            motor.OnMovement += HandleUpdateMovement;
            motor.OnRunning += HandleUpdateRunning;
            motor.OnJumping += HandleUpdateJump;
            _headMesh.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;

            StartCoroutine(StepWalk());
        }

        private void Update()
        {
            CheckGround();
            UpdateAnimationMovement();
        }

        private void UpdateAnimationMovement()
        {
            _animator.SetFloat(MoveXAnimationTrigger, AxisX, 0.1f, Time.deltaTime);
            _animator.SetFloat(MoveZAnimationTrigger, AxisZ, 0.1f, Time.deltaTime);
            _animator.SetFloat(SpineAnimationTrigger, Spine, 0.1f, Time.deltaTime);
        }

        private void PlayStepSound()
        {
            if (!_isMoving) { return; }

            CmdPlayStepSound();
        }

        private void CheckGround()
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, -transform.up, out hit, 5))
            {
                if (hit.collider.GetComponent<MaterialType>())
                {
                    MaterialType ground = hit.collider.GetComponent<MaterialType>();
                    ground = hit.collider.GetComponent<MaterialType>();

                    _floorMaterial = ground.Material;
                }
            }
        }

        private void HandleUpdateMovement(bool isMove)
        {
            _isMoving = isMove;
        }

        private void HandleUpdateRunning(bool isRunning)
        {
            if (isRunning)
            {
                _timeStep = _timeStep_Run;
                CmdSetAnimationBool(RunningAnimationTrigger, true);
            }
            else
            {
                _timeStep = _timeStep_Walk;
                CmdSetAnimationBool(RunningAnimationTrigger, false);
            }
        }

        private void HandleUpdateJump()
        {
            CmdSetAnimationTrigger("Jump");
        }

        [Command]
        private void CmdPlayStepSound()
        {
            RpcPlayStepSound();
        }

        [Command]
        private void CmdSetAnimationBool(string animationId, bool active)
        {
            RpcSetAnimationBool(animationId, active);
        }

        [Command]
        private void CmdSetAnimationTrigger(string animationId)
        {
            RpcSetAnimationTrigger(animationId);
        }

        [Command]
        private void CmdUpdateVisualMovement(MovementCallBack msg)
        {
            AxisX = msg.AxisX;
            AxisZ = msg.AxisZ;
            Spine = msg.Spine;
        }

        [ClientRpc]
        private void RpcPlayStepSound()
        {
            _soundOutput.PlayOneShot(SoundPool.GetStepSound(_floorMaterial), SoundPool.Volume);
        }

        [ClientRpc]
        private void RpcSetAnimationBool(string animationId, bool active)
        {
            _animator.SetBool(animationId, active);
        }

        [ClientRpc]
        private void RpcSetAnimationTrigger(string animationId)
        {
            _animator.SetTrigger(animationId);
        }

        private IEnumerator StepWalk()
        {
            while (true)
            {
                yield return new WaitForSeconds(_timeStep);

                PlayStepSound();
            }
        }
    }
}
