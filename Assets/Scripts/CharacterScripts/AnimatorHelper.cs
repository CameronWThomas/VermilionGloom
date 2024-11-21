using UnityEngine;

public static class AnimatorHelper
{
    public static bool Dead(this Animator animator) => animator.GetBool("dead");
    public static void Dead(this Animator animator, bool value) => animator.SetBool("dead", value);

    public static bool Conversing(this Animator animator) => animator.GetBool("conversing");
    public static void Conversing(this Animator animator, bool value) => animator.SetBool("conversing", value);

    public static Animator GetAnimator(this Component component) => component.GetComponent<Animator>();
}