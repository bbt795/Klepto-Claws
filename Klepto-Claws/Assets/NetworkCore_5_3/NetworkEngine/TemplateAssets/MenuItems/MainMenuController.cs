﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{

    public GameObject creditsPanel;
    public GameObject mainPanel;
    public GameObject optionsPanel;

    // Start is called before the first frame update
    void Start()
    {
        string[] args = System.Environment.GetCommandLineArgs();
        //ArgDisplay.text = System.Environment.CommandLine;
        foreach (string a in args)
        {
            if (a.StartsWith("PORT_") || a.Contains("MASTER"))
            {
                //Load Wan scene.
                SceneManager.LoadScene(1);
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    public void WANConnect()
    {
        //Load Wan scene.
        SceneManager.LoadScene(1);
    }

    public void LANConnect()
    {
        //Load Lan Scene;
        SceneManager.LoadScene(2);
    }

    public void QuitMe()
    {
        Application.Quit();
    }

    public void CreditsPanel()
    {

        if (mainPanel.activeSelf == true)
        {

            mainPanel.SetActive(false);

        }

        creditsPanel.SetActive(true);

    }

    public void OptionsPanel()
    {

        if (mainPanel.activeSelf == true)
        {

            mainPanel.SetActive(false);

        }

        optionsPanel.SetActive(true);

    }

}
