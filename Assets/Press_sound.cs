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

    // Enums for left and right controller nodes
    public XRNode leftControllerNode = XRNode.LeftHand; // Left Hand Controller
    public XRNode rightControllerNode = XRNode.RightHand; // Right Hand Controller
    private InputDevice leftDevice;
    private InputDevice rightDevice;

    void Start()
    {
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
                PlaySound(rightGripButtonAudioSource);
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
}