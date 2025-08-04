using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ComponentHandler : MonoBehaviour
{
    private ThirdPersonController _controller;
    private PlayerInput _playerInput;
    private Attack _attack;

    private void Awake()
    {
        _controller = GetComponent<ThirdPersonController>();
        _attack = GetComponent<Attack>();

        _controller.enabled = false;
        _attack.enabled = false;
    }

    private void OnEnable()
    {
        Actions.GameStart += GameStart;
    }

    private void OnDisable()
    {
        Actions.GameStart -= GameStart;
    }

    private void GameStart()
    {
        _controller.enabled = true;
        _attack.enabled = true;
    }
}
