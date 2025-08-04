using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationRebinder : MonoBehaviour
{
    Animator _anim;

    private void Awake()
    {
        _anim = GetComponent<Animator>();
        _anim.Rebind();
    }
}
