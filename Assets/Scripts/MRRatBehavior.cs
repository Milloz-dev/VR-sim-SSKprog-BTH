/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Meta.XR.MRUtilityKit; // ✅ This is correct for SDK 60+

public class MRRoomAwareRatSpawner : MonoBehaviour
{
    public GameObject ratPrefab;
    public float spawnYOffset = -0.1f; // Spawn slightly under couch
    public int roamPointCount = 3;

    private void Start()
    {
        StartCoroutine(WaitForMRUKReady());
    }

    IEnumerator WaitForMRUKReady()
    {
        while (MRUK.Instance == null || !MRUK.Instance.Initialized)
        {
            yield return null;
        }

        OnMRUKReady();
    }

    private void OnMRUKReady()
    {
        Debug.Log("✅ MR Utility Kit ready. Searching for couches...");

        var allObjects = MRUK.Instance.SceneObjects;
        var couches = new List<MRUKAnchor>();

        foreach (var obj in allObjects)
        {
            if (obj.HasSemanticLabel("Couch"))
            {
                couches.Add(obj);
            }
        }

        if (couches.Count < 2)
        {
            Debug.LogWarning("⚠️ Need at least two couches to spawn and hide the rat.");
            return;
        }

        // Choose start and end couches
        var spawnCouch = couches[Random.Range(0, couches.Count)];
        MRUKAnchor hideCouch;
        do
        {
            hideCouch = couches[Random.Range(0, couches.Count)];
        } while (hideCouch == spawnCouch);

        Vector3 spawnPos = spawnCouch.transform.position + Vector3.up * spawnYOffset;
        MRUKRoom room = GetRoomContainingPoint(spawnPos);
        if (room == null)
        {
            Debug.LogWarning("❌ Spawn point is not inside a recognized room.");
            return;
        }

        GameObject rat = Instantiate(ratPrefab, spawnPos, Quaternion.identity);
        var mover = rat.GetComponent<MRRatMovementWithAnim>();
        if (mover == null)
        {
            Debug.LogError("🐀 Rat prefab missing MRRatMovementWithAnim component!");
            return;
        }

        List<Transform> roamPoints = new List<Transform>();

        for (int i = 0; i < roamPointCount; i++)
        {
            Vector3 randomInRoom = GetRandomPointInsideBounds(room);
            GameObject tempPoint = new GameObject($"RoamPoint_{i}");
            tempPoint.transform.position = randomInRoom;
            roamPoints.Add(tempPoint.transform);
        }

        // Add final hide point under couch
        Vector3 hidePos = hideCouch.transform.position + Vector3.up * spawnYOffset;
        GameObject hidePoint = new GameObject("HidePoint");
        hidePoint.transform.position = hidePos;
        roamPoints.Add(hidePoint.transform);

        mover.anchorPoints = roamPoints.ToArray();
    }

    MRUKRoom GetRoomContainingPoint(Vector3 position)
    {
        foreach (var room in MRUK.Instance.Rooms)
        {
            Bounds bounds = new Bounds(room.transform.position, room.RoomExtents);
            if (bounds.Contains(position))
                return room;
        }
        return null;
    }

    Vector3 GetRandomPointInsideBounds(MRUKRoom room)
    {
        Bounds bounds = new Bounds(room.transform.position, room.RoomExtents);
        return new Vector3(
            Random.Range(bounds.min.x, bounds.max.x),
            bounds.center.y,
            Random.Range(bounds.min.z, bounds.max.z)
        );
    }
}
*/