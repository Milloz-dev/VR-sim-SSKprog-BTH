using UnityEngine;
using UnityEngine.XR;

public class ControllerButtonSound : MonoBehaviour
{
    // Audio sources for different buttons on both left and right controllers
    public AudioSource leftPrimaryButtonAudioSource;
    public AudioSource leftSecondaryButtonAudioSource;
    public AudioSource leftTriggerButtonAudioSource;
    public AudioSource leftGripButtonAudioSource;

    public AudioSource rightPrimaryButtonAudioSource;
    public AudioSource rightSecondaryButtonAudioSource;
    public AudioSource rightTriggerButtonAudioSource;

    public AudioSource rightGripButtonAudioSource;
    
    //public AudioSource rightGripButtonAudioSource;

    // Enums for left and right controller nodes
    public XRNode leftControllerNode = XRNode.LeftHand; // Left Hand Controller
    public XRNode rightControllerNode = XRNode.RightHand; // Right Hand Controller
    public CharacterSpawner characterSpawner; // Drag this in via Inspector

    private InputDevice leftDevice;
    private InputDevice rightDevice;

    void Start()
    {
        Debug.Log("ControllerButtonSound is running!");
        // Get the input devices for both the left and right controllers
        leftDevice = InputDevices.GetDeviceAtXRNode(leftControllerNode);
        rightDevice = InputDevices.GetDeviceAtXRNode(rightControllerNode);
    }

    void Update()
    {
        // Check if the devices are valid, otherwise update them
        if (!leftDevice.isValid)
        {
            leftDevice = InputDevices.GetDeviceAtXRNode(leftControllerNode);
        }

        if (!rightDevice.isValid)
        {
            rightDevice = InputDevices.GetDeviceAtXRNode(rightControllerNode);
        }


        if (Input.GetKeyDown(KeyCode.L))
        {
            LogDeviceInputs(rightDevice);
        }

        // Check button presses on left and right controllers
        CheckButtonPress(leftDevice, "Left Controller");
        CheckButtonPress(rightDevice, "Right Controller");
    }

    // Method to check which button is pressed on a given controller
    private void CheckButtonPress(InputDevice device, string controllerName)
    {
        // Primary button press (e.g., "A" on the right, "X" on the left)
        bool primaryButtonPressed;
        if (device.TryGetFeatureValue(CommonUsages.primaryButton, out primaryButtonPressed) && primaryButtonPressed)
        {
            Debug.Log($"{controllerName}: Primary Button Pressed");
            if (controllerName == "Left Controller")
            {
                PlaySound(leftPrimaryButtonAudioSource);
            }
            else
            {
                PlaySound(rightPrimaryButtonAudioSource);
            }
        }

        // Secondary button press (e.g., "B" on the right, "Y" on the left)
        bool secondaryButtonPressed;
        if (device.TryGetFeatureValue(CommonUsages.secondaryButton, out secondaryButtonPressed) && secondaryButtonPressed)
        {
            Debug.Log($"{controllerName}: Secondary Button Pressed");
            if (controllerName == "Left Controller")
            {
                PlaySound(leftSecondaryButtonAudioSource);
            }
            else
            {
                PlaySound(rightSecondaryButtonAudioSource);
            }
        }

        // Trigger press
        bool triggerPressed;
        if (device.TryGetFeatureValue(CommonUsages.triggerButton, out triggerPressed) && triggerPressed)
        {
            Debug.Log($"{controllerName}: Trigger Button Pressed");
            if (controllerName == "Left Controller")
            {
                PlaySound(leftTriggerButtonAudioSource);
            }
            else
            {
                PlaySound(rightTriggerButtonAudioSource);
            }
        }

        // Grip press
        bool gripPressed;
        if (device.TryGetFeatureValue(CommonUsages.gripButton, out gripPressed) && gripPressed)
        {
            Debug.Log($"{controllerName}: Grip Button Pressed");
            if (controllerName == "Left Controller")
            {
                PlaySound(leftGripButtonAudioSource);
            }
            else
            {
                //PlaySound(rightGripButtonAudioSource);
                    // 🧠 Check if characterSpawner is assigned
                if (characterSpawner == null)
                {
                    Debug.LogWarning("⚠️ characterSpawner is NOT assigned!");
                    PlaySound(rightGripButtonAudioSource);
                }
                else
                {
                    Debug.Log("✅ characterSpawner is assigned!");
                    characterSpawner.TriggerAction();
                    PlaySound(rightGripButtonAudioSource);
                }
            }
        }
    }

    // Play sound on the given AudioSource
    private void PlaySound(AudioSource audioSource)
    {
        if (audioSource != null)
        {
            if (!audioSource.isPlaying)
            {
                audioSource.Play(); // Play the sound
                Debug.Log("Playing sound: " + audioSource.name); // Log sound being played
            }
            else
            {
                Debug.Log("AudioSource is already playing: " + audioSource.name);
            }
        }
        else
        {
            Debug.LogWarning("AudioSource is not assigned!");
        }
    }
    void LogDeviceInputs(InputDevice device)
    {
        Debug.Log($"🧤 Logging inputs for: {device.name}");

        bool boolVal;
        float floatVal;

        if (device.TryGetFeatureValue(CommonUsages.primaryButton, out boolVal))
            Debug.Log("→ primaryButton: " + boolVal);

        if (device.TryGetFeatureValue(CommonUsages.secondaryButton, out boolVal))
            Debug.Log("→ secondaryButton: " + boolVal);

        if (device.TryGetFeatureValue(CommonUsages.triggerButton, out boolVal))
            Debug.Log("→ triggerButton: " + boolVal);

        if (device.TryGetFeatureValue(CommonUsages.gripButton, out boolVal))
            Debug.Log("→ gripButton (bool): " + boolVal);

        if (device.TryGetFeatureValue(CommonUsages.grip, out floatVal))
            Debug.Log("→ grip (float): " + floatVal);

        if (device.TryGetFeatureValue(CommonUsages.trigger, out floatVal))
            Debug.Log("→ trigger (float): " + floatVal);
    }

}
