using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public static SceneController instance;

    public Canvas _canvas;
    public Animator _anim;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }

        _canvas = GetComponentInChildren<Canvas>();
        _anim = GetComponentInChildren<Animator>();
        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            _canvas.enabled = false;
        }

        DontDestroyOnLoad(this);
    }

    public void MainMenu()
    {
        StartCoroutine(Loading(0));
    }

    public void LoadScene(int city)
    {
        StartCoroutine(Loading(city));
    }

    public void RestartScene()
    {
        StartCoroutine(Loading(SceneManager.GetActiveScene().buildIndex));
    }

    IEnumerator Loading(int city)
    {
        _canvas.enabled = true;
        _anim.SetTrigger("Start");
        yield return new WaitForSeconds(5f);
        
        // Reset any game state before loading new scene
        Actions.GameStart = null;
        
        SceneManager.LoadScene(city);
        SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().buildIndex);
        
        _anim.Play("LoadingEnd");
        yield return new WaitForSeconds(3f);
        
        if (city != 0) // Don't invoke GameStart for MainMenu
        {
            Actions.GameStart?.Invoke();
        }
    }
}
