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

[RequireComponent(typeof(UMI3DNode))]
public class BindingActivation : MonoBehaviour
{
    #region Fields

    [Tooltip("Distance to grab the object with hands")]
    public float HandDistActivation;

    /// <summary>
    /// Orignal local position of <see cref="node"/>.
    /// </summary>
    Vector3 tempLocalPos;

    /// <summary>
    /// Original local rotation of <see cref="node"/>.
    /// </summary>
    Quaternion tempLocalRot;

    /// <summary>
    /// Current binding when <see cref="node"/> is grabbed.
    /// </summary>
    UMI3DBinding tempBinding;

    /// <summary>
    /// Id of the current <see cref="UMI3DUser"/> who is grabbing <see cref="node"/>.
    /// </summary>
    ulong tempUserID;

    /// <summary>
    /// Transform of the bone which is currently grabbing <see cref="node"/>.
    /// </summary>
    Transform bindingAnchor;

    /// <summary>
    /// Local position offset between <see cref="node"/> and <see cref="bindingAnchor"/>.
    /// </summary>
    Vector3 localPosOffset;

    /// <summary>
    /// Local rotation offset between <see cref="node"/> and <see cref="bindingAnchor"/>.
    /// </summary>
    Quaternion localRotOffset;

    /// <summary>
    /// Is the object grabbed ?
    /// </summary>
    bool activation = false;

    /// <summary>
    /// Reference to the node grabbed.
    /// </summary>
    UMI3DNode node;

    #endregion

    void Start()
    {
        tempLocalPos = transform.localPosition;
        tempLocalRot = transform.localRotation;

        node = GetComponent<UMI3DNode>();
        Debug.Assert(node != null);
    }

    void Update()
    {
        if (activation)
        {
            node.objectPosition.SetValue(transform.parent.InverseTransformPoint(bindingAnchor.TransformPoint(localPosOffset)));
            node.objectRotation.SetValue(Quaternion.Inverse(transform.parent.rotation) * bindingAnchor.rotation * localRotOffset);
        }
    }

    /// <summary>
    /// Grabs the object or releases it if it was already hold by a user.
    /// </summary>
    /// <param name="content"></param>
    public void UpdateBindingActivation(umi3d.edk.interaction.AbstractInteraction.InteractionEventContent content)
    {
        UMI3DTrackedUser user = content.user as UMI3DTrackedUser;
        uint bonetype = content.boneType;

        if (!activation)
        {
            if (!bonetype.Equals(BoneType.Viewpoint) && Vector3.Distance(transform.position, user.Avatar.skeletonAnimator.GetBoneTransform(bonetype.ConvertToBoneType().GetValueOrDefault()).transform.position) > HandDistActivation)
                return;

            bindingAnchor = user.Avatar.skeletonAnimator.GetBoneTransform(bonetype.ConvertToBoneType().GetValueOrDefault()).transform;
            activation = true;

            localPosOffset = bonetype.Equals(BoneType.Viewpoint) ? Vector3.forward * 1.5f : bindingAnchor.InverseTransformPoint(transform.position);
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
