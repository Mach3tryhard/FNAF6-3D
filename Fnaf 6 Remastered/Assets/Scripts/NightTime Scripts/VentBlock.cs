using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VentBlock : MonoBehaviour
{
    private GameObject ventblock;
    public BoxCollider boxCollider;
    private PlayerVentInteraction playerVentInteraction;

    void Start()
    {
        playerVentInteraction = GameObject.FindWithTag("Player").GetComponent<PlayerVentInteraction>();
        ventblock = GameObject.Find("VentBlock");
        //boxCollider = ventblock.GetComponent<BoxCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        if(playerVentInteraction.inVent==true)
        {
            boxCollider.enabled=true;
        }
        else
        {
            boxCollider.enabled=false;
        }
    }
}
