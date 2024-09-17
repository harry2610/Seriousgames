using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.Splines;

public struct WalkInfo
{
    public Vector3 [] destinationPath;
    public float speed;
    public UnityAction onDestinationReached;
}

public class Walk : MonoBehaviour
{
    private Pathfinding pathfinding;
    public Spline path;
    public SpeedController move;
    private UnityAction m_OnDestinationReached;
    private BodySettings m_Settings;
    private Legs m_Legs;
    private Trunk m_Trunk;
    public int m_LegsRestingCount;
    public int m_LegID;
    private IEnumerator m_Coroutine;
    private float RearStandOnPath
    {
        get { return move.position - m_Trunk.Length; }
    }
    public void Init(BodySettings settings, Legs legs, Trunk trunk)
    {
        m_LegID = 0;
        m_Settings = settings;
        m_Legs = legs;
        m_Trunk = trunk;
        m_LegsRestingCount = 0;
        path = new Spline(new BezierKnot[] {
            new BezierKnot(m_Legs.rearStand.position),
            new BezierKnot(m_Legs.frontStand.position),
        });
        path.SetTangentMode(TangentMode.AutoSmooth);
        move = new SpeedController(m_Trunk.Length, path.GetLength(), m_Settings.acceleration, m_Settings.deceleration);
        m_Coroutine = null;
        var navMeshAgent = GetComponent<NavMeshAgent>();
        pathfinding = new Pathfinding(navMeshAgent);
    }
    private IEnumerator FootSteps()
    {
        while (m_LegsRestingCount < 4 && !move.HasFinished())
        {
            var leg = m_Legs.legs[m_LegID];
            float legMoveTime = 0.5f * m_Settings.stepSize / move.travelSpeed;
            SpeedController movePrediction = move;
            movePrediction.Simulate(legMoveTime);
            var nextPathPos = movePrediction.position - leg.relativeRestPosition +
                (leg.IsFront() ? m_Settings.frontPawMaxDisplacement : m_Settings.rearPawMaxDisplacement);
            var restingPathPos = move.routeLength - leg.relativeRestPosition;
            if (nextPathPos >= restingPathPos)
            {
                nextPathPos = restingPathPos;
                m_LegsRestingCount += 1;
            }
            var pose = GetPathPose(nextPathPos);
            var stepPosition = pose.position + pose.right * leg.offset.x;
            var controlPosition = Vector3.Lerp(leg.paw.position, stepPosition, 0.5f);
            leg.StepOn(stepPosition, pose.rotation, controlPosition, legMoveTime);
            yield return new WaitForSeconds(leg.walkCycleStart * m_Settings.stepSize / move.travelSpeed);
            m_LegID = (m_LegID + 1) % 4;
        }
        m_Coroutine = null;
    }

    private Pose GetPathPose(float pathPosition)
    {
        float convertedPathPos = pathPosition / path.GetLength();
        var position = path.EvaluatePosition(convertedPathPos);
        var rotation = Quaternion.LookRotation(path.EvaluateTangent(convertedPathPos), Vector3.up);
        return new Pose(position, rotation);
    }
    void Update()
    {
        move.Simulate(Time.deltaTime);
        var frontPose = GetPathPose(move.position);
        m_Legs.frontStand.transform.position = frontPose.position;
        m_Legs.frontStand.transform.rotation = Quaternion.Slerp(m_Legs.frontStand.transform.rotation, frontPose.rotation, 1f - Mathf.Pow(0.001f, Time.deltaTime));
        var rearPose = GetPathPose(RearStandOnPath);
        m_Legs.rearStand.transform.position = rearPose.position;
        m_Legs.rearStand.transform.rotation = Quaternion.Slerp(m_Legs.rearStand.transform.rotation, rearPose.rotation, 1f - Mathf.Pow(0.001f, Time.deltaTime));
        if (move.HasFinished())
        {
            var drainedOnDestinationReached = m_OnDestinationReached;
            m_OnDestinationReached = null;
            if (m_Coroutine != null)
            {
                StopCoroutine(m_Coroutine);
                m_Coroutine = null;
            }
            this.enabled = false;
            if (drainedOnDestinationReached != null)
                drainedOnDestinationReached.Invoke();
        }
    }
    private void ClearPath()
    {
        var oldSpeed = move.speed;
        path = new Spline(new BezierKnot[] {
            new BezierKnot(m_Legs.rearStand.position),
            new BezierKnot(m_Legs.frontStand.position),
        });
        move = new SpeedController(m_Trunk.Length, path.GetLength(), m_Settings.acceleration, m_Settings.deceleration);
        move.speed = oldSpeed;
        path.SetTangentMode(TangentMode.AutoSmooth);
    }

    public bool StartWalk(WalkInfo info)
    {
        m_OnDestinationReached = info.onDestinationReached;
        this.enabled = true;
        ClearPath();

        foreach (Vector3 waypoint in info.destinationPath)
        {
            pathfinding.WalkToDestinationPoint(waypoint, path);
        }

        this.move.travelSpeed = info.speed;
        this.move.routeLength = path.GetLength();
        m_LegsRestingCount = 0;
        if (m_Coroutine == null)
        {
            m_Coroutine = FootSteps();
            StartCoroutine(m_Coroutine);
        }
        return true;
    }

    public bool IsWalking()
    {
        return m_Coroutine != null;
    }
    public void Abort(UnityAction onDestinationReached)
    {
        m_OnDestinationReached = onDestinationReached;
        this.move.Abort();
    }
}
