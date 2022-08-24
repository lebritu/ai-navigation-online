using System;
using UnityEngine;
using FMT.Player;

namespace AINO.AI
{
    public class AICheckTrigger : MonoBehaviour
    {
        public event Action<Transform> OnCatchPlayer;
        public event Action OnMissingPlayer;

        private bool _Catch;
        private PlayerMotor _currentPlayer;

        private void OnTriggerStay(Collider other)
        {
            if (!_Catch)
            {
                if (other.GetComponent<PlayerMotor>())
                {
                    _Catch = true;
                    _currentPlayer = other.GetComponent<PlayerMotor>();
                    OnCatchPlayer?.Invoke(other.transform);
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (_Catch)
            {
                if(_currentPlayer == other.GetComponent<PlayerMotor>())
                {
                    _Catch = false;
                    OnMissingPlayer?.Invoke();
                }
            }
        }
    }
}
