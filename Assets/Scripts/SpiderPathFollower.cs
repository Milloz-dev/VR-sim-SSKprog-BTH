using System.Collections;
using UnityEngine;

public class SpiderPathFollower : MonoBehaviour
{
    private Transform target;
    public float speed = 2f;
    public float swayAmount = 0.5f;         // How far it sways side to side
    public float swaySpeed = 1f;            // How fast the sway moves

    private Vector3 swayOffset;
    private Vector3 swayAxis;
    private float lifetime = 10f;           // Optional: safety destroy after X seconds
    private bool isDisappearing = false;

    void Start()
    {
        // Pick a random sideways direction to sway in
        swayAxis = Vector3.Cross(Vector3.up, (target.position - transform.position).normalized);
        swayOffset = Random.insideUnitSphere * 0.5f;
        Destroy(gameObject, lifetime); // Failsafe auto-destroy
    }

    void Update()
    {
        if (target == null || isDisappearing) return;

        Vector3 toTarget = target.position - transform.position;
        Vector3 direction = toTarget.normalized;

        // Add sway
        float sway = Mathf.Sin(Time.time * swaySpeed) * swayAmount;
        Vector3 swayedDirection = direction + swayAxis * sway;

        transform.position += swayedDirection.normalized * speed * Time.deltaTime;

        // Face the direction it's moving
        if (swayedDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(swayedDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
        }

        // Check if it's behind the player to disappear
        float dot = Vector3.Dot(target.forward, (transform.position - target.position).normalized);
        if (dot < -0.8f && Vector3.Distance(transform.position, target.position) < 1f)
        {
            StartCoroutine(Disappear());
        }
    }

    IEnumerator Disappear()
    {
        isDisappearing = true;
        // Add fade or squish animation here if you want
        yield return new WaitForSeconds(0.2f);
        Destroy(gameObject);
    }

    public void SetTarget(Transform t)
    {
        target = t;
    }
}
