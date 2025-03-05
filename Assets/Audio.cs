using UnityEngine;
using System.Collections;

public class AudioController : MonoBehaviour
{
    public AudioSource audiosource1;
    public AudioSource audiosource2;
    public AudioSource audiosource3;

    void Start()
    {
        StartCoroutine(PlayAudioSequence());
    }

    IEnumerator PlayAudioSequence()
    {
        yield return new WaitForSeconds(5f);
        audiosource1.Play();
        yield return new WaitForSeconds(14f); // Wait for 14 seconds before playing the next audio
        audiosource2.Play();
        yield return new WaitForSeconds(21f); // Wait for 21 seconds before doing anything else
        audiosource3.Play();
    }
}