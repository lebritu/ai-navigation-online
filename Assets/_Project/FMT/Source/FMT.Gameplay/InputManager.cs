using System;
using UnityEngine;

namespace FMT.Gameplay
{
    public class InputManager : MonoBehaviour
    {
        public Vector2 MovementAxis { get; private set; }
        public Vector2 CameraAxis { get; private set; }
        public bool Jump { get; private set; }
        public bool Running { get; private set; }

        private void Update()
        {
            UpdateMovementAxis();
            UpdateMouseAxis();
            UpdateJump();
            UpdateRunning();
        }

        private void UpdateMovementAxis()
        {
            MovementAxis = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        }

        private void UpdateMouseAxis()
        {
            CameraAxis = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        }

        private void UpdateJump()
        {
            if (Input.GetButtonDown("Jump"))
            {
                Jump = true;
            }
            else
            {
                Jump = false;
            }
        }

        private void UpdateRunning()
        {
            if (Input.GetButton("Run"))
            {
                Running = true;
            }
            else
            {
                Running = false;
            }
        }
    }
}
