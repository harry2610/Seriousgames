using UnityEngine;

[System.Serializable]
public class Tail
{
    public enum Mode
    {
        Wag,
        Dangle,
        Clamp,
    }
    public Mode mode;
    // Wags per minute
    [Range(0f, 300f)]
    public float waggingRate;
    public float WaggingRateSI { get { return waggingRate / 60f; } }
    private Animator m_Animator;
    public void Start(Animator animator)
    {
        m_Animator = animator;
    }
    public void Update()
    {
        m_Animator.SetFloat("WaggingRate", WaggingRateSI);
    }
}