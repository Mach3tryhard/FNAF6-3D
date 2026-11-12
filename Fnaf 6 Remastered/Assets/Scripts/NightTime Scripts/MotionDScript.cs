using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotionDScript : MonoBehaviour
{
    private OfficeMechanics _OfficeMechanicsScript;
    private string Result;
    public AudioClip[] _AudioVector = new AudioClip[12];

    void Start()
    {
        _OfficeMechanicsScript = GameObject.FindWithTag("Player").GetComponent<OfficeMechanics>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.name=="SpringTrap" || other.name=="MoltenFreddy" || other.name=="ScrapBaby" || other.name=="Lefty")
        {
            other.gameObject.GetComponent<AudioSource>().Play();
            Result = gameObject.name.Substring(gameObject.name.Length - 2);
            _OfficeMechanicsScript.MotionDLastRoom=Result;
        }
    }
}
