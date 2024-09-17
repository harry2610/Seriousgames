using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Activity
{

    [CreateAssetMenu(menuName = "Dog/Activity/WalkToDestination")]
    public class WalkToDestinationSO : ActivitySO
    {
        public enum Destination
        {
            RandomPoint = 0,
            DogOwner = 1,
        }
        [Header("Walk to destination")]
        public Destination destination;
        public float speed;
        public Vector3 predefinedCoordinate;
        public override ActivityExecution StartActivity(GameObject dog, object argument)
        {
            return new WalkingToDestination(this, dog);
        }
    }
    [System.Serializable]
    public class WalkingToDestination : ActivityExecution
    {
        private Body m_Body;
        private GameObject m_Dog;
        public WalkingToDestination(WalkToDestinationSO walkToDestination, GameObject dog) : base(walkToDestination)
        {
            m_Dog = dog;
            m_Body = dog.GetComponent<Body>();
            var info = new WalkInfo()
            {
                speed = walkToDestination.speed,
                onDestinationReached = () => EndActivity(),
            };
            switch (walkToDestination.destination)
            {
                case WalkToDestinationSO.Destination.RandomPoint:
                    info.destinationPath = new Vector3[] { m_Body.walkableMesh.RandomPoint() };
                    break;
                case WalkToDestinationSO.Destination.DogOwner:
                    var playerPos = Camera.main.transform.position;
                    playerPos += Camera.main.transform.forward * 1f;
                    playerPos.y = 0f;
                    info.destinationPath = new Vector3[] { playerPos };
                    break;
            }
            m_Body.Walk(info);
        }
        public override void Update()
        {
        }
    }
}