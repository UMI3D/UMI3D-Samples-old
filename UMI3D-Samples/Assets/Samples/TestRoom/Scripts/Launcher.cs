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

using umi3d.edk;
using umi3d.edk.collaboration;
using UnityEngine;
using UnityEngine.UI;

namespace test
{
    public class Launcher : UMI3DLauncher
    {
        public InputField Name;
        public InputField Pin;
        public Button StartButton;
        public Button StopButton;
        public Button IpButton;
        public Text Ip;

        void OnStart()
        {
            StartButton.interactable = false;
            StopButton.interactable = true;

            UMI3DEnvironment.Instance.environmentName = Name.text;
            if (UMI3DCollaborationServer.Instance.Identifier is PinIdentifierApi)
                (UMI3DCollaborationServer.Instance.Identifier as PinIdentifierApi).pin = Pin.text;

            UMI3DServer.Instance.Init();

            Ip.text = UMI3DServer.GetHttpUrl();
        }

        void OnStop()
        {
            UMI3DCollaborationServer.Stop();
            Ip.text = "_";
            StartButton.interactable = true;
            StopButton.interactable = false;
        }

        public override void LaunchServer()
        {
            OnStart();
        }

        protected override void Start()
        {
            base.Start();
            StartButton.interactable = !LaunchServerOnStart;
            StopButton.interactable = LaunchServerOnStart;
            StartButton.onClick.AddListener(OnStart);
            StopButton.onClick.AddListener(OnStop);
            IpButton.onClick.AddListener(() => GUIUtility.systemCopyBuffer = Ip.text);
            if (UMI3DCollaborationServer.Instance.Identifier is PinIdentifierApi)
                Pin.text = (UMI3DCollaborationServer.Instance.Identifier as PinIdentifierApi).pin;
            Name.text = UMI3DEnvironment.Instance.environmentName;
        }
    }
}