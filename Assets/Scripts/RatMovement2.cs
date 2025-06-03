using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RatMovement2 : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 1.5f;
    public float minWaitTime = 1f;
    public float maxWaitTime = 3f;

    [Header("Disappearance Settings")]
    public float escapeSpeed = 4f;
    public float disappearAfterSeconds = 20f;

    [Header("Audio")]
    public AudioSource audioSource;
    public List<AudioClip> ratSounds;

    [Header("Run Sound")]
    public AudioClip ratRunSound;
    public float minSoundInterval = 3f;
    public float maxSoundInterval = 8f;

    private Animator animator;
    private bool escaping = false;
    private AudioSource runAudioSource;
    private bool runSoundCooldown = false;
    private float floorY;
    private RatSpawner spawner;

    void Start()
    {
        animator = GetComponent<Animator>();

        // Get the Y-position of the floor from MRUK room (default to 0 if not available)
        floorY = Meta.XR.MRUtilityKit.MRUK.Instance.GetCurrentRoom()?.FloorAnchor?.transform.position.y ?? 0f;

        // Setup separate audio source for running sound
        runAudioSource = gameObject.AddComponent<AudioSource>();
        runAudioSource.loop = false;
        runAudioSource.playOnAwake = false;

        // Start wandering and sound coroutines
        StartCoroutine(WanderRoutine());
        StartCoroutine(PlayRandomSounds());

        // Schedule escape sequence
        Invoke(nameof(BeginEscape), disappearAfterSeconds);
    }

    // Reference setter for the spawner (optional tracking)
    public void SetSpawner(RatSpawner spawnerRef)
    {
        spawner = spawnerRef;
    }

    // Random roaming routine (with obstacle avoidance and idle pauses)
    IEnumerator WanderRoutine()
    {
        while (!escaping)
        {
            Vector3 randomDir = GetRandomDirection();
            float moveTime = Random.Range(2f, 4f);
            float elapsed = 0f;

            animator.SetBool("isMoving", true);
            StartRunSound();

            int retryCount = 0;

            while (elapsed < moveTime && !escaping)
            {
                Vector3 nextPos = transform.position + randomDir * moveSpeed * Time.deltaTime;
                nextPos.y = floorY;

                // Retry movement if the rat would leave the room bounds
                if (!IsInsideRoom(nextPos))
                {
                    retryCount++;
                    if (retryCount > 10)
                    {
                        Debug.LogWarning("🐀 Rat stuck near wall — breaking early");
                        break;
                    }

                    randomDir = GetRandomDirection();
                    continue;
                }

                retryCount = 0;

                // Turn smoothly toward direction of movement
                Quaternion targetRotation = Quaternion.LookRotation(randomDir);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 2f);
                transform.position = nextPos;

                elapsed += Time.deltaTime;
                yield return null;
            }

            animator.SetBool("isMoving", false);

            // Idle for a random time if in an open space (not near other objects)
            if (IsInOpenSpace())
            {
                float idleTime = Random.Range(minWaitTime, maxWaitTime);
                yield return new WaitForSeconds(idleTime);
            }
        }
    }

    // Check if rat is not surrounded by colliders (open space)
    bool IsInOpenSpace()
    {
        float checkRadius = 0.5f;
        Collider[] hits = Physics.OverlapSphere(transform.position, checkRadius);
        int obstacleCount = 0;

        foreach (var hit in hits)
        {
            if (hit.gameObject != this.gameObject && !hit.isTrigger)
                obstacleCount++;
        }

        return obstacleCount == 0;
    }

    // Plays random squeaky rat sounds at intervals
    IEnumerator PlayRandomSounds()
    {
        while (!escaping)
        {
            float wait = Random.Range(minSoundInterval, maxSoundInterval);
            yield return new WaitForSeconds(wait);

            if (ratSounds.Count > 0)
            {
                AudioClip clip = ratSounds[Random.Range(0, ratSounds.Count)];
                audioSource.PlayOneShot(clip);
            }
        }
    }

    // Generate a random 2D direction on the XZ plane
    Vector3 GetRandomDirection()
    {
        return new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f)).normalized;
    }

    // Trigger escape behavior (stop current routines and run away)
    void BeginEscape()
    {
        escaping = true;
        StopAllCoroutines();
        StartCoroutine(EscapeThroughWall());
    }

    // Play a run sound (with cooldown to prevent overlap)
    void StartRunSound()
    {
        if (!runSoundCooldown && ratRunSound != null)
        {
            runAudioSource.pitch = Random.Range(0.95f, 1.05f);
            runAudioSource.PlayOneShot(ratRunSound);
            StartCoroutine(RunSoundCooldown(ratRunSound.length));
        }
    }

    // Escape in a straight line and destroy self after a few seconds
    IEnumerator EscapeThroughWall()
    {
        animator.SetBool("isMoving", true);
        Vector3 escapeDirection = GetRandomDirection();

        float escapeTime = 3f;
        float elapsed = 0f;

        while (elapsed < escapeTime)
        {
            Vector3 next = transform.position + escapeDirection * escapeSpeed * Time.deltaTime;
            next.y = floorY;

            transform.position = next;
            transform.forward = escapeDirection;

            elapsed += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject); // Remove rat after escaping
    }

    // Check if a given position is still inside the scanned MRUK room
    bool IsInsideRoom(Vector3 position)
    {
        var room = Meta.XR.MRUtilityKit.MRUK.Instance.GetCurrentRoom();
        return room != null && room.IsPositionInRoom(position, true);
    }

    // Prevents rapid re-triggering of the run sound
    IEnumerator RunSoundCooldown(float duration)
    {
        runSoundCooldown = true;
        yield return new WaitForSeconds(duration);
        runSoundCooldown = false;
    }
}
