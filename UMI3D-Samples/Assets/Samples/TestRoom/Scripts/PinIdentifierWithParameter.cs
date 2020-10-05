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

using System;
using umi3d.common;
using umi3d.common.collaboration;
using umi3d.common.interaction;
using umi3d.edk.collaboration;
using UnityEngine;

[CreateAssetMenu(fileName = "PinIdentifierWithForm", menuName = "UMI3D/Test/Pin Identifier")]
public class PinIdentifierWithParameter : PinIdentifierApi
{
    public Func<string, FormDto> GetParameter;

    public override FormDto GetParameterDtosFor(string login)
    {
        return GetParameter != null ? GetParameter(login) : null;
    }

    public override StatusType UpdateIdentity(UMI3DCollaborationUser user, UserConnectionDto identity)
    {
        var state = base.UpdateIdentity(user, identity);
        //debugForm(identity.parameters);

        return state;
    }

    void debugForm(FormDto form)
    {
        if(form != null && form.Fields != null)
        foreach (var dto in form.Fields)
            switch (dto)
            {
                case BooleanParameterDto booleanParameterDto:
                    Debug.Log(booleanParameterDto.value);
                    break;
                case FloatRangeParameterDto floatRangeParameterDto:
                    Debug.Log(floatRangeParameterDto.value);
                    break;
                case EnumParameterDto<string> enumParameterDto:
                    Debug.Log(enumParameterDto.value);
                    break;
                case StringParameterDto stringParameterDto:
                    Debug.Log(stringParameterDto.value);
                    break;
                default:
                    Debug.Log(dto);
                    break;
            }
    }

}
