using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RuntimeNodeEditor.Eggode
{
    public class ChangeServerSettingsNode : Node
    {
        public TMP_Dropdown dropdown;
        public SocketInput inputSocket;
        public SocketOutput outputSocket;

        private List<IOutput> _incomingOutputs;


        public override void Setup()
        {
            _incomingOutputs = new List<IOutput>();

            Register(outputSocket);
            Register(inputSocket);

            SetHeader("ChangeServerSettings");
            outputSocket.SetValue(0f);

            dropdown.AddOptions(new List<TMP_Dropdown.OptionData>()
            {
                new TMP_Dropdown.OptionData(ServerSettings.Breake.ToString()),
                new TMP_Dropdown.OptionData(ServerSettings.Create.ToString()),
                new TMP_Dropdown.OptionData(ServerSettings.Uron.ToString()),
            });

            dropdown.onValueChanged.AddListener(HandleFieldValue);

        }

            private void HandleFieldValue(int ddInfo)
            {
                outputSocket.SetValue(ddInfo);
            }

   /*         public override void OnSerialize(Serializer serializer)
            {
                serializer.Add("intValue", dropdown.);
            }*/

/*            public override void OnDeserialize(Serializer serializer)
        {
                var value = serializer.Get("intValue");
            dropdown.value = value;

                HandleFieldValue(value);
            }*/
        }
    }