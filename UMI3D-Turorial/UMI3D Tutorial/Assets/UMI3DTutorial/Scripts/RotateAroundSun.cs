using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using umi3d.edk;
using umi3d.common;
using umi3d.edk.collaboration;

public class RotateAroundSun : MonoBehaviour
{
    public float orbit_speed;
    public float rotation_speed;
    public Transform sun;

    private void Start()
    {
        StartCoroutine(NetworkingUpdate());
        UMI3DCollaborationServer.Instance.OnUserActive.AddListener(CreateInterpolation);
    }

    void Update()
    {
        if (sun != null)
            transform.RotateAround(sun.position, Vector3.up, orbit_speed * SolarSystemHelper.Instance.time_speed * Time.deltaTime);
        transform.Rotate(Vector3.up, rotation_speed * SolarSystemHelper.Instance.time_speed * Time.deltaTime);
    }

    IEnumerator NetworkingUpdate()
    {
        double val = TransactionHelper.Instance.rand.NextDouble() * 1 / TransactionHelper.Instance.updatesPerSec;
        yield return new WaitForSeconds((float)val);

        UMI3DNode node = GetComponent<UMI3DNode>();

        while (true)
        {
            List<Operation> ops = new List<Operation>();
            ops.Add(node.objectPosition.SetValue(transform.localPosition));
            ops.Add(node.objectRotation.SetValue(transform.localRotation));
            TransactionHelper.Instance.Dispatch(ops, false);

            yield return new WaitForSeconds(1 / TransactionHelper.Instance.updatesPerSec);
        }
    }

    void CreateInterpolation(UMI3DUser user)
    {
        UMI3DNode node = GetComponent<UMI3DNode>();
        List<Operation> ops = new List<Operation>();
        ops.Add(new StartInterpolationProperty { entityId = node.Id(), property = UMI3DPropertyKeys.Position, startValue = (SerializableVector3)transform.localPosition, users = new HashSet<UMI3DUser> { user } });
        ops.Add(new StartInterpolationProperty { entityId = node.Id(), property = UMI3DPropertyKeys.Rotation, startValue = (SerializableVector4)transform.localRotation, users = new HashSet<UMI3DUser> { user } });
        TransactionHelper.Instance.Dispatch(ops, true);
    }
}
