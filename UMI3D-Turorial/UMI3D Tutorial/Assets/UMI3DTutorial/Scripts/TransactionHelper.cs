using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using umi3d.edk;
using umi3d.edk.collaboration;
using umi3d.edk.interaction;

public class TransactionHelper : MonoBehaviour
{
    static TransactionHelper instance = null;
    public float updatesPerSec = 15f;
    public System.Random rand;

    public static TransactionHelper Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType(typeof(TransactionHelper)) as TransactionHelper;
            }
            if (instance == null)
            {
                GameObject obj = new GameObject("TransactionHelper");
                instance = obj.AddComponent<TransactionHelper>();
            }
            return instance;
        }
    }


    public void RegisterNodeRec(UMI3DAbstractNode node)
    {
        List<Operation> ops = new List<Operation>();
        foreach (var n in node.gameObject.GetComponentsInChildren<UMI3DAbstractNode>())
            ops.Add(n.GetLoadEntity());

        foreach (var inter in node.GetComponentsInChildren<UMI3DInteractable>())
        {
            ops.Add(inter.GetLoadEntity());
        }

        foreach (var audio in node.GetComponentsInChildren<UMI3DAudioPlayer>())
        {
            ops.Add(audio.GetLoadEntity());
        }
        Dispatch(ops, true);
    }

    public void Dispatch(List<Operation> ops, bool reliable)
    {
        var transaction = new Transaction();
        transaction.reliable = reliable;
        transaction.AddIfNotNull(ops);
        transaction.Dispatch();
    }

    public void Dispatch(Operation op, bool reliable)
    {
        var transaction = new Transaction();
        transaction.reliable = reliable;
        transaction.AddIfNotNull(op);
        transaction.Dispatch();
    }

    private void Awake()
    {
        rand = new System.Random();
    }
}