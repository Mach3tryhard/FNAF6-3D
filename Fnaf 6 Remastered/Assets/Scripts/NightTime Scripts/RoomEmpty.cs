using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomEmpty : MonoBehaviour
{
    private OfficeMechanics _OfficeMechanicsScript;
    private Night_Mechanics nightScript;

    private GameObject _SpringTrap;
    private GameObject _Lefty;
    private GameObject _ScrapBaby;
    private GameObject _MoltenFreddy;

    private GameObject _Room11;
    private GameObject _Room12;
    private GameObject _Room14;
    private GameObject _Room15;

    void Start()
    {
        _Room11 = GameObject.FindGameObjectWithTag("NightTime").transform.Find("Room11").gameObject;
        _Room12 = GameObject.FindGameObjectWithTag("NightTime").transform.Find("Room12").gameObject;
        _Room14 = GameObject.FindGameObjectWithTag("NightTime").transform.Find("Room14").gameObject;
        _Room15 = GameObject.FindGameObjectWithTag("NightTime").transform.Find("Room15").gameObject;

        _SpringTrap = GameObject.FindGameObjectWithTag("NightTime").transform.Find("SpringTrap").gameObject;
        _Lefty = GameObject.FindGameObjectWithTag("NightTime").transform.Find("Lefty").gameObject;
        _ScrapBaby = GameObject.FindGameObjectWithTag("NightTime").transform.Find("ScrapBaby").gameObject;
        _MoltenFreddy = GameObject.FindGameObjectWithTag("NightTime").transform.Find("MoltenFreddy").gameObject;

        nightScript = GameObject.FindWithTag("NightTime").GetComponent<Night_Mechanics>();
        _OfficeMechanicsScript = GameObject.FindWithTag("Player").GetComponent<OfficeMechanics>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.name=="SpringTrap" || other.name=="MoltenFreddy" || other.name=="ScrapBaby" || other.name=="Lefty")
        if(_OfficeMechanicsScript.NotInVent==false && other.name=="Lefty")
        {
            nightScript.AnimatronicsRST[0]=true;

            float distanceToPointA = Vector3.Distance(_Lefty.transform.position, _Room11.transform.position);
            float distanceToPointB = Vector3.Distance(_Lefty.transform.position, _Room15.transform.position);

            if(distanceToPointA < distanceToPointB)
                nightScript.AnimatronicsPOS[0]=nightScript.Origin1;
            else
                nightScript.AnimatronicsPOS[0]=nightScript.Origin4;
        }
        if(_OfficeMechanicsScript.NotInVent==false && other.name=="ScrapBaby")
        {
            nightScript.AnimatronicsRST[1]=true;

            float distanceToPointA = Vector3.Distance(_ScrapBaby.transform.position, _Room12.transform.position);
            float distanceToPointB = Vector3.Distance(_ScrapBaby.transform.position, _Room14.transform.position);

            if(distanceToPointA < distanceToPointB)
                nightScript.AnimatronicsPOS[1]=nightScript.Origin2;
            else
                nightScript.AnimatronicsPOS[1]=nightScript.Origin3;
        }
        if(_OfficeMechanicsScript.NotInVent==false && other.name=="SpringTrap")
        {
            nightScript.AnimatronicsRST[2]=true;

            float distanceToPointA = Vector3.Distance(_SpringTrap.transform.position, _Room12.transform.position);
            float distanceToPointB = Vector3.Distance(_SpringTrap.transform.position, _Room14.transform.position);

            if(distanceToPointA < distanceToPointB)
                nightScript.AnimatronicsPOS[2]=nightScript.Origin2;
            else
                nightScript.AnimatronicsPOS[2]=nightScript.Origin3;
        }
        if(_OfficeMechanicsScript.NotInVent==false && other.name=="MoltenFreddy")
        {
            nightScript.AnimatronicsRST[3]=true;

            float distanceToPointA = Vector3.Distance(_MoltenFreddy.transform.position, _Room11.transform.position);
            float distanceToPointB = Vector3.Distance(_MoltenFreddy.transform.position, _Room15.transform.position);

            if(distanceToPointA < distanceToPointB)
                nightScript.AnimatronicsPOS[3]=nightScript.Origin1;
            else
                nightScript.AnimatronicsPOS[3]=nightScript.Origin4;
        }
    }
}
