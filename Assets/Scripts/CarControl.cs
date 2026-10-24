using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;

public class CarControl : MonoBehaviour
{
    [SerializeField] SteeringWheel steeringWheel;
    [SerializeField] GearLever gearLever;
    [SerializeField] FuelController fuelController;
    [SerializeField] float motorTorque = 2000f;
    [SerializeField] float brakeTorque = 2000f;
    [SerializeField] float maxSpeed = 20f;
    [SerializeField] float steeringRange = 30f;
    [SerializeField] float steeringRangeAtMaxSpeed = 10f;
    [SerializeField] float centreOfGravityOffset = -1f;
    [SerializeField] bool useKeyboard;

    private WheelControl[] wheels;
    private Rigidbody rigidBody;

    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();

        Vector3 centerOfMass = rigidBody.centerOfMass;
        centerOfMass.y += centreOfGravityOffset;
        rigidBody.centerOfMass = centerOfMass;

        wheels = GetComponentsInChildren<WheelControl>();

        if (useKeyboard)
        {
            steeringWheel.SetForKeyboard();
            gearLever.SetForKeyboard();
        }
    }

    void Update()
    {
        float forwardSpeed = Vector3.Dot(transform.forward, rigidBody.linearVelocity);
        fuelController.UseFuel(Mathf.Abs(forwardSpeed));
    }


    void FixedUpdate()
    {
        float forwardSpeed = Vector3.Dot(transform.forward, rigidBody.linearVelocity);
        float speedFactor = Mathf.InverseLerp(0, maxSpeed, Mathf.Abs(forwardSpeed));

        float steeringFactor = steeringWheel.GetNormalizedSteeringValue();
        float power = fuelController.HasFuel ? gearLever.GetNormalizedSpeedValue() : -Mathf.Sign(forwardSpeed);

        float currentMotorTorque = Mathf.Lerp(motorTorque, 0, speedFactor);
        float currentSteerRange = Mathf.Lerp(steeringRange, steeringRangeAtMaxSpeed, speedFactor);

        bool isAccelerating = Mathf.Sign(power) == Mathf.Sign(forwardSpeed);

        foreach (var wheel in wheels)
        {
            if (wheel.Steerable)
                wheel.WheelCollider.steerAngle = steeringFactor * currentSteerRange;

            if (isAccelerating)
            {
                if (wheel.Motorized)
                    wheel.WheelCollider.motorTorque = power * currentMotorTorque;
                wheel.WheelCollider.brakeTorque = 0f;
            }
            else
            {
                wheel.WheelCollider.motorTorque = 0f;
                wheel.WheelCollider.brakeTorque = Mathf.Abs(power) * brakeTorque;
            }
        }
    }
}
