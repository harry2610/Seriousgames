using System;
using System.Collections;
using System.Collections.Generic;
using Activity;
using Dog;
using UnityEngine;
namespace walkie
{
    public class WalkieManager : MonoBehaviour
    {
        public ActivitySO walkie;
        public WalkToDestinationSO walkToPredefinedCoordinate;
        private GameObject m_Dog;
        private Mind m_DogMind;
        private Body m_DogBody;

        public void Awake()
        {
            GlobalEvents.Instance.onDogSpawned.AddListener(SetDogObject);
        }

        void SetDogObject(GameObject obj)
        {
            m_Dog = obj;
            m_DogMind = m_Dog.GetComponent<Mind>();
            m_DogBody = m_Dog.GetComponent<Body>();
        }

        public void StartWalkie(changeVisibilityDelegate changeVisibility)
        {
            changeVisibility(IsVisible.Hidden);
            m_DogMind.AddActivityInterruptingCurrent(walkie, null, () =>
            {
                changeVisibility(IsVisible.Visible);
            });

        }
        
    }
}
