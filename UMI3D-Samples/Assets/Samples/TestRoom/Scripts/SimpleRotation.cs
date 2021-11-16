/*
Copyright 2019 - 2021 Inetum
Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at
    http://www.apache.org/licenses/LICENSE-2.0
Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

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
