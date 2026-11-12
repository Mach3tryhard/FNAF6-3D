using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

[ExecuteInEditMode]
public class zoom : MonoBehaviour
{
    public float defaultFOV = 70;
    public float maxZoomFOV = 15;
    [Range(0, 1)]
    public float currentZoom;
    public float sensitivity = 1;

    private CinemachineVirtualCamera cinemachineCamera;

    void Awake()
    {
        cinemachineCamera = GetComponent<CinemachineVirtualCamera>();
        if (cinemachineCamera)
        {
            defaultFOV = cinemachineCamera.m_Lens.FieldOfView;
        }
    }

    void Update()
    {
        currentZoom += Input.mouseScrollDelta.y * sensitivity * .05f;
        currentZoom = Mathf.Clamp01(currentZoom);
        cinemachineCamera.m_Lens.FieldOfView = Mathf.Lerp(defaultFOV, maxZoomFOV, currentZoom);
    }
}

