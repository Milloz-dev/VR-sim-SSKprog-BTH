using UnityEngine;
using Meta.XR.MRUtilityKit;
using System.Collections;
using System.Collections.Generic;

public class CreepGuyPlacerAndLook : MonoBehaviour
{
    public GameObject creepyGuyPrefab;
    public Transform headset;
    public float maxBaseHeight = 0.2f;
    public float minDistanceFromPlayer = 2.5f;
    public float offsetFromWall = 0.2f;
    public float rotationSpeed = 1.5f; // how fast he turns to face you

    private GameObject spawnedGuy;
    private Transform creepyVisual; // will point to visual root inside prefab
    private MRUKRoom room;

    void Start()
    {
        StartCoroutine(WaitForRoomAndSpawn());
    }

    IEnumerator WaitForRoomAndSpawn()
    {
        Debug.Log("🕒 Waiting for MRUK room...");
        while (MRUK.Instance.GetCurrentRoom() == null)
            yield return null;

        Debug.Log("✅ MRUK room found.");
        room = MRUK.Instance.GetCurrentRoom();
        StartCoroutine(TryPlaceGuyWithTimeout());
    }

    IEnumerator TryPlaceGuyWithTimeout()
    {
        yield return new WaitForSeconds(1f); // shorter wait since no scanning needed

        Vector3 spawnPos = headset.position;
        spawnPos.y = 0f; // snap to world floor

        Debug.Log("🧍 Spawning on player position.");
        SpawnGuy(spawnPos, "📍 On Player");
    }







    void SpawnGuy(Vector3 position, string label)
    {
        spawnedGuy = Instantiate(creepyGuyPrefab, position, Quaternion.identity);
        creepyVisual = spawnedGuy.transform; // update this if your model is nested!

        Debug.Log($"{label} ✅ Spawned creepy guy at: {position}");
    }

    void Update()
    {
        if (creepyVisual == null || headset == null)
            return;

        Vector3 direction = headset.position - creepyVisual.position;
        direction.y = 0f;
        if (direction.sqrMagnitude < 0.001f) return;

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        creepyVisual.rotation = Quaternion.Slerp(creepyVisual.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
}
