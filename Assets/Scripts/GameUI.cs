﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    public GroundCycler groundCycler;

    public Text speedText;
    public Text distanceText;
    public Text failuresText;
    public Train train;
    public Text gameOverDistance;
    public Text gameOverModulas;
    public Text gameOverSpeed;
    public GameObject pauseScreen;
    public GameObject gameoverScreen;
    public bool GameIsOver = false;
    private float distance = 0f;
    private const int possableMistakes = 5;
    private int Mistakes = 0;
    private static GameUI _instance;

    private int CurTarget;
    public int FirstCarMilestone;
    public int SubsequentCarMilestone;


    public static GameUI Instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = GameObject.FindObjectOfType<GameUI>();
            }

            return _instance;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        CurTarget = FirstCarMilestone;
    }


    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            PauseToggle();
        }
        if (Mistakes >= possableMistakes)
        {
            GameOver();
        }

        distance += (groundCycler.speed * Time.deltaTime);

        speedText.text = ("Speed: " + KPHConvert(groundCycler.speed).ToString("F2") + " KPH");
        distanceText.text = ("Distance: " + distance.ToString("F2") + " M");
        failuresText.text = ("Failures: " + Mistakes.ToString() + " / " + possableMistakes.ToString());


        if (distance > CurTarget)
        {
            TryToaddCar();
            CurTarget = SubsequentCarMilestone * train.cars.Count - 1;
        }
    }

    public float KPHConvert(float input)
    {
        return (input * 3.6f);
    }

    public void PauseToggle()
    {
        if(pauseScreen.activeSelf)
        {
            Time.timeScale = 1;
            pauseScreen.SetActive(false);
        }
        else
        {
            Time.timeScale = 0;
            pauseScreen.SetActive(true);
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void GameOver()
    {

        if (!GameIsOver)
        {
            Time.timeScale = 0;
            GameIsOver = true;
            gameOverDistance.text = ("Final Distance : " + distance.ToString("F2") + " M");
            gameOverModulas.text = ("Train Langth :  " + train.cars.Count + " cars");
            gameOverSpeed.text = ("Final Speed : " + KPHConvert(groundCycler.speed).ToString("F2") + " KPH");
            gameoverScreen.SetActive(true);            
        }





    }



    public void TryToaddCar()
    {
        train.RevealCar();
    }

    public void LoseMiniGame()
    {
        Mistakes++; 
    }

}
