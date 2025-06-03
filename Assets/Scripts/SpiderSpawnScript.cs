using UnityEngine;
using System.Collections.Generic;

public class SpiderSpawnScript : MonoBehaviour
{
    public GameObject spiderPrefab;           // Prefab to spawn
    public Transform headset;                // Player's headset (used for direction and position)
    public int maxSpidersInScene = 10;       // Max number of spiders allowed at once
    public int spiderCount = 5;              // How many to try to spawn on call
    public float spawnRadius = 5f;           // Radius from headset to spawn spiders

    private List<GameObject> activeSpiders = new List<GameObject>();

    // Call this to spawn spiders
    public void SpawnSpiders()
    {
        // ✅ Safety checks
        if (headset == null)
        {
            Debug.LogError("Headset reference not set!");
            return;
        }

        if (spiderPrefab == null)
        {
            Debug.LogError("Spider prefab is missing!");
            return;
        }

        // 🔢 Determine how many spiders we can still spawn
        int spidersToSpawn = Mathf.Min(spiderCount, maxSpidersInScene - activeSpiders.Count);

        for (int i = 0; i < spidersToSpawn; i++)
        {
            // 🎲 Choose a random angle in front of the player (half-circle) and distance
            float angle = Random.Range(-90f, 90f);
            float distance = Random.Range(spawnRadius * 0.8f, spawnRadius * 1.2f);
            Quaternion rot = Quaternion.AngleAxis(angle, Vector3.up);
            Vector3 dir = rot * headset.forward;

            // 💠 Calculate position relative to headset
            Vector3 spawnPos = headset.position + dir.normalized * distance;

            // 📡 Try to snap spider to floor using raycast
            Vector3 rayStart = spawnPos + Vector3.up * 2f;
            RaycastHit hit;
            if (Physics.Raycast(rayStart, Vector3.down, out hit, 5f))
            {
                spawnPos.y = hit.point.y;
            }
            else
            {
                spawnPos.y = 0f; // Fallback to world ground
                Debug.LogWarning("No ground detected for spider spawn! Using default Y = 0");
            }

            // 👁 Rotate spider to face toward center (or headset)
            Quaternion lookRotation = Quaternion.LookRotation(-dir);

            // 🕷 Instantiate spider
            GameObject spawnedSpider = Instantiate(spiderPrefab, spawnPos, lookRotation);
            activeSpiders.Add(spawnedSpider);

            // 🔗 Link the path follower to target and spawner
            SpiderPathFollower pathFollower = spawnedSpider.GetComponent<SpiderPathFollower>();
            if (pathFollower != null)
            {
                pathFollower.SetTarget(headset);
                pathFollower.SetSpawner(this);
                Debug.Log("Spider spawned and linked.");
            }
            else
            {
                Debug.LogWarning("Spawned spider missing SpiderPathFollower component!");
            }
        }
    }

    // Called by spider to deregister itself on destruction
    public void RemoveSpider(GameObject spider)
    {
        if (activeSpiders.Contains(spider))
            activeSpiders.Remove(spider);
    }
}
