using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

namespace Activity
{

    [CreateAssetMenu(menuName = "Dog/Activity/Destroy")]
    public class DestroySO : ActivitySO
    {
        public float speed;
        public QuicktipSO quicktip;
        public override ActivityExecution StartActivity(GameObject dog, object argument)
        {
            return new Destroying(this, dog);
        }
    }
    [System.Serializable]
    public class Destroying : ActivityExecution
    {
        private Body m_Body;
        private GameObject m_Dog;
        private DestructionPoint m_DestructionPoint;
        public Destroying(DestroySO destruction, GameObject dog) : base(destruction)
        {
            m_Dog = dog;
            m_Body = dog.GetComponent<Body>();
            var points = GameObject.FindObjectsByType<DestructionPoint>(FindObjectsSortMode.None);
            if (points.Length > 0)
            {
                m_DestructionPoint = points[Random.Range(0, points.Length)];
                var point = m_DestructionPoint.transform.position;
                point.y = 0.0f;
                if (NavMesh.SamplePosition(point, out NavMeshHit hit, 1.0f, NavMesh.AllAreas))
                {
                    point = hit.position;
                }
                var info = new WalkInfo()
                {
                    speed = destruction.speed,
                    onDestinationReached = Destruct,
                    destinationPath = new Vector3[] { point },
                };
                m_Body.Walk(info);
            }
            else
            {
                EndActivity();
            }
        }
        public override void Update()
        {

        }
        void Destruct()
        {
            m_Body.StartAnimation(DogAnimation.Destroy, () => {
                EndActivity();
                m_DestructionPoint.Destruct();
            });
        }
        public override void OnEnd()
        {
            QuicktipSO tipForDestroyedFurniture = GameStateManager.Instance.resourceList.quicktips.Single(x => x.name == "TipForDestroyedFurniture");
            GlobalEvents.Instance.triggerQuickTip.Invoke(tipForDestroyedFurniture);
        }
    }
}