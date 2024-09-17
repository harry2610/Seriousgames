using Activity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Activity
{
    [CreateAssetMenu(menuName = "Dog/Activity/Wait")]
    public class WaitSO : ActivitySO
    {

        [Header("Wait")]
        public float time;
        public override ActivityExecution StartActivity(GameObject dog, object argument)
        {
            return new Waiting(this);
        }
    }
    [System.Serializable]
    public class Waiting : ActivityExecution
    {
        private float m_RemainingTime;
        public Waiting(WaitSO wait) : base(wait)
        {
            m_RemainingTime = wait.time;
        }
        public override void Update()
        {
            m_RemainingTime -= Time.deltaTime;
            if (m_RemainingTime <= 0f)
                EndActivity();
        }
    }
}
