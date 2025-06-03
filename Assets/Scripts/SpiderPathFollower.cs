using System.Collections;
using UnityEngine;

public class SpiderPathFollower : MonoBehaviour
{
    private Transform target;
    private Vector3 swayOffset;
    private Vector3 swayAxis;
    private bool isDisappearing = false;
    private bool isClone = false;

    [Header("Movement Settings")]
    public float lifetime = 30f;
    public float minSpeed = 1.5f;
    public float maxSpeed = 2.5f;

    public float minSwayAmount = 0.3f;
    public float maxSwayAmount = 0.7f;

    public float minSwaySpeed = 0.8f;
    public float maxSwaySpeed = 1.5f;

    [Header("Animation Settings")]
    public float animationSpeedMultiplier = 4f;
    public float animationSlowSpeedMultiplier = 0.6f;

    [Header("Size Settings")]
    public float minScale = 0.8f;
    public float maxScale = 1.2f;

    private float speed;
    private float swayAmount;
    private float swaySpeed;

    private SpiderSpawnScript spawner;

    void Start()
    {
        if (!isClone) return;

        // 🎲 Randomize movement speed, sway, and scale
        speed = Random.Range(minSpeed, maxSpeed);
        swayAmount = Random.Range(minSwayAmount, maxSwayAmount);
        swaySpeed = Random.Range(minSwaySpeed, maxSwaySpeed);

        swayOffset = Random.insideUnitSphere * 0.5f;

        float scale = Random.Range(minScale, maxScale);
        transform.localScale = new Vector3(scale, scale, scale);

        // Adjust animation speed based on movement speed and multiplier
        Animator anim = GetComponentInChildren<Animator>();
        if (anim != null)
        {
            anim.speed = speed * animationSpeedMultiplier;
            anim.speed *= animationSlowSpeedMultiplier;
        }

        // Auto-destroy after lifetime seconds
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        if (!isClone || target == null || isDisappearing) return;

        // 1. Flatten headset position (ignore Y)
        Vector3 headsetPos = target.position;
        Vector3 flatHeadsetPos = new Vector3(headsetPos.x, 0, headsetPos.z);

        // 2. Get a vector behind the headset (flat)
        Vector3 headsetBack = -target.forward;
        headsetBack.y = 0;
        headsetBack.Normalize();

        // 3. Create despawn target behind the player
        Vector3 despawnPoint = flatHeadsetPos + headsetBack * 0.75f;

        // 4. Move toward despawn point with sway
        Vector3 toTarget = despawnPoint - transform.position;
        Vector3 direction = toTarget.normalized;

        float sway = Mathf.Sin(Time.time * swaySpeed) * swayAmount;
        Vector3 swayedDirection = direction + swayAxis * sway;

        Vector3 move = swayedDirection.normalized * speed * Time.deltaTime;
        transform.position += move;

        // 5. Snap spider to ground using raycast
        RaycastHit hit;
        Vector3 rayOrigin = transform.position + Vector3.up * 0.2f;
        if (Physics.Raycast(rayOrigin, Vector3.down, out hit, 1f))
        {
            Vector3 pos = transform.position;
            pos.y = hit.point.y;
            transform.position = pos;
        }

        // 6. Smoothly rotate to match movement direction, then flip 180 to face opposite
        if (swayedDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(swayedDirection);
            targetRotation *= Quaternion.Euler(0, 180f, 0); // flip
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
        }

        // 7. If spider gets close enough to despawn point, start disappearing
        float distanceToDespawn = Vector3.Distance(transform.position, despawnPoint);
        if (distanceToDespawn < 0.5f)
        {
            StartCoroutine(Disappear());
        }
    }

    // Smooth disappearance, with spawner cleanup if assigned
    IEnumerator Disappear()
    {
        isDisappearing = true;

        if (spawner != null)
        {
            spawner.RemoveSpider(gameObject);
        }
        else
        {
            Debug.LogWarning("Spider's spawner reference is missing!");
        }

        Destroy(gameObject);
        yield break;
    }

    // Inject spawner reference
    public void SetSpawner(SpiderSpawnScript s)
    {
        spawner = s;
    }

    // Set the player/headset target and initialize sway direction
    public void SetTarget(Transform t)
    {
        target = t;

        // Calculate perpendicular axis to sway around (horizontal plane)
        Vector3 toTarget = (target.position - transform.position).normalized;
        swayAxis = Vector3.Cross(Vector3.up, toTarget);

        isClone = true; // mark as active clone to enable logic in Start/Update
    }
}
