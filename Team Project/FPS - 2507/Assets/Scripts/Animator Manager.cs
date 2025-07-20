using UnityEngine;

public class AnimatorManager : MonoBehaviour
{
    Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void HandleAnimatorValues(float horizontalMovement, float verticalMovement)
    {
        animator.SetFloat("Horizontal", horizontalMovement, 0.1f, Time.deltaTime);
        animator.SetFloat("Vertical", horizontalMovement, 0.1f, Time.deltaTime);
    }
}
