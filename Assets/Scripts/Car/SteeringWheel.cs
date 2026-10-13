using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class SteeringWheel : MonoBehaviour
{
    [SerializeField] XRSimpleInteractable interactable;
    [SerializeField] Transform wheel;
    [SerializeField] float maximumRotationLimitAngle = 360f;
    [Space]
    [SerializeField] float keyboardSpinSpeed;
    [SerializeField] Transform keyboardDirection;

    Transform activeHandTrackerAnchor;
    float totalCalculatedWheelAngle;
    float handStartingAngleOffset;
    float keyboardAngle = 0f;
    bool useKeyboard;

    void Start()
    {
        interactable.selectEntered.AddListener(OnHandGripSecured);
        interactable.selectExited.AddListener(OnHandRelease);
    }

    public void SetForKeyboard()
    {
        useKeyboard = true;
        handStartingAngleOffset = 90f;
        keyboardAngle = 90f;
        activeHandTrackerAnchor = keyboardDirection;
    }

    void OnHandGripSecured(SelectEnterEventArgs args)
    {
        activeHandTrackerAnchor = args.interactorObject.transform;

        Vector3 localHandPoint = wheel.parent.InverseTransformPoint(activeHandTrackerAnchor.position);
        handStartingAngleOffset = Mathf.Atan2(localHandPoint.y, localHandPoint.x) * Mathf.Rad2Deg;
    }

    void OnHandRelease(SelectExitEventArgs args)
    {
        activeHandTrackerAnchor = null;
    }

    void Update()
    {
        if (useKeyboard)
        {
            if (Keyboard.current.rKey.wasPressedThisFrame)
                activeHandTrackerAnchor = activeHandTrackerAnchor != null ? null : keyboardDirection;

            if (Keyboard.current.aKey.isPressed)
                keyboardAngle -= keyboardSpinSpeed * Time.deltaTime;

            if (Keyboard.current.dKey.isPressed)
                keyboardAngle += keyboardSpinSpeed * Time.deltaTime;

            keyboardDirection.transform.localPosition =
                new Vector3((float)Mathf.Cos(keyboardAngle * Mathf.Deg2Rad) * -0.25f, (float)Mathf.Sin(keyboardAngle * Mathf.Deg2Rad) * 0.25f, 0);
        }

        if (activeHandTrackerAnchor != null)
            ProcessWheelRotationTracking();
    }

    void ProcessWheelRotationTracking()
    {
        Vector3 currentLocalHandPoint = wheel.parent.InverseTransformPoint(activeHandTrackerAnchor.position);
        float currentHandAngle = Mathf.Atan2(currentLocalHandPoint.y, currentLocalHandPoint.x) * Mathf.Rad2Deg;

        float angleDelta = currentHandAngle - handStartingAngleOffset;

        if (angleDelta > 180.0f)
            angleDelta -= 360.0f;

        if (angleDelta < -180.0f)
            angleDelta += 360.0f;

        totalCalculatedWheelAngle += angleDelta;
        totalCalculatedWheelAngle = Mathf.Clamp(totalCalculatedWheelAngle, -maximumRotationLimitAngle, maximumRotationLimitAngle);
        wheel.localRotation = Quaternion.Euler(0f, 0f, totalCalculatedWheelAngle);

        handStartingAngleOffset = currentHandAngle;
    }

    public float GetNormalizedSteeringValue()
    {
        return -totalCalculatedWheelAngle / maximumRotationLimitAngle;
    }
}
