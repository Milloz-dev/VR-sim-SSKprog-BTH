using UnityEngine;

public class RatSimpleMovement : MonoBehaviour
{
    public Transform[] waypoints;
    public float speed = 2f;

    public AudioSource[] waypointAudioSources; // Assign 3 AudioSources in Inspector

    private int currentIndex = 0;

    void Update()
    {
        if (waypoints.Length == 0) return;

        Vector3 targetPos = waypoints[currentIndex].position;
        Vector3 direction = (targetPos - transform.position).normalized;

        transform.position += direction * speed * Time.deltaTime;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * 5f);

        if (Vector3.Distance(transform.position, targetPos) < 0.1f)
        {
            PlayRandomAudioSource();
            currentIndex = (currentIndex + 1) % waypoints.Length;
        }
    }

    void PlayRandomAudioSource()
    {
        if (waypointAudioSources.Length == 0) return;

        int randomIndex = Random.Range(0, waypointAudioSources.Length);
        AudioSource chosenSource = waypointAudioSources[randomIndex];

        if (!chosenSource.isPlaying)
        {
            chosenSource.Play();
        }
    }
}