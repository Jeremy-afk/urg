using Mirror;
using System;
using UnityEngine;
using UnityEngine.Audio;

public class CarSound : NetworkBehaviour
{
    [SerializeField]
    private bool autoStartEngine = false;

    [Header("Target")]
    [SerializeField, Tooltip("Automatically listens to the target rigidbody, and adjusts the sound accordingly.\nThat makes this component autonomous that doesn't need any extern call.")]
    private bool autoTargetRigidbody = false;
    [SerializeField]
    private Rigidbody targetRigidbody;

    [Header("Volume Settings")]
    [SerializeField, Range(0f, 1f)]
    private float selfEngineVolume = 1.0f;
    [SerializeField, Range(0f, 1f)]
    private float otherEngineVolume = 0.5f;

    [Header("Smoothing")]
    [SerializeField]
    private float smoothingFactor = 0.9f;

    [Header("Pitching")]
    [SerializeField]
    private float baseRpmPitch = 1f;
    [SerializeField, Tooltip("Pitch rise per speed unit.")]
    private float risingPitchSpeed = 0.1f;

    [Header("Blending")]
    [SerializeField, Tooltip("The max speed from which the engineRpm sound will be at max volume compared to the engineBase sound")]
    private float blendSpeed = 100;

    [Header("Sources")]
    [SerializeField]
    private AudioSource engineBaseSource;
    [SerializeField]
    private AudioSource engineRpmSource;

    [SerializeField]
    private AudioMixer audioMixer;

    private bool engineStarted = false;
    private float currentSpeedSound = 0;
    private float targetSpeedSound = 0;

    private void Start()
    {
        // Set the volume of the engine sound based on whether this is the local player or not
        audioMixer.SetFloat("CarEngineVolume", isLocalPlayer ? selfEngineVolume : otherEngineVolume);

        engineBaseSource.volume = 1;
        engineRpmSource.volume = 0;

        if (autoStartEngine) StartEngine();
    }

    public void StartEngine()
    {
        engineStarted = true;
        engineBaseSource.Play();
        engineRpmSource.Play();
    }

    // Use this to update the sound corresponding to the passed speed, useless if autoTargetRigidbody is enabled
    public void UpdateSpeed(float speed)
    {
        targetSpeedSound = speed;
    }

    private void Update()
    {
        if (!engineStarted) return;

        if (autoTargetRigidbody) UpdateRigidbodySpeed();

        UpdateSpeedSound();
        currentSpeedSound = Mathf.Lerp(currentSpeedSound, targetSpeedSound, smoothingFactor);
    }

    private void UpdateRigidbodySpeed()
    {
        if (!targetRigidbody)
        {
            autoTargetRigidbody = false;
            Debug.LogWarning("No target Rigidbody set for CarSound script but 'autoTargetRigidbody' is checked.");
            return;
        }

        targetSpeedSound = targetRigidbody.velocity.magnitude;
    }

    private void UpdateSpeedSound()
    {
        // Modulate the volume (blend the engineBaseSound to engineRpmSound) based on the speed of the car
        engineBaseSource.volume = Mathf.Clamp01(1 - currentSpeedSound / blendSpeed);
        engineRpmSource.volume = Mathf.Clamp01(currentSpeedSound / blendSpeed);

        // Update the pitch of the engine sound based on the speed of the car
        engineRpmSource.pitch = baseRpmPitch + currentSpeedSound * risingPitchSpeed;
    }
}
