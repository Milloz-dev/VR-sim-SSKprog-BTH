using UnityEngine;

public class CharacterSpawner : MonoBehaviour
{
    public GameObject characterPrefab;
    public AudioSource CharacterSpawnSound;
    public Transform spawnPoint;
    public Transform[] waypoints;

    private GameObject spawnedCharacter;
    private bool characterReachedEnd = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (spawnedCharacter == null)
            {
                SpawnCharacter();
                
            }
            else if (!characterReachedEnd)
            {
                PlayVoiceLine();
            }
        }
    }

    void SpawnCharacter()
    {
        Debug.Log("🚀 Spawning character at: " + spawnPoint.position);
        
        // ✅ Play spawn sound if available
        if (CharacterSpawnSound != null && CharacterSpawnSound.clip != null)
        {
            CharacterSpawnSound.PlayOneShot(CharacterSpawnSound.clip);
        }
        else
        {
            Debug.LogWarning("⚠️ CharacterSpawnSound is not assigned or has no clip");
        }


        spawnedCharacter = Instantiate(characterPrefab, spawnPoint.position, spawnPoint.rotation);
        characterReachedEnd = false;

        PathFollower pathFollower = spawnedCharacter.GetComponent<PathFollower>();
        if (pathFollower != null)
        {
            Debug.Log("✅ PathFollower found — assigning waypoints");
            pathFollower.waypoints = waypoints;
            pathFollower.onReachedEnd += OnCharacterReachedEnd;
        }
        else
        {
            Debug.LogWarning("⚠️ PathFollower component NOT found on spawned character");
        }
    }

    void PlayVoiceLine()
    {
        if (spawnedCharacter == null) return;

        CharacterAudioManager audioManager = spawnedCharacter.GetComponent<CharacterAudioManager>();
        if (audioManager != null)
        {
            audioManager.PlayRandomVoiceLine();
        }
    }

    void OnCharacterReachedEnd()
    {
        characterReachedEnd = true;
        // The character will destroy itself via FadeAndDestroy
        // We reset spawnedCharacter in the next frame when it's null
    }

    public void CharacterTriggerAction()
    {
        Debug.Log("🎯 TriggerAction was called!");

        if (spawnedCharacter == null)
        {
            Debug.Log("🆕 No character exists — spawning new one...");
            SpawnCharacter();
        }
        else if (!spawnedCharacter.activeInHierarchy)
        {
            Debug.Log("♻️ Character exists but is inactive — respawning...");
            spawnedCharacter = null;
            SpawnCharacter();
        }
        else
        {
            Debug.Log("🗣️ Character is alive — playing voice line");
            PlayVoiceLine();
        }
    }
}
