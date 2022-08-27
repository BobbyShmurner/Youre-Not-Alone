using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Pathfind : MazeObject
{
    [field: SerializeField] public bool DisablePathfinding { get; set; }
    [field: SerializeField] public MazeObject Target { get; private set; }
    [field: SerializeField] public LayerMask RaycastIgnore { get; private set; } = 768;
    [field: SerializeField] public float CheckForTargetInterval { get; private set; }
    [field: SerializeField] public float InsightTime { get; private set; }
    [field: SerializeField] public float InsightInterval { get; private set; }
    [field: SerializeField] public float NextWaypointDistance { get; private set; }
    [field: SerializeField] public float ReachTargetDistance { get; private set; }
    [field: SerializeField] public float SlowDownTargetDistance { get; private set; }
    [field: SerializeField] public float MoveSpeed { get; private set; }
    [field: SerializeField] public float Acceleration { get; private set; }
    [field: SerializeField] public float RotateSpeed { get; private set; }

    public Rigidbody Rigidbody { get; private set; }
    public List<Vector2> Waypoints { get; private set; } =  new List<Vector2>();

    public float CurrentMoveSpeed { get; private set; }
    public float RemainingInsight { get; private set; }
    public float TimeSinceLastWaypointReached { get; private set; }
    public bool LineOfSight { get; private set; }

    void Awake() {
        Rigidbody = GetComponent<Rigidbody>();
    }

    protected override void Start() {
        base.Start();
        InvokeRepeating("CheckForTarget", 0, CheckForTargetInterval);
        InvokeRepeating("GatherInsight", 0, InsightInterval);
    }

    void GatherInsight() {
        if (LineOfSight) return;
        if (RemainingInsight <= 0) return;

        Waypoints.Add(Target.WorldPosition);
    }

    public void CheckForTarget() {
        LineOfSight = false;
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Target.transform.position - transform.position, out hit, Mathf.Infinity, ~RaycastIgnore, QueryTriggerInteraction.Ignore)) {
            MazeObject hitObject = hit.transform.GetComponent<MazeObject>();
            if (!hitObject || hitObject != Target) return;

            Waypoints.Clear();
            Waypoints.Add(Target.WorldPosition);
            LineOfSight = true;
        }
    }

    void Update() {
        if (DisablePathfinding) {
            CurrentMoveSpeed = 0;
            return;
        }

        if (LineOfSight) {
            RemainingInsight = InsightTime;
        } else {
            RemainingInsight -= Time.deltaTime;
        }

        // If stuck and out of insight, tp to the next waypoint
        if (RemainingInsight <= 0 && Waypoints.Count > 0 && Time.realtimeSinceStartup > TimeSinceLastWaypointReached + 1) {
            SetWorldPosition(Waypoints[0]);
        }

        if (LineOfSight && Vector3.Distance(WorldPosition, Target.WorldPosition) <= ReachTargetDistance) {
            CurrentMoveSpeed = 0;
        } else if (Waypoints.Count == 0 || (LineOfSight && Vector3.Distance(WorldPosition, Target.WorldPosition) <= SlowDownTargetDistance)) {
            CurrentMoveSpeed -= Acceleration * Time.deltaTime;
        } else {
            CurrentMoveSpeed += Acceleration * Time.deltaTime;
        }

        CurrentMoveSpeed = Mathf.Clamp(CurrentMoveSpeed, 0, MoveSpeed);
    }

    protected override void LateUpdate() {
        base.LateUpdate();

        if (Rigidbody.velocity.sqrMagnitude > 0.0001f) transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(Rigidbody.velocity), RotateSpeed);
        else transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(Target.transform.position - transform.position), RotateSpeed);

        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);

        if (Waypoints.Count > 0 && Vector3.Distance(Waypoints[0], WorldPosition) <= NextWaypointDistance) {
            TimeSinceLastWaypointReached = Time.realtimeSinceStartup;
            Waypoints.RemoveAt(0);
        }
    }

    void FixedUpdate() {
        Rigidbody.velocity = Vector3.zero;
        if (Waypoints.Count == 0) return;

        Vector2 newVel = (Waypoints[0] - WorldPosition).normalized * CurrentMoveSpeed;
        Rigidbody.velocity = new Vector3(newVel.x, 0, newVel.y);
    }

    void OnDrawGizmos() {
        Gizmos.color = LineOfSight ? Color.green : Color.red;

        for (int i = 0; i < Waypoints.Count; i++) {
            Vector3 localPos = new Vector3(
                transform.position.x + Waypoints[i].x - WorldPosition.x,
                0,
                transform.position.z + Waypoints[i].y - WorldPosition.y
            );
                
            Gizmos.DrawSphere(localPos, 0.1f);
            Vector3 lastPos;

            if (i != 0) {
                lastPos = new Vector3(
                    transform.position.x + Waypoints[i - 1].x - WorldPosition.x,
                    0,
                    transform.position.z + Waypoints[i - 1].y - WorldPosition.y
                );
            } else {
                lastPos = new Vector3(
                    transform.position.x,
                    0,
                    transform.position.z
                );

                Gizmos.DrawSphere(lastPos, 0.1f);
            }

            Gizmos.DrawLine(localPos, lastPos);
        }
    }
}
