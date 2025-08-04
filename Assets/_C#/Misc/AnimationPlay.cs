using UnityEngine;

public class AnimationPlay : MonoBehaviour
{
    public bool angry;
    public bool standingGreeting;
    public bool salute;
    public bool armStretching;
    public bool happy;

    private Animator _anim;

    private void Awake()
    {
        _anim = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        if (angry)
        {
            _anim.SetTrigger("Angry");
        }
        else if (armStretching)
        {
            _anim.SetTrigger("Arm Stretching");
        }
        else if (standingGreeting)
        {
            _anim.SetTrigger("Greeting");
        }
        else if (salute)
        {
            _anim.SetTrigger("Salute");
        }
        else if (happy)
        {
            _anim.SetTrigger("Happy");
        }
    }
}
