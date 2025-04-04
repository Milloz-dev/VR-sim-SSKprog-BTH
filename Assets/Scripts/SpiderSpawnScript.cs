using UnityEngine;

public class SpiderSpawnScript : MonoBehaviour
{
    public GameObject spiderPrefab;
    public Transform headset;

    public int spiderCount = 5;
    public float spawnRadius = 5f;
    public float disappearOffset = 1f;
    public float spawnInterval = 5f;

    void Start()
    {
        if (headset == null)
        {
            Debug.LogError("Headset reference not set!");
            return;
        }

        InvokeRepeating(nameof(SpawnSpiders), 1f, spawnInterval);
    }

    void SpawnSpiders()
    {
        for (int i = 0; i < spiderCount; i++)
        {
            float angle = Random.Range(-90f, 90f); // half-circle
            float distance = Random.Range(spawnRadius * 0.8f, spawnRadius * 1.2f);
            Quaternion rot = Quaternion.AngleAxis(angle, Vector3.up);
            Vector3 dir = rot * headset.forward;
            Vector3 spawnPos = headset.position + dir.normalized * distance;
            spawnPos.y = 0.1f;

            GameObject spider = Instantiate(spiderPrefab, spawnPos, Quaternion.identity);
            spider.GetComponent<SpiderPathFollower>().SetTarget(headset);
        }
    }
}
