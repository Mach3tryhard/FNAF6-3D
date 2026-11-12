using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;

public class SC_HeadBobber : MonoBehaviour
{
    [Header("Head Bobbing Settings")]
    public float bobFrequency = 2f; // Frequency of the bobbing
    public float bobAmount = 0.08f; // Amplitude of the bobbing

    [Header("Camera Tilt Settings")]
    public float tiltAmount = 5f; // Maximum tilt angle
    public float tiltSpeed = 5f; // Speed of tilting

    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private float bobTimer = 0f;
    private Transform playerTransform;
    public StarterAssetsInputs input;

    public 

    void Start()
    {
        originalPosition = transform.localPosition;
        playerTransform = GetComponent<FirstPersonController>().transform;
        //input = playerTransform.GetComponent<FirstPersonController>();
    }

    void Update()
    {
        ApplyHeadBobbing();
        ApplyTilt();
    }

    void ApplyHeadBobbing()
    {
        if (input.move != Vector2.zero) // Only bob the head if the player is moving
        {
            bobTimer += Time.deltaTime * bobFrequency;
            Vector3 bobOffset = new Vector3(0, Mathf.Sin(bobTimer) * bobAmount, 0);
            transform.localPosition = originalPosition + bobOffset;
        }
        else
        {
            bobTimer = 0f; // Reset the bob timer when the player stops moving
            transform.localPosition = originalPosition;
        }
    }

    void ApplyTilt()
    {
        float targetTilt = 0f;

        if (input.move.x != 0)
        {
            targetTilt = -input.move.x * tiltAmount;
        }

        Quaternion targetRotation = originalRotation * Quaternion.Euler(0, 0, targetTilt);
        transform.localRotation = Quaternion.Lerp(transform.localRotation, targetRotation, Time.deltaTime * tiltSpeed);
    }
}