using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RatMovement : MonoBehaviour
{
    public List<Transform> waypoints;
    public float moveSpeed = 0.5f;
    public float waitTime = 2f;
    public float idleChance = 0.2f; // 20% chance to idle instead of moving
    public float idleDuration = 1.5f;

    private Transform targetWaypoint;
    private Animator animator;
    private bool isMoving = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        StartCoroutine(MovementLoop());
    }

    IEnumerator MovementLoop()
    {
        while (true)
        {
            // Maybe idle instead of moving
            if (Random.value < idleChance)
            {
                animator.SetTrigger("Idle"); // requires an "Idle" trigger
                yield return new WaitForSeconds(idleDuration);
            }
            else
            {
                // Pick a random waypoint
                targetWaypoint = waypoints[Random.Range(0, waypoints.Count)];
                animator.SetBool("Moving", true);

                // Move toward it slowly
                while (Vector3.Distance(transform.position, targetWaypoint.position) > 0.05f)
                {
                    Vector3 direction = (targetWaypoint.position - transform.position).normalized;
                    transform.position += direction * moveSpeed * Time.deltaTime;
                    transform.rotation = Quaternion.LookRotation(direction);
                    yield return null;
                }

                animator.SetBool("Moving", false);
                yield return new WaitForSeconds(waitTime);
            }
        }
    }
}
