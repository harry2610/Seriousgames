using UnityEngine;
using static UnityEngine.Networking.UnityWebRequest;
using UnityEngine.AI;

namespace Activity
{

    [CreateAssetMenu(menuName = "Dog/Activity/FetchBall")]
    public class FetchBallSO : ActivitySO
    {
        [Header("FetchBall")]
        public float speed;
        public override ActivityExecution StartActivity(GameObject dog, object argument)
        {
            if (argument is GameObject ball)
            {
                return new FetchingBall(dog, ball, speed, this);
            }
            else
            {
                GameObject foundedBall = FindAnyObjectByType<Throwable>().gameObject;
                return new FetchingBall(dog, foundedBall, speed, this);
            }
        }
    }

    [System.Serializable]
    public class FetchingBall : ActivityExecution
    {
        private Body m_Body;
        private GameObject m_Dog;
        private GameObject m_Ball;
        private Vector3 m_Destination;
        private float m_Speed;
        private bool m_BallIsAtXZLocation;
        private bool m_IsGrabbing;
        public FetchingBall(GameObject dog, GameObject ball, float speed, FetchBallSO parent) : base(parent)
        {
            m_Speed = speed;
            m_Dog = dog;
            m_Body = dog.GetComponent<Body>();
            m_Ball = ball;
            m_Destination = m_Ball.transform.position;
            m_BallIsAtXZLocation = false;
            var info = new WalkInfo()
            {
                destinationPath = new Vector3[] { m_Destination },
                speed = m_Speed,
                onDestinationReached = () => m_BallIsAtXZLocation = true,
            };
            m_Body.Walk(info);
        }
        public override void Update()
        {
            if (m_Ball != null && m_Ball.gameObject.scene != null)
            {
                if (!m_IsGrabbing)
                {
                    m_Body.LookAt(m_Ball.transform.position);
                    Vector3 ballPos = m_Ball.transform.position;
                    ballPos.y = m_Destination.y;
                    if (Vector3.Distance(ballPos, m_Destination) > 0.1f)
                    {
                        m_Destination = m_Ball.transform.position;
                        NavMeshHit hit;
                        if (NavMesh.SamplePosition(m_Destination, out hit, 1.0f, NavMesh.AllAreas))
                        {
                            m_Destination = hit.position;
                            m_BallIsAtXZLocation = false;
                            var info = new WalkInfo()
                            {
                                destinationPath = new Vector3[] { m_Destination },
                                speed = m_Speed,
                                onDestinationReached = () => m_BallIsAtXZLocation = true,
                            };
                            m_Body.Walk(info);
                        }
                    }
                    if (m_BallIsAtXZLocation && Mathf.Abs(m_Ball.transform.position.y - m_Dog.transform.position.y) < 0.1)
                    {
                        if(m_Ball.TryGetComponent<ConditionContainer>(out ConditionContainer container))
                        {
                            container.Apply();
                        }
                        
                        m_IsGrabbing = true;
                        if (m_Ball.TryGetComponent<Throwable>(out Throwable throwable) && throwable.consumable)
                        {
                            m_Body.ChangeBodyPose(DogPose.Consume, () => 
                            {
                                m_Body.StartAnimation(DogAnimation.Eat, () =>
                                {
                                    m_Ball.SetActive(false);
                                    EndActivity();
                                });
                            });
                        }
                        else
                        {
                            m_Body.Grab(m_Ball, () => EndActivity());
                        }

                    }
                }
            }
            else
            {
                EndActivity();
            }
        }
        public override int GetPoints()
        {
            if (m_IsGrabbing)
                return base.GetPoints();
            else
                return 0;
        }
    }
}
