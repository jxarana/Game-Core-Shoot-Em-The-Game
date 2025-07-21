using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class MovingPlatform : MonoBehaviour
{
    [SerializeField] float movementSpeed = 5f;
    [SerializeField] float waitTime = 2f;
    [SerializeField] bool reverseDirection;
    [SerializeField] List<Transform> waypoints = new List<Transform>();
    [SerializeField] List<Rigidbody> rbInArea = new List<Rigidbody>();

    bool isWaiting;
    int currentWaypointIndex;
    Transform currentWaypoint;
    WaitForFixedUpdate wait = new WaitForFixedUpdate();


    Rigidbody rb;
    Collider col;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();

        rb.freezeRotation = true;
        rb.useGravity = false;
        rb.isKinematic = true;

        if (waypoints.Count <= 0)
            Debug.LogWarning($"No waypoints have been assigned to {name}!");
        else
        {
            currentWaypoint = waypoints[currentWaypointIndex];
        }

        StartCoroutine(WaitRoutine());
        StartCoroutine(LateFixedUpdate());
    }

    IEnumerator WaitRoutine()
    {
        WaitForSeconds duration = new WaitForSeconds(waitTime);
        while (true)
        {
            if (isWaiting)
            {
                yield return duration;
                isWaiting = false;
            }

            yield return null;
        }

    }

    IEnumerator LateFixedUpdate()
    {
        while (true)
        {
            yield return wait;
            MovePlatform();
        }
    }

    void UpdateWaypoint()
    {
        currentWaypointIndex += reverseDirection ? -1 : 1;

        currentWaypointIndex = (currentWaypointIndex + waypoints.Count) % waypoints.Count;

        currentWaypoint = waypoints[currentWaypointIndex];
        isWaiting = true;
    }

    void MovePlatform()
    {
        if (waypoints.Count <= 0 || isWaiting) return;

        Vector3 toNextWaypoint = currentWaypoint.position - transform.position;
        Vector3 movement = toNextWaypoint.normalized * (movementSpeed * Time.deltaTime);

        if (movement.magnitude >= toNextWaypoint.magnitude || movement.magnitude == 0f)
        {
            rb.transform.position = currentWaypoint.position;
            UpdateWaypoint();
        }
        else
        {
            rb.transform.position += movement;

        }

        for (int i = 0; i < rbInArea.Count; i++)
        {
            Rigidbody rigidBody = rbInArea[i];
            rigidBody.MovePosition(rigidBody.position + movement);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Rigidbody hasRB = other.GetComponent<Rigidbody>();
        if (hasRB != null) { rbInArea.Add(hasRB); }
    }

    private void OnTriggerExit(Collider other)
    {
        Rigidbody hasRB = other.GetComponent<Rigidbody>(); ;
        if (hasRB != null) { rbInArea.Remove(hasRB); }
    }
}
