using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class FadeScript : MonoBehaviour
{
    [SerializeField] private Color _fadeInColor = Color.black;
    [SerializeField] private Color _fadeOutColor = Color.black;
    
    [SerializeField] private bool _fadeInOnStart = false;
    private Image _blackQuad;



    void Awake()
    {
        _blackQuad = GetComponent<Image>();
    }

    private void Start()
    {
        if (_fadeInOnStart)
        {            
            FadeIn(2f);
        }
    }

    public void FadeOut(float duration, int sceneIndex)
    {
        _fadeOutColor.a = 0;
        _blackQuad.color = _fadeOutColor;
        _blackQuad = GetComponent<Image>();
        _blackQuad.DOFade(1, duration).SetUpdate(true).OnComplete(() => 
        {
            Time.timeScale = 1;
            DOTween.KillAll();           
            SceneManager.LoadScene(sceneIndex); 
        });
    }

    public void FadeIn(float duration)
    {
        _fadeInColor.a = 1;
        _blackQuad.color = _fadeInColor;
        _blackQuad = GetComponent<Image>();
        _blackQuad.DOFade(0, duration);
    }
}
