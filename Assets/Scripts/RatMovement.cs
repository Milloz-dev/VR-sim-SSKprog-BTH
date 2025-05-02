using System.Collections;
using UnityEngine;

public class RatMovement : MonoBehaviour
{
    public Transform[] waypoints;
    public float walkSpeed = 1.0f;
    public float runSpeed = 2.0f;
    public float idleTime = 2.0f;

    public Animator animator;

    private int currentWaypoint = 0;
    private bool isMoving = false;
    private bool isRunning = false;
    private Coroutine movementCoroutine;

    void Start()
    {
        if (waypoints.Length > 0)
        {
            // ✅ Delay just one frame before triggering movement (after Entry loads)
            StartCoroutine(StartupDelay());
        }
    }

    IEnumerator StartupDelay()
    {
        yield return null; // wait 1 frame
        StartCoroutine(MovementLoop());
    }

    IEnumerator MovementLoop()
    {
        while (true)
        {
            // Pick walk or run
            isRunning = Random.value > 0.5f;

            // 🔄 Reset all movement triggers before setting new one
            animator.ResetTrigger("StartWalk");
            animator.ResetTrigger("StartRun");
            animator.ResetTrigger("EndWalk");
            animator.ResetTrigger("EndRun");

            // Trigger animation start (this fires Entry transition based on trigger)
            animator.SetTrigger(isRunning ? "StartRun" : "StartWalk");

            float moveSpeed = isRunning ? runSpeed : walkSpeed;

            Vector3 target = waypoints[currentWaypoint].position;
            isMoving = true;

            while (Vector3.Distance(transform.position, target) > 0.1f)
            {
                Vector3 direction = (target - transform.position).normalized;
                transform.position += direction * moveSpeed * Time.deltaTime;

                Quaternion lookRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);

                yield return null;
            }

            isMoving = false;

        if (isRunning)
        {
            // From Run, go to Walk first
            animator.ResetTrigger("EndWalk");
            animator.SetTrigger("EndRun");

            // Wait for run to end before starting EndWalk
            yield return new WaitForSeconds(0.4f); // match your EndRun clip duration

            animator.ResetTrigger("EndRun");
            animator.SetTrigger("EndWalk");

            yield return new WaitForSeconds(0.4f); // match your EndWalk duration
        }
        else
        {
            animator.SetTrigger("EndWalk");
            yield return new WaitForSeconds(0.4f);
        }

        // Now in idle — chill
        yield return new WaitForSeconds(idleTime);


            currentWaypoint = (currentWaypoint + 1) % waypoints.Length;
        }
    }
}
