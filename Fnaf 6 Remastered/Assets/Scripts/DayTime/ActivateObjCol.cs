using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateObjCol : MonoBehaviour
{
    private GameObject PlayerCapsuleCol;
    private Collider ActivateCollider;
    private Collider PlayerCollider;

    private bool PlayerIsColliding = false;
    public bool JustPlaced = false;

    void Start()
    {
        PlayerCapsuleCol = GameObject.Find("PlayerCapsuleCol");
        PlayerCollider = PlayerCapsuleCol.GetComponent<Collider>();
        ActivateCollider = GetComponent<Collider>();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == PlayerCapsuleCol && JustPlaced)
        {
            PlayerIsColliding = false;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject == PlayerCapsuleCol && JustPlaced)
        {
            PlayerIsColliding = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == PlayerCapsuleCol && JustPlaced)
        {
            PlayerIsColliding = true;
        }
    }

    void Update()
    {
        if (JustPlaced)
        {
            if (!ActivateCollider.bounds.Intersects(PlayerCollider.bounds))
            {
                EnableCollider();
            }
            else
            {
                PlayerIsColliding = true;
            }
            if (!PlayerIsColliding)
            {
                EnableCollider();
            }
        }
    }

    void EnableCollider()
    {
        Transform childTransform = gameObject.transform.Find("ColliderAndSize");
        if (childTransform != null)
        {
            Collider childCollider = childTransform.gameObject.GetComponent<Collider>();
            if (childCollider != null)
            {
                childCollider.enabled = true;
            }
        }

        JustPlaced = false;
    }
}