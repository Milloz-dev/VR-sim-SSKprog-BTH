using UnityEngine;
using Meta.XR.MRUtilityKit;
using static OVRSemanticLabels;
using System.Collections.Generic;

public class RatSpawner : MonoBehaviour
{
    public GameObject ratPrefab;
    private GameObject spawnedRat;
    private bool ratReachedEnd = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            RatTriggerAction();
        }
    }

    void SpawnRat()
    {
        var room = MRUK.Instance.GetCurrentRoom();
        if (room == null)
        {
            Debug.LogWarning("⚠️ No current MRUK room found.");
            SpawnRatOutsideFallback(Vector3.zero); // Use world center if no room
            return;
        }

        List<MRUKAnchor> anchors = room.Anchors;

        MRUKAnchor.SceneLabels furnitureLabels = 
        MRUKAnchor.SceneLabels.COUCH | 
        MRUKAnchor.SceneLabels.TABLE | 
        MRUKAnchor.SceneLabels.STORAGE;

        var furnitureAnchors = anchors.FindAll(anchor =>
            anchor.HasAnyLabel(furnitureLabels)
        );

        if (furnitureAnchors.Count == 0)
        {
            Debug.LogWarning("⚠️ No valid furniture anchors found — spawning outside room.");
            MRUKAnchor floor = room.FloorAnchor;
            Vector3 roomCenter = floor.transform.position; // ✅ This is the floor position
            SpawnRatOutsideFallback(roomCenter);
            return;
        }

        // ✅ Spawn near furniture anchor
        MRUKAnchor chosen = furnitureAnchors[Random.Range(0, furnitureAnchors.Count)];
        Vector3 forwardOffset = -chosen.transform.forward * 0.3f;
        Vector3 spawnPos = chosen.GetAnchorCenter() + forwardOffset;
        spawnPos.y = 0f;

        spawnedRat = Instantiate(ratPrefab, spawnPos, Quaternion.LookRotation(chosen.transform.forward));
        ratReachedEnd = false;
        Debug.Log($"✅ Spawned rat behind anchor at {spawnPos}");

        var runner = spawnedRat.GetComponent<RatRunMovement>();
        if (runner != null)
            runner.SetSpawner(this);
    }
    

    void PlayRatSound()
    {
        if (spawnedRat == null) return;

        RatAudioManager audioManager = spawnedRat.GetComponent<RatAudioManager>();
        if (audioManager != null)
        {
            audioManager.PlayRandomSqueak();
        }
        else
        {
            Debug.LogWarning("⚠️ RatAudioManager component NOT found on spawned rat");
        }
    }

    public void OnRatReachedEnd()
    {
        ratReachedEnd = true;

        if (spawnedRat != null)
        {
            Debug.Log("💀 Rat reached end — destroying rat.");
            Destroy(spawnedRat);
            spawnedRat = null;
        }
    }

    public void RatTriggerAction()
    {
        Debug.Log("🎯 TriggerAction called on RatSpawner!");

        if (spawnedRat == null || !spawnedRat.activeInHierarchy || ratReachedEnd)
        {
            Debug.Log("🆕 Spawning rat...");
            SpawnRat();
        }
        else
        {
            Debug.Log("🗣️ Rat already active — playing squeak");
            PlayRatSound();
        }
    }

    void SpawnRatOutsideFallback(Vector3 roomCenter)
    {
        if (roomCenter == Vector3.zero)
            roomCenter = new Vector3(0f, 0f, 0f); // fallback world center

        Vector2 randomCircle = Random.insideUnitCircle.normalized * 1.0f;
        Vector3 offset = new Vector3(randomCircle.x, 0f, randomCircle.y);
        Vector3 spawnPos = roomCenter + offset;
        Vector3 directionToRoom = (roomCenter - spawnPos).normalized;

        spawnedRat = Instantiate(ratPrefab, spawnPos, Quaternion.LookRotation(directionToRoom));
        ratReachedEnd = false;

        Debug.Log($"✅ Rat fallback-spawned outside room at {spawnPos}");

        var runner = spawnedRat.GetComponent<RatRunMovement>();
        if (runner != null)
            runner.SetSpawner(this);
    }
}
