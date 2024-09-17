using UnityEngine.Animations.Rigging;
using UnityEngine;
using UnityEngine.Splines;
using System.Collections.Generic;
using Unity.Mathematics;

[System.Serializable]
public class Leg
{
    public OverrideTransform foot;
    public Transform paw;
    public float stepHeight;
    [Range(0f, 90f)]
    public float footStepRotation;
    [Range(-1f, 1f)]
    public float stepPositionOffset;
    public float walkCycleStart;
    public Vector3 offset;
    public Vector3 oldTargetPosition;
    public float relativeRestPosition;
    private Quaternion m_OldTargetRotation;
    private Vector3 m_NewTargetPosition;
    private Quaternion m_NewTargetRotation;
    private float m_TargetProgress;
    private float m_StepDuration;
    private Spline m_Path;
    public bool IsFront()
    {
        return offset.z > 0f;
    }
    public void Update()
    {
        this.m_TargetProgress = Mathf.Min(1.0f, this.m_TargetProgress + Time.deltaTime / this.m_StepDuration);
        var position = m_Path.EvaluatePosition(m_TargetProgress);
        var midTimeFactor = (0.5f - Mathf.Cos(m_TargetProgress * Mathf.PI * 2.0f) * 0.5f);
        position.y += midTimeFactor * stepHeight;
        this.foot.data.rotation = new Vector3(midTimeFactor * footStepRotation, 0f, 0f);
        paw.position = position;
        paw.rotation = Quaternion.Lerp(this.m_OldTargetRotation, this.m_NewTargetRotation, m_TargetProgress);
    }
    public void Start(string positionalTag, float frontPosition)
    {
        m_Path = new Spline(new BezierKnot[] {
            new BezierKnot(this.paw.position, 0, 0),
            new BezierKnot(this.paw.position, 0, 0),
            new BezierKnot(this.paw.position, 0, 0),
        });
        m_Path.SetTangentMode(TangentMode.AutoSmooth);
        this.oldTargetPosition = this.paw.position;
        this.m_OldTargetRotation = this.paw.rotation;
        this.m_NewTargetPosition = this.paw.position;
        this.m_NewTargetRotation = this.paw.rotation;
        this.m_TargetProgress = 1.0f;
        this.offset = paw.localPosition;
        this.relativeRestPosition = frontPosition - paw.localPosition.z;
    }
    public void StepOn(Vector3 position, Quaternion rotation, Vector3 controlPoint, float duration)
    {
        RaycastHit hit;
        if (!Physics.Raycast(position + 0.3f * Vector3.up, Vector3.down, out hit, Mathf.Infinity, 0xff))
            return;
        if (Vector3.Distance(hit.point, m_NewTargetPosition) < 0.001f)
            return;
        this.oldTargetPosition = this.m_NewTargetPosition;
        this.m_OldTargetRotation = this.m_NewTargetRotation;
        this.m_NewTargetPosition = hit.point;
        this.m_NewTargetRotation = rotation;
        this.m_TargetProgress = 0.0f;
        this.m_StepDuration = duration;
        m_Path.SetKnot(0, new BezierKnot(oldTargetPosition, 0, 0));
        m_Path.SetKnot(1, new BezierKnot(controlPoint, 0, 0));
        m_Path.SetKnot(2, new BezierKnot(m_NewTargetPosition, 0, 0));
    }
}


[System.Serializable]
public class Legs
{
    public enum Mode
    {
        Walk,
        Stand,
        Sit,
        Place,
    }
    public Mode mode;
    public Transform destination;
    public Leg leftFront;
    public Leg rightFront;
    public Leg leftBack;
    public Leg rightBack;
    public Transform frontStand;
    public Transform rearStand;
    public IEnumerator<Leg> GetEnumerator()
    {
        yield return leftFront;
        yield return rightFront;
        yield return leftBack;
        yield return rightBack;
    }
    public Leg[] legs
    {
        get
        {
            return new Leg[] { leftFront, rightBack, rightFront, leftBack };
        }
    }
    public void Start(Transform transform, Trunk trunk)
    {
        frontStand = new GameObject("FrontStand").transform;
        frontStand.transform.position = new Vector3(trunk.neck.position.x, transform.position.y, trunk.neck.position.z);
        frontStand.transform.rotation = trunk.neck.rotation;
        rearStand = new GameObject("RearStand").transform;
        rearStand.transform.position = new Vector3(trunk.pelvis.position.x, transform.position.y, trunk.pelvis.position.z);
        rearStand.transform.rotation = trunk.pelvis.rotation;
        var frontPosition = trunk.neck.localPosition.z;
        leftFront.Start("LF", frontPosition);
        rightFront.Start("RF", frontPosition);
        leftBack.Start("LB", frontPosition);
        rightBack.Start("RB", frontPosition);
    }
    public void Update()
    {
        foreach (var leg in this) leg.Update();
    }
}