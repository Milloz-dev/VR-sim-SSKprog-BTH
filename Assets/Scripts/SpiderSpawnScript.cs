using UnityEngine;

public class SpiderSpawnScript : MonoBehaviour
{
    public GameObject spiderPrefab; // ← renamed this!
    public Transform headset;

    public int spiderCount = 5;
    public float spawnRadius = 5f;
    public float spawnInterval = 5f;

    void Start()
    {
        if (headset == null)
        {
            Debug.LogError("Headset reference not set!");
            return;
        }

        InvokeRepeating(nameof(SpawnSpiders), 0f, spawnInterval);
    }

    void SpawnSpiders()
    {
        if (spiderPrefab == null)
        {
            Debug.LogError("Spider prefab is missing!");
            return;
        }

        for (int i = 0; i < spiderCount; i++)
        {
            float angle = Random.Range(-90f, 90f); // half-circle
            float distance = Random.Range(spawnRadius * 0.8f, spawnRadius * 1.2f);
            Quaternion rot = Quaternion.AngleAxis(angle, Vector3.up);
            Vector3 dir = rot * headset.forward;

            Vector3 spawnPos = headset.position + dir.normalized * distance;

            // Raycast downward from above to find the floor
            Vector3 rayStart = spawnPos + Vector3.up * 2f;
            RaycastHit hit;
            if (Physics.Raycast(rayStart, Vector3.down, out hit, 5f))
            {
                spawnPos.y = hit.point.y;
            }
            else
            {
                spawnPos.y = 0f;
                Debug.LogWarning("No ground detected for spider spawn! Using default Y = 0");
            }

            Quaternion lookRotation = Quaternion.LookRotation(-dir);

            GameObject spawnedSpider = Instantiate(spiderPrefab, spawnPos, lookRotation);


            SpiderPathFollower pathFollower = spawnedSpider.GetComponent<SpiderPathFollower>();
            if (pathFollower != null)
            {
                pathFollower.SetTarget(headset);
                Debug.Log("SetTarget called on: " + spawnedSpider.name);

            }
            else
            {
                Debug.LogWarning("Spawned spider missing SpiderPathFollower component!");
            }
        }
    }
}
