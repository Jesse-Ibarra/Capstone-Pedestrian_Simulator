using UnityEngine;

public class KATAnimSync : MonoBehaviour
{
    public Animator animator;
    public Transform rootTransform;

    private Vector3 lastPosition;
    private float smoothSpeed = 0f;

    private float smoothedForward = 0f;
    private float smoothedHorizontal = 0f;

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

        Vector3 worldDelta = (rootTransform.position - lastPosition);

        // 🛠️ NEW: Deadzone world movement
        if (worldDelta.magnitude < 0.02f) 
            worldDelta = Vector3.zero;

        Vector3 localDelta = rootTransform.InverseTransformDirection(worldDelta);

        float rawForward = localDelta.z / Time.deltaTime;
        float rawHorizontal = localDelta.x / Time.deltaTime;

        smoothedForward = Mathf.Lerp(smoothedForward, rawForward, Time.deltaTime * 5f);
        smoothedHorizontal = Mathf.Lerp(smoothedHorizontal, rawHorizontal, Time.deltaTime * 5f);

        if (Mathf.Abs(smoothedForward) < 0.05f)
            smoothedForward = 0f;
        if (Mathf.Abs(smoothedHorizontal) < 0.05f)
            smoothedHorizontal = 0f;

        float scaledForward = Mathf.Clamp(smoothedForward / 2.5f, -2f, 2f);
        float scaledHorizontal = Mathf.Clamp(smoothedHorizontal / 2.5f, -2f, 2f);

        animator.SetFloat("MoveSpeed", Mathf.Abs(scaledForward)); 
        animator.SetFloat("Forward", scaledForward);
        animator.SetFloat("Horizontal", scaledHorizontal);

        lastPosition = rootTransform.position;
    }

}
