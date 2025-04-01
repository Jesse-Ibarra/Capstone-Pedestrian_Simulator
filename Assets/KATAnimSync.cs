using UnityEngine;

public class KATAnimSync : MonoBehaviour
{
    public Animator animator;
    public Transform rootTransform;

    private Vector3 lastPosition;
    private float smoothSpeed = 0f;

    void Start()
    {
        if (rootTransform == null && transform.parent != null)
        {
            rootTransform = transform.parent;
        }

        lastPosition = rootTransform.position;
    }

    void Update()
    {
        if (animator == null || rootTransform == null)
            return;

        float rawSpeed = (rootTransform.position - lastPosition).magnitude / Time.deltaTime;

        // Clamp tiny speeds to 0 to prevent flickering
        if (rawSpeed < 0.05f)
            rawSpeed = 0f;

        // Smooth the speed using Lerp
        smoothSpeed = Mathf.Lerp(smoothSpeed, rawSpeed, Time.deltaTime * 10f); // 10f is smoothing factor

        // Normalize and clamp
        float scaledSpeed = Mathf.Clamp(smoothSpeed / 2.5f, 0f, 2f);

        animator.SetFloat("MoveSpeed", scaledSpeed);

        lastPosition = rootTransform.position;
    }
}
