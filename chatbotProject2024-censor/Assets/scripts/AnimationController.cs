using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    public Animator animator;

    //º¸Åë: Normal
    public void BotNormalAnimation()
    {
        animator.Play("bot_normal");
    }

    //±â»Ý: Joy
    public void BotJoyAnimation()
    {
        animator.Play("bot_joy");
    }

    //È­³²: Anger
    public void BotAngerAnimation()
    {
        animator.Play("bot_anger");
    }

    //½½ÇÄ: Sadness
    public void BotSadnessAnimation()
    {
        animator.Play("bot_sadness");
    }
}