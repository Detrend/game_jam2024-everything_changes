using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private int _firstSceneIndex;

    public void StartNewGame()
    {
        GameObject.FindWithTag("BlackQuad").GetComponent<FadeScript>().FadeOut(0.5f, _firstSceneIndex);
    }

    public void Quit()
    {
        Time.timeScale = 1.0f;

#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
	    Application.Quit();
#endif
    }

}
