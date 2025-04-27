using UnityEngine;

public class AnimatorIKWeightLock : MonoBehaviour
{
    public Animator animator;

    void OnAnimatorIK(int layerIndex)
    {
        if (animator == null) return;

        // Full control to IK hands
        animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
        animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1);

        animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
        animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);
    }
}