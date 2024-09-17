using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

public class RemovingExcretion : MonoBehaviour
{
    public float cameraDistanceExcretion;
    public float cameraHeightOffsetExcretion;
    public float bagFloorYOffset;
    public bool requireWiping;
    public ParticleSystem m_CleanedParticles;
    public float wipeThreshold;
    public float requiredWipeTime;
    private ExcretionInstance m_ExcretionInstance;
    private bool m_ItemMoving = false;
    private bool m_Removed = false;
    private Collider m_Collider;
    private Collider m_ExcretionCollider;
    private Vector3 m_LastTouchPosition = new Vector3(0, 0, 0);
    private float m_WipeTimer;
    private Vector3 m_ExcretionMaxScale;
    private StaticCam m_Camera;

    // Start is called before the first frame update
    void Start()
    {
        m_Camera = Camera.main.GetComponent<StaticCam>();
        ExcretionInstance[] excretions = FindObjectsOfType<ExcretionInstance>();
        
        // Find first Excretion fitting the removal style
        m_ExcretionInstance = Array.Find(excretions, (ExcretionInstance e) => e.requiresWiping == requireWiping);
        m_ExcretionMaxScale = m_ExcretionInstance.transform.localScale;
        if (m_ExcretionInstance == null)
        {
            Destroy(this);
            return;
        }
        
        m_Collider = this.GetComponent<Collider>();
        m_ExcretionCollider = m_ExcretionInstance.GetComponent<Collider>();
        
        // Set camera to excretion
        m_Camera.ObjectFocus(m_ExcretionInstance.gameObject, cameraDistanceExcretion, cameraHeightOffsetExcretion, false, 0.5f * Mathf.PI);
        
        Ray cameraRay = new Ray(m_Camera.transform.position, m_Camera.transform.forward);
        this.transform.LookAt(m_ExcretionInstance.transform);
        if (Placeable.FloorHit(cameraRay, out Vector3 floorHit))
        {
            Vector3 cameraDown = (-Camera.main.transform.up).normalized;
            cameraDown.y = 0;
            floorHit.y += bagFloorYOffset;
            this.transform.position = floorHit + cameraDown * 0.1f;
            Vector3 cameraAngles = m_Camera.transform.eulerAngles;
            cameraAngles.x = 0;
            this.transform.eulerAngles = cameraAngles;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
        if (m_ItemMoving)
        {
            DragItem();
        }
        else
        {
            ItemInHand();
        }
    }

    private void ItemInHand()
    {
        if (Input.touchCount <= 0) return;
        Ray hitRay = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
        // Check if item was touched
        if (Physics.Raycast(hitRay, out RaycastHit raycastHit, 100))
        {
            if (raycastHit.collider.Equals(m_Collider)) m_ItemMoving = true;
        };
    }

    private void DragItem()
    {
        Touch touch = Input.GetTouch(0);
        if (touch.phase == TouchPhase.Ended)
        {
            m_ItemMoving = false;
            return;
        }
        
        Camera cameraMain = Camera.main;
        Vector2 touchPos = touch.position;
        Ray worldRay = cameraMain.ScreenPointToRay(touchPos);
        
        // Move item on floor
        if (Placeable.FloorHit(worldRay, out Vector3 floorHit))
        {
            floorHit.y += bagFloorYOffset;
            transform.position = floorHit;
        }
        if (m_Removed) return;
        Ray hitRay = cameraMain.ScreenPointToRay(Input.GetTouch(0).position);
        RaycastHit[] hits = Physics.RaycastAll(hitRay, 100);
        
        // Test if poo is in hit elements
        RaycastHit hit = Array.Find(hits, (RaycastHit raycastHit) => raycastHit.collider.Equals(m_ExcretionCollider));
        if (hit.colliderInstanceID == 0) return;
        if(requireWiping)
        {
            // Calculate distance of input between frames
            float touchDistance = Vector3.Distance(m_LastTouchPosition, touch.position) / Time.deltaTime;
            m_LastTouchPosition = touch.position;
            
            // If no motion above threshold return
            if (touchDistance < wipeThreshold) return;
            
            // Calculate new size from time spent wiping
            m_WipeTimer += Time.deltaTime;
            float sizeMultiplier = m_WipeTimer / requiredWipeTime;
            
            if (sizeMultiplier < 0.7f)
            {
                // Shrink if shrinkage below threshold
                m_ExcretionInstance.transform.localScale = m_ExcretionMaxScale * (1 - sizeMultiplier);
            }
            else
            {
                // Remove excretion if shrinkage above threshold
                CleanExcretion();
            }
            
        }
        else
        {
            transform.localScale = 1.2f * transform.localScale;
            CleanExcretion();
        }

    }

    private void CleanExcretion()
    {
        if(TryGetComponent<ConditionContainer>(out ConditionContainer container))
        {
            container.Apply();
        }
        m_ExcretionInstance.gameObject.SetActive(false);
        m_Removed = true;
        AudioManager.Instance.PlaySoundEffectByIndex(0);
        Instantiate(m_CleanedParticles, transform.position, Quaternion.Euler(90,0,0));
    }
    private void OnDestroy()
    {
        m_Camera.EndObjectFocus();
        if(m_Removed)
            Destroy(m_ExcretionInstance);
    }
}
