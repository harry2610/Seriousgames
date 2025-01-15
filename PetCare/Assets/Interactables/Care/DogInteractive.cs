using Dog;
using Impulses;
using UnityEngine;

public abstract class DogInteractive : MonoBehaviour
{
    [Range(0, 5)]
    public float cameraDistance;
    [Range(0, 5)]
    public float cameraHeightOffset;
    public ImpulseSO interactionImpulse;
    public ImpulseSO idleImpulse;
    public ParticleSystem interactionParticles;
    public float timeToApplyConditions;
    public ConditionSO growlCondition;
    public double growlThreshold;
    private float m_ConditionTimer;
    private StaticCam m_Camera;
    private ConditionContainer m_Conditions;
    protected FurSimulation m_FurSimulation;
    protected ParticleSystem m_ParticleInstance;
    private float m_RewardTimer;
    private readonly float timeLimit = 10.0f;

    void Start()
    {
        m_RewardTimer = 0.0f;
        m_ConditionTimer = timeToApplyConditions;
        m_Conditions = GetComponent<ConditionContainer>();
        m_FurSimulation = FindObjectOfType<FurSimulation>();
        if (m_FurSimulation == null)
        {
            Destroy(gameObject);
            return;
        }

        GlobalEvents.Instance.onDogImpulse.Invoke(interactionImpulse, null);

        double conditionValue = GameStateManager.Instance.gameState.dogs[0].GetCondition(growlCondition);
        if (conditionValue < growlThreshold)
        {
            m_FurSimulation.GetComponent<Body>().StartAnimation(DogAnimation.Growl, null);
        }

        // Set Focus on Dog
        m_Camera = Camera.main.GetComponent<StaticCam>();
        // m_Camera.focusYaw = Mathf.PI * 0.5f;
        m_Camera.ObjectFocus(m_FurSimulation.gameObject, cameraDistance, cameraHeightOffset);
        // Send Impulse

        m_ParticleInstance = Instantiate(interactionParticles, this.transform);
        m_ParticleInstance.transform.LookAt(m_Camera.transform);
        m_ParticleInstance.Stop();
    }

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            //m_ParticleInstance.transform.LookAt(m_Camera.transform);
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);



            if (!Physics.Raycast(ray, out RaycastHit hitData, 1000)) return;
            if (hitData.transform.parent == null || hitData.transform.parent.gameObject != m_FurSimulation.gameObject) return;

            m_ConditionTimer -= Time.deltaTime;
            if (m_ConditionTimer <= 0.0f)
            {
                m_Conditions.Apply();
                m_ConditionTimer = timeToApplyConditions;
            }

            this.transform.rotation = Quaternion.LookRotation(hitData.normal);
            m_ParticleInstance.Play();
            m_RewardTimer += Time.deltaTime;
            if (m_RewardTimer >= timeLimit)
            {
                m_RewardTimer = 0.0f;
                RewardManager.instance.UpdateScoreWithoutAnimation(1);
            }
            OnDogHit(hitData);
            return;
        }
        else
        {
            m_ParticleInstance.Stop();
        }
    }

    protected abstract void OnDogHit(RaycastHit hitData);

    private void OnDestroy()
    {
        if (m_Camera != null)
            m_Camera.EndObjectFocus();

        if (GlobalEvents.Instance != null)
            GlobalEvents.Instance.onDogImpulse.Invoke(idleImpulse, null);
    }
}