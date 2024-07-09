using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using UnityEngine.InputSystem;

public class MedicalVRAgent : Agent
{
    [Header("Transforms")]
    public Transform headTransform;
    public Transform leftHandTransform;
    public Transform rightHandTransform;

    [Header("Input Actions")]
    public InputAction leftHandPositionAction;
    public InputAction leftHandRotationAction;
    public InputAction rightHandPositionAction;
    public InputAction rightHandRotationAction;

    [Header("Environment")]
    public StoneArea stoneArea;
    public Stone stone;

    [Header("Target")]
    public Transform targetTransform;
    public float placementThreshold = 0.035f;

    public Vector3 InitialStonePosition { get; private set; }

    private Vector3 initialHeadPosition;
    private Quaternion initialHeadRotation;
    private Vector3 initialLeftHandPosition;
    private Quaternion initialLeftHandRotation;
    private Vector3 initialRightHandPosition;
    private Quaternion initialRightHandRotation;

    private bool isStoneStable = false;
    private float stableTimer = 0f;
    private const float STABLE_TIME = 0.5f; // Time the stone needs to be stable
    private bool isEpisodeEnding = false;
    private bool isStationEnding = false;

    private void OnEnable()
    {
        leftHandPositionAction.Enable();
        leftHandRotationAction.Enable();
        rightHandPositionAction.Enable();
        rightHandRotationAction.Enable();
    }

    private void OnDisable()
    {
        leftHandPositionAction.Disable();
        leftHandRotationAction.Disable();
        rightHandPositionAction.Disable();
        rightHandRotationAction.Disable();
    }

    public override void Initialize()
    {
        base.Initialize();

        // Store initial positions and rotations
        initialHeadPosition = headTransform.localPosition;
        initialHeadRotation = headTransform.localRotation;
        initialLeftHandPosition = leftHandTransform.localPosition;
        initialLeftHandRotation = leftHandTransform.localRotation;
        initialRightHandPosition = rightHandTransform.localPosition;
        initialRightHandRotation = rightHandTransform.localRotation;
        InitialStonePosition = stone.transform.localPosition;

        if (stoneArea == null)
        {
            Debug.LogError("Stone Area not assigned!");
        }
        if (stone == null)
        {
            Debug.LogError("Stone not assigned!");
        }
        if (targetTransform == null)
        {
            Debug.LogError("Target Transform not assigned!");
        }
    }

    private void Update()
    {
        if (!isEpisodeEnding && IsStonePlacedOnTarget())
        {
            Debug.Log("Stone placed on target. Ending episode.");
            isEpisodeEnding = true;
            AddReward(1.0f);
            EndEpisode();
        }
    }


    public override void OnEpisodeBegin()
    {
        base.OnEpisodeBegin();
        isEpisodeEnding = false;

        // Reset all relevant transforms to their initial positions and rotations
        headTransform.localPosition = initialHeadPosition;
        headTransform.localRotation = initialHeadRotation;
        leftHandTransform.localPosition = initialLeftHandPosition;
        leftHandTransform.localRotation = initialLeftHandRotation;
        rightHandTransform.localPosition = initialRightHandPosition;
        rightHandTransform.localRotation = initialRightHandRotation;

        // Reset the stone
        if (stoneArea != null)
        {
            stoneArea.ResetStone();
        }
        else
        {
            Debug.LogError("StoneArea is not assigned in MedicalVRAgent!");
        }

        Debug.Log("Episode begun. Scene reset.");
    }

    private bool IsStonePlacedOnTarget()
    {
        if (stone == null || targetTransform == null)
        {
            return false;
        }

        float distanceToTarget = Vector3.Distance(stone.transform.position, targetTransform.position);
        bool isPlaced = distanceToTarget < placementThreshold;

        if (isPlaced)
        {
            Debug.Log($"Stone placed on target. Distance: {distanceToTarget}");
        }

        return isPlaced;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Collect observations for head and hands
        sensor.AddObservation(headTransform.localPosition);
        sensor.AddObservation(headTransform.localRotation);

        sensor.AddObservation(leftHandTransform.localPosition);
        sensor.AddObservation(leftHandTransform.localRotation);

        sensor.AddObservation(rightHandTransform.localPosition);
        sensor.AddObservation(rightHandTransform.localRotation);

        // Collect observation for the stone's position
        sensor.AddObservation(stone.transform.localPosition);

        // Collect observation for the target's position
        sensor.AddObservation(targetTransform.localPosition);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        // Apply actions to VR hands (if needed for your training logic)
        float leftHandX = actions.ContinuousActions[0];
        float leftHandY = actions.ContinuousActions[1];
        float leftHandZ = actions.ContinuousActions[2];
        float leftHandRotX = actions.ContinuousActions[3];
        float leftHandRotY = actions.ContinuousActions[4];
        float leftHandRotZ = actions.ContinuousActions[5];
        float leftHandRotW = actions.ContinuousActions[6];

        float rightHandX = actions.ContinuousActions[7];
        float rightHandY = actions.ContinuousActions[8];
        float rightHandZ = actions.ContinuousActions[9];
        float rightHandRotX = actions.ContinuousActions[10];
        float rightHandRotY = actions.ContinuousActions[11];
        float rightHandRotZ = actions.ContinuousActions[12];
        float rightHandRotW = actions.ContinuousActions[13];

        leftHandTransform.localPosition = new Vector3(leftHandX, leftHandY, leftHandZ);
        leftHandTransform.localRotation = new Quaternion(leftHandRotX, leftHandRotY, leftHandRotZ, leftHandRotW);

        rightHandTransform.localPosition = new Vector3(rightHandX, rightHandY, rightHandZ);
        rightHandTransform.localRotation = new Quaternion(rightHandRotX, rightHandRotY, rightHandRotZ, rightHandRotW);

        // Check if the stone is placed on the target game object
        if (IsStonePlacedOnTarget())
        {
            Debug.Log("Stone placed on the target.");
            AddReward(1.0f);
            EndEpisode();
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActions = actionsOut.ContinuousActions;
        // Use VR controller inputs as heuristic for training
        continuousActions[0] = leftHandPositionAction.ReadValue<Vector3>().x;
        continuousActions[1] = leftHandPositionAction.ReadValue<Vector3>().y;
        continuousActions[2] = leftHandPositionAction.ReadValue<Vector3>().z;

        continuousActions[3] = leftHandRotationAction.ReadValue<Quaternion>().x;
        continuousActions[4] = leftHandRotationAction.ReadValue<Quaternion>().y;
        continuousActions[5] = leftHandRotationAction.ReadValue<Quaternion>().z;
        continuousActions[6] = leftHandRotationAction.ReadValue<Quaternion>().w;

        continuousActions[7] = rightHandPositionAction.ReadValue<Vector3>().x;
        continuousActions[8] = rightHandPositionAction.ReadValue<Vector3>().y;
        continuousActions[9] = rightHandPositionAction.ReadValue<Vector3>().z;

        continuousActions[10] = rightHandRotationAction.ReadValue<Quaternion>().x;
        continuousActions[11] = rightHandRotationAction.ReadValue<Quaternion>().y;
        continuousActions[12] = rightHandRotationAction.ReadValue<Quaternion>().z;
        continuousActions[13] = rightHandRotationAction.ReadValue<Quaternion>().w;
    }

    
}
