using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using umi3d.common.userCapture;
using umi3d.common;
using umi3d.edk;
using umi3d.edk.userCapture;
using System.Linq;

public class SkeletonAnimation : MonoBehaviour
{
    public GameObject SkeletonPrefab;

    Dictionary<string, GameObject> skeletons = new Dictionary<string, GameObject>();
    Dictionary<string, Animator> animators = new Dictionary<string, Animator>();

    // Start is called before the first frame update
    void Start()
    {
        UMI3DEmbodimentManager.Instance.NewEmbodiment.AddListener(SkeletonInstantiation);
        UMI3DEmbodimentManager.Instance.UpdateEvent.AddListener(UpdateBone);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SkeletonInstantiation(UMI3DAvatarNode node)
    {
        Vector3 userSize = UMI3DEmbodimentManager.Instance.embodimentSize[node.userId];
        GameObject skeleton = Instantiate(SkeletonPrefab, node.transform);
        skeletons.Add(node.userId, skeleton);
        skeleton.transform.localScale = userSize;
        animators.Add(node.userId, skeleton.GetComponentInChildren<Animator>());
    }

    void UpdateBone(UMI3DUserEmbodimentBone bone)
    {
        Animator userAnimator = animators[bone.userId];
        Transform transform = userAnimator.GetBoneTransform(BoneTypeConverter.Convert(bone.boneType).GetValueOrDefault());
        transform.localRotation = bone.spatialPosition.localRotation;
    }
}
