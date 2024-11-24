using UnityEngine;

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
        
    }
}
