using UnityEngine;

public class RatAnimatorController : MonoBehaviour
{
    private Animator anim;

    [Range(0f, 1f)]
    public float idleChance = 0.2f; // 20% chance to idle

    void Start()
    {
        anim = GetComponent<Animator>();

        // Randomly pick one of the two walk animations
        int walkChoice = Random.Range(0, 2); // 0 or 1
        anim.SetInteger("RandomWalk", walkChoice);

        // Optionally idle after a delay
        InvokeRepeating("TryIdle", 2f, 5f); // Try idling every 5s
    }

    public void PlayEndWalk()
    {
        anim.SetTrigger("EndWalk");
    }

    void TryIdle()
    {
        if (Random.value < idleChance)
        {
            anim.SetTrigger("Idle");
        }
    }
}
