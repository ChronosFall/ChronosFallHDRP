using UnityEngine;

public class TestAnimationScript : MonoBehaviour
{
    Animator animator;
    void Start()
    {
        this.animator = GetComponent<Animator>();
    }

    void Update()
    {
        animator.SetFloat("Speed", Input.GetAxis("Vertical"));
    }
}
