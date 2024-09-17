using UnityEngine;
using static UnityEngine.Networking.UnityWebRequest;
using UnityEngine.AI;

namespace Activity
{
    [CreateAssetMenu(menuName = "Dog/Activity/Give")]
    public class GiveSO : ActivitySO
    {
        public GiveSO()
        {
        }
        public override ActivityExecution StartActivity(GameObject dog, object argument)
        {
            return new Giving(dog, this);
        }
    }

    [System.Serializable]
    public class Giving : ActivityExecution
    {
        private Body m_Body;
        public Giving(GameObject dog, GiveSO give) : base(give)
        {
            m_Body = dog.GetComponent<Body>();
            m_Body.head.GrabbedObject = null;
            EndActivity();
        }
        public override void Update()
        {
        }
    }
}
