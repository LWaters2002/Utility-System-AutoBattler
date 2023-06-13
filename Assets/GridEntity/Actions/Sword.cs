using System.Collections;
using UnityEngine;

public class Sword : MonoBehaviour
{
    public void PlayAnimation(string animationName, System.Action callback)
    {
        StartCoroutine(IEPlayAnimation(animationName, callback));
    }

    IEnumerator IEPlayAnimation(string animationName, System.Action callback)
    {
        Animator animator = GetComponentInChildren<Animator>();
        animator.Play(animationName);

        float length = animator.GetCurrentAnimatorStateInfo(0).length;

        yield return new WaitForSeconds(length);

        callback?.Invoke();

        Destroy(gameObject);
    }

}
