using UnityEngine;

public class BallCannon : MonoBehaviour
{
    public GameObject ballPrefab;
    public float shootForce = 10f;
    public AudioSource fireAudioSource;
    public AudioClip fireSound;
    public Transform shootOrigin;

    void Start()
    {
        if (shootOrigin == null)
        {
            Debug.LogError("❌ shootOrigin not assigned in inspector. Please assign a Transform.");
        }
    }

    public void ShootBall()
    {
        Debug.Log("\uD83D\uDD2B ShootBall called");

        if (fireAudioSource && fireSound)
            fireAudioSource.PlayOneShot(fireSound);

        if (ballPrefab == null || shootOrigin == null)
        {
            Debug.LogWarning("\uD83D\uDEAB Missing ballPrefab or shootOrigin");
            return;
        }

        Vector3 spawnPos = shootOrigin.position + shootOrigin.forward * 0.1f; // small offset
        GameObject ball = Instantiate(ballPrefab, spawnPos, shootOrigin.rotation);
        Rigidbody rb = ball.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = shootOrigin.forward * shootForce;
        }
    }
}
