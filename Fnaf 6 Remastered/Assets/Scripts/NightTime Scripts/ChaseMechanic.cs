using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ChaseMechanic : MonoBehaviour
{
    private GameObject _PlayerOBJ;
    private OfficeMechanics _OfficeMechanics;
    private Night_Mechanics _NightScript;

    private NavMeshAgent _SpringTrap;
    private NavMeshAgent _Lefty;
    private NavMeshAgent _ScrapBaby;
    private NavMeshAgent _MoltenFreddy;

    void Start()
    {
        _NightScript = GameObject.FindWithTag("NightTime").GetComponent<Night_Mechanics>();
        _OfficeMechanics = GameObject.FindWithTag("Player").GetComponent<OfficeMechanics>();
        _PlayerOBJ = GameObject.FindWithTag("Player");

        _SpringTrap = GameObject.FindGameObjectWithTag("NightTime").transform.Find("SpringTrap").gameObject.GetComponent<NavMeshAgent>();
        _Lefty = GameObject.FindGameObjectWithTag("NightTime").transform.Find("Lefty").gameObject.GetComponent<NavMeshAgent>();
        _ScrapBaby = GameObject.FindGameObjectWithTag("NightTime").transform.Find("ScrapBaby").gameObject.GetComponent<NavMeshAgent>();
        _MoltenFreddy = GameObject.FindGameObjectWithTag("NightTime").transform.Find("MoltenFreddy").gameObject.GetComponent<NavMeshAgent>();
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Animatronic"))
        {
            Debug.Log("IsCHASING");
            if(other.name == "Lefty" && _OfficeMechanics.NotInVent==false && _Lefty.enabled==true)
            {
                _NightScript.AnimatronicChase[0]=true;
                other.gameObject.GetComponent<NavMeshAgent>().SetDestination(_PlayerOBJ.transform.position);
            }
            if(other.name == "ScrapBaby" && _OfficeMechanics.NotInVent==false && _ScrapBaby.enabled==true)
            {
                _NightScript.AnimatronicChase[1]=true;
                other.gameObject.GetComponent<NavMeshAgent>().SetDestination(_PlayerOBJ.transform.position);
            }
            if(other.name == "SpringTrap" && _OfficeMechanics.NotInVent==false && _SpringTrap.enabled==true)
            {
                _NightScript.AnimatronicChase[2]=true;
                other.gameObject.GetComponent<NavMeshAgent>().SetDestination(_PlayerOBJ.transform.position);
            }
            if(other.name == "MoltenFreddy" && _OfficeMechanics.NotInVent==false && _MoltenFreddy.enabled==true)
            {
                _NightScript.AnimatronicChase[3]=true;
                other.gameObject.GetComponent<NavMeshAgent>().SetDestination(_PlayerOBJ.transform.position);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Animatronic"))
        {
            if(other.name == "Lefty")
            {
                _NightScript.AnimatronicChase[0]=false;
            }
            if(other.name=="ScrapBaby")
            {
                _NightScript.AnimatronicChase[1]=false;
            }
            if(other.name=="SpringTrap")
            {
                _NightScript.AnimatronicChase[2]=false;
            }
            if(other.name=="MoltenFreddy")
            {
                _NightScript.AnimatronicChase[3]=false;
            }
        }
    }

    void Update()
    {
        
    }
}
