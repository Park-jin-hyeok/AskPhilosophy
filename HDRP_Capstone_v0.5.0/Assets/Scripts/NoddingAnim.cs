using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoddingAnim : MonoBehaviour
{
    //public Animator a_Animator;
    //public Animator s_Animator;
    public Animator animator;

    void Start()
    {
        //a_Animator = GetComponent<Animator>();
        //s_Animator = GetComponent<Animator>();
        animator = GetComponent<Animator>();

        AristoSleeping(true);
        AristoNodding(false);
        AristoThinking(false);
        SenekaSleeping(true);
        SenekaNodding(false);
        SenekaThinking(false);
    }

    public void AristoSleeping(bool turth) {
        //a_Animator.SetBool("is_sleeping", turth);
        animator.SetBool("is_sleeping", turth);

    }
    public void AristoNodding(bool turth)
    {
        //a_Animator.SetBool("is_nodding", turth);
        animator.SetBool("is_nodding", turth);
    }
    public void AristoThinking(bool turth)
    {
        //a_Animator.SetBool("is_thinking", turth);
        animator.SetBool("is_thinking", turth);
    }
    public void SenekaSleeping(bool turth)
    {
        //s_Animator.SetBool("is_sleeping", turth);
        animator.SetBool("is_sleeping", turth);
    }
    public void SenekaNodding(bool turth)
    {
        //s_Animator.SetBool("is_nodding", turth);
        animator.SetBool("is_nodding", turth);
    }
    public void SenekaThinking(bool turth)
    {
        //s_Animator.SetBool("is_thinking", turth);
        animator.SetBool("is_thinking", turth);
    }
}
