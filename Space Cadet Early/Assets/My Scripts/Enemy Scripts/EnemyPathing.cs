using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPathing : MonoBehaviour
{
    [SerializeField] private List<Transform> waypoints;
    public float moveSpeed = 2f;
    int waypointIndex = 0;
    private bool finalWaypoint = false;

    void Start()
    {
        transform.position = waypoints[waypointIndex].transform.position;
    }

    void Update()
    {
        if (!finalWaypoint)
        {
            Move();
        }
        else
        {
            MoveBack();
        }
    }

    private void Move()
    {
        finalWaypoint = false;
        if (waypointIndex < waypoints.Count - 1)
        {
            var targetPosition = waypoints[waypointIndex].transform.position;
            var movementThisFrame = moveSpeed * Time.deltaTime;
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, movementThisFrame);


            if (transform.position == targetPosition)
            {
                waypointIndex++;
            }
        }       
        if (waypointIndex == waypoints.Count - 1)
            finalWaypoint = true;
    }

    private void MoveBack()
    {
        if (finalWaypoint)
        {
            var targetPosition = waypoints[waypointIndex].transform.position;
            var movementThisFrame = moveSpeed * Time.deltaTime;
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, movementThisFrame);

            if (transform.position == targetPosition)
            {
                waypointIndex--;
            }
        }
        if(waypointIndex == 0)
        {
            finalWaypoint = false;
        }
    }
}
