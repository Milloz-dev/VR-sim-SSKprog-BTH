using UnityEngine;

public class CharacterAudioManager : MonoBehaviour
{
    [Header("Whispers")]
    public AudioClip[] wisper1;
    public AudioClip[] wisper2;
    public AudioClip[] wisper3;

    [Header("Demanding")]
    public AudioClip[] demanding1;
    public AudioClip[] demanding2;
    public AudioClip[] demanding3;
    public AudioClip[] demanding4;
    public AudioClip[] demanding5;

    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayWisper(int index)
    {
        switch (index)
        {
            case 1: PlayRandomClip(wisper1); break;
            case 2: PlayRandomClip(wisper2); break;
            case 3: PlayRandomClip(wisper3); break;
            default: Debug.LogWarning("Invalid wisper index"); break;
        }
    }

    public void PlayDemanding(int index)
    {
        switch (index)
        {
            case 1: PlayRandomClip(demanding1); break;
            case 2: PlayRandomClip(demanding2); break;
            case 3: PlayRandomClip(demanding3); break;
            case 4: PlayRandomClip(demanding4); break;
            case 5: PlayRandomClip(demanding5); break;
            default: Debug.LogWarning("Invalid demanding index"); break;
        }
    }

    public void PlayRandomVoiceLine()
    {
        int category = Random.Range(0, 2); // 0 = whisper, 1 = demanding
        int index = 1;

        if (category == 0) index = Random.Range(1, 4); // whisper1–3
        else index = Random.Range(1, 6);               // demanding1–5

        if (category == 0)
            PlayWisper(index);
        else
            PlayDemanding(index);
    }

    private void PlayRandomClip(AudioClip[] clips)
    {
        if (clips.Length == 0) return;
        AudioClip clip = clips[Random.Range(0, clips.Length)];
        audioSource.PlayOneShot(clip);
    }
}
