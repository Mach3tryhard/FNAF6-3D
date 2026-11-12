using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPlacement : MonoBehaviour
{
    BuildManager buildingManager;

    void Start()
    {
        buildingManager = GameObject.Find("Building_Manager").GetComponent<BuildManager>();
    }

    private void OnTriggerStay(Collider other)
    {
        // Make sure the object we are checking is not the current pendingObject
        if (other.gameObject.CompareTag("Placeable")/* && other.gameObject != buildingManager.pendingObject*/)
        {
            buildingManager.canPlace = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // When the other object leaves the area, allow placement
        if (other.gameObject.CompareTag("Placeable")/* && other.gameObject != buildingManager.pendingObject*/)
        {
            buildingManager.canPlace = true;
        }
    }
}
