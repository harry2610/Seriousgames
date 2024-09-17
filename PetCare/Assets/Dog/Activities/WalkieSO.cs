using System.Collections;
using System.Collections.Generic;
using Activity;
using Dog;
using UnityEngine;
using walkie;
using static UnityEngine.AdaptivePerformance.Provider.AdaptivePerformanceSubsystemDescriptor;


namespace Activity
{
    [CreateAssetMenu(menuName = "Dog/Activity/Walkie")]
    public class WalkieSO : ActivitySO
    {
        public float walkieSpeed;
        public float comeSpeed;
        public WayPointSO[] wayPoints;
        public ConditionSO poopingCondition;
        public ConditionSO peeingCondition;

        public override ActivityExecution StartActivity(GameObject dog, object argument)
        {
            ActivityExecution activityExecution = new WalkieWalkie(this, dog, wayPoints);
            return activityExecution;
        }
    }

    public class WalkieWalkie : ActivityExecution
    {
        private Body m_Body;
        private GameObject m_Dog;
        private Mind m_Mind;
        private WayPointSO [] m_WayPoints;
        private Camera m_Cam;
        public WalkieWalkie(WalkieSO walkieSO, GameObject dog, WayPointSO[] wayPoints): base(walkieSO)
        {
            m_Dog = dog;
            m_Body = dog.GetComponent<Body>();
            m_Cam = Camera.main;
            m_WayPoints = wayPoints;
            m_Mind = dog.GetComponent<Mind>();

            var info = new WalkInfo()
            {
                speed = walkieSO.comeSpeed,
                onDestinationReached = () =>
                {
                    //Extension idea: play pooping and peeing activity followed by cleaning activity during walkie
                    m_Mind.dogState.SetCondition(walkieSO.poopingCondition, 0);
                    m_Mind.dogState.SetCondition(walkieSO.peeingCondition, 0);

                    m_Body.ChangeBodyPose(DogPose.Sit, () =>
                    {
                        StaticCam dogCam = m_Cam.GetComponent<StaticCam>();
                        dogCam.SetMode(StaticCam.CamModus.Follow);

                        Vector3[] path = new Vector3[wayPoints.Length + 1];

                        int index = FindClosestWaypoint();

                        for (int i = 0; i <= wayPoints.Length; i++)
                        {
                            if (index > wayPoints.Length - 1)
                            {
                                index = 0;
                            }
                            path[i] = wayPoints[index].position;
                            index++;
                        }
                        var walkInfo = new WalkInfo()
                        {
                            speed = walkieSO.walkieSpeed,
                            onDestinationReached = () =>
                            {
                                m_IsReadyToEnd = true;
                                dogCam.SetMode(StaticCam.CamModus.Static);
                            },
                            destinationPath = path,
                        };
                        m_Body.Walk(walkInfo);
                    });
                    
                }

            };

            var playerPos = m_Cam.transform.position;
            playerPos += m_Cam.transform.forward * 1f;
            playerPos.y = 0f;
            info.destinationPath = new Vector3[] { playerPos };
            m_Body.Walk(info);
        }

        private int FindClosestWaypoint()
        {
            float closestDistance = Mathf.Infinity;
            int index = -1;
            //Find closest waypoint to staticCam position
            for (int i = 0; i < m_WayPoints.Length; i++)
            {
                float currentDist = Vector3.Distance(m_Cam.transform.position, m_WayPoints[i].position);
                if (currentDist < closestDistance)
                {
                    index = i;
                    closestDistance = currentDist;
                }
            }
            return index;
        }

        public override void Update()
        {
        }
        public override void EndRequested()
        {
            
        }
    }
}
