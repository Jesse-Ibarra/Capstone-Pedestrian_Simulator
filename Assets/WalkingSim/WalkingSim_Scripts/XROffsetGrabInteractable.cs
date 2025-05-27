using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
public class XROffsetGrabInteractable : XRGrabInteractable
{
    private Vector3 initialAttachLocalPosition;
    private Quaternion initialAttachLocalRotation;
    // Start is called before the first frame update
    void Start()
    {
        if (!attachTransform)
        {
            GameObject grab = new GameObject("Grab Pivot");
            grab.transform.SetParent(transform, false);
            attachTransform = grab.transform;
        }
        initialAttachLocalPosition = attachTransform.localPosition;
        initialAttachLocalRotation = attachTransform.localRotation;
    }
    

    // Soon to be depricated OnSelectEntering will have a new args
    protected override void OnSelectEntering(XRBaseInteractor interactor)
    {
        if (interactor is XRBaseInteractor)
        {
            attachTransform.position = interactor.transform.position;
            attachTransform.rotation = interactor.transform.rotation;
        }
        else 
        {
            attachTransform.localPosition = initialAttachLocalPosition;
            attachTransform.localRotation = initialAttachLocalRotation;
        }
        base.OnSelectEntering(interactor);
    }
}
