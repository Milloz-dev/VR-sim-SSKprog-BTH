using UnityEngine;
using System.Collections;

public class PathFollower : MonoBehaviour
{
    public Transform[] waypoints;
    public float speed = 2f;
    private int currentWaypointIndex = 0;

    public System.Action onReachedEnd;

    [Header("Fade Out Settings")]
    public Renderer characterRenderer; // Assign this in Inspector (SkinnedMeshRenderer or MeshRenderer)
    public float fadeDuration = 2f;

    void Update()
    {
        if (currentWaypointIndex < waypoints.Length)
        {
            Transform target = waypoints[currentWaypointIndex];
            Vector3 direction = (target.position - transform.position).normalized;
            transform.position += direction * speed * Time.deltaTime;

            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
            }

            if (Vector3.Distance(transform.position, target.position) < 0.2f)
            {
                currentWaypointIndex++;

                if (currentWaypointIndex >= waypoints.Length)
                {
                    onReachedEnd?.Invoke();       // Notify others
                    enabled = false;              // Stop moving
                    StartCoroutine(FadeAndDestroy()); // Start fade out
                }
            }
        }
    }

    IEnumerator FadeAndDestroy()
    {
        if (characterRenderer == null)
        {
            Debug.LogWarning("No renderer assigned for fading.");
            Destroy(gameObject);
            yield break;
        }

        Material mat = characterRenderer.material;
        Color startColor = mat.color;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            mat.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
            elapsed += Time.deltaTime;
            yield return null;
        }

        mat.color = new Color(startColor.r, startColor.g, startColor.b, 0f);
        Destroy(gameObject);
    }
}
