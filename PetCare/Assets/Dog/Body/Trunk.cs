using UnityEngine;

[System.Serializable]
public class Trunk
{
    public Transform neck;
    public Transform pelvis;
    private BodySettings _settings;
    private Vector3 _neckOffset;
    private Vector3 _pelvisOffset;
    private float _wobbleTimer;
    public float Length { get { return _neckOffset.z - _pelvisOffset.z; } }
    public void Start(BodySettings settings)
    {
        _settings = settings;
        _neckOffset = neck.localPosition;
        _pelvisOffset = pelvis.localPosition;
    }
    public void Update(Head head, Legs legs, Breath breathing)
    {
        _wobbleTimer = (_wobbleTimer + breathing.RespiratoryRateSI * Time.deltaTime) % 1f;
        var wobbleHeight = (Mathf.Sin(_wobbleTimer * Mathf.PI * 2f) - 1f) * _settings.breathingWobbleAmplitude;
        var pelvisHeight = _pelvisOffset.y + wobbleHeight + 
            0.5f * (legs.leftBack.paw.position.y + legs.rightBack.paw.position.y) * _settings.pawInfluenceForPelvis;
        var pelvisPosition = legs.rearStand.position + Vector3.up * pelvisHeight;
        pelvis.SetPositionAndRotation(pelvisPosition, legs.rearStand.rotation);
        var neckHeight = _neckOffset.y + wobbleHeight + 
            0.5f * (legs.leftFront.paw.position.y + legs.rightFront.paw.position.y) * _settings.pawInfluenceForNeck;
        var neckPosition = legs.frontStand.position + Vector3.up * neckHeight + legs.frontStand.forward * _settings.spineStretching;
        neck.SetPositionAndRotation(neckPosition, legs.frontStand.rotation);
    }
}
