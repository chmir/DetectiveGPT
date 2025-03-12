using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    public Animator animator;

    //����: Normal
    public void BotNormalAnimation()
    {
        animator.Play("bot_normal");
    }

    //���: Joy
    public void BotJoyAnimation()
    {
        animator.Play("bot_joy");
    }

    //ȭ��: Anger
    public void BotAngerAnimation()
    {
        animator.Play("bot_anger");
    }

    //����: Sadness
    public void BotSadnessAnimation()
    {
        animator.Play("bot_sadness");
    }
}