using UnityEngine;
using Meta.XR.MRUtilityKit;
using System.Collections;
using System.Collections.Generic;

public class CreepGuyPlacer3 : MonoBehaviour
{
    public GameObject creepyGuyPrefab;
    public Transform headset;
    public float maxBaseHeight = 0.2f;
    public float minDistanceFromPlayer = 2.5f;
    public float offsetFromWall = 0.2f;

    private MRUKRoom room;
    private GameObject spawnedGuy;

    void Start()
    {
        StartCoroutine(WaitForRoomAndTryPlace());
    }

    IEnumerator WaitForRoomAndTryPlace()
    {
        Debug.Log("🕒 Waiting for MRUK room...");

        while (MRUK.Instance.GetCurrentRoom() == null)
        {
            yield return null;
        }

        Debug.Log("✅ MRUK room found — proceeding to placement.");
        TryPlaceGuy();
    }

    void TryPlaceGuy()
    {
        room = MRUK.Instance.GetCurrentRoom();
        if (room == null)
        {
            Debug.LogWarning("❌ No MRUK room found.");
            return;
        }

        Vector3 floorCenter = room.FloorAnchor?.transform.position ?? Vector3.zero;
        float floorY = floorCenter.y;

        // Try placing at windows first
        foreach (var wall in room.WallAnchors)
        {
            Vector3 wallPos = wall.transform.position;
            float baseHeight = wallPos.y - floorY;
            float distToPlayer = Vector3.Distance(wallPos, headset.position);
            Vector3 spawnPos = wallPos - wall.transform.forward * offsetFromWall;

            Debug.Log($"🔍 Checking wall at {wallPos} | baseHeight: {baseHeight} | dist: {distToPlayer}");

            if (baseHeight <= maxBaseHeight && distToPlayer >= minDistanceFromPlayer)
            {
                SpawnGuy(spawnPos, "🪟 Window");
                return;
            }
        }

        // Try placing at room center
        SpawnGuy(floorCenter + Vector3.up * 0.05f, "🏠 Center");
    }

    void SpawnGuy(Vector3 position, string label)
    {
        spawnedGuy = Instantiate(creepyGuyPrefab, position, Quaternion.identity);
        spawnedGuy.transform.LookAt(new Vector3(headset.position.x, position.y, headset.position.z));
        Debug.Log($"{label} ✅ Spawned creepy guy at: {position}");
    }
} 
