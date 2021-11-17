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
using UnityEngine;

public class BindingActivation : MonoBehaviour
{
    public float HandDistActivation;

    Vector3 tempLocalPos;
    Quaternion tempLocalRot;
    UMI3DBinding tempBinding;
    ulong tempUserID;

    Transform bindingAnchor;
    Vector3 localPosOffset;
    Quaternion localRotOffset;

    bool activation = false;

    // Start is called before the first frame update
    void Start()
    {
        tempLocalPos = transform.localPosition;
        tempLocalRot = transform.localRotation;
    }

    // Update is called once per frame
    void Update()
    {
        if (activation)
        {
            GetComponent<UMI3DNode>().objectPosition.SetValue(transform.parent.InverseTransformPoint(bindingAnchor.TransformPoint(localPosOffset)));
            GetComponent<UMI3DNode>().objectRotation.SetValue(Quaternion.Inverse(transform.parent.rotation) * bindingAnchor.rotation * localRotOffset);
        }
    }

    public void UpdateBindingActivation(umi3d.edk.interaction.AbstractInteraction.InteractionEventContent content)
    {
        UMI3DTrackedUser user = content.user as UMI3DTrackedUser;
        uint bonetype = content.boneType;

        if (!activation)
        {
            if (!bonetype.Equals(BoneType.Head) && Vector3.Distance(transform.position, user.Avatar.skeletonAnimator.GetBoneTransform(bonetype.ConvertToBoneType().GetValueOrDefault()).transform.position) < HandDistActivation)
                return;

            bindingAnchor = user.Avatar.skeletonAnimator.GetBoneTransform(bonetype.ConvertToBoneType().GetValueOrDefault()).transform;

            activation = true;

            localPosOffset = bindingAnchor.InverseTransformPoint(transform.position);
            localRotOffset = Quaternion.Inverse(bindingAnchor.rotation) * transform.rotation;

            tempUserID = user.Id();

            tempBinding = new UMI3DBinding()
            {
                node = GetComponent<UMI3DNode>(),
                boneType = bonetype,
                isBinded = true,
                syncPosition = true,
                offsetPosition = localPosOffset,
                offsetRotation = localRotOffset
            };

            SetEntityProperty op = UMI3DEmbodimentManager.Instance.AddBinding(user.Avatar, tempBinding);

            Transaction transaction = new Transaction();
            transaction.AddIfNotNull(op);
            transaction.reliable = true;


            UMI3DServer.Dispatch(transaction);
        }
        else
        {
            if (!user.Id().Equals(tempUserID))
                return;

            activation = false;

            Transaction transaction = new Transaction();

            transaction.AddIfNotNull(UMI3DEmbodimentManager.Instance.RemoveBinding(user.Avatar, tempBinding));
            transaction.AddIfNotNull(GetComponent<UMI3DNode>().objectPosition.SetValue(tempLocalPos));
            transaction.AddIfNotNull(GetComponent<UMI3DNode>().objectRotation.SetValue(tempLocalRot));

            transaction.reliable = true;

            UMI3DServer.Dispatch(transaction);
        }
    }


}
