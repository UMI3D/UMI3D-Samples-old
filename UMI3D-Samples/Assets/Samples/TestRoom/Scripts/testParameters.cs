/*
Copyright 2019 Gfi Informatique

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

using System.Linq;
using System.Runtime.InteropServices;
using umi3d.common.interaction;
using umi3d.edk;
using umi3d.edk.collaboration;
using umi3d.edk.interaction;
using UnityEngine;

public class TestParameters : MonoBehaviour
{
    public void DebugS(UMI3DUser user, string value)
    {
        Debug.Log($"{(user as UMI3DCollaborationUser)?.login} changed value to {value}");
    }

    public void DebugF(UMI3DUser user, float value)
    {
        Debug.Log($"{(user as UMI3DCollaborationUser)?.login} changed value to {value}");
    }

    public void DebugB(UMI3DUser user, bool value)
    {
        Debug.Log($"{(user as UMI3DCollaborationUser)?.login} changed value to {value}");
    }
    
    public void DebugString(AbstractParameter.ParameterEventContent<string> content) { Debug.Log($"{(content.user as UMI3DCollaborationUser)?.login} changed value to {content.value}"); }
    public void DebugFloat(AbstractParameter.ParameterEventContent<float> content) { Debug.Log($"{(content.user as UMI3DCollaborationUser)?.login} changed value to {content.value}"); }
    public void DebugBoolean(AbstractParameter.ParameterEventContent<bool> content) { Debug.Log($"{(content.user as UMI3DCollaborationUser)?.login} changed value to {content.value}"); }
    public void DebugForm(UMI3DForm.FormEventContent content) { Debug.Log($"{(content.user as UMI3DCollaborationUser)?.login} changed value to [{content.form.fields.Select(a=>ToString(a)).Aggregate((a,b)=> $"{a}; {b}")}]"); }
    public void DebugEventTrigger(AbstractInteraction.InteractionEventContent content) { Debug.Log($"{(content.user as UMI3DCollaborationUser)?.login} event Trigger"); }
    public void DebugEventRelease(AbstractInteraction.InteractionEventContent content) { Debug.Log($"{(content.user as UMI3DCollaborationUser)?.login} event Release"); }
    public void DebugEventHold(AbstractInteraction.InteractionEventContent content) { Debug.Log($"{(content.user as UMI3DCollaborationUser)?.login} event Hold"); }


    string ToString(AbstractParameterDto dto)
    {
        switch (dto)
        {
            case BooleanParameterDto b:
                return b.value.ToString();
            case EnumParameterDto<string> Es:
                return Es.value.ToString();
            case FloatParameterDto f:
                return f.value.ToString();
            case StringParameterDto s:
                return s.value.ToString();
            case IntegerParameterDto i:
                return i.value.ToString();
            case FloatRangeParameterDto fr:
                return $"{fr.min}<{fr.value}<{fr.max}";
            case IntegerRangeParameterDto ir:
                return $"{ir.min}<{ir.value}<{ir.max}";
        }
        return $"missing case for {dto}";

    }


}
