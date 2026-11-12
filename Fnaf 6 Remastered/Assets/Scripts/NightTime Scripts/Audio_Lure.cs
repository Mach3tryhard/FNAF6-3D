using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Audio_Lure : MonoBehaviour
{
    private AudioSource _audioSource;
    private Night_Mechanics nightScript;
    public string _position = "no";
    public Vector2 TranslatePosition;
    private AudioSource _OfficeAudio;

    public AudioClip _NotFooled;
    public AudioClip _Fooled;

    void Start()
    {
        _OfficeAudio = GameObject.FindWithTag("Office").GetComponent<AudioSource>();
        nightScript = GameObject.FindWithTag("NightTime").GetComponent<Night_Mechanics>();
        _audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        int cointoss = Random.Range(0, 2);
        if (other.CompareTag("Animatronic") && cointoss==0)
        {
            _OfficeAudio.PlayOneShot(_Fooled);
            TranslatePosition.y = (int)_position[0]-48;
            TranslatePosition.x = (int)_position[1]-48;
            if(other.name=="Lefty")
            {
                nightScript.AnimatronicsRST[0]=true;
                nightScript.AnimatronicsPOS[0]=TranslatePosition;
            }
            if(other.name=="ScrapBaby")
            {
                nightScript.AnimatronicsRST[1]=true;
                nightScript.AnimatronicsPOS[1]=TranslatePosition;
            }
            if(other.name=="SpringTrap")
            {
                nightScript.AnimatronicsRST[2]=true;
                nightScript.AnimatronicsPOS[2]=TranslatePosition;
            }
            if(other.name=="MoltenFreddy")
            {
                nightScript.AnimatronicsRST[3]=true;
                nightScript.AnimatronicsPOS[3]=TranslatePosition;
            }
        }
        else
        {
            _OfficeAudio.PlayOneShot(_NotFooled);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        int cointoss = Random.Range(0, 2);
        if (other.CompareTag("Animatronic") && cointoss==0)
        {
            _OfficeAudio.PlayOneShot(_Fooled);
            TranslatePosition.y = (int)_position[0]-48;
            TranslatePosition.x = (int)_position[1]-48;
            if(other.name=="Lefty")
            {
                nightScript.AnimatronicsRST[0]=true;
                nightScript.AnimatronicsPOS[0]=TranslatePosition;
            }
            if(other.name=="ScrapBaby")
            {
                nightScript.AnimatronicsRST[1]=true;
                nightScript.AnimatronicsPOS[1]=TranslatePosition;
            }
            if(other.name=="SpringTrap")
            {
                nightScript.AnimatronicsRST[2]=true;
                nightScript.AnimatronicsPOS[2]=TranslatePosition;
            }
            if(other.name=="MoltenFreddy")
            {
                nightScript.AnimatronicsRST[3]=true;
                nightScript.AnimatronicsPOS[3]=TranslatePosition;
            }
        }
        else
        {
            _OfficeAudio.PlayOneShot(_NotFooled);
        }
    }
}
