using UnityEngine;
using UnityEngine.XR;

public class ControllerButtonSound : MonoBehaviour
{
    public AudioSource leftPrimaryButtonAudioSource;
    public AudioSource leftSecondaryButtonAudioSource;
    public AudioSource leftTriggerButtonAudioSource;
    public AudioSource leftGripButtonAudioSource;

    public AudioSource rightPrimaryButtonAudioSource;
    public AudioSource rightSecondaryButtonAudioSource;
    public AudioSource rightTriggerButtonAudioSource;

    public XRNode leftControllerNode = XRNode.LeftHand;
    public XRNode rightControllerNode = XRNode.RightHand;

    public BallCannon ballCannon;

    public CharacterSpawner characterSpawner;
    public RatSpawner ratSpawner;
    public SpiderSpawnScript spiderSpawner;

    private InputDevice leftDevice;
    private InputDevice rightDevice;

    // Per-button state tracking
    private bool leftPrimaryWasPressed;
    private bool leftSecondaryWasPressed;
    private bool leftTriggerWasPressed;
    private bool leftGripWasPressed;

    private bool rightPrimaryWasPressed;
    private bool rightSecondaryWasPressed;
    private bool rightTriggerWasPressed;
    private bool rightGripWasPressed;
    private bool spawnerEnabled = true;

    void Start()
    {
        leftDevice = InputDevices.GetDeviceAtXRNode(leftControllerNode);
        rightDevice = InputDevices.GetDeviceAtXRNode(rightControllerNode);

        if (ballCannon == null)
            ballCannon = GetComponent<BallCannon>();
    }

    void Update()
    {
        if (!leftDevice.isValid)
            leftDevice = InputDevices.GetDeviceAtXRNode(leftControllerNode);
        if (!rightDevice.isValid)
            rightDevice = InputDevices.GetDeviceAtXRNode(rightControllerNode);

        CheckButtonPress(leftDevice, true);
        CheckButtonPress(rightDevice, false);
    }

    private void CheckButtonPress(InputDevice device, bool isLeft)
    {
        bool primaryPressed, secondaryPressed, triggerPressed, gripPressed;

        device.TryGetFeatureValue(CommonUsages.primaryButton, out primaryPressed);
        device.TryGetFeatureValue(CommonUsages.secondaryButton, out secondaryPressed);
        device.TryGetFeatureValue(CommonUsages.triggerButton, out triggerPressed);
        device.TryGetFeatureValue(CommonUsages.gripButton, out gripPressed);

        // === Primary Button ===
        if (primaryPressed && !(isLeft ? leftPrimaryWasPressed : rightPrimaryWasPressed))
        {
            if (!isLeft)
            {
                // Check if grip is also held for cannon combo
                if (rightGripWasPressed)
                {
                        //Spawning
                        spiderSpawner.SpawnSpiders();
                        Debug.Log("Spawn spiders");
   
                }
                else
                {
                    // Only play sound if NOT in combo
                    PlaySound(rightPrimaryButtonAudioSource);
                }
            }
            else
            {
                // Left controller primary (if needed)
                PlaySound(leftPrimaryButtonAudioSource);
            }
        }

        // === Secondary Button ===
        if (secondaryPressed && !(isLeft ? leftSecondaryWasPressed : rightSecondaryWasPressed))
        {
            if (isLeft || !rightGripWasPressed)
                PlaySound(isLeft ? leftSecondaryButtonAudioSource : rightSecondaryButtonAudioSource);

            // Right combo: Grip + Secondary = spawn rat
            if (!isLeft && rightGripWasPressed)
            {
                Debug.Log("🐀 Right Grip + Secondary → RatSpawner");
                if (ratSpawner != null)
                    ratSpawner.RatTriggerAction();
                else
                    Debug.LogWarning("ratSpawner reference is NULL!");
            }
        }

        // === Trigger Button ===
        if (triggerPressed && !(isLeft ? leftTriggerWasPressed : rightTriggerWasPressed))
        {
            if (isLeft || !rightGripWasPressed)
                PlaySound(isLeft ? leftTriggerButtonAudioSource : rightTriggerButtonAudioSource);

            // Right combo: Grip + Trigger = spawn character
            if (!isLeft && rightGripWasPressed)
            {
                Debug.Log("👤 Right Grip + Trigger → CharacterSpawner");
                if (characterSpawner != null)
                    characterSpawner.CharacterTriggerAction();
                else
                    Debug.LogWarning("characterSpawner reference is NULL!");
            }
        }

        // === Grip Button ===
        if (gripPressed && !(isLeft ? leftGripWasPressed : rightGripWasPressed))
        {
            if (isLeft)
                PlaySound(leftGripButtonAudioSource); // Only left grip plays sound
        }

        // === Update State Tracking ===
        if (isLeft)
        {
            leftPrimaryWasPressed = primaryPressed;
            leftSecondaryWasPressed = secondaryPressed;
            leftTriggerWasPressed = triggerPressed;
            leftGripWasPressed = gripPressed;
        }
        else
        {
            rightPrimaryWasPressed = primaryPressed;
            rightSecondaryWasPressed = secondaryPressed;
            rightTriggerWasPressed = triggerPressed;
            rightGripWasPressed = gripPressed;
        }
    }

    private void PlaySound(AudioSource audioSource)
    {
        if (audioSource != null && audioSource.clip != null)
            audioSource.PlayOneShot(audioSource.clip);
    }
}
