using System.Collections;
using System.Collections.Generic;
using umi3d.edk;
using umi3d.edk.collaboration;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(UMI3DNode))]
public class InterpolationBehavior : MonoBehaviour
{
    public enum InterpolationType
    {
        Rotation,
        Translation,
        Scale
    }

    public InterpolationManager Manager;

    public InterpolationType Interpolation;

    public float Speed;

    public float Dist = 5;

    public int UpdateFPS = 5;

    public SimpleModificationListener Listener;

    private float tempValue = Mathf.PI/2;
    



    // Start is called before the first frame update
    void Start()
    {
        Listener.SetNodes.AddListener(() => Listener.RemoveNode(this.GetComponent<UMI3DNode>()));
        Manager.UpdateEvent.AddListener(() => ChangeUpdateStatus());
        Manager.InterpolationEvent.AddListener(() => ChangeInterpolation());
    }

    // Update is called once per frame
    void Update()
    {
        if (Interpolation.Equals(InterpolationType.Rotation))
            transform.Rotate(0, 0, Speed * Time.deltaTime);
        else if (Interpolation.Equals(InterpolationType.Translation))
        {
            tempValue += Speed * Time.deltaTime;
            transform.localPosition = new Vector3(0, 0, Dist * Mathf.Cos(tempValue));
        }
        else
        {
            tempValue += Speed * Time.deltaTime;
            transform.localScale = new Vector3(Dist * Mathf.Abs(Mathf.Cos(tempValue + Mathf.PI / 4)), Dist * Mathf.Abs(Mathf.Cos(tempValue + Mathf.PI / 4)), Dist * Mathf.Abs(Mathf.Cos(tempValue + Mathf.PI / 4)));
        }
    }

    public void ChangeUpdateStatus()
    {
        if (Manager.updateTransform)
            StartCoroutine(UpdateInterpolation());

    }

    public void ChangeInterpolation()
    {
        if (Manager.isInterpolating)
            StartCoroutine(StartInterpolation());
        else
            StartCoroutine(StopInterpolation());
    }

    IEnumerator UpdateInterpolation()
    {
        while (Manager.updateTransform)
        {
            SetEntityProperty setEntity;
            
            if (Interpolation.Equals(InterpolationType.Translation))
                setEntity = this.GetComponent<UMI3DAbstractNode>().objectPosition.SetValue(this.transform.localPosition);
            else if(Interpolation.Equals(InterpolationType.Rotation))
                setEntity = this.GetComponent<UMI3DAbstractNode>().objectRotation.SetValue(this.transform.localRotation);
            else
                setEntity = this.GetComponent<UMI3DAbstractNode>().objectScale.SetValue(this.transform.localScale);

            if (setEntity == null)
            {
                yield return new WaitForEndOfFrame();
                continue;
            }

            Transaction transaction = new Transaction();
            transaction.AddIfNotNull(setEntity);
            transaction.reliable = true;
            
            UMI3DServer.Dispatch(transaction);

            yield return new WaitForSeconds(1f/UpdateFPS);
        }   
    }

    IEnumerator StartInterpolation()
    {
        SetEntityProperty setEntity = null;

        while (setEntity == null)
        {
            if (Interpolation.Equals(InterpolationType.Translation))
                setEntity = this.GetComponent<UMI3DAbstractNode>().objectPosition.SetValue(this.transform.localPosition);
            else if (Interpolation.Equals(InterpolationType.Rotation))
                setEntity = this.GetComponent<UMI3DAbstractNode>().objectRotation.SetValue(this.transform.localRotation);
            else
                setEntity = this.GetComponent<UMI3DAbstractNode>().objectScale.SetValue(this.transform.localScale);

            yield return new WaitForEndOfFrame();
        }

        StartInterpolationProperty start = new StartInterpolationProperty()
        {
            users = new HashSet<UMI3DUser>(UMI3DCollaborationServer.Collaboration.Users),
            property = setEntity.property,
            entityId = setEntity.entityId,
            startValue = setEntity.value,
        };

        Transaction transaction = new Transaction();
        transaction.AddIfNotNull(start);
        transaction.reliable = true;

        UMI3DServer.Dispatch(transaction);
    }

    IEnumerator StopInterpolation()
    {
        SetEntityProperty setEntity = null;

        while (setEntity == null)
        {
            if (Interpolation.Equals(InterpolationType.Translation))
                setEntity = this.GetComponent<UMI3DAbstractNode>().objectPosition.SetValue(this.transform.localPosition);
            else if (Interpolation.Equals(InterpolationType.Rotation))
                setEntity = this.GetComponent<UMI3DAbstractNode>().objectRotation.SetValue(this.transform.localRotation);
            else
                setEntity = this.GetComponent<UMI3DAbstractNode>().objectScale.SetValue(this.transform.localScale);

            yield return new WaitForEndOfFrame();
        }

        StopInterpolationProperty stop = new StopInterpolationProperty()
        {
            users = new HashSet<UMI3DUser>(UMI3DCollaborationServer.Collaboration.Users),
            property = setEntity.property,
            entityId = setEntity.entityId,
            stopValue = setEntity.value,
        };

        Transaction transaction = new Transaction();
        transaction.AddIfNotNull(stop);
        transaction.reliable = true;

        UMI3DServer.Dispatch(transaction);
    }
}
