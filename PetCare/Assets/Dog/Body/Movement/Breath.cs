using UnityEngine;

[System.Serializable]
public class Breath
{
    // Pants per minute
    [Range(0f, 200f)]
    public float respiratoryRate;
    public float RespiratoryRateSI { get { return respiratoryRate / 60f; } }
    private Animator _animator;
    public void Update()
    {
        _animator.SetFloat("RespiratoryRate", RespiratoryRateSI);
    }
    public void Start(Animator animator)
    {
        _animator = animator;
    }
}