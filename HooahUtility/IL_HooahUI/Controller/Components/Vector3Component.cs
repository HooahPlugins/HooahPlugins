using System;
using System.Globalization;
using TMPro;
using UnityEngine;

namespace HooahUtility.Controller.Components
{
    public class Vector3Component : FormComponentBase
    {
        public TMP_InputField[] inputs;
        public DragArea[] buttons;

        private float Sensitivity
        {
            get
            {
                var b = 1000;
                if (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.LeftAlt))
                    b = b / 100;
                if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.LeftShift))
                    b = b * 100;

                return b;
            }
        }

        public override void AssignValues()
        {
            var value = GetValue<Vector3>();
            for (var i = 0; i < Math.Min(3, inputs.Length); i++)
            {
                var input = inputs[i];
                input.text = value[i].ToString(CultureInfo.InvariantCulture);
                input.contentType = TMP_InputField.ContentType.DecimalNumber;
                var targetIndex = i;
                input.onSubmit.AddListener((newValue) =>
                {
                    var oldValue = GetValue<Vector3>();
                    oldValue[targetIndex] = Convert.ToSingle(newValue);
                    SetValue(oldValue, () => { });
                });
            }

            for (var i = 0; i < buttons.Length; i++)
            {
                var button = buttons[i];
                var targetIndex = i;
                button.onDrag += (e) =>
                {
                    var oldValue = GetValue<Vector3>();
                    oldValue[targetIndex] += (
                        Vector2.Dot(Vector2.left, e.delta) * -e.delta.magnitude / 1000
                    );
                    SetValue(oldValue, () =>
                    {
                        inputs[0].text = oldValue.x.ToString(CultureInfo.InvariantCulture);
                        inputs[1].text = oldValue.y.ToString(CultureInfo.InvariantCulture);
                        inputs[2].text = oldValue.z.ToString(CultureInfo.InvariantCulture);
                    });
                };
            }
            // drag the button 
            //  set the ui values
            //  and set the actual values.
        }
    }
}
