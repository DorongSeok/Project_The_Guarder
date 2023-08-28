using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    public Animator PlayerUpAnimator;
    public Animator PlayerDownAnimator;

    public string PlayerUpIdleAnimation = "";
    public string PlayerDownIdleAnimation = "";
    public string PlayerUpMoveAnimation = "";
    public string PlayerDownMoveAnimation = "";

    private string upNowMode;
    private string downNowMode;

    void Start()
    {
        //upNowMode = PlayerUpIdleAnimation;
        //downNowMode = PlayerDownIdleAnimation;
        upNowMode = PlayerUpMoveAnimation;
        downNowMode = PlayerDownMoveAnimation;
    }
    void FixedUpdate()
    {
        PlayerUpAnimator.Play(upNowMode);
        PlayerDownAnimator.Play(downNowMode);
    }

    public void ChangePlayerAnimation(string _animation)
    {
        if(_animation == "Idle")
        {
            Debug.Log("Idle");

            upNowMode = PlayerUpIdleAnimation;
            downNowMode = PlayerDownIdleAnimation;
        }
        else if (_animation == "Move")
        {
            Debug.Log("Move");

            upNowMode = PlayerUpMoveAnimation;
            downNowMode = PlayerDownMoveAnimation;
        }
        else
        {
            upNowMode = PlayerUpIdleAnimation;
            downNowMode = PlayerDownIdleAnimation;

            Debug.Log("ChangePlayerAnimation ERROR !!!");
        }
        PlayerUpAnimator.Play(upNowMode);
        PlayerDownAnimator.Play(downNowMode);
    }
}
