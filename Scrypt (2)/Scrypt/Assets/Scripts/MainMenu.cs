using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    /*
    public void PlayGame()
    { 

    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    
    }

    public void QuitGame()

    {
        Debug.Log("QUIT");
        Application.Quit();
    }

    */

 
    private int mainMenuSceneIndex = 0;
    private int nextSceneIndex = 1;

    private void Start()
    {
        mainMenuSceneIndex = SceneManager.GetActiveScene().buildIndex;
    }

    public void PlayGame()
    {
        
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex + nextSceneIndex);
    }

    public void QuitGame()
    {
        Debug.Log("QUIT");
        Application.Quit();
    }

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene(mainMenuSceneIndex);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ReturnToMainMenu();
        }
    }
}









