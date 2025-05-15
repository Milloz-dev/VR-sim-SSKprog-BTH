using UnityEngine;
using Meta.XR.MRUtilityKit;

public class CreepyGuySpawnerMRUK : MonoBehaviour
{
    public GameObject creepyGuy;
    public Transform headset;

    public float minDistanceFromPlayer = 2.5f;
    public float maxBaseHeight = 0.2f; // max height above floor to simulate "window"
    public float offsetFromWall = 0.2f;

    private bool placed = false;

    void Start()
    {
        var room = MRUK.Instance.GetCurrentRoom();
        if (room == null)
        {
            Debug.LogWarning("❌ No room anchor found via MRUK.");
            return;
        }

        float floorY = room.FloorAnchor?.transform.position.y ?? 0f;

        // Try to place at a "floor-level wall" (simulate window)
        foreach (var wall in room.WallAnchors)
        {
            Vector3 wallPos = wall.transform.position;
            float dist = Vector3.Distance(wallPos, headset.position);
            float baseHeight = wallPos.y - floorY;

            if (baseHeight <= maxBaseHeight && dist >= minDistanceFromPlayer)
            {
                Vector3 spawnPos = wallPos - wall.transform.forward * offsetFromWall;
                creepyGuy.transform.position = spawnPos;
                placed = true;
                Debug.Log("🪟 Creepy guy placed at simulated window.");
                return;
            }
        }

        // If no floor-level wall found, fallback to any far wall
        foreach (var wall in room.WallAnchors)
        {
            Vector3 wallPos = wall.transform.position;
            float dist = Vector3.Distance(wallPos, headset.position);

            if (dist >= minDistanceFromPlayer)
            {
                creepyGuy.transform.position = wallPos;
                placed = true;
                Debug.Log("📐 Creepy guy placed at fallback wall.");
                return;
            }
        }

        Debug.LogWarning("⚠️ Could not place creepy guy.");
    }

    void Update()
    {
        if (placed && creepyGuy && headset)
        {
            Vector3 target = new Vector3(headset.position.x, creepyGuy.transform.position.y, headset.position.z);
            creepyGuy.transform.LookAt(target);
        }
    }
}
