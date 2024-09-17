using Impulses;
using System.Collections;
using System.Collections.Generic;
using Activity;
using Dog;
using UnityEngine;
using UnityEngine.Serialization;

internal enum ThrowState
{
    InHand,
    ThrowInitiated,
    ThrowEnded,
    ThrowDelay,
}
public class Throwable : MonoBehaviour
{
    // Time to reach new touch position
    [Range(0, 1)]
    public float throwSmooth;
    // Velocity multiplier of the Throwable after throw
    [Range(0, 1)]
    public float throwStrength;
    // Delay between throwable pickup and subsequent throw
    [Range(0, 5)]
    public float throwTorqueY;
    // Delay between throwable pickup and subsequent throw
    [Range(0, 5)]
    public float throwDelayTime;
    // Distance between throwable and camera
    [Range(0, 5)]
    public float cameraDistance;
    public float maxVelocityMagnitude;
    // Impulse for flying objects
    public ImpulseSO flyingBall;
    // Is consumable
    public bool consumable;
    // Current smoothing velocity
    private Vector2 m_ThrowableVelocity;
    // State of throwable
    private ThrowState m_ThrowState;
    // Timer for pickup delay
    private float m_Timer = 0.0f;
    private Rigidbody m_RigidBody;
    
    // Start is called before the first frame update
    private void Start()
    {
        m_RigidBody = this.GetComponent<Rigidbody>();
        m_RigidBody.useGravity = true;
        m_RigidBody.isKinematic = true;
        transform.position = Camera.main.transform.position + (cameraDistance * Camera.main.transform.forward);
    }

    // Update is called once per frame
    private void Update()
    {
        switch (m_ThrowState)
        {
            case ThrowState.InHand:
                ThrowableInHand();
                break;
            case ThrowState.ThrowDelay:
                ThrowDelay();
                break;
            case ThrowState.ThrowInitiated:
                ThrowInitiated();
                break;
            case ThrowState.ThrowEnded:
                ThrowEnded();
                break;
        }
    }

    private void ThrowDelay()
    {
        m_Timer += Time.deltaTime;
        Transform cameraTransform = Camera.main.transform;
        transform.position = cameraTransform.position + (cameraDistance * cameraTransform.forward);
        if (m_Timer < throwDelayTime) return;
        m_Timer = 0.0f;
        m_ThrowState = ThrowState.InHand;
    }
    
    private void ThrowEnded()
    {
        if (Input.touchCount <= 0) return;
        Touch touch = Input.GetTouch(0);
        // Retrieve throwable if interacted
        if (!ThrowableInteracted(touch.position)) return;

        // Ungrab throwable if in dogs mouth
        foreach(Body body in FindObjectsOfType<Body>())
        {
            if (body.head.GrabbedObject == this.gameObject)
            {
                body.head.Grab(null);
            }
        }
        this.transform.rotation = Quaternion.identity;
        Transform cameraTransform = Camera.main.transform;
        // Reset throwable position
        transform.position = cameraTransform.position + (cameraDistance * cameraTransform.forward);
        m_RigidBody.isKinematic = true;
        m_ThrowState = ThrowState.ThrowDelay;
    }
    
    private void ThrowInitiated()
    {
        Touch touch = Input.GetTouch(0);
        if (touch.phase is TouchPhase.Moved or TouchPhase.Stationary)
        {
            Camera cameraMain = Camera.main;
            Vector2 newScreenPosition = touch.position;
            // Calculations in screen space for directional independence 
            Vector2 ballScreenPosition = cameraMain.WorldToScreenPoint(transform.position);
            // Smoothly follow touch position
            Vector3 newBallPosition = Vector2.SmoothDamp(ballScreenPosition, newScreenPosition, ref m_ThrowableVelocity, throwSmooth);
            newBallPosition.z = cameraDistance;
            transform.position = cameraMain.ScreenToWorldPoint(newBallPosition);
        }
        else
        {
            Camera cameraMain = Camera.main;
            Transform cameraTransform = cameraMain.transform;
            m_RigidBody.isKinematic = false;
            
            Vector2 screenBallPosition = cameraMain.WorldToScreenPoint(transform.position);
            // Distance between touch and actual ball
            Vector2 ballForce = screenBallPosition - touch.position;
            Vector3 velocity = cameraTransform.right * (-ballForce.x * throwStrength) - cameraTransform.transform.up * (ballForce.y * throwStrength);
            
            // Ceil magnitude at preset value
            float magnitude = velocity.magnitude;
            print(magnitude);
            if (magnitude > maxVelocityMagnitude)
            {
                velocity = velocity.normalized * maxVelocityMagnitude;
                magnitude = maxVelocityMagnitude;
            }
            
            // Add forward momentum depending on throw magnitude
            velocity += cameraTransform.forward * magnitude;
            m_RigidBody.AddForce(velocity);
            m_RigidBody.AddTorque(0, throwTorqueY, 0);

            // Fire object thrown event
            GlobalEvents.Instance.onDogImpulse.Invoke(flyingBall, gameObject);
            
            m_ThrowState = ThrowState.ThrowEnded;
        }
    }

    private void ThrowableInHand()
    {
        Transform cameraTransform = Camera.main.transform;
        transform.position = cameraTransform.position + (cameraDistance * cameraTransform.forward);
        if (Input.touchCount <= 0) return;
        Touch touch = Input.GetTouch(0);
        // Initiate throw if throwable was touched
        if (ThrowableInteracted(touch.position))
        {
            m_ThrowState = ThrowState.ThrowInitiated;
        }
    }
    
    private bool ThrowableInteracted(Vector2 touchPosition)
    {
        Ray hitRay = Camera.main.ScreenPointToRay(touchPosition);
        // Throwable interaction if ray hits rigid body
        if (Physics.Raycast(hitRay, out RaycastHit raycastHit, 100) && raycastHit.rigidbody)
        {
            return raycastHit.rigidbody.Equals(m_RigidBody);
        };
        return false;
    }
}
