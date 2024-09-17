using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class Head
{
    public enum Mode
    {
        Look,
        Reach,
    }

    public Mode mode;
    public Transform target;
    public UnityEvent onObjectGrabbed;
    public Transform gripPoint;
    private Vector3 m_CurrentVelocity;
    private Vector3 m_StartPosition;
    private Vector3 m_FixPoint;
    public Vector3 startPosition
    {
        get
        {
            return m_StartPosition;
        }
    }
    public GameObject GrabbedObject
    {
        get
        {
            return gripPoint.childCount != 0 ? gripPoint.GetChild(0).gameObject : null;
        }
        set
        {
            if (gripPoint.childCount != 0)
            {
                var grabbable = gripPoint.GetChild(0);
                grabbable.transform.SetParent(null, true);
                if (grabbable.TryGetComponent(out Rigidbody rigidbody))
                    rigidbody.isKinematic = false;
            }
            if (value != null)
            {
                if (value.TryGetComponent(out Rigidbody rigidbody))
                    rigidbody.isKinematic = true;
                value.transform.SetParent(gripPoint, false);
                value.transform.SetLocalPositionAndRotation(new Vector3(0f, 0.05f, 0f), Quaternion.Euler(-90, 0,0));
            }
        }
    }
    public Vector3 position
    {
        get
        {
            return this.target.localPosition;
        }
    }
    public void Update()
    {
        target.transform.position = Vector3.SmoothDamp(target.transform.position, m_FixPoint, ref m_CurrentVelocity, 0.3f);
    }
    public void Start()
    {
        this.m_StartPosition = this.target.localPosition;
    }
    public void LookAt(Vector3 position)
    {
        m_FixPoint = position;
    }
    public void Grab(GameObject grabbable)
    {
        GrabbedObject = grabbable;
    }
    public void Touch(Vector3 position, UnityAction onTouch)
    {

    }
}