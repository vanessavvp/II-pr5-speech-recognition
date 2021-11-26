using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AxleInfo
{
    public WheelCollider leftWheel;
    public WheelCollider rightWheel;
    public bool motor;     // Está este eje conectado al motor?
    public bool steering;  // Pueden estas ruedas girar con la direccion del vehiculo?
}
public class CarController : MonoBehaviour
{
    [SerializeField]
    private List<AxleInfo> axleInfos; 
    [SerializeField]
    private float maxMotorTorque;
    [SerializeField]
    private float maxBrakeTorque;
    [SerializeField]
    private float maxSteeringAngle;
    [SerializeField]
    private bool useVoiceRecognition;
    private float pedalPosition = 0;
    private float brakePosition = 0;
    private float directionPosition = 0;

    public void ApplyLocalPositionToVisuals(WheelCollider collider)
    {
        if (collider.transform.childCount == 0)
        {
            return;
        }

        Transform visualWheel = collider.transform.GetChild(0);
        Vector3 position;
        Quaternion rotation;

        collider.GetWorldPose(out position, out rotation);
        visualWheel.transform.position = position;
        visualWheel.transform.rotation = rotation;
    }

    public void FixedUpdate()
    {
        float motor;
        float steering;
        float brake;

        if (useVoiceRecognition)
        {
            motor = maxMotorTorque * pedalPosition;
            steering = maxSteeringAngle * directionPosition;
            brake = maxBrakeTorque * brakePosition;

            foreach (AxleInfo axleInfo in axleInfos)
            {
                if (axleInfo.steering)
                {
                    axleInfo.leftWheel.steerAngle = steering;
                    axleInfo.rightWheel.steerAngle = steering;
                }
            
                if (axleInfo.motor)
                {
                    axleInfo.leftWheel.motorTorque = motor;
                    axleInfo.rightWheel.motorTorque = motor;
                    axleInfo.leftWheel.brakeTorque = brake;
                    axleInfo.rightWheel.brakeTorque = brake;
                }

                ApplyLocalPositionToVisuals(axleInfo.leftWheel);
                ApplyLocalPositionToVisuals(axleInfo.rightWheel);
            }
        }

        
    }

    public void MoveForward()
    {
        brakePosition = 0;
        pedalPosition += 0.2f;
        pedalPosition = Mathf.Clamp(pedalPosition, -1, 1);
    }

    public void MoveBackward()
    {
        brakePosition = 0;
        pedalPosition -= 0.2f;
        pedalPosition = Mathf.Clamp(pedalPosition, -1, 1);
    }

    public void SteerLeft()
    {
        directionPosition -= 0.2f;
        directionPosition = Mathf.Clamp(directionPosition, -1, 1);
    }

    public void SteerRight()
    {
        directionPosition += 0.2f;
        directionPosition = Mathf.Clamp(directionPosition, -1, 1);
    }

    public void SteerAllRight()
    {
        directionPosition = 1f;
    }

    public void SteerAllLeft()
    {
        directionPosition = -1f;
    }

    public void Stop()
    {
        brakePosition = 1f;
        pedalPosition = 0f;
    }

    public void SteerStraightAhead()
    {
        directionPosition = 0;
    }

    public void MaxSpeed()
    {
        pedalPosition *= 100f;
        pedalPosition = Mathf.Clamp(pedalPosition, -1, 1);
    }

    public void ResetCarPosition() 
    {
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        SteerStraightAhead();
        brakePosition = 100f;
        pedalPosition = 0f;
        transform.position = Vector3.up;
        transform.rotation = Quaternion.identity;
    }
}
