using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InterpolationManager : MonoBehaviour
{
    public bool updateTransform = false;
    public bool isInterpolating = false;

    public UnityEvent UpdateEvent;
    public UnityEvent InterpolationEvent;

    public void UpdateStatus()
    {
        updateTransform = !updateTransform;
        UpdateEvent.Invoke();
    }

    public void InterpolationStatus()
    {
        isInterpolating = !isInterpolating;
        InterpolationEvent.Invoke();
    }
}
