using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AnimateHandOnInput : MonoBehaviour
{
    public InputActionProperty PinchAnimation;
    public InputActionProperty GripAnimation;
    public Animator handAnimator;
    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float TriggerValue = PinchAnimation.action.ReadValue<float>();
        handAnimator.SetFloat("Trigger", TriggerValue);

        float GripValue = GripAnimation.action.ReadValue<float>();
        handAnimator.SetFloat("Grip", GripValue);
    }
}
