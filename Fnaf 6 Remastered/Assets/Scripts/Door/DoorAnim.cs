using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorAnim : MonoBehaviour
{
    public bool Open=false;
    private float PlayedAnim=0;
    private Animator _animator;
    private AudioSource _audioSource;

    void Start()
    {
        _animator = GetComponent<Animator>();
        _audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if(Open==true && PlayedAnim==0f)
            PlayedAnim=1f;
        if(Open==true && PlayedAnim==1f)
        {
            _animator.Play("Door_Anim");
            _audioSource.Play();
            PlayedAnim=2f;
        }
        if(Open==false && PlayedAnim==2f)
        {
            _animator.Play("Door_Anim1");
            _audioSource.Play();
            PlayedAnim=0f;
        }
        
    }
}
