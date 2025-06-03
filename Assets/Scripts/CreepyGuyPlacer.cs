using UnityEngine;
using Meta.XR.MRUtilityKit;
using System.Collections.Generic;

public class CreepyGuySpawnerMRUK : MonoBehaviour
{
    public GameObject creepyGuyPrefab;
    public Transform headset;
    public float minDistanceFromPlayer = 2.5f;
    public float maxBaseHeight = 0.2f;
    public float offsetFromWall = 0.2f;
    public float minTime = 3f;
    public float maxTime = 8f;
    public LayerMask obstructionLayers;

    private MRUKRoom currentRoom;
    private GameObject spawnedGuy;
    private float nextMoveTime = 0f;
    private Vector3 lastPosition;
    private bool isActive = false;

    void Update()
    {
        currentRoom = MRUK.Instance.GetCurrentRoom();
        if (currentRoom == null) return;
        if (Time.time < nextMoveTime) return;

        List<Vector3> validSpots = GetHiddenSpots();

        if (validSpots.Count > 0)
        {
            bool shouldDespawn = Random.value < 0.3f; // ~30% chance to despawn instead of moving

            if (isActive && shouldDespawn)
            {
                // Despawn by choice
                Destroy(spawnedGuy);
                spawnedGuy = null;
                isActive = false;
                Debug.Log("👻 Creepy guy chose to despawn.");
            }
            else
            {
                Vector3 chosenPos = PickDifferentSpot(validSpots, lastPosition);

                if (!isActive)
                {
                    spawnedGuy = Instantiate(creepyGuyPrefab, chosenPos, Quaternion.identity);
                    isActive = true;
                    Debug.Log("🧟 Creepy guy spawned.");
                }
                else if (Vector3.Distance(chosenPos, lastPosition) > 0.05f)
                {
                    spawnedGuy.transform.position = chosenPos;
                    Debug.Log("🔁 Creepy guy moved.");
                }

                spawnedGuy.transform.LookAt(GetFlatLookTarget(chosenPos));
                lastPosition = chosenPos;
            }
        }
        else
        {
            // No valid spots — only despawn if he's NOT visible
            if (isActive && !IsInLineOfSight(lastPosition))
            {
                Destroy(spawnedGuy);
                spawnedGuy = null;
                isActive = false;
                Debug.Log("💨 Creepy guy despawned — hidden and no valid spots left.");
            }
            else
            {
                Debug.Log("👁 Still visible — staying put.");
            }
        }

        nextMoveTime = Time.time + Random.Range(minTime, maxTime);
    }

    List<Vector3> GetHiddenSpots()
    {
        List<Vector3> spots = new List<Vector3>();
        float floorY = currentRoom.FloorAnchor?.transform.position.y ?? 0f;

        foreach (var wall in currentRoom.WallAnchors)
        {
            Vector3 wallPos = wall.transform.position;
            float baseHeight = wallPos.y - floorY;
            float dist = Vector3.Distance(wallPos, headset.position);

            if (baseHeight <= maxBaseHeight && dist >= minDistanceFromPlayer)
            {
                Vector3 spawnPos = wallPos - wall.transform.forward * offsetFromWall;
                if (!IsInLineOfSight(spawnPos))
                    spots.Add(spawnPos);
            }
        }

        if (currentRoom.FloorAnchor != null)
        {
            Vector3 centerPos = currentRoom.FloorAnchor.transform.position;
            if (!IsInLineOfSight(centerPos))
                spots.Add(centerPos);
        }

        return spots;
    }

    bool IsInLineOfSight(Vector3 targetPos)
    {
        Vector3 eyeLevel = headset.position;
        Vector3 direction = targetPos - eyeLevel;
        float distance = direction.magnitude;

        if (Physics.Raycast(eyeLevel, direction.normalized, out RaycastHit hit, distance, obstructionLayers))
        {
            Debug.DrawLine(eyeLevel, hit.point, Color.green, 1f);
            return false;
        }

        Debug.DrawLine(eyeLevel, targetPos, Color.red, 1f);
        return true;
    }

    Vector3 PickDifferentSpot(List<Vector3> candidates, Vector3 current)
    {
        candidates.RemoveAll(pos => Vector3.Distance(pos, current) < 0.05f);
        if (candidates.Count == 0)
            return current;

        return candidates[Random.Range(0, candidates.Count)];
    }

    Vector3 GetFlatLookTarget(Vector3 from)
    {
        return new Vector3(headset.position.x, from.y, headset.position.z);
    }
}
