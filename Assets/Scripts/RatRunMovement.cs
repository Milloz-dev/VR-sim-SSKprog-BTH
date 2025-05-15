using UnityEngine;

public class RatRunMovement : MonoBehaviour
{
    public Transform[] waypoints;
    public float speed = 2f;
    public AudioSource[] waypointAudioSources;

    private int currentIndex = 0;
    private bool hasReachedEnd = false;
    private RatSpawner spawner;

    public void SetSpawner(RatSpawner spawnerRef)
    {
        spawner = spawnerRef;
    }

    void Update()
    {
        if (waypoints.Length == 0 || hasReachedEnd) return;

        Vector3 targetPos = waypoints[currentIndex].position;
        Vector3 direction = (targetPos - transform.position).normalized;

        transform.position += direction * speed * Time.deltaTime;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * 5f);

        if (Vector3.Distance(transform.position, targetPos) < 0.1f)
        {
            PlayRandomAudioSource();

            currentIndex++;

            if (currentIndex >= waypoints.Length)
            {
                hasReachedEnd = true;

                if (spawner != null)
                {
                    spawner.SendMessage("OnRatReachedEnd");
                }
                else
                {
                    Debug.LogWarning("RatSpawner reference not set on RatRunMovement!");
                }
            }
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
