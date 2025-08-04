using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TitleController : MonoBehaviour
{
    [SerializeField] private DOTweenAnimation _titleTween;

    private void Update()
    {
        if (Input.anyKeyDown)
        {
            _titleTween.DOPlay();
            this.enabled = false;
        }
    }
}
