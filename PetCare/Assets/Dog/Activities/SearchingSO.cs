
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;


namespace Activity
{

    [CreateAssetMenu(menuName = "Dog/Activity/Search")]
    public class SearchSO : ActivitySO
    {
        [Header("Search")]
        public float speed;
        public override ActivityExecution StartActivity(GameObject dog, object argument)
        {
            return new Searching(this, dog);
        }
    }
    [System.Serializable]
    public class Searching : ActivityExecution
    {
        private SearchSO m_Search;
        private Body m_Body;
        private GameObject m_Dog;
        private List<SearchPoint> m_SearchPoints;
        private SearchPoint m_HidingSpot;
        private SearchPoint m_NextSearchPoint;
        public Searching(SearchSO search, GameObject dog) : base(search)
        {
            m_Search = search;
            m_Dog = dog;
            m_Body = dog.GetComponent<Body>();
            m_SearchPoints = new List<SearchPoint>(UnityEngine.Object.FindObjectsByType<SearchPoint>(FindObjectsSortMode.InstanceID));
            try
            {
                m_HidingSpot = m_SearchPoints.Single((pnt) => pnt.isHidingSpot);
            }
            catch (System.InvalidOperationException)
            {
                EndActivity();
                GlobalEvents.Instance.onDogEndedSearchGame.Invoke(false);
                return;
            }
            m_NextSearchPoint = null;
            FindNextSpot();
        }
        public void FindNextSpot()
        {
            if (m_NextSearchPoint != null)
            {
                m_SearchPoints.Remove(m_NextSearchPoint);
            }
            m_NextSearchPoint = m_SearchPoints[Random.Range(0, m_SearchPoints.Count)];
            Debug.Log($"Suchspiel: Hund sucht bei {m_NextSearchPoint.title}");
            var info = new WalkInfo()
            {
                speed = m_Search.speed,
                destinationPath = new Vector3[] { m_NextSearchPoint.transform.position },
                onDestinationReached = () => CheckSpot(),
            };
            m_Body.Walk(info);
        }
        public void CheckSpot()
        {
            Debug.Log($"Suchspiel: Hund pr�ft Position");
            GlobalEvents.Instance.onDogCheckSearchSpot.Invoke(m_NextSearchPoint);
            m_Body.ChangeBodyPose(DogPose.Consume, () =>
            m_Body.StartAnimation(DogAnimation.Sniff, () =>
            m_Body.ChangeBodyPose(DogPose.Stand, () =>
            {
                Debug.Log($"Suchspiel: Hund ist fertig mit Pr�fen");
                if (m_NextSearchPoint.isHidingSpot)
                {
                    m_NextSearchPoint.isHidingSpot = false;
                    Debug.Log($"Suchspiel: Hund hat Gegenstand Gefunden");
                    m_Body.StartAnimation(DogAnimation.Bark, () =>
                    m_Body.StartAnimation(DogAnimation.Bark, () =>
                    m_Body.StartAnimation(DogAnimation.Bark, () => EndActivity())));
                    GlobalEvents.Instance.onDogEndedSearchGame.Invoke(true);
                }
                else
                {
                    GlobalEvents.Instance.onDogHasNothingFound.Invoke();
                    Debug.Log($"Suchspiel: Hund sucht n�chste Position");
                    FindNextSpot();
                }
            })));
        }
        public override void Update()
        {
        }
    }
}