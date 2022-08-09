using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SolarSystemHelper : MonoBehaviour
{
    static SolarSystemHelper instance = null;

    public float time_speed = 1;


    public static SolarSystemHelper Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType(typeof(SolarSystemHelper)) as SolarSystemHelper;
            }
            if (instance == null)
            {
                GameObject obj = new GameObject("SolarSystemHelper");
                instance = obj.AddComponent<SolarSystemHelper>();
            }
            return instance;
        }
    }
}
