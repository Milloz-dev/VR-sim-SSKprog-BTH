using UnityEngine;
using UnityEngine.XR;

public class ControllerButtonSound : MonoBehaviour
{
    // 🎧 AudioSources for left hand controller buttons
    public AudioSource leftPrimaryButtonAudioSource;
    public AudioSource leftSecondaryButtonAudioSource;
    public AudioSource leftTriggerButtonAudioSource;
    public AudioSource leftGripButtonAudioSource;
    public AudioSource leftJoystickClickAudioSource;

    // 🎧 AudioSources for right hand trigger and grip
    public AudioSource rightTriggerButtonAudioSource;
    public AudioSource rightGripButtonAudioSource;

    // 🔧 References to gameplay systems triggered from right controller
    public CharacterSpawner characterSpawner;
    public RatSpawner ratSpawner;
    public SpiderSpawnScript spiderSpawner;

    // Cached references to left/right XR controllers
    private InputDevice leftDevice;
    private InputDevice rightDevice;

    // Track which buttons were pressed last frame (to detect new presses)
    private bool[] leftPressed = new bool[5];  // 0: Primary, 1: Secondary, 2: Trigger, 3: Grip, 4: Stick
    private bool[] rightPressed = new bool[5];

    void Start()
    {
        // Get references to XR devices at startup
        leftDevice = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
        rightDevice = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
    }

    void Update()
    {
        // Ensure devices are valid (re-acquire if disconnected)
        if (!leftDevice.isValid) leftDevice = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
        if (!rightDevice.isValid) rightDevice = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);

        // Check button input for both hands
        CheckButton(leftDevice, true);
        CheckButton(rightDevice, false);
    }

    // Detect button state changes and trigger actions or sounds
    private void CheckButton(InputDevice device, bool isLeft)
    {
        // Read current button states from the controller
        device.TryGetFeatureValue(CommonUsages.primaryButton, out bool primary);
        device.TryGetFeatureValue(CommonUsages.secondaryButton, out bool secondary);
        device.TryGetFeatureValue(CommonUsages.triggerButton, out bool trigger);
        device.TryGetFeatureValue(CommonUsages.gripButton, out bool grip);
        device.TryGetFeatureValue(CommonUsages.primary2DAxisClick, out bool stick);

        // Get previous state array
        bool[] previous = isLeft ? leftPressed : rightPressed;

        // If button is newly pressed this frame, call handler
        if (primary && !previous[0]) OnButtonPressed(isLeft, 0);
        if (secondary && !previous[1]) OnButtonPressed(isLeft, 1);
        if (trigger && !previous[2]) OnButtonPressed(isLeft, 2);
        if (grip && !previous[3]) OnButtonPressed(isLeft, 3);
        if (stick && !previous[4]) OnButtonPressed(isLeft, 4);

        // Update previous state tracking
        if (isLeft)
        {
            leftPressed[0] = primary;
            leftPressed[1] = secondary;
            leftPressed[2] = trigger;
            leftPressed[3] = grip;
            leftPressed[4] = stick;
        }
        else
        {
            rightPressed[0] = primary;
            rightPressed[1] = secondary;
            rightPressed[2] = trigger;
            rightPressed[3] = grip;
            rightPressed[4] = stick;
        }
    }

    // Handles action or audio per button press
    private void OnButtonPressed(bool isLeft, int buttonIndex)
    {
        if (isLeft)
        {
            // Left-hand button sounds
            switch (buttonIndex)
            {
                case 0: PlaySound(leftPrimaryButtonAudioSource); break;
                case 1: PlaySound(leftSecondaryButtonAudioSource); break;
                case 2: PlaySound(leftTriggerButtonAudioSource); break;
                case 3: PlaySound(leftGripButtonAudioSource); break;
                case 4: PlaySound(leftJoystickClickAudioSource); break;
            }
        }
        else
        {
            // Right-hand button actions and sounds
            switch (buttonIndex)
            {
                case 0: if (characterSpawner != null) characterSpawner.CharacterTriggerAction(); break;
                case 1: if (ratSpawner != null) ratSpawner.RatTriggerAction(); break;
                case 2: PlaySound(rightTriggerButtonAudioSource); break;
                case 3: PlaySound(rightGripButtonAudioSource); break;
                case 4: if (spiderSpawner != null) spiderSpawner.SpawnSpiders(); break;
            }
        }
    }

    // Plays a sound from the given AudioSource
    private void PlaySound(AudioSource source)
    {
        if (source != null && source.clip != null)
            source.PlayOneShot(source.clip);
    }
}
