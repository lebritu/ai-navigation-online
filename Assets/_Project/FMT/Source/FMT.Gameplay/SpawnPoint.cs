using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FMT.Gameplay
{
    public class SpawnPoint : MonoBehaviour
    {
        public Vector3 Position { get => transform.position; }
        public Quaternion Rotation { get => transform.rotation; }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(transform.position, 0.2f);
        }
    }
}