using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;


public class PauseMenuController : MonoBehaviour
{
    [SerializeField] GameObject _pauseMenuUI;
    [SerializeField] GameObject _toggleButton;

    private Image _toggleButtonImage;
    private Sprite _playSprite;
    private Sprite _pauseSprite;

    private void Start()
    {
        _toggleButtonImage = _toggleButton.GetComponent<Image>();
        _playSprite = _toggleButton.GetComponent<ButtonImagesHolder>().PlayImage;
        _pauseSprite = _toggleButton.GetComponent<ButtonImagesHolder>().PauseImage;

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

    public void TogglePauseMenuUI()
    {
        if (_pauseMenuUI.activeSelf)
        {
            Time.timeScale = 1;
            _pauseMenuUI.SetActive(false);
            _toggleButtonImage.sprite = _pauseSprite;
        }
        else
        {
            _toggleButtonImage.sprite = _playSprite;
            Time.timeScale = 0;
            _pauseMenuUI.SetActive(true);
        }
    }

    public void Resume()
    {
        Time.timeScale = 1;
        _pauseMenuUI.SetActive(false);
        _toggleButtonImage.sprite = _pauseSprite;
    }

}
