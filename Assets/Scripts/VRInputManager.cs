using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR;

public class VRInputManager : MonoBehaviour
{
    public UnityEvent<bool> PrimaryButtonEvent;
    public UnityEvent<bool> SecondaryButtonEvent;
    public UnityEvent<bool> TriggerEvent;

    List<InputDevice> inputDevices;
    InputDevice rightController;
    InputDeviceCharacteristics rightControllerChar = InputDeviceCharacteristics.Right | InputDeviceCharacteristics.Controller;

    private bool lastPrimaryButtonState = false;
    private bool lastSecondaryButtonState = false;
    private bool lastTriggerState = false;

    void Start()
    {
        if (PrimaryButtonEvent == null) PrimaryButtonEvent = new UnityEvent<bool>();
        if (SecondaryButtonEvent == null) SecondaryButtonEvent = new UnityEvent<bool>();
        if (TriggerEvent == null) TriggerEvent = new UnityEvent<bool>();
        inputDevices = new List<InputDevice>();
        InputTracking.nodeAdded += InputTracking_nodeAdded;
        updateDevices();
    }

    void Update()
    {
        if (rightController != null)
        {
            bool primaryButtonState = false;
            rightController.TryGetFeatureValue(CommonUsages.primaryButton, out primaryButtonState);
            if (lastPrimaryButtonState != primaryButtonState)
            {
                PrimaryButtonEvent.Invoke(primaryButtonState);
                lastPrimaryButtonState = primaryButtonState;
            }

            bool secondaryButtonState = false;
            rightController.TryGetFeatureValue(CommonUsages.secondaryButton, out secondaryButtonState);
            if (lastSecondaryButtonState != secondaryButtonState)
            {
                SecondaryButtonEvent.Invoke(secondaryButtonState);
                lastSecondaryButtonState = secondaryButtonState;
            }

            bool triggerState = false;
            rightController.TryGetFeatureValue(CommonUsages.triggerButton, out triggerState);
            if (lastTriggerState != triggerState)
            {
                TriggerEvent.Invoke(triggerState);
                lastTriggerState = triggerState;
            }
        }
    }

    private void updateDevices()
    {
        InputDevices.GetDevicesWithCharacteristics(rightControllerChar, inputDevices);
        if (inputDevices.Count > 0)
        {
            rightController = inputDevices[0];
        }
    }

    private void InputTracking_nodeAdded(XRNodeState obj)
    {
        updateDevices();
    }
}
