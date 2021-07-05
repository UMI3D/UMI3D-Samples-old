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
    List<UMI3DResource> Avatars;
    [SerializeField, EditorReadOnly]
    List<string> avartChoice = new List<string>() { "default", "Woman", "Man" };

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
    List<BindList> binds;
    [SerializeField, EditorReadOnly]
    Vector3 positionOffset;
    [SerializeField, EditorReadOnly]
    Vector3 rotationOffset;
    // Start is called before the first frame update
    void Start()
    {
        UMI3DCollaborationServer.Instance.OnUserJoin.AddListener(NewUser);
        UMI3DEmbodimentManager.Instance.NewEmbodiment.AddListener(NewAvatar);
        UMI3DCollaborationServer.Instance.OnUserLeave.AddListener(DeleteUser);
    }

    // user, bonetype, transform

    Dictionary<ulong, Dictionary<ulong, Transform>> bones = new Dictionary<ulong, Dictionary<ulong, Transform>>();

    // user, model

    Dictionary<ulong, Transform> models = new Dictionary<ulong, Transform>();

    void NewUser(UMI3DUser user)
    {
        if (!bones.ContainsKey(user.Id()))
        {
            bones[user.Id()] = new Dictionary<ulong, Transform>();
            models[user.Id()] = null;
        }
    }

    void DeleteUser(UMI3DUser user)
    {
        var userId = user.Id();
        if (bones.ContainsKey(userId))
            bones.Remove(userId);
        if (models.ContainsKey(userId))
            models.Remove(userId);
        Handled.Remove(user);
    }

    private UMI3DResource ChooseAvatar(UMI3DUser user)
    {
        return Avatars[0];
    }

    private List<Bind> ChooseBinds(UMI3DUser user)
    {
        return binds[0].binds;
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
        UMI3DModel avatarModel = avatarModelnode.AddComponent<UMI3DModel>();

        avatarModel.objectModel.SetValue(ChooseAvatar(user));
        avatarModel.objectScale.SetValue(UMI3DEmbodimentManager.Instance.embodimentSize[avatarnode.userId]);

        List<Operation> ops = new List<Operation>();

        LoadEntity op = avatarModel.GetLoadEntity();
        ops.Add(op);
        BindingsTransactionManager.Instance.Dispatch(ops, true);

        StartCoroutine(Binding(avatarModel, avatarnode, user));
    }

    IEnumerator Binding(UMI3DModel avatarModel, UMI3DAvatarNode avatarnode, UMI3DUser user)
    {
        WaitForFixedUpdate wait = new WaitForFixedUpdate();

        List<Operation> ops = new List<Operation>();

        SetEntityProperty op;
        if (bindRig)
        {
            foreach (Bind bind in ChooseBinds(user))
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
                op = UMI3DEmbodimentManager.Instance.AddBinding(avatarnode, binding);
                ops.Add(op);
            }
        }
        BindingsTransactionManager.Instance.Dispatch(ops, true);
        yield break;
    }

}