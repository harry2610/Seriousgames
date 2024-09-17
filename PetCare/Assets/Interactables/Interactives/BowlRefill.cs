using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UI.Items;
using UnityEngine;
using UnityEngine.Serialization;

public class BowlRefill : MonoBehaviour
{
    // Type of Bowl to refill
    public BowlType refillType;
    // Distance between camera and refill item
    [Range(0, 5)]
    public float cameraDistance;
    // Max distance of the turn
    [Range(-1, 1)] 
    public float cameraTurnOffset;
    // Distance from the camera center refill item is displayed
    [Range(-1, 1)] 
    public float cameraHeightOffset;
    // Speed at which the bowl is filled
    [Range(0, 10)] 
    public float fillRate;
    // Angle threshold where bowl is filled
    [Range(-1, 1)] 
    public float startThreshold;
    // Impulse on refill
    public Impulses.ImpulseSO refillImpulse;
    // Generated particles
    public ParticleSystem sustenanceParticles;
    private ParticleSystem.EmissionModule m_ParticleEmission;
    private ParticleSystem.MainModule m_ParticleMain;
    private ParticleSystem m_SustenanceParticleInstance;
    private Transform m_CameraTransform;
    private StaticCam m_CameraScript;
    private Bowl m_BowlScript;
    private Gyroscope m_Gyro;
    private bool m_Refilled;
    private float m_BaseEmission;
    private float m_BaseSpeed;
    
    // Start is called before the first frame update
    void Start()
    {
        Bowl[] activeBowls = FindObjectsOfType<Bowl>();

        // Find first BowlScript of refill Type
        m_BowlScript = Array.Find(activeBowls, bowl => bowl.bowlType == refillType);
        
        // Don't spawn food if no bowl is in scene
        if (m_BowlScript == null)
        {
            Destroy(this.gameObject);
            return;
        }

        m_SustenanceParticleInstance = Instantiate(sustenanceParticles, this.transform);
        m_SustenanceParticleInstance.Stop();
        m_ParticleMain = m_SustenanceParticleInstance.main;
        m_ParticleEmission = m_SustenanceParticleInstance.emission;
        m_BaseEmission = m_ParticleEmission.rateOverTimeMultiplier;
        
        // Camera zoom on bowl
        m_CameraScript = Camera.main.GetComponent<StaticCam>();
        m_CameraScript.ObjectFocus(m_BowlScript.gameObject, cameraDistance, cameraHeightOffset, true);

        m_CameraTransform = Camera.main.transform;
        
        // Position bag in front of camera
        transform.position = m_CameraTransform.position + (cameraDistance * m_CameraTransform.forward) + m_CameraTransform.up * (cameraTurnOffset);
        transform.LookAt(m_CameraTransform.position);

#if UNITY_EDITOR
        m_Refilled = true;
        m_BowlScript.FillValue = 100.0f;
        
        m_BowlScript.conditionEffects = GetComponent<ConditionContainer>().Effects;
#else
        // Gyro setup
        m_Gyro = Input.gyro;
        m_Gyro.enabled = true;
        m_Refilled = false;
#endif
    }

    
    // Update is called once per physics simulation frame
    void FixedUpdate()
    {
#if UNITY_EDITOR
#else
        // Gyro rotation calculation
        Vector3 currentAngles = transform.eulerAngles;
        Vector3 gyroAngles = Input.gyro.rotationRate;
        Vector3 targetAngles = currentAngles - gyroAngles * (Time.deltaTime * Mathf.Rad2Deg);
        
        // Optionally lock x rotation for different rotation experience
        targetAngles.x = 0;
        targetAngles.y = 0;
        
        transform.eulerAngles = targetAngles;
        
        // Magnitude of angle
        float dot = Vector3.Dot(transform.up, Vector3.up);
        
        transform.position = m_CameraTransform.position + (cameraDistance * m_CameraTransform.forward) + m_CameraTransform.up * (cameraTurnOffset * dot - 0.15f);
        
        if(dot < startThreshold)
        {
            if (!m_Refilled)
            {
                // Set bowl condition effects to the current sustenance
                m_BowlScript.conditionEffects = GetComponent<ConditionContainer>().Effects;
                // Only set refill to true if bowl has actually been refilled 
                m_Refilled = true;
            } 
            
            // Refill faster with greater angle
            float angleMultiplier = Mathf.Abs(dot - 1);
            float totalMultiplier = (m_Gyro.userAcceleration.magnitude + 1) * angleMultiplier;
            m_ParticleEmission.rateOverTimeMultiplier = totalMultiplier * m_BaseEmission;
            m_ParticleMain.startSpeed = totalMultiplier * 0.5f;
            // Shaking additionally increases bowl refill speed
            float additionalFill = Time.deltaTime * fillRate * totalMultiplier;
            
            // Adjust bowl fill
            m_BowlScript.FillValue = additionalFill + m_BowlScript.FillValue;
            
            m_SustenanceParticleInstance.Play();
        }
        else
        {
            m_SustenanceParticleInstance.Stop();
        }
#endif
    }

    private void OnDestroy()
    {
        // If bowl refilled at all send impulse
        if(m_Refilled)
            GlobalEvents.Instance.onDogImpulse.Invoke(refillImpulse, gameObject);
        
        
        m_CameraScript.EndObjectFocus();
#if UNITY_EDITOR
#else
        m_Gyro.enabled = false;
#endif
    }
}
