using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class HomeMenuDog : MonoBehaviour
{
    private Animator m_Animator;
    private bool m_IsSitting;
    private double m_PoseChangeTimer;
    void Start()
    {
        m_Animator = transform.GetComponent<Animator>();
        m_IsSitting = true;
        m_Animator.SetInteger("Pose", 1);
        StartPoseChangeTimer();
    }
    void Update()
    {
        m_PoseChangeTimer -= Time.deltaTime;
        if (m_PoseChangeTimer <= 0f)
        {
            m_IsSitting = !m_IsSitting;
            m_Animator.SetInteger("Pose", m_IsSitting ? 1 : 0);
            StartPoseChangeTimer();
        }
    }
    void StartPoseChangeTimer()
    {
        // Change body pose after a random time between 5 and 20 seconds
        m_PoseChangeTimer = Random.Range(5f, 20f);
    }
}
