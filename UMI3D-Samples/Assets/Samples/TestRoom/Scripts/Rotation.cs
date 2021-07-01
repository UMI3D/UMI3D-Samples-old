/*
Copyright 2019 Gfi Informatique

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
using umi3d.edk;
using UnityEngine;

public class Rotation : MonoBehaviour
{
    public GameObject frameOfReference;

    bool isInit = false;

    Quaternion lastRotation;

    GameObject helper;

    /// <summary>
    /// Last time a user used UMI3DManipualtion linked to this class.
    /// </summary>
    float lastTimeUsed;

    /// <summary>
    /// If OnUserManipulation has not been triggered for this time, this class will consider that the user stopped using it.
    /// </summary>
    float timeToDetectStopUsing = .2f;

    private void Start()
    {
        helper = new GameObject("Rotation helper");
        if (frameOfReference != null)
            helper.transform.SetParent(frameOfReference.transform);

        helper.transform.rotation = Quaternion.identity;
    }

    /// <summary>
    /// This have on purpose to be call by a OnManipulated Event.
    /// </summary>
    /// <param name="user">The who performed the manipulation</param>
    /// <param name="trans">The position delta of the manipulation</param>
    /// <param name="rot">The rotation delta of the manipulation</param>
    public void OnUserManipulation(umi3d.edk.interaction.UMI3DManipulation.ManipulationEventContent content)
    {
        if (!isInit)
        {
            isInit = true;
            lastRotation = Quaternion.Euler(content.rotation.eulerAngles);
        } else
        {
            Quaternion newRotation = Quaternion.Euler(content.rotation.eulerAngles);

            Quaternion delta = Quaternion.Inverse(lastRotation) * newRotation;

            helper.transform.rotation = transform.rotation;
            helper.transform.localRotation *= delta;

            transform.rotation = helper.transform.rotation;

            lastRotation = newRotation;
        }

        lastTimeUsed = Time.time;
    }

    private void Update()
    {
        if (Time.time > lastTimeUsed + timeToDetectStopUsing)
        {
            isInit = false;
        }
    }
}
