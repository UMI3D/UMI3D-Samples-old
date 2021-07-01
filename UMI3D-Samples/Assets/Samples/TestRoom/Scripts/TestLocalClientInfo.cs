using System.Collections;
using System.Collections.Generic;
using System.Linq;
using umi3d.edk.collaboration;
using umi3d.edk.interaction;
using UnityEngine;
using WebSocketSharp;

public class TestLocalClientInfo : MonoBehaviour
{
    public string value;
    public bool tryToWrite = false;
    private bool oldWriteValue;

    void Start()
    {
        oldWriteValue = tryToWrite;
        UMI3DApi.receiveLocalInfoListener.AddListener((key, user, data) => { if (key == "testdata") value = System.Text.Encoding.Default.GetString(data); });
        //byte[] responseData = System.Text.Encoding.Default.GetBytes(value);
        UMI3DApi.sendLocalInfoListener.AddListener((key, user, response) => { if (key == "testdata") response.WriteContent(System.Text.Encoding.Default.GetBytes(value)); });
    }

    void Update()
    {
        if(tryToWrite != oldWriteValue)
        {
            UMI3DCollaborationUser user = UMI3DCollaborationServer.Collaboration.Users.FirstOrDefault();
            if (LocalInfoParameter.userResponses.ContainsKey((user, "testdata")) && LocalInfoParameter.userResponses[(user, "testdata")].Item2)
            {
                Debug.Log(LocalInfoParameter.userResponses[(user, "testdata")]);
                UMI3DCollaborationServer.ForgeServer.SendData(user.networkPlayer, new umi3d.common.RequestHttpGetDto() { key = "testdata" }, true);
            }
            else
                Debug.Log("unautorized acces to : " + "testdata");
        }
        oldWriteValue = tryToWrite;
    }

}
