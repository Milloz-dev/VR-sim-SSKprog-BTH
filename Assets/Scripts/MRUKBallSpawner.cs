using UnityEngine;
using Meta.XR.MRUtilityKit;
using static OVRSemanticLabels; // for easy access to SceneLabels enums

public class MRUKBallSpawner : MonoBehaviour
{
    public GameObject redBallPrefab;

    void Start()
    {
        Invoke(nameof(SpawnBallsOnAnchors), 2f); // Delay for MRUK initialization
    }

    void SpawnBallsOnAnchors()
    {
        if (redBallPrefab == null)
        {
            Debug.LogError("Red ball prefab is not assigned.");
            return;
        }

        var room = MRUK.Instance.GetCurrentRoom();
        int count = 0;

        foreach (var anchor in room.Anchors)
        {
        
            Vector3 pos = anchor.GetAnchorCenter();
            GameObject ball = Instantiate(redBallPrefab, pos + Vector3.up * 0.05f, Quaternion.identity);
            ball.transform.SetParent(anchor.transform); // Optional

            count++;
            
        }
    }    
}
