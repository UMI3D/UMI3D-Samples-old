using System.Collections;
using System.Collections.Generic;
using umi3d.edk;
using umi3d.edk.userCapture;
using UnityEngine;

public class EmoteTest : MonoBehaviour
{
    public UMI3DEmotesConfig emoteConfig;

    public bool startTest = false;
    private bool shouldTest = true;

    uint i = 0;
    void Update()
    {
        if (startTest)
        {
            if (shouldTest)
            {
                ChangeEmoteAvailabilityTest(i%2==0);
                shouldTest = false;
                i++;
            }
        }
        else
        {
            if (!shouldTest)
                shouldTest=true;
        }
    }

    void ChangeEmoteAvailabilityTest(bool value)
    {
        UMI3DEmote emote = emoteConfig.IncludedEmotes.Find(x => x.name.Contains("Waving"));
        SetEntityProperty op = emote.Available.SetValue(value);
        Transaction tr = new Transaction()
        {
            reliable=true
        };
        tr.Add(op);
        tr.Dispatch();
    }

}
