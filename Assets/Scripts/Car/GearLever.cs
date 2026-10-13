using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class GearLever : MonoBehaviour
{
    //0.75 : 1,  0.25 : 0.5, -0.25 : 0, -0.75 : -0.5

    [SerializeField] XRSimpleInteractable interactable;
    [SerializeField] Transform lever;
    [SerializeField] float maxMove;
    [Space]
    [SerializeField] float keyboardMoveSpeed;
    [SerializeField] Transform keyboardPosition;

    Transform activeHandTrackerAnchor;
    float handStartingPos;
    float totalCalculatedPos;
    float keyboardPos;
    bool useKeyboard;

    void Start()
    {
        interactable.selectEntered.AddListener(OnHandGripSecured);
        interactable.selectExited.AddListener(OnHandRelease);

        totalCalculatedPos = -0.25f * maxMove;
        keyboardPos = -0.25f * maxMove;
        handStartingPos = -0.25f * maxMove;
    }

    public void SetForKeyboard()
    {
        useKeyboard = true;
        activeHandTrackerAnchor = keyboardPosition;
    }

    void OnHandGripSecured(SelectEnterEventArgs args)
    {
        activeHandTrackerAnchor = args.interactorObject.transform;
        handStartingPos = lever.parent.InverseTransformPoint(activeHandTrackerAnchor.position).z;
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
                activeHandTrackerAnchor = activeHandTrackerAnchor != null ? null : keyboardPosition;

            if (Keyboard.current.wKey.isPressed)
                keyboardPos += keyboardMoveSpeed * Time.deltaTime;

            if (Keyboard.current.sKey.isPressed)
                keyboardPos -= keyboardMoveSpeed * Time.deltaTime;

            keyboardPos = Mathf.Clamp(keyboardPos, -maxMove, maxMove);
            keyboardPosition.localPosition = new(keyboardPosition.localPosition.x, keyboardPosition.localPosition.y, keyboardPos);
        }

        if (activeHandTrackerAnchor != null)
            ProcessLevelPositionTracking();
    }

    void ProcessLevelPositionTracking()
    {
        Vector3 currentLocalHandPoint = lever.parent.InverseTransformPoint(activeHandTrackerAnchor.position);
        float currentHandPos = currentLocalHandPoint.z;

        float delta = currentHandPos - handStartingPos;

        totalCalculatedPos += delta;
        totalCalculatedPos = Mathf.Clamp(totalCalculatedPos, -maxMove, maxMove);
        lever.localPosition = new(lever.localPosition.x, lever.localPosition.y, totalCalculatedPos);

        handStartingPos = currentHandPos;
    }

    public float GetNormalizedSpeedValue()
    {
        float normalizedPos = totalCalculatedPos / maxMove;

        if (normalizedPos > 0.5f)
            return 1f;

        if (normalizedPos > 0f)
            return 0.5f;

        if (normalizedPos > -0.5f)
            return 0f;

        return -1f;
    }
}
