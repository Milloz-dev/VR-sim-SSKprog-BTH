using UnityEngine;
using System.Collections.Generic;

public class SpiderSpawnScript : MonoBehaviour
{
    public GameObject spiderPrefab;
    public Transform headset;
    public int maxSpidersInScene = 10;
    public int spiderCount = 5;
    public float spawnRadius = 5f;

    private List<GameObject> activeSpiders = new List<GameObject>();
    public void SpawnSpiders()
    {
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

       int spidersToSpawn = Mathf.Min(spiderCount, maxSpidersInScene - activeSpiders.Count);
        for (int i = 0; i < spidersToSpawn; i++)
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
            activeSpiders.Add(spawnedSpider);

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
        public void RemoveSpider(GameObject spider)
    {
        if (activeSpiders.Contains(spider))
            activeSpiders.Remove(spider);
    }
}
