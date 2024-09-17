using UnityEngine;

[System.Serializable]
public struct SpeedController
{
    public float position;
    public float speed;
    public float travelSpeed;
    public float routeLength;
    public float acceleration;
    public float deceleration;
    public SpeedController(float position, float routeLength, float acceleration, float deceleration)
    {
        this.position = position;
        this.speed = 0f;
        this.travelSpeed = 0f;
        this.routeLength = routeLength;
        this.acceleration = acceleration;
        this.deceleration = deceleration;
    }
    public void Abort()
    {
        GetStopDistance(out float stopTime, out float stopDistance);
        routeLength = Mathf.Min(routeLength, position + stopDistance);
    }
    public readonly bool HasFinished()
    {
        GetStopDistance(out float stopTime, out float stopDistance);
        return speed <= 0.0001 && this.position >= this.routeLength - stopDistance;
    }
    private readonly void GetStopDistance(out float stopTime, out float stopDistance)
    {
        stopTime = travelSpeed / deceleration;
        stopDistance = AccDist(travelSpeed, -deceleration, stopTime);
    }
    private static float AccDist(float s, float a, float t)
    {
        return s * t + 0.5f * a * t * t;
    }
    public readonly float GetRemainingTime()
    {
        SpeedController speedController = this; // Copy
        return speedController.Simulate(10000f);
    }
    public float Simulate(float time)
    {
        float remainingTime = time;
        GetStopDistance(out float stopTime, out float stopDistance);
        float stopStart = routeLength - stopDistance;
        if (remainingTime > 0f && position < stopStart && !Mathf.Approximately(speed, travelSpeed))
        {   // Acceleration to Travel Speed Phase
            float accelerationTarget = travelSpeed > speed ? acceleration : -deceleration;
            float accelerationTime = Mathf.Min(remainingTime, (travelSpeed - speed) / accelerationTarget);
            float accelerationDistance = AccDist(speed, accelerationTarget, accelerationTime);
            remainingTime -= accelerationTime;
            position += accelerationDistance;
            speed += accelerationTime * accelerationTarget;
        }
        if (remainingTime > 0f && position < stopStart && Mathf.Approximately(speed, travelSpeed))
        {   // Move at Travel Speed Phase
            float distanceToStopStart = stopStart - position;
            float travelTime = Mathf.Min(remainingTime, distanceToStopStart / speed);
            float travelDistance = travelTime * speed;
            remainingTime -= travelTime;
            position += travelDistance;
        }
        if (remainingTime > 0f)
        {   // Stopping Phase
            float decelerationTime = Mathf.Min(remainingTime, speed / deceleration);
            float decelerationDistance = AccDist(speed, -deceleration, decelerationTime);
            position += decelerationDistance;
            speed -= deceleration * decelerationTime;
            remainingTime -= decelerationTime;
        }
        return time - remainingTime;
    }
}
