using UnityEngine;
using UnityEngine.AI;

public class VampireController : MonoBehaviour
{
    Animator animator;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
        animator = GetComponent<Animator>();
        animator.SetFloat("vampyness", 1);
    }

    // Update is called once per frame
    void Update()
    {
        var agent = GetComponent<NavMeshAgent>();
        if (agent.enabled)
            animator.SetFloat("speedPercent", agent.velocity.magnitude);
        else
            animator.SetFloat("speedPercent", 0f);
    }

    public void Suck(bool suckEm)
    {
        animator.SetBool("v-suck", suckEm);
    }

    public void Bless()
    {
        animator.SetTrigger("v-bless");
    }
}
