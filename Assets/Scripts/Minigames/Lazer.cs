﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lazer : MonoBehaviour
{
    public AudioSource warningBeeper;
    public AudioSource targetBeeper;
    public Control myController;
    private const float DESTRUCTION_TIMER_BASE = 7f;
    private const float CHECK_TIMER_BASE = 3f;

    public float changeCheck = CHECK_TIMER_BASE;

    public static explicit operator Lazer(GameObject v)
    {
        throw new System.NotImplementedException();
    }

    public float timeUntilDestruction = DESTRUCTION_TIMER_BASE; 
    public GameObject PlusButton;
    public GameObject MinusButton;
    public float movePerClick;
    Animator anim;
    public const float RIGHT_TARGET = 1;
    public const float LEFT_TARGET = 0;
    public Alienshipcontroller alienship;
    private bool alienAttack = false;
    private bool Destroied = false;

    private int CurrentTarget  = -1;

    private LazerCar lazerCar;

    private void Awake()
    {
        foreach(LazerCar car in FindObjectsOfType<LazerCar>())
        {
            if(!car.HasGame())
            {
                car.SetGame(this);
                lazerCar = car;
                alienship = lazerCar.alienshipcontroller;
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        myController = gameObject.GetComponent<Control>();

    }

    void Update()
    {
        if (!myController.Broken)
        {
            workingUpdate();
        }
        else
        {
            brokenUpdate();
        }
    }

    /// <summary>
    ///  TODO if you want any logical while broken run it in here
    /// </summary>
    public void brokenUpdate()
    {

    }


    // Update is called once per frame
    private void workingUpdate()
    {
        if(Input.GetKeyDown(KeyCode.Mouse0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                if(hit.transform == PlusButton.transform)
                {
                    anim.SetTrigger("Plus Button");
                    float move = Mathf.Clamp(anim.GetFloat("Hand") + movePerClick,0,1);
                    if (move < 0.07f)
                    {
                        move = 0f;
                    }  
                    anim.SetFloat("Hand",move);
                }
                if (hit.transform == MinusButton.transform)
                {
                    anim.SetTrigger("Minus Button");


                    float move = Mathf.Clamp(anim.GetFloat("Hand") - movePerClick,0,1);
                    if (move > 0.93f)
                    {
                        move = 1f;
                    }  
                    anim.SetFloat("Hand", move);
                }
            }
        }
        else
        {
            float move = anim.GetFloat("Hand");
            if (move > 0.5f)
            {
                move -= 0.03f * Time.deltaTime;
            }
            else 
            {
                move += 0.03f * Time.deltaTime;
            }
            anim.SetFloat("Hand", move);
            
        }

        if (alienAttack)
        {
            AlienAttackUpdate();
        }
        else if (Destroied)
        {
            
        }
        else
        {
            WaitingOnAliens();
        }
    }


    private void AlienAttackUpdate()
    {
        timeUntilDestruction -= Time.deltaTime;
        if (timeUntilDestruction <= 0)
        {
            myController.loseMiniGame();
            DestoryControl();
            Destroied = true;
            anim.SetBool("Alien Left", true);
            anim.SetBool("Alien Right", true);
            anim.SetFloat("Alien Urgency", 12f);
            return;
        }

        anim.SetFloat("Alien Urgency", DESTRUCTION_TIMER_BASE / timeUntilDestruction);

        if (Mathf.Abs (anim.GetFloat("Hand") - CurrentTarget) < 0.06f)
        {
            anim.SetTrigger("Alien Targeted");
            resolveAlienAttack();
        }
    }


    private void WaitingOnAliens()
    {
        changeCheck -= Time.deltaTime;
        if (changeCheck <= 0f)
        {
            CurrentTarget = Random.Range(-1,2);

            // randomly no aliens
            if (CurrentTarget == -1)
            {
                changeCheck = CHECK_TIMER_BASE;
                return;
            }
            else
            {
                timeUntilDestruction = DESTRUCTION_TIMER_BASE;
                alienAttack = true;

                alienship.alienEnter(CurrentTarget);
                if (CurrentTarget == 0)
                {
                    anim.SetBool("Alien Left", true);
                    anim.SetFloat("Alien Urgency", 1f);
                    anim.SetBool("Alien Right", false);
                }
                else if (CurrentTarget == 1)
                {
                    anim.SetBool("Alien Right", true);
                    anim.SetFloat("Alien Urgency", 1f);
                    anim.SetBool("Alien Left", false);
                }
            }
        }
    }


    private void resolveAlienAttack()
    {
        changeCheck = CHECK_TIMER_BASE;
        anim.SetBool("Alien Left", false);
        anim.SetBool("Alien Right", false);
        anim.SetFloat("Alien Urgency", 1f);
        alienship.alienExit();
        alienAttack = false;

        targetBeeper.Play();
        lazerCar.ShootAlien();
    }


    public void DestoryControl()
    {
        
    }

    public float GetLeftOrRight()
    {
        return anim.GetFloat("Hand");
    }

    public void Beep()
    {
        if (!myController.Broken)
        {
            warningBeeper.Play();    
        }
    }
}
