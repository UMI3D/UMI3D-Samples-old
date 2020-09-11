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
using umi3d.common.interaction;
using umi3d.edk.interaction;
using UnityEngine;

public class ConnectionParameters : MonoBehaviour
{

    public PinIdentifierWithParameter pinIdentifier;
    AbstractInteraction[] interactions;
    // Start is called before the first frame update
    void Start()
    {
        pinIdentifier.GetParameter = GetParameter;
        interactions = GetComponents<AbstractInteraction>();
    }

    FormDto GetParameter(string login) {
        FormDto form = new FormDto() { name = "Connection Form", interactions = interactions.Select(i => i.ToDto(null)).ToList() };
        return form;
    }
}
