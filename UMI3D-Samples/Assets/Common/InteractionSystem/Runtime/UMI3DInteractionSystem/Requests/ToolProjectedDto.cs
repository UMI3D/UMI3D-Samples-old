﻿/*
Copyright 2019 - 2021 Inetum

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

namespace umi3d.common.interaction
{
    public class ToolProjectedDto : AbstractBrowserRequestDto
    {

        public ulong toolId;
        public uint boneType;

        protected override uint GetOperationId() { return UMI3DOperationKeys.ToolProjected; }

        public override Bytable ToByteArray(params object[] parameters)
        {
            return base.ToByteArray(parameters) 
                + UMI3DNetworkingHelper.Write(toolId) 
                + UMI3DNetworkingHelper.Write(boneType);
        }

    }
}
