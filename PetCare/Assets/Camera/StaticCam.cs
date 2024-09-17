using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;


public class StaticCam : MonoBehaviour
{
    public enum CamModus
    {
        Static,
        Follow,
        Focus,
        Custom,
    }

    private CamModus m_CamModus;
    private bool m_GyroEnable;
    public float smoothness;
    private Transform m_DefaultTargetObject;
    public Transform focusTargetObject;
    private float m_Pitch;
    private Vector3 m_InitialOffset;
    private Vector3 m_CameraPosition;
    public enum RelativePosition { InitalPosition, Position1, Position2 }
    public RelativePosition relativePosition;
    public Vector3 position1;
    public Vector3 position2;
    private Vector3 focusOffset;
    private float m_FocusDistance;
    private float m_FocusHeightOffset;
    private Vector3 m_InitialPosition;
    private float m_RotationSpeed;

    //Follow Cam parameters
    private float m_DistanceFromTarget = 1.5f;
    private float m_HeightAboveTarget = 1.5f;
    private float m_SmoothTime = 0.3f;
    private Vector3 m_SmoothVelocity = Vector3.zero;

    private void Awake()
    {
        GlobalEvents.Instance.onDogSpawned.AddListener(SetDefaultTargetObject);
        relativePosition = RelativePosition.InitalPosition;
        m_InitialPosition = transform.position;
        m_CamModus = CamModus.Static;
    }

    public void SetMode(CamModus mode)
    {
        m_CamModus = mode;
    }

    public void SetInitialCameraPosition(Vector3 position)
    {
        m_InitialPosition = position;
        transform.position = position;
    }

    void SetDefaultTargetObject(GameObject obj)
    {
        m_DefaultTargetObject = obj.transform;
        m_InitialOffset = transform.position - m_DefaultTargetObject.position;
    }

    public void ObjectFocus(GameObject obj, float distance, float heightOffset, bool gyroEnable = false, float pitch = 0f)
    {
        m_Pitch = pitch;
        m_FocusDistance = distance;
        m_FocusHeightOffset = heightOffset;
        m_GyroEnable = gyroEnable;
        m_CamModus = CamModus.Focus;
        focusTargetObject = obj.transform;
        transform.position = focusTargetObject.forward * m_FocusDistance * Mathf.Cos(pitch) +
                             focusTargetObject.up * m_FocusDistance * Mathf.Sin(pitch) +
                             focusTargetObject.position;
        transform.LookAt(focusTargetObject.position);
        focusOffset = m_FocusHeightOffset * transform.up;
        transform.position += focusOffset;
    }

    public void EndObjectFocus()
    {
        SetFollowModus();
        m_GyroEnable = false;
    }

    void FixedUpdate()
    {
        switch (m_CamModus)
        {
            case CamModus.Static:
                transform.position = m_InitialPosition;
                transform.LookAt(m_DefaultTargetObject);
                break;
            case CamModus.Follow:
                // Calculate the desired position
                Vector3 desiredPosition = m_DefaultTargetObject.position - m_DefaultTargetObject.forward * m_DistanceFromTarget + Vector3.up * m_HeightAboveTarget;
                // Smoothly move the camera to the desired position
                transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref m_SmoothVelocity, m_SmoothTime);
                // Ensure the camera always looks at the target
                transform.LookAt(m_DefaultTargetObject.position + Vector3.up);
                break;
            case CamModus.Focus:
                if (m_GyroEnable)
                {
                    Vector3 targetAngles = transform.eulerAngles + Input.gyro.rotationRate * (Time.fixedDeltaTime * Mathf.Rad2Deg);
                    targetAngles.x = transform.eulerAngles.x;
                    targetAngles.y = transform.eulerAngles.y;
                    transform.eulerAngles = targetAngles;
                }
                else
                {

                    //transform.position = focusTargetObject.forward * m_FocusDistance * Mathf.Sin(m_Pitch) +
                    //                     focusTargetObject.up * m_FocusDistance * Mathf.Cos(m_Pitch) + focusTargetObject.position;
                    transform.RotateAround(focusTargetObject.position, Vector3.up, m_RotationSpeed * Time.deltaTime);
                    // transform.position = focusTargetObject.forward * m_FocusDistance + focusTargetObject.position + m_FocusHeightOffset * focusTargetObject.up;
                    transform.LookAt(focusTargetObject.position + focusOffset);
                }
                break;
            case CamModus.Custom:
                break;
        }
    }
    public void SetCustomTransform(Vector3 position, Quaternion rotation)
    {
        m_CamModus = CamModus.Custom;
        transform.position = position;
        transform.rotation = rotation;
        Debug.Log("Camera: Custom Mode activated");
    }
    public void SetFollowModus()
    {
        m_CamModus = CamModus.Static;
        Debug.Log("Camera: Follow Mode activated");
    }
    public void setRotation(float speed)
    {
        m_RotationSpeed = speed;
    }

    Vector3 CameraOffset(RelativePosition relativePos)
    {
        Vector3 currentOffset;

        switch (relativePos)
        {
            case RelativePosition.Position1:
                currentOffset = position1;
                break;

            case RelativePosition.Position2:
                currentOffset = position2;
                break;

            default:
                currentOffset = m_InitialOffset;
                break;
        }
        return currentOffset;
    }
}