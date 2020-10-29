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
using umi3d.edk;
using umi3d.edk.interaction;
using UnityEngine;
using UnityEngine.UI;

public class ParametersDisplay : MonoBehaviour
{
    public StringParameter stringParameter;
    public StringEnumParameter stringEnum;
    public FloatRangeParameter rangeParameter;
    public BooleanParameter booleanParameter;

    public Text enumText;
    public Text stringText;
    public Text rangeText;
    public Text boolText;

    private void Start()
    {
        enumText.text = stringEnum.value;
        stringText.text = stringParameter.value;
        rangeText.text = rangeParameter.value.ToString();
        boolText.text = booleanParameter.value.ToString();

        stringEnum.onChange.AddListener(EnumParameterChange);
        stringParameter.onChange.AddListener(StringParameterChange);
        booleanParameter.onChange.AddListener(BoolParameterChange);
        rangeParameter.onChange.AddListener(RangeParameterChange);
    }

    void StringParameterChange(AbstractParameter.ParameterEventContent<string> content) { stringText.text = content.value; }
    void EnumParameterChange(AbstractParameter.ParameterEventContent<string> content) { enumText.text = content.value; }
    void BoolParameterChange(AbstractParameter.ParameterEventContent<bool> content) { boolText.text = content.value.ToString(); }
    void RangeParameterChange(AbstractParameter.ParameterEventContent<float> content) { rangeText.text = content.value.ToString(); }

}
