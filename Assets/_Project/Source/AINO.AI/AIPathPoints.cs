using UnityEngine;

namespace AINO.AI
{
    public class AIPathPoints : MonoBehaviour
    {
        [SerializeField]
        private PathTypes _type;

        public PathTypes Type => _type;

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(transform.position, 0.2f);
        }
    }
}
