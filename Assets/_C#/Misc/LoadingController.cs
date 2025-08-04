using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingController : MonoBehaviour
{
    public Canvas _canvas;
    public Animator _anim;

    private void Awake()
    {
        _canvas = GetComponent<Canvas>();
        _anim = GetComponent<Animator>();
        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            _canvas.enabled = false;
        }
    }

    public void StartLoading()
    {
        _canvas.enabled = true;
        _anim.SetTrigger("Start");
    }
}
