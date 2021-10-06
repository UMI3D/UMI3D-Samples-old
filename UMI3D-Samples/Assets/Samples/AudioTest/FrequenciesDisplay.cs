﻿using System.Collections;
using System.Collections.Generic;
using umi3d.edk;
using umi3d.edk.collaboration;
using umi3d.edk.interaction;
using UnityEngine;
using UnityEngine.UI;

public class FrequenciesDisplay : MonoBehaviour
{
    public GameObject prefab;
    public Transform board;
    Dictionary<UMI3DCollaborationUser, info> map = new Dictionary<UMI3DCollaborationUser, info>();
    Dictionary<UMI3DCollaborationUser, StringEnumParameter> mapEnum = new Dictionary<UMI3DCollaborationUser, StringEnumParameter>();

    public UMI3DTool tool;
    public UMI3DInteractable interactable;

    class info
    {
        Text _Label;
        Text _Frequency;
        GameObject _Info;

        public info(GameObject prefab,Transform parent,string Name, int Frequency)
        {
            _Info = Instantiate(prefab, parent);
            _Label = _Info.transform.GetChild(0).GetComponent<Text>();
            _Frequency = _Info.transform.GetChild(1).GetComponent<Text>();
            setName(Name);
            setFrequency(Frequency);
            //var t = new Transaction()
            //{
            //    reliable = true,
            //    Operations = new List<Operation>() { _Info.transform.GetComponent<UMI3DNode>().GetLoadEntity() }
            //};
            //UMI3DServer.Dispatch(t);
        }

        public void setName(string Name)
        {
            _Label.text = Name;
        }

        public void setFrequency(int freq)
        {
            _Frequency.text = freq.ToString();
        }

        public void Destroy()
        {
            _Label = null;
            _Frequency = null;
            GameObject.Destroy(_Info);
        }

    }


    private void Start()
    {
        UMI3DCollaborationServer.Instance.OnUserJoin.AddListener((u) => {
            if (u is UMI3DCollaborationUser user)
            {
                map[user] = new info(prefab, board, user.Id().ToString(), user.audioFrequency);
                var Enum = tool.gameObject.AddComponent<StringEnumParameter>();
                Enum.Display.name = user.Id().ToString();
                Enum.options = new List<string>()
                {
                    "8000",
                    "12000",
                    "16000",
                    "24000",
                    "48000",
                };
                Enum.value = user.audioFrequency.ToString();
                Enum.onChange.AddListener(
                    (s) =>
                    {
                        int value;
                        if (int.TryParse(s.value, out value))
                        {
                            map[user].setFrequency(value);
                            user.audioFrequency = value;
                            user.NotifyUpdate();
                        }
                    }
                    );
                var t = new Transaction()
                {
                    reliable = true,
                    Operations = new List<Operation>() { tool.objectInteractions.Add(Enum), interactable.objectInteractions.Add(Enum) }
                };
                UMI3DServer.Dispatch(t);
            }
        });
        UMI3DCollaborationServer.Instance.OnUserLeave.AddListener((u) => {
            if (u is UMI3DCollaborationUser user)
            {
                if (map.ContainsKey(user))
                {
                    map[user].Destroy();
                    map.Remove(user);
                }
                if (mapEnum.ContainsKey(user))
                {
                    var Enum = mapEnum[user];
                    var t = new Transaction()
                    {
                        reliable = true,
                        Operations = new List<Operation>() { tool.objectInteractions.Remove(Enum), interactable.objectInteractions.Remove(Enum) }
                    };
                    UMI3DServer.Dispatch(t);
                    Destroy(Enum);
                    mapEnum.Remove(user);
                }
            }
        });
    }

    public void UpdateFrequency(UMI3DCollaborationUser user)
    {
        if (map.ContainsKey(user))
        {
            map[user].setFrequency(user.audioFrequency);
        }
    }
}
