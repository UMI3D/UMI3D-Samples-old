using inetum.unityUtils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using umi3d.common;
using umi3d.common.collaboration;
using umi3d.common.interaction;
using umi3d.common.userCapture;
using umi3d.edk;
using umi3d.edk.collaboration;
using umi3d.edk.userCapture;
using UnityEngine;
using UnityEngine.XR;
using static umi3d.edk.interaction.UMI3DForm;

public class AvatarManager : MonoBehaviour
{
    [SerializeField, EditorReadOnly]
    UMI3DResource Avatar;
    
    HashSet<UMI3DUser> Handled = new HashSet<UMI3DUser>();

    [System.Serializable]
    public class Bind
    {
        [ConstEnum(typeof(BoneType),typeof(uint))]
        public uint boneType;
        public string rigName;
        public Vector3 positionOffset;
        public Vector3 rotationOffset;
    }

    [System.Serializable]
    public class BindList
    {
        public List<Bind> binds;
    }

    [SerializeField, EditorReadOnly]
    bool bindRig;

    [SerializeField, EditorReadOnly]
    BindList binds;

    [SerializeField, EditorReadOnly]
    Vector3 positionOffset;

    [SerializeField, EditorReadOnly]
    Vector3 rotationOffset;
    // Start is called before the first frame update
    void Start()
    {
        UMI3DEmbodimentManager.Instance.NewEmbodiment.AddListener(NewAvatar);
    }

    void NewAvatar(UMI3DAvatarNode node)
    {
        if (UMI3DCollaborationServer.Collaboration.GetUser(node.userId) != null && !Handled.Contains(UMI3DCollaborationServer.Collaboration.GetUser(node.userId)))
        {
            Handled.Add(UMI3DCollaborationServer.Collaboration.GetUser(node.userId));
            StartCoroutine(_NewAvatar(UMI3DCollaborationServer.Collaboration.GetUser(node.userId)));
        }
    }

    IEnumerator _NewAvatar(UMI3DCollaborationUser user)
    {
        if (user == null) yield break;
        var wait = new WaitForFixedUpdate();

        UMI3DAvatarNode avatarnode = user.Avatar;

        while (avatarnode == null)
        {
            yield return wait;
            avatarnode = user.Avatar;
        }

        while (user.status.Equals(StatusType.READY))
        {
            yield return wait;
        }

        GameObject avatarModelnode = new GameObject("AvatarModel");
        avatarModelnode.transform.SetParent(avatarnode.transform);
        avatarModelnode.transform.localPosition = Vector3.zero;
        avatarModelnode.transform.localRotation = Quaternion.identity;

        UMI3DModel avatarModel = avatarModelnode.AddComponent<UMI3DModel>();

        avatarModel.objectModel.SetValue(Avatar);
        avatarModel.objectScale.SetValue(UMI3DEmbodimentManager.Instance.embodimentSize[avatarnode.userId]);

        List<Operation> ops = new List<Operation>();

        LoadEntity op = avatarModel.GetLoadEntity();
        ops.Add(op);
        BindingsTransactionManager.Instance.Dispatch(ops, true);

        StartCoroutine(Binding(avatarModel, avatarnode, user));
    }

    IEnumerator Binding(UMI3DModel avatarModel, UMI3DAvatarNode avatarnode, UMI3DUser user)
    {
        List<Operation> ops = new List<Operation>();

        SetEntityProperty op;
        if (bindRig)
        {
            foreach (Bind bind in binds.binds)
            {
                UMI3DBinding binding = new UMI3DBinding()
                {
                    boneType = bind.boneType,
                    rigName = bind.rigName,
                    offsetRotation = Quaternion.Euler(bind.rotationOffset),
                    offsetPosition = bind.positionOffset,
                    node = avatarModel,
                    isBinded = true,
                };

                //if (bind.boneType.Equals(BoneType.CenterFeet))
                //    binding.syncPosition = true;

                op = UMI3DEmbodimentManager.Instance.AddBinding(avatarnode, binding);
                ops.Add(op);
            }
        }
        BindingsTransactionManager.Instance.Dispatch(ops, true);
        yield break;
    }

}