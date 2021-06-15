/*
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

namespace umi3d.common
{
    /// <summary>
    /// A class to describe a starting interpolation operation
    /// </summary>
<<<<<<< HEAD:UMI3D-Samples/Assets/Common/Core/Runtime/Dto/UMI3DCoreDto/StartInterpolationPropertyDto.cs
    public class StartInterpolationPropertyDto : AbstractInterpolationPropertyDto
=======
    [Serializable]
    public abstract class AbstractBrowserRequestDto : UMI3DDto, IByte
>>>>>>> e58c168cebce48f2369d166a2e5a296723ac64de:UMI3D-Samples/Assets/Common/Core/Runtime/Dto/UMI3DCoreDto/AbstractBrowserRequestDto.cs
    {
        /// <summary>
        /// The value with witch to start interpolation
        /// </summary>
<<<<<<< HEAD:UMI3D-Samples/Assets/Common/Core/Runtime/Dto/UMI3DCoreDto/StartInterpolationPropertyDto.cs
        public object startValue;
=======
        protected bool reliable = true;


        protected abstract uint GetOperationId();
        public virtual Bytable ToByteArray(params object[] parameters)
        {
            return UMI3DNetworkingHelper.Write(GetOperationId());
        }
>>>>>>> e58c168cebce48f2369d166a2e5a296723ac64de:UMI3D-Samples/Assets/Common/Core/Runtime/Dto/UMI3DCoreDto/AbstractBrowserRequestDto.cs
    }
}
