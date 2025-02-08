using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class SuperSceneManager : MonoBehaviour
{
    public void JoinGame()
{
    UnityEngine.SceneManagement.SceneManager.LoadScene("Main1");
        //ABRE ESCENA MAIN 1
}

public void Exit()
{
    Application.Quit();
        //CIERRA APLICACIÓN
}

}