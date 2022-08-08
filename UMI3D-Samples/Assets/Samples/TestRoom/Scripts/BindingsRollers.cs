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
using umi3d.edk;
using umi3d.edk.userCapture;
using umi3d.common.userCapture;
using umi3d.edk.collaboration;
using UnityEngine;

public class BindingsRollers : MonoBehaviour
{
    public UMI3DModel VehicleModel;
    public Transform Center;
    public Transform LeftRoller;
    public Transform RightRoller;

    public int UpdateFPS = 5;

    public float MovementRadius;
    public float MovementSpeed;

    public SimpleModificationListener Listener;

    private UMI3DTrackedUser tempUser;
    private List<UMI3DBinding> rollerBindings = new List<UMI3DBinding>();

    private float angle;
    private Coroutine updateCoroutine;

    private Vector3 ResetPosition;
    private Quaternion ResetRotation;

    // Start is called before the first frame update
    void Start()
    {
        Listener.SetNodes.AddListener(() => Listener.RemoveNode(VehicleModel));
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void BindRollers(umi3d.edk.interaction.AbstractInteraction.InteractionEventContent content)
    {
        if (tempUser != null)
            return;

        tempUser = content.user as UMI3DTrackedUser;

        ResetPosition = tempUser.Avatar.transform.localPosition;
        ResetRotation = tempUser.Avatar.transform.localRotation;

        UMI3DBinding LeftBinding = new UMI3DBinding()
        {
            node = LeftRoller.GetComponent<UMI3DNode>(),
            boneType = BoneType.LeftAnkle,
            isBinded = true,
            syncPosition = true,
            syncRotation = true,
            freezeWorldScale = true,
            offsetRotation = Quaternion.Euler(0, 80.7f, 0),
            offsetPosition = new Vector3(0.034f, 0, 0.065f)
        };

        UMI3DBinding RightBinding = new UMI3DBinding()
        {
            node = RightRoller.GetComponent<UMI3DNode>(),
            boneType = BoneType.RightAnkle,
            isBinded = true,
            syncPosition = true,
            syncRotation = true,
            freezeWorldScale = true,
            offsetRotation = Quaternion.Euler(0, 94.42f, 0),
            offsetPosition = new Vector3(-0.034f, 0, 0.065f)
        };

        rollerBindings.Add(LeftBinding);
        rollerBindings.Add(RightBinding);

        SetEntityProperty op = UMI3DEmbodimentManager.Instance.UpdateBindingList(tempUser.Avatar, rollerBindings);

        Transaction transaction = new Transaction();
        transaction.AddIfNotNull(op);
        transaction.reliable = true;

        UMI3DServer.Dispatch(transaction);

        UMI3DEmbodimentManager.Instance.VehicleEmbarkment(tempUser, VehicleModel);

        StartVehicleInterpolation();

        updateCoroutine = StartCoroutine(UpdateInterpolation());
        StartCoroutine(MoveAroundTheScene());
    }

    public void UnbindRollers()
    {
        if (tempUser == null)
            return;

        Transaction transaction = new Transaction();
        transaction.reliable = true;

        transaction.AddIfNotNull(UMI3DEmbodimentManager.Instance.UpdateBindingList(tempUser.Avatar, new List<UMI3DBinding>()));

        UMI3DServer.Dispatch(transaction);
        tempUser = null;
        rollerBindings = new List<UMI3DBinding>();
    }

    IEnumerator MoveAroundTheScene()
    {
        yield return new WaitForSeconds(1);

        float startAngle = Vector3.Angle(Vector3.forward, VehicleModel.transform.position - Center.position) * Mathf.Deg2Rad;

        angle = startAngle;

        while (angle < 2*Mathf.PI + startAngle)
        {
            angle += MovementSpeed * Time.deltaTime;

            Vector3 offset = new Vector3(Mathf.Sin(angle), 0, Mathf.Cos(angle)) * MovementRadius;

            VehicleModel.transform.position = Center.position + offset + new Vector3(0, VehicleModel.transform.position.y, 0);
            VehicleModel.transform.rotation = Quaternion.Euler(0, Mathf.Atan2(Mathf.Sin(angle), Mathf.Cos(angle)) * (180 / Mathf.PI), 0) * Quaternion.Euler(0, 90, 0);

            yield return new WaitForEndOfFrame();
        }

        StopVehicleInterpolation();
        StopCoroutine(updateCoroutine);

        yield return new WaitForSeconds(1);

        UMI3DEmbodimentManager.Instance.VehicleEmbarkment(tempUser, 0, default, default, UMI3DEmbodimentManager.Instance.EmbodimentsScene, default, ResetPosition, ResetRotation);

        UnbindRollers();
    }

    void StartVehicleInterpolation()
    {
        StartInterpolationProperty startPos = new StartInterpolationProperty()
        {
            users = UMI3DCollaborationServer.Collaboration.UsersSet(),
            property = VehicleModel.objectPosition.propertyId,
            entityId = VehicleModel.Id(),
            startValue = VehicleModel.objectPosition.GetValue()
        };

        StartInterpolationProperty startRot = new StartInterpolationProperty()
        {
            users = UMI3DCollaborationServer.Collaboration.UsersSet(),
            property = VehicleModel.objectRotation.propertyId,
            entityId = VehicleModel.Id(),
            startValue = VehicleModel.objectRotation.GetValue()
        };

        Transaction tr = new Transaction();
        tr.AddIfNotNull(startPos);
        tr.AddIfNotNull(startRot);
        tr.reliable = true;

        UMI3DServer.Dispatch(tr);
    }

    IEnumerator UpdateInterpolation()
    {
        while (true)
        {
            var setEntityPos = VehicleModel.objectPosition.SetValue(VehicleModel.transform.localPosition);
            var setEntityRot = VehicleModel.objectRotation.SetValue(VehicleModel.transform.localRotation);

            if (setEntityPos == null && setEntityRot == null)
            {
                yield return new WaitForEndOfFrame();
                continue;
            }

            Transaction tr = new Transaction();
            tr.AddIfNotNull(setEntityPos);
            tr.AddIfNotNull(setEntityRot);
            tr.reliable = true;

            UMI3DServer.Dispatch(tr);

            yield return new WaitForSeconds(1f / UpdateFPS);
        }
    }

    void StopVehicleInterpolation()
    {
        StopInterpolationProperty stopPos = new StopInterpolationProperty()
        {
            users = UMI3DCollaborationServer.Collaboration.UsersSet(),
            property = VehicleModel.objectPosition.propertyId,
            entityId = VehicleModel.Id(),
            stopValue = VehicleModel.objectPosition.GetValue()
        };

        StopInterpolationProperty stopRot = new StopInterpolationProperty()
        {
            users = UMI3DCollaborationServer.Collaboration.UsersSet(),
            property = VehicleModel.objectRotation.propertyId,
            entityId = VehicleModel.Id(),
            stopValue = VehicleModel.objectRotation.GetValue()
        };

        Transaction tr = new Transaction();
        tr.AddIfNotNull(stopPos);
        tr.AddIfNotNull(stopRot);
        tr.reliable = true;

        UMI3DServer.Dispatch(tr);
    }

}
