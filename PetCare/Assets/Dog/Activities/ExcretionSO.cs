using Dog;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Activity
{
    [CreateAssetMenu(menuName = "Dog/Activity/Excretion")]
    public class ExcretionSO : ActivitySO
    {
        public enum BodyOpening
        {
            Butt = 0,
            Bladder = 1,
            Mouth = 2,
        }
        [Header("Excretion")]
        public GameObject excrements;
        public BodyOpening bodyOpening;
        public ConditionSO releavingCondition;
        public QuicktipSO quicktip;
        public override ActivityExecution StartActivity(GameObject dog, object argument)
        {
            ActivityExecution activityExecution = new Excrete(dog, this);
            return activityExecution;
        }

    }

    [System.Serializable]
    public class Excrete : ActivityExecution
    {
        private enum ExcretionState
        {
            GoIntoPosition,
            Releave,
            ResetPosition,
        }
        private ExcretionSO m_Excretion;
        private Body m_Body;
        private Mind m_Mind;
        private ExcretionState m_ExcretionState;
        public Excrete(GameObject dog, ExcretionSO excretion) : base(excretion)
        {
            m_Excretion = excretion;
            m_ExcretionState = ExcretionState.GoIntoPosition;
            m_Body = dog.GetComponent<Body>();
            m_Mind = dog.GetComponent<Mind>();
            if (m_Excretion.bodyOpening == ExcretionSO.BodyOpening.Butt)
                m_Body.ChangeBodyPose(DogPose.Sit, () => { Releave(); });
            else
                m_Body.ChangeBodyPose(DogPose.Pee, () => { Releave(); });
        }
        private void Releave()
        {
            Vector3 position = m_Body.legs.rearStand.position + Vector3.up * 0.001f;
            if (m_Excretion.excrements != null)
                Object.Instantiate(m_Excretion.excrements, position, Quaternion.identity);
            m_ExcretionState = ExcretionState.Releave;
        }
        public override void Update()
        {
            if (m_ExcretionState == ExcretionState.Releave)
            {
                var currentReleave = m_Mind.dogState.GetCondition(m_Excretion.releavingCondition);
                currentReleave -= 25.0f * Time.deltaTime;
                if (currentReleave <= 0f)
                {
                    currentReleave = 0f;
                    m_ExcretionState = ExcretionState.ResetPosition;
                    m_Body.ChangeBodyPose(DogPose.Stand, () => EndActivity());
                }
                m_Mind.dogState.SetCondition(m_Excretion.releavingCondition, currentReleave);
            }
        }
        public override void EndRequested()
        {

        }
        public override void OnEnd()
        {
            if (SceneManager.GetActiveScene().name != "Park")
                GlobalEvents.Instance.triggerQuickTip.Invoke(m_Excretion.quicktip);
        }
        public override int GetPoints()
        {
            if (SceneManager.GetActiveScene().name != "Park")
            {
                return m_Excretion.points * (-1);
            }
            else
            {
                return m_Excretion.points;
            }
        }
    }
}
