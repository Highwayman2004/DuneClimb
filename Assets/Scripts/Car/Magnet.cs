using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class Magnet : MonoBehaviour
{
    [SerializeField] XRSimpleInteractable interactable;
    [SerializeField] Transform lever;
    [SerializeField] float maxMove;
    [SerializeField] float activateDistance;
    [SerializeField] float returnSpeed;
    [SerializeField] float collectDistance;

    Transform activeHandTrackerAnchor;
    float handStartingPos;
    float totalCalculatedPos;
    float keyboardPos;
    bool useKeyboard;
    bool activated = false;

    void Start()
    {
        interactable.selectEntered.AddListener(OnHandGripSecured);
        interactable.selectExited.AddListener(OnHandRelease);

        totalCalculatedPos = 0;
        keyboardPos = 0;
        handStartingPos = 0;
    }

    public void SetForKeyboard()
    {
        useKeyboard = true;
    }

    void OnHandGripSecured(SelectEnterEventArgs args)
    {
        activeHandTrackerAnchor = args.interactorObject.transform;
        handStartingPos = -lever.parent.InverseTransformPoint(activeHandTrackerAnchor.position).y;
    }

    void OnHandRelease(SelectExitEventArgs args)
    {
        activeHandTrackerAnchor = null;
    }

    void Update()
    {
        if (useKeyboard)
        {
            if (Keyboard.current.mKey.wasPressedThisFrame)
            {
                Activate();
            }
        }
        else
        {
            if (activeHandTrackerAnchor != null)
            {
                ProcessLevelPositionTracking();

                if (!activated && totalCalculatedPos > activateDistance)
                    Activate();
            }
            else
            {
                Return();
            }
        }
    }

    void ProcessLevelPositionTracking()
    {
        Vector3 currentLocalHandPoint = lever.parent.InverseTransformPoint(activeHandTrackerAnchor.position);
        float currentHandPos = -currentLocalHandPoint.y;

        float delta = currentHandPos - handStartingPos;

        totalCalculatedPos += delta;
        totalCalculatedPos = Mathf.Clamp(totalCalculatedPos, 0, maxMove);
        lever.localPosition = new(lever.localPosition.x, -totalCalculatedPos, lever.localPosition.z);

        handStartingPos = currentHandPos;
    }

    void Return()
    {
        totalCalculatedPos -= returnSpeed * Time.deltaTime;

        totalCalculatedPos = Mathf.Max(0, totalCalculatedPos);
        lever.localPosition = new(lever.localPosition.x, -totalCalculatedPos, lever.localPosition.z);
    }

    void Activate()
    {
        //activated = true;

        LootManager.Instance.CollectNearby(transform.position, collectDistance);
    }
}
