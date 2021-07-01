using System.Collections;
using System.Collections.Generic;
using umi3d.edk;
using UnityEngine;

public class BindingsTransactionManager : MonoBehaviour
{
    static BindingsTransactionManager instance = null;

    public static BindingsTransactionManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType(typeof(BindingsTransactionManager)) as BindingsTransactionManager;
            }
            if (instance == null)
            {
                GameObject obj = new GameObject("BindingsTransactionManager");
                instance = obj.AddComponent<BindingsTransactionManager>();
            }
            return instance;
        }
    }

    public void Dispatch(List<Operation> ops, bool reliable)
    {
        var transaction = new Transaction();
        foreach (Operation op in ops)
        {
            if (op != null)
                transaction.Operations.Add(op);
        }

        if (transaction.Operations.Count > 0)
        {
            transaction.reliable = reliable;
            UMI3DServer.Dispatch(transaction);
        }
    }

    public void Dispatch(List<SetEntityProperty> ops, bool reliable)
    {
        var transaction = new Transaction();
        foreach (Operation op in ops)
        {
            if (op != null)
                transaction.Operations.Add(op);
        }

        if (transaction.Operations.Count > 0)
        {
            transaction.reliable = reliable;
            UMI3DServer.Dispatch(transaction);
        }
    }

    public void Dispatch(Operation op, bool reliable)
    {
        Dispatch(new List<Operation> { op }, reliable);
    }
}
