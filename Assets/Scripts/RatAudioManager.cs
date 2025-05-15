using UnityEngine;

public class RatAudioManager : MonoBehaviour
{
    [Header("Rat Squeak Clips")]
    public AudioClip[] squeakClips; // assign 2 short squeaks in inspector

    [Header("Rat Run Loop")]
    public AudioClip runningSqueakLoop; // assign long running+sqeak clip

    private AudioSource oneShotSource;
    private AudioSource runningSource;

    void Awake()
    {
        // Setup two audio sources: one for one-shots, one for looping run
        var sources = GetComponents<AudioSource>();
        if (sources.Length < 2)
        {
            oneShotSource = gameObject.AddComponent<AudioSource>();
            runningSource = gameObject.AddComponent<AudioSource>();
        }
        else
        {
            oneShotSource = sources[0];
            runningSource = sources[1];
        }

        runningSource.loop = true;
        runningSource.playOnAwake = false;
    }

    public void PlayRandomSqueak()
    {
        if (squeakClips.Length == 0) return;
        var clip = squeakClips[Random.Range(0, squeakClips.Length)];
        oneShotSource.PlayOneShot(clip);
    }

    public void StartRunAudio()
    {
        if (runningSqueakLoop == null) return;
        if (!runningSource.isPlaying)
        {
            runningSource.clip = runningSqueakLoop;
            runningSource.time = 1f; // Skip the first 1 second
            runningSource.Play();
        }
    }

    public void StopRunAudio()
    {
        if (runningSource.isPlaying)
            runningSource.Stop();
    }
}
