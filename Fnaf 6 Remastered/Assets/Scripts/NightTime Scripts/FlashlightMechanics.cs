using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FlashlightMechanics : MonoBehaviour
{
    [Header("Animation Controls and key")]
    public KeyCode interactKey = KeyCode.F;
    public bool TurnedOn = false;
    public bool set = false;

    [Header("Audio Clips")]
    public AudioClip turnOnSound;
    public AudioClip turnOffSound;
    public AudioClip NoBattery;

    [Header("Flashlight Timer")]
    public float flashlightUsageTime = 0f;
    public float flashlightTime=12f;

    private AudioSource _audioSource;
    private Light lightController;
    private Night_Mechanics nightScript;

    private GameObject _SpringTrap;
    private GameObject _Lefty;
    private GameObject _ScrapBaby;
    private GameObject _MoltenFreddy;

    private GameObject _Room11;
    private GameObject _Room12;
    private GameObject _Room14;
    private GameObject _Room15;
    private GameObject _Room31;
    private GameObject _Room35;

    private GameObject _Player;

    private OfficeMechanics _OfficeMechanics;

    private int cointoss;

    void Start()
    {
        _Player = GameObject.FindWithTag("Player");
        _OfficeMechanics = GameObject.FindWithTag("Player").GetComponent<OfficeMechanics>();

        _Room11 = GameObject.FindGameObjectWithTag("NightTime").transform.Find("Room11").gameObject;
        _Room12 = GameObject.FindGameObjectWithTag("NightTime").transform.Find("Room12").gameObject;
        _Room14 = GameObject.FindGameObjectWithTag("NightTime").transform.Find("Room14").gameObject;
        _Room15 = GameObject.FindGameObjectWithTag("NightTime").transform.Find("Room15").gameObject;
        _Room31 = GameObject.FindGameObjectWithTag("NightTime").transform.Find("Room31").gameObject;
        _Room35 = GameObject.FindGameObjectWithTag("NightTime").transform.Find("Room35").gameObject;

        _SpringTrap = GameObject.FindGameObjectWithTag("NightTime").transform.Find("SpringTrap").gameObject;
        _Lefty = GameObject.FindGameObjectWithTag("NightTime").transform.Find("Lefty").gameObject;
        _ScrapBaby = GameObject.FindGameObjectWithTag("NightTime").transform.Find("ScrapBaby").gameObject;
        _MoltenFreddy = GameObject.FindGameObjectWithTag("NightTime").transform.Find("MoltenFreddy").gameObject;

        nightScript = GameObject.FindWithTag("NightTime").GetComponent<Night_Mechanics>();
        _audioSource = GetComponent<AudioSource>();
        lightController = GetComponent<Light>();
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Animatronic"))
        {
            //other.gameObject.GetComponent<NavMeshAgent>().acceleration = 1;
            //other.gameObject.GetComponent<NavMeshAgent>().speed = 1;
            //other.gameObject.GetComponent<NavMeshAgent>().updateRotation=true;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Animatronic"))
        {
            /*if((other.name=="Lefty" || other.name=="ScrapBaby" || other.name=="SpringTrap" || other.name=="MoltenFreddy") && TurnedOn && _OfficeMechanics.NotInVent==false)
            {
                Vector3 _dirToPlayer = other.gameObject.transform.position - _Player.transform.position;
                Vector3 _newpos = other.gameObject.transform.position + _dirToPlayer;
                //other.gameObject.GetComponent<NavMeshAgent>().updateRotation=false;
                other.gameObject.GetComponent<NavMeshAgent>().acceleration = 5;
                other.gameObject.GetComponent<NavMeshAgent>().speed = 5;
                other.gameObject.GetComponent<NavMeshAgent>().SetDestination(_newpos);
            }*/

            if(other.name=="Lefty" && TurnedOn && _OfficeMechanics.NotInVent==true)
            {
                nightScript.Agro[0]+=Time.deltaTime;
            }
            if(other.name=="ScrapBaby" && TurnedOn && _OfficeMechanics.NotInVent==true)
            {
                nightScript.Agro[1]+=Time.deltaTime;
            }
            if(other.name=="SpringTrap" && TurnedOn && _OfficeMechanics.NotInVent==true)
            {
                nightScript.Agro[2]+=Time.deltaTime;
            }
            if(other.name=="MoltenFreddy" && TurnedOn && _OfficeMechanics.NotInVent==true)
            {
                nightScript.Agro[3]+=Time.deltaTime;
            }

            /// AGRO AND NOISE MECHANICS FOR OFFICE

            cointoss = Random.Range(1, 12);
            float noise = _OfficeMechanics.NoiseLevel+_OfficeMechanics.NoiseSilent+_OfficeMechanics.NoiseTask;
            if(nightScript.Agro[0]>2)
            {
                nightScript.Agro[0]=0;
                Debug.Log("FLASHED LEFTY :P");
                nightScript.AnimatronicsRST[0]=true;
                nightScript.AnimatronicsAI[0].updateRotation=false;
                nightScript.AnimatronicsAI[0].speed=5;

                if(cointoss>noise)
                {
                    float distanceToPointA = Vector3.Distance(_Lefty.transform.position, _Room11.transform.position);
                    float distanceToPointB = Vector3.Distance(_Lefty.transform.position, _Room15.transform.position);

                    if(distanceToPointA < distanceToPointB)
                        nightScript.AnimatronicsPOS[0]=nightScript.Origin1;
                    else
                        nightScript.AnimatronicsPOS[0]=nightScript.Origin4;
                }
                else
                {
                    float distanceToPointA = Vector3.Distance(_Lefty.transform.position, _Room31.transform.position);
                    float distanceToPointB = Vector3.Distance(_Lefty.transform.position, _Room35.transform.position);

                    if(distanceToPointA < distanceToPointB)
                    {
                        nightScript.AnimatronicsPOS[0].y=3;nightScript.AnimatronicsPOS[0].x=1;
                    }
                    else
                    {
                        nightScript.AnimatronicsPOS[0].y=3;nightScript.AnimatronicsPOS[0].x=5;
                    }
                }
            }
            cointoss = Random.Range(1, 12);
            noise = _OfficeMechanics.NoiseLevel+_OfficeMechanics.NoiseSilent+_OfficeMechanics.NoiseTask;
            if(nightScript.Agro[1]>2)
            {
                nightScript.Agro[1]=0;
                Debug.Log("FLASHED BABYY :P");
                nightScript.AnimatronicsRST[1]=true;
                nightScript.AnimatronicsAI[1].updateRotation=false;
                nightScript.AnimatronicsAI[1].speed=5;

                if(cointoss>noise)
                {
                    float distanceToPointA = Vector3.Distance(_ScrapBaby.transform.position, _Room12.transform.position);
                    float distanceToPointB = Vector3.Distance(_ScrapBaby.transform.position, _Room14.transform.position);

                    if(distanceToPointA < distanceToPointB)
                        nightScript.AnimatronicsPOS[1]=nightScript.Origin2;
                    else
                        nightScript.AnimatronicsPOS[1]=nightScript.Origin3;
                }
                else
                {
                    float distanceToPointA = Vector3.Distance(_ScrapBaby.transform.position, _Room31.transform.position);
                    float distanceToPointB = Vector3.Distance(_ScrapBaby.transform.position, _Room35.transform.position);

                    if(distanceToPointA < distanceToPointB)
                    {
                        nightScript.AnimatronicsPOS[1].y=3;nightScript.AnimatronicsPOS[1].x=1;
                    }
                    else
                    {
                        nightScript.AnimatronicsPOS[1].y=3;nightScript.AnimatronicsPOS[1].x=5;
                    }
                }
            }
            cointoss = Random.Range(1, 12);
            noise = _OfficeMechanics.NoiseLevel+_OfficeMechanics.NoiseSilent+_OfficeMechanics.NoiseTask;
            if(nightScript.Agro[2]>2)
            {
                nightScript.Agro[2]=0;
                Debug.Log("FLASHED purple guy :P");
                nightScript.AnimatronicsRST[2]=true;
                nightScript.AnimatronicsAI[2].updateRotation=false;
                nightScript.AnimatronicsAI[2].speed=5;

                if(cointoss>noise)
                {
                    float distanceToPointA = Vector3.Distance(_SpringTrap.transform.position, _Room12.transform.position);
                    float distanceToPointB = Vector3.Distance(_SpringTrap.transform.position, _Room14.transform.position);

                    if(distanceToPointA < distanceToPointB)
                        nightScript.AnimatronicsPOS[2]=nightScript.Origin2;
                    else
                        nightScript.AnimatronicsPOS[2]=nightScript.Origin3;
                }
                else
                {
                    float distanceToPointA = Vector3.Distance(_SpringTrap.transform.position, _Room31.transform.position);
                    float distanceToPointB = Vector3.Distance(_SpringTrap.transform.position, _Room35.transform.position);

                    if(distanceToPointA < distanceToPointB)
                    {
                        nightScript.AnimatronicsPOS[2].y=3;nightScript.AnimatronicsPOS[2].x=1;
                    }
                    else
                    {
                        nightScript.AnimatronicsPOS[2].y=3;nightScript.AnimatronicsPOS[2].x=5;
                    }
                }
            }
            cointoss = Random.Range(1, 12);
            noise = _OfficeMechanics.NoiseLevel+_OfficeMechanics.NoiseSilent+_OfficeMechanics.NoiseTask;
            if(nightScript.Agro[3]>2)
            {
                nightScript.Agro[3]=0;
                Debug.Log("FLASHED moleten ber :P");
                nightScript.AnimatronicsRST[3]=true;
                nightScript.AnimatronicsAI[3].updateRotation=false;
                nightScript.AnimatronicsAI[3].speed=5;
                
                if(cointoss>noise)
                {
                    float distanceToPointA = Vector3.Distance(_MoltenFreddy.transform.position, _Room11.transform.position);
                    float distanceToPointB = Vector3.Distance(_MoltenFreddy.transform.position, _Room15.transform.position);

                    if(distanceToPointA < distanceToPointB)
                        nightScript.AnimatronicsPOS[3]=nightScript.Origin1;
                    else
                        nightScript.AnimatronicsPOS[3]=nightScript.Origin4;
                }
                else
                {
                    float distanceToPointA = Vector3.Distance(_MoltenFreddy.transform.position, _Room31.transform.position);
                    float distanceToPointB = Vector3.Distance(_MoltenFreddy.transform.position, _Room35.transform.position);

                    if(distanceToPointA < distanceToPointB)
                    {
                        nightScript.AnimatronicsPOS[3].y=3;nightScript.AnimatronicsPOS[3].x=1;
                    }
                    else
                    {
                        nightScript.AnimatronicsPOS[3].y=3;nightScript.AnimatronicsPOS[3].x=5;
                    }
                }
            }
        }
    }

    void Update()
    {
        if(flashlightUsageTime>flashlightTime && set==false)
        {
            set=true;
            lightController.enabled = false;
            TurnedOn=false;
            Debug.Log("No More Battery");
        }
        if(Input.GetKeyDown(interactKey) && TurnedOn==false && set==false)
        {
            _audioSource.PlayOneShot(turnOnSound);
            lightController.enabled = true;
            TurnedOn=true;
            Debug.Log("Flashlight on");
        }
        else
        if(Input.GetKeyDown(interactKey) && TurnedOn==true  && set==false)
        {
            _audioSource.PlayOneShot(turnOffSound);
            lightController.enabled = false;
            TurnedOn=false;
            Debug.Log("Flashlight off");
        }
        else
        if(Input.GetKeyDown(interactKey) && TurnedOn==false && flashlightUsageTime>flashlightTime)
        {
            _audioSource.PlayOneShot(NoBattery);
            lightController.enabled = false;
            TurnedOn=false;
            Debug.Log("Flashlight cant turn on");
        }
        if (TurnedOn)
        {
            flashlightUsageTime += Time.deltaTime;
        }
    }
}
