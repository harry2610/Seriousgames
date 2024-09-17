using UnityEngine;

public class ExcretionInstance : MonoBehaviour
{
    // Type of the excretion
    public bool requiresWiping;
    // Time until full Scale is reached
    public float timeEndScale = 1.5f;
    // Final object scale
    private Vector3 m_FinalSize;

    private float m_Timer;
    // Start is called before the first frame update
    void Start()
    {
        m_FinalSize = transform.localScale;
        transform.localScale = 0.01f * m_FinalSize;
    }

    // Update is called once per frame
    void Update()
    {
        m_Timer += Time.deltaTime;
        if (m_Timer >= timeEndScale)
        {
            transform.localScale = m_FinalSize;
            enabled = false;
            return;
        }
        transform.localScale = (m_Timer / timeEndScale) * m_FinalSize;
    }
}
