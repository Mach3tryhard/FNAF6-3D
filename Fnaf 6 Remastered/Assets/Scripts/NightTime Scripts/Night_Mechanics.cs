using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Night_Mechanics : MonoBehaviour
{
    [Header("Animatronics Control")]
    public Vector2 Origin1;
    public Vector2 Origin2;
    public Vector2 Origin3;
    public Vector2 Origin4;
    public NavMeshAgent _agent;
    public float TheyCanMove=0f;
    public float moveTime=8f;

    [Header("Animatronics Data")]
    public NavMeshAgent[] AnimatronicsAI = new NavMeshAgent[4];
    public GameObject[] AnimatronicsGO = new GameObject[4];
    public Vector2[] AnimatronicsPOS = new Vector2[4];
    public bool[] AnimatronicsRST = new bool[4];
    public bool[] AnimatronicChase = new bool[4];

    [Header("Agro Timer")]
    public float[] Agro = new float[4];

    void Start()
    {
        AnimatronicsPOS[0]=Origin1;
        AnimatronicsPOS[1]=Origin2;
        AnimatronicsPOS[2]=Origin3;
        AnimatronicsPOS[3]=Origin4;
    }

    void Update()
    {
        if(TheyCanMove>moveTime)
        {
            TheyCanMove=0f;
            for(int i=0;i<4;i++)
            {
                int cointoss = Random.Range(0, 3);
                if(AnimatronicsRST[i]==false && cointoss==0)
                {
                    if(AnimatronicsPOS[i].x==1 && AnimatronicsPOS[i].y==1)//1-1
                    {
                        int direction = Random.Range(0, 2);
                        if(direction==0)
                            AnimatronicsPOS[i].y+=1;
                        else
                            AnimatronicsPOS[i].x+=1;
                    }
                    else
                    if(AnimatronicsPOS[i].x==2 && AnimatronicsPOS[i].y==1)//1-2
                    {
                        int direction = Random.Range(0, 4);
                        if(direction==0)
                            AnimatronicsPOS[i].x+=1;
                        else
                        if(direction==1)
                            AnimatronicsPOS[i].x-=1;
                        else
                            AnimatronicsPOS[i].y+=1;
                    }
                    else
                    if(AnimatronicsPOS[i].x==3 && AnimatronicsPOS[i].y==1)//1-3
                    {
                        int direction = Random.Range(0, 2);
                        if(direction==0)
                            AnimatronicsPOS[i].x-=1;
                        else
                            AnimatronicsPOS[i].x+=1;
                    }
                    else
                    if(AnimatronicsPOS[i].x==4 && AnimatronicsPOS[i].y==1)//1-4
                    {
                        int direction = Random.Range(0, 4);
                        if(direction==0)
                            AnimatronicsPOS[i].x+=1;
                        else
                        if(direction==1)
                            AnimatronicsPOS[i].x-=1;
                        else
                            AnimatronicsPOS[i].y+=1;
                    }
                    else
                    if(AnimatronicsPOS[i].x==5 && AnimatronicsPOS[i].y==1)//1-5
                    {
                        int direction = Random.Range(0, 2);
                        if(direction==0)
                            AnimatronicsPOS[i].y+=1;
                        else
                            AnimatronicsPOS[i].x-=1;
                    }
                    else
                    if(AnimatronicsPOS[i].x==1 && AnimatronicsPOS[i].y==2)//2-1
                    {
                        int direction = Random.Range(0, 4);
                        if(direction==0)
                            AnimatronicsPOS[i].y+=1;
                        else
                        if(direction==1)
                            AnimatronicsPOS[i].y-=1;
                        else
                            AnimatronicsPOS[i].x+=1;
                    }
                    else
                    if(AnimatronicsPOS[i].x==2 && AnimatronicsPOS[i].y==2)//2-2
                    {
                        int direction = Random.Range(0, 4);
                        if(direction==0)
                            AnimatronicsPOS[i].y-=1;
                        else
                        if(direction==1)
                            AnimatronicsPOS[i].x-=1;
                        else
                            AnimatronicsPOS[i].y+=1;
                    }
                    else
                    if(AnimatronicsPOS[i].x==4 && AnimatronicsPOS[i].y==2)//2-4
                    {
                        int direction = Random.Range(0, 4);
                        if(direction==0)
                            AnimatronicsPOS[i].y-=1;
                        else
                        if(direction==1)
                            AnimatronicsPOS[i].x+=1;
                        else
                            AnimatronicsPOS[i].y+=1;
                    }
                    else
                    if(AnimatronicsPOS[i].x==5 && AnimatronicsPOS[i].y==2)//2-5
                    {
                        int direction = Random.Range(0, 4);
                        if(direction==0)
                            AnimatronicsPOS[i].y+=1;
                        else
                        if(direction==1)
                            AnimatronicsPOS[i].y-=1;
                        else
                            AnimatronicsPOS[i].x-=1;
                    }
                    else
                    if(AnimatronicsPOS[i].x==1 && AnimatronicsPOS[i].y==3)//3-1
                    {
                        int direction = Random.Range(0, 2);
                        if(direction==0)
                            AnimatronicsPOS[i].y-=1;
                        else
                            AnimatronicsPOS[i].x+=1;
                    }
                    else
                    if(AnimatronicsPOS[i].x==2 && AnimatronicsPOS[i].y==3)//3-2
                    {
                        int direction = Random.Range(0, 4);
                        if(direction==0)
                            AnimatronicsPOS[i].y-=1;
                        else
                            AnimatronicsPOS[i].x+=1;
                    }
                    else
                    if(AnimatronicsPOS[i].x==4 && AnimatronicsPOS[i].y==3)//3-4
                    {
                        int direction = Random.Range(0, 4);
                        if(direction==0)
                            AnimatronicsPOS[i].y-=1;
                        else
                            AnimatronicsPOS[i].x-=1;
                    }
                    else
                    if(AnimatronicsPOS[i].x==5 && AnimatronicsPOS[i].y==3)//3-5
                    {
                        int direction = Random.Range(0, 2);
                        if(direction==0)
                            AnimatronicsPOS[i].y-=1;
                        else
                            AnimatronicsPOS[i].x-=1;
                    }
                }
                if(AnimatronicChase[i]==false)
                {
                    Transform PositionCoordonates = transform.Find("Room"+AnimatronicsPOS[i].y+AnimatronicsPOS[i].x);
                    AnimatronicsAI[i].SetDestination(PositionCoordonates.position);
                }
            }
        }
        for(int i=0;i<4;i++)
        {
            if(AnimatronicsRST[i]==true)
            {
                Transform PositionCoordonates = transform.Find("Room"+AnimatronicsPOS[i].y+AnimatronicsPOS[i].x);
                AnimatronicsAI[i].SetDestination(PositionCoordonates.position);
                float distance = Vector3.Distance(AnimatronicsGO[i].GetComponent<Transform>().position, PositionCoordonates.position);
                if(distance<0.1f)
                {
                    AnimatronicsAI[i].updateRotation=true;
                    Debug.Log("Reset Ended ->"+i);
                    AnimatronicsRST[i]=false;
                }
            }
        }

        // ANIMATION PART
        for(int i=0;i<4;i++)
        {
            if(AnimatronicsAI[i].velocity != Vector3.zero)
            {
                AnimatronicsGO[i].GetComponent<Animator>().enabled=true;
            }
            else
            {
                AnimatronicsGO[i].GetComponent<Animator>().enabled=false;
            }
        }
        TheyCanMove+=Time.deltaTime;
        //agent.SetDestination(target.position);
    }
}
