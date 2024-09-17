using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Splines;

public class Pathfinding
{
    public NavMeshAgent m_Agent;
    public Spline path;

    public Pathfinding(NavMeshAgent agent)
    {
        m_Agent = agent;
    }



    /// <summary>
    /// Sets a new destination point to reach. If an attempt is already being made to reach a goal, this will be aborted first.
    /// </summary>
    /// <param name="destination">This is a position directly in front of the destination point?, to easily interact with objects</param>
    public void WalkToDestinationPoint(Vector3 destination, Spline path)
    {
        this.path = path;
        var waypoints = GetNavMeshWayPoints(destination);
        if (waypoints != null && waypoints.Length > 1)
        {
            //first point of Navmesh is equal to last point of path
            waypoints = waypoints.Skip(1).ToArray();

            AddWaypointsToPath(waypoints);
        }
    }

    public void AddWaypointsToPath(Vector3[] waypoints)
    {
        for (int i = 0; i < waypoints.Length - 1; i++)
        {
            this.path.Add(new BezierKnot(waypoints[i]), TangentMode.AutoSmooth);
        }
        this.path.Add(new BezierKnot(waypoints[^1]), TangentMode.AutoSmooth);
    }

    private Vector3[] GetNavMeshWayPoints(Vector3 destination)
    {
        NavMeshPath navPath = m_Agent.path;
        if (NavMesh.CalculatePath(path[^1].Position, destination, NavMesh.AllAreas, navPath))
        {
            Vector3[] cornerPoints = navPath.corners;
            Vector3[] waypoints = new Vector3[cornerPoints.Length * 2 - 1];
            for (int i = 0, j = 0; i < cornerPoints.Length; i++, j = j + 2)
            {
                //Navmesh has y-Offset of 0.0166667f. Ensure that path is on the ground with y = 0
                Vector3 currentWaypoint = new(cornerPoints[i].x, 0f, cornerPoints[i].z);
                waypoints[j] = currentWaypoint;
                if (i < cornerPoints.Length - 1)
                {
                    Vector3 nextWayPoint = new(cornerPoints[i + 1].x, 0f, cornerPoints[i + 1].z);
                    Vector3 position;
                    if (i == 0)
                    {
                        float3 pathPointLocation = path[^2].Position;
                        position = CalculateTurningPoint(currentWaypoint, nextWayPoint, pathPointLocation);
                    }
                    else
                    {
                        position = CalculateTurningPoint(currentWaypoint, nextWayPoint, waypoints[j - 1]);
                    }
                    waypoints[j + 1] = position;
                }
            }
            return waypoints;
        }
        else
        {
            //No valid path has been found
            return null;
        }
    }

    //summary
    //Calculates a point which functions as turning point for smoother curves when changing direction based on the current dog position and the direction, in which he is facing
    private Vector3 CalculateTurningPoint(Vector3 currentPoint, Vector3 nextPoint, Vector3 previousPoint)
    {

        Vector3 nextPointDirection = nextPoint - currentPoint;
        Vector3 incomingDirection = currentPoint - previousPoint;
        float turningAngleFactor = 0.5f;
        Vector3 turningVector = Vector3.Normalize(Vector3.Slerp(incomingDirection, nextPointDirection, turningAngleFactor));
        //Ensure that turning vector only rotates in the x-z-plane. Slerp tends to introduce y-coordinate errors
        turningVector.y = 0;
        //Adjust turning vector length
        float defaultTurningVectorLength = 0.2f;
        float distance = Vector3.Distance(currentPoint, nextPoint);
        if (distance < defaultTurningVectorLength)
        {
            turningVector = turningVector * distance;
        }
        else
        {
            turningVector = turningVector * defaultTurningVectorLength;
        }
        Vector3 turningPoint = new(turningVector.x + currentPoint.x, turningVector.y + currentPoint.y, turningVector.z + currentPoint.z);
        return turningPoint;
    }
}
