using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dog;
using UI.Items;
using UnityEngine.Rendering;

namespace Activity
{
    [CreateAssetMenu(menuName = "Dog/Activity/ReplenishSustenance")]
    public class ReplenishSustenanceSO : ActivitySO
    {
        [Header("ReplenishSustenance")]
        public BowlType replenishType;
        public float movementSpeed;
        public float bowlDepletionSpeed;
        [System.NonSerialized]
        public Bowl activityBowl = null;
        public override ActivityExecution StartActivity(GameObject dog, object argument)
        {
            // Get all Bowls
            Bowl[] bowls = FindObjectsOfType<Bowl>();
            // If no filled bowl is found, go to last checked bowl
            foreach (Bowl bowl in bowls)
            {
                // If bowl has correct Type
                if (bowl.bowlType == replenishType)
                {
                    // Stop searching if full bowl is found
                    activityBowl = bowl;
                    if (!bowl.IsEmpty()) break;
                }
            }

            return new ReplenishingSustenance(dog, this);
        }
    }

    public class ReplenishingSustenance : ActivityExecution
    {
        private ReplenishSustenanceSO m_ReplenishSustenanceSO;
        private float m_BowlDepletionSpeed;
        private Body m_Body;
        private Mind m_Mind;
        private Bowl m_Bowl;
        private BowlType m_ReplenishType;
        private float m_ConsumedFood;
        public ReplenishingSustenance(GameObject dog, ReplenishSustenanceSO replenishSustenanceSO) : base(replenishSustenanceSO)
        {
            this.m_ReplenishSustenanceSO = replenishSustenanceSO;
            m_Bowl = replenishSustenanceSO.activityBowl;

            m_Mind = dog.GetComponent<Mind>();
            m_Body = dog.GetComponent<Body>();

            if (m_Bowl == null)
            {
                BeSad();
                return;
            }

            m_IsReadyToEnd = false;
            m_ReplenishType = replenishSustenanceSO.replenishType;
            m_BowlDepletionSpeed = replenishSustenanceSO.bowlDepletionSpeed;

            Vector3 dogDirection = (m_Body.transform.position - m_Bowl.transform.position);
            dogDirection.Normalize();
            Vector3 endPosition = m_Bowl.transform.position + dogDirection * 0.07f;

            WalkInfo info = new WalkInfo()
            {
                destinationPath = new Vector3[] { endPosition },
                speed = replenishSustenanceSO.movementSpeed,
                onDestinationReached = SniffBowl,
            };
            m_Body.Walk(info);
        }
        public override void Update()
        {
            if (m_Bowl != null && m_Bowl.gameObject.scene != null)
            {
                m_Body.LookAt(m_Bowl.transform.position);
            }
            else
            {
                EndActivity();
            }
        }

        void SniffBowl()
        {
            m_Body.ChangeBodyPose(DogPose.Consume, () => { m_Body.StartAnimation(DogAnimation.Sniff, m_Bowl.IsEmpty() ? BeSad : ConsumeSustenance); });
        }
        void ConsumeSustenance()
        {
            if (m_IsReadyToEnd) return;
            if (m_Bowl.IsEmpty())
            {
                UpdateConditions();
            }

            m_ConsumedFood += m_BowlDepletionSpeed;
            float updatedFillValue = m_Bowl.FillValue - m_BowlDepletionSpeed;
            if (updatedFillValue < 0)
            {
                m_ConsumedFood += updatedFillValue;
                m_Bowl.FillValue = 0;
                EndActivity();
                return;
            }
            else
            {
                m_Bowl.FillValue = updatedFillValue;
            }
            m_Body.StartAnimation(m_ReplenishType == BowlType.Food ? DogAnimation.Eat : DogAnimation.Drink, ConsumeSustenance);
        }

        void UpdateConditions()
        {
            foreach (ConditionEffect conditionEffect in m_Bowl.conditionEffects)
            {
                double currentCondition = m_Mind.dogState.GetCondition(conditionEffect.condition);
                m_Mind.dogState.SetCondition(conditionEffect.condition, currentCondition + m_ConsumedFood * conditionEffect.modifier / 100);
            }

            m_Body.ChangeBodyPose(DogPose.Stand, () => EndActivity());
        }

        void BeSad()
        {
            m_Body.StartAnimation(DogAnimation.Howl, () => EndActivity());
        }
        public override void EndRequested()
        {
            EndActivity();
            UpdateConditions();
        }

        public override int GetPoints()
        {
            if (m_Bowl == null || m_Bowl.IsEmpty())
            {
                return 0;
            }
            else
            {
                return m_ReplenishSustenanceSO.points;
            }
        }
    }
}

