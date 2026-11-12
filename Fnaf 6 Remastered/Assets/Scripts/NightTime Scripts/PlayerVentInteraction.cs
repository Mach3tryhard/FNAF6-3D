using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;

public class PlayerVentInteraction : MonoBehaviour
{
    [Header("Animation Controls and key")]
    public float interactionDistance = 2f;
    public KeyCode interactKey = KeyCode.E;
    public float moveToVentSpeed = 5f;
    public float scaleTransitionSpeed = 10f; // Adjust as needed

    private Vector3 originalScale;
    private Vector3 targetScale;

    [Header("Vent Controls")]
	private FirstPersonController firstpersoncontroller;

    private bool canInteract = false;
    private Transform ventTransform;
    private Transform ventEntryPoint;
    private Transform ventExitPoint;
    private Transform TransitionPoint;
    public bool isVenting = false;
    public bool inVent = false;
    private OfficeMechanics _OfficeMechanics;

    void Start()
    {
        _OfficeMechanics = GameObject.FindWithTag("Player").GetComponent<OfficeMechanics>();
        firstpersoncontroller = GetComponent<FirstPersonController>();
        originalScale = transform.localScale;
    }

    void Update()
    {
        if (!isVenting && canInteract && Input.GetKeyDown(interactKey) && inVent==false && firstpersoncontroller.Grounded==true)
        {
            StartCoroutine(JumpIntoVent());
        }
        if (!isVenting && canInteract && Input.GetKeyDown(interactKey) && inVent==true)
        {
            StartCoroutine(JumpOutVent());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Vent"))
        {
            ventTransform = other.transform;
            ventEntryPoint = ventTransform.Find("VentEntryPoint");
            ventExitPoint = ventTransform.Find("ventExitPoint");
            TransitionPoint = ventTransform.Find("TransitionPoint");
            if (ventEntryPoint != null)
            {
                canInteract = true;
                // Optionally, show a UI prompt to interact
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Vent"))
        {
            canInteract = false;
            ventTransform = null;
            ventEntryPoint = null;
            // Optionally, hide the UI prompt
        }
    }

    IEnumerator JumpIntoVent()
    {
        isVenting = true;
        canInteract= false;

        targetScale = new Vector3(originalScale.x, originalScale.y * 0.4f, originalScale.z);
        while (Vector3.Distance(transform.position, TransitionPoint.position) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, TransitionPoint.position, moveToVentSpeed * Time.deltaTime);
            transform.localScale = Vector3.Lerp(transform.localScale, targetScale, scaleTransitionSpeed * Time.deltaTime);
            yield return null;
        }

        transform.position = TransitionPoint.position;

        Debug.Log("Scaled down before vent");

        /*
        while (transform.localScale != targetScale)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, targetScale, scaleTransitionSpeed * Time.deltaTime);
            yield return null;
        }
        Debug.Log("Scaled down for vent");*/

        while (Vector3.Distance(transform.position, ventEntryPoint.position) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, ventEntryPoint.position, moveToVentSpeed * Time.deltaTime);
            yield return null;
        }
        Debug.Log("Moved in vent");
        // Ensure exact position
        transform.position = ventEntryPoint.position;

        // Optionally, handle other interactions like changing scenes, playing additional animations, etc.

        inVent = true;
        isVenting = false;
        canInteract= true;
        _OfficeMechanics._computerColor.color = new Color32(121,17,17,255);
        _OfficeMechanics._fanColor.color = new Color32(121,17,17,225);
        _OfficeMechanics.NotInVent=!inVent;
    }

    IEnumerator JumpOutVent()
    {
        isVenting = true;
        canInteract = false;

        while (Vector3.Distance(transform.position, TransitionPoint.position) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, TransitionPoint.position, moveToVentSpeed * Time.deltaTime);
            yield return null;
        }

        transform.position = TransitionPoint.position;

        while (Vector3.Distance(transform.position, ventExitPoint.position) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, ventExitPoint.position, moveToVentSpeed * Time.deltaTime);
            yield return null;
        }
        Debug.Log("Moved out of vent");

        transform.position = ventExitPoint.position;

        while (transform.localScale != originalScale)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, originalScale, scaleTransitionSpeed * Time.deltaTime);
            yield return null;
        }
        Debug.Log("Scaled up after vent");
        
        inVent = false;
        isVenting = false;
        canInteract= true;
        if(_OfficeMechanics.ComputerOn==true)
            _OfficeMechanics._computerColor.color = new Color32(35,94,25,255);
        else
            _OfficeMechanics._computerColor.color = new Color32(100,100,100,255);
        if(_OfficeMechanics.FanOn==true)
            _OfficeMechanics._fanColor.color = new Color32(38,141,154,255);
        else
            _OfficeMechanics._fanColor.color = new Color32(100,100,100,255);
        _OfficeMechanics.NotInVent=!inVent;
    }
}