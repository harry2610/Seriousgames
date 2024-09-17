using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Activity
{
    [CreateAssetMenu(menuName = "Dog/Activity/ChangeBodyPose")]
    public class ChangeBodyPoseSO : ActivitySO
    {
        [Header("Change body pose")]
        public DogPose pose;
        public float holdingPeriod;
        public override ActivityExecution StartActivity(GameObject dog, object argument)
        {
            return new ChangingBodyPose(this, dog);
        }
    }

    [System.Serializable]
    public class ChangingBodyPose : ActivityExecution
    {
        private bool m_HasHoldingPeriodStarted;
        private float m_RemainingHoldingPeriod;
        public ChangingBodyPose(ChangeBodyPoseSO info, GameObject dog) : base(info)
        {
            m_HasHoldingPeriodStarted = false;
            m_RemainingHoldingPeriod = info.holdingPeriod;
            if (dog.TryGetComponent(out Body body))
            {
                body.ChangeBodyPose(info.pose, () => m_HasHoldingPeriodStarted = true);
            }
        }
        public override void Update()
        {
            if (m_HasHoldingPeriodStarted)
            {
                m_RemainingHoldingPeriod -= Time.deltaTime;
                if (m_RemainingHoldingPeriod <= 0.0f)
                    EndActivity();
            }
        }
    }
}
