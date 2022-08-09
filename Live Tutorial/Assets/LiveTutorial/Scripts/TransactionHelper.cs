using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using umi3d.edk;

public class TransactionHelper : MonoBehaviour
{
    static TransactionHelper instance = null;

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

    public float updatesPerSec = 15f;
    public System.Random rand;

    public void Dispatch(List<Operation> ops, bool reliable)
    {
        Transaction transaction = new Transaction();
        transaction.reliable = reliable;
        transaction.AddIfNotNull(ops);
        transaction.Dispatch();
    }

    public void Dispatch(Operation op, bool reliable)
    {
        Transaction transaction = new Transaction();
        transaction.reliable = reliable;
        transaction.AddIfNotNull(op);
        transaction.Dispatch();
    }

    private void Awake()
    {
        rand = new System.Random();
    }
}
