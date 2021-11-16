using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleRotation : MonoBehaviour
{
    public enum Axes
    {
        X, Y, Z
    }

    public Axes Axe;

    public float Speed;

    // Update is called once per frame
    void Update()
    {
        switch (Axe)
        {
            case Axes.X:
                transform.Rotate(Speed * Time.deltaTime, 0, 0);
                break;
            case Axes.Y:
                transform.Rotate(0, Speed * Time.deltaTime, 0);
                break;
            case Axes.Z:
                transform.Rotate(0, 0, Speed * Time.deltaTime);
                break;
            default:
                break;
        }
    }
}
