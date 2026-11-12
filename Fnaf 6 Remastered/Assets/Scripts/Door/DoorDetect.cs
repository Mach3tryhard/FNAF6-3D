using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorDetect : MonoBehaviour
{
    public DoorAnim DoorAnimScript;

    private void OnTriggerEnter(Collider other)
    {
        if(other.name=="PlayerCapsule")
        {
            DoorAnimScript.Open=true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.name=="PlayerCapsule")
        {
            DoorAnimScript.Open=false;
        }
    }
}
