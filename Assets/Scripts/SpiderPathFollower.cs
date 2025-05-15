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

    void Start()
    {
        if (!isClone) return;

        // 🎲 Randomize values based on Inspector-configured ranges
        speed = Random.Range(minSpeed, maxSpeed);
        swayAmount = Random.Range(minSwayAmount, maxSwayAmount);
        swaySpeed = Random.Range(minSwaySpeed, maxSwaySpeed);

        swayOffset = Random.insideUnitSphere * 0.5f;

        float scale = Random.Range(minScale, maxScale);
        transform.localScale = new Vector3(scale, scale, scale);

    Animator anim = GetComponentInChildren<Animator>();
    if (anim != null)
    {
        anim.speed = speed * animationSpeedMultiplier;
        //Customizable multiplier for exaggeration
        anim.speed = anim.speed * animationSlowSpeedMultiplier; // e.g. multiplier = 0.6f
    }

        Destroy(gameObject, lifetime);
    }

void Update()
{
    if (!isClone || target == null || isDisappearing) return;

    // 1. Get headset position & orientation (flattened)
    Vector3 headsetPos = target.position;
    Vector3 flatHeadsetPos = new Vector3(headsetPos.x, 0, headsetPos.z);

    Vector3 headsetBack = -target.forward;
    headsetBack.y = 0;
    headsetBack.Normalize();

    // 2. Despawn point is always behind the headset (live updated)
    Vector3 despawnPoint = flatHeadsetPos + headsetBack * 0.75f;

    // 3. Move toward the despawn point with sway
    Vector3 toTarget = despawnPoint - transform.position;
    Vector3 direction = toTarget.normalized;

    float sway = Mathf.Sin(Time.time * swaySpeed) * swayAmount;
    Vector3 swayedDirection = direction + swayAxis * sway;

    Vector3 move = swayedDirection.normalized * speed * Time.deltaTime;
    transform.position += move;

    // 4. Snap to ground
    RaycastHit hit;
    Vector3 rayOrigin = transform.position + Vector3.up * 0.2f;
    if (Physics.Raycast(rayOrigin, Vector3.down, out hit, 1f))
    {
        Vector3 pos = transform.position;
        pos.y = hit.point.y;
        transform.position = pos;
    }

    // 5. Rotate spider to face movement direction
    if (swayedDirection != Vector3.zero)
    {
        Quaternion targetRotation = Quaternion.LookRotation(swayedDirection);
        targetRotation *= Quaternion.Euler(0, 180f, 0);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
    }

    // 6. Despawn when spider reaches the despawn point
    float distanceToDespawn = Vector3.Distance(transform.position, despawnPoint);
    if (distanceToDespawn < 0.5f)
    {
        StartCoroutine(Disappear());
    }
}

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

    private SpiderSpawnScript spawner;

    public void SetSpawner(SpiderSpawnScript s)
    {
        spawner = s;
    }

    public void SetTarget(Transform t)
    {
        target = t;
        Vector3 toTarget = (target.position - transform.position).normalized;
        swayAxis = Vector3.Cross(Vector3.up, toTarget);
        isClone = true;
    }
}
