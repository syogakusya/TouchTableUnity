using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.UIElements;
using Unity.Burst.Intrinsics;

[RequireComponent(typeof(Rigidbody))]//RigidBody�K�{

public class Boid : MonoBehaviour
{
    public Simulation simulation { get; set; }
    public Transform simulationTransform { get; set; }
    public Param param { get; set; }
    public WallParam wallParam { get; set; }

    public Vector3 pos { get; private set; }
    public Vector3 velocity { get; private set; }
    private protected Vector3 accel = Vector3.zero;
    private Vector3 simPos;
    List<Boid> neighbors = new List<Boid>();

    private void Start()
    {
        pos = transform.position;
        simPos = simulationTransform.position;
        velocity = transform.forward * param.initSpeed;
    }

    private void Update()
    {
        UpdateNeighbors();

        UpdateWalls();
        UpdateAlignment();
        UpdateSeparation();
        UpdateCohesion();

        UpdateMove();//UpdateMove�͍Ō�
    }

    private protected void UpdateMove()
    {
        float dt = Time.deltaTime;
        velocity += accel * dt;
        Vector3 dir = velocity.normalized;
        float speed = velocity.magnitude;
        velocity = Mathf.Clamp(speed, param.minSpeed, param.maxSpeed) * dir;
        pos += velocity * dt;

        var rot = Quaternion.LookRotation(velocity);
        transform.SetPositionAndRotation(pos, rot);

        accel = Vector3.zero;
    }

    private protected void UpdateWalls()
    {
        if (!simulation) return;

        //float scale = wallParam.wallDistance * 0.5f;
        float scaleX = wallParam.wallX * 0.5f;
        float scaleY = wallParam.wallY * 0.5f;
        float scaleZ = wallParam.wallZ * 0.5f;

        accel +=
            CalcAccelAgainstWall(-scaleX + simPos.x - pos.x, Vector3.right) +
            CalcAccelAgainstWall(-scaleY + simPos.y - pos.y, Vector3.up) +
            CalcAccelAgainstWall(-scaleZ + simPos.z - pos.z, Vector3.forward) +
            CalcAccelAgainstWall(+scaleX + simPos.x - pos.x, Vector3.left) +
            CalcAccelAgainstWall(+scaleY + simPos.y - pos.y, Vector3.down) +
            CalcAccelAgainstWall(+scaleZ + simPos.z - pos.z, Vector3.back);

    }

    Vector3 CalcAccelAgainstWall(float distance, Vector3 dir)
    {
        if (distance < wallParam.wallDistance)
        {
            return dir * (wallParam.wallPower / Mathf.Abs(distance / wallParam.wallDistance));
        }
        return Vector3.zero;
    }

    private protected void UpdateNeighbors()
    {
        neighbors.Clear();

        if (!simulation) return;

        float prodThresh = Mathf.Cos(param.boidsFov * Mathf.Deg2Rad);
        float distThresh = param.boidsDistance;

        foreach (Boid other in simulation.Boids)
        {
            if (other == this) continue;

            Vector3 to = other.pos - pos;
            float dist = to.magnitude;

            if (dist < distThresh)
            {
                Vector3 dir = to.normalized;
                Vector3 fwd = velocity.normalized;
                var prod = Vector3.Dot(fwd, dir);
                if (prod > prodThresh)
                {
                    neighbors.Add(other);
                }
            }
        }
    }
    private protected void UpdateSeparation()
    {
        if (neighbors.Count == 0) return;

        Vector3 force = Vector3.zero;
        foreach (Boid neighbor in neighbors)
        {
            force += (pos - neighbor.pos).normalized;
        }
        force /= neighbors.Count;

        accel += force * param.separationPower;
    }

    private protected void UpdateAlignment()
    {
        if (neighbors.Count == 0) return;

        Vector3 averageVelocity = Vector3.zero;
        foreach (Boid neighbor in neighbors)
        {
            averageVelocity += neighbor.velocity;
        }
        averageVelocity /= neighbors.Count;

        accel += (averageVelocity - velocity) * param.alignmentPower;
    }

    private protected void UpdateCohesion()
    {
        if (neighbors.Count == 0) return;

        Vector3 averagePos = Vector3.zero;
        foreach (Boid neighbor in neighbors)
        {
            averagePos += neighbor.pos;
        }
        averagePos /= neighbors.Count;

        accel += (averagePos - pos) * param.cohesionPower;
    }

}