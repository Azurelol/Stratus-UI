using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using Stratus.Extensions;
using Stratus.Models;
using Stratus.Reflection;
using Stratus.Types;

namespace Stratus.UI
{
	public class StratusRuntimeInspectorNumericDrawer : StratusRuntimeInspectorDrawer
    {
        [SerializeField]
        private TMP_InputField inputField;
        [SerializeField]
        private Slider slider;
        [SerializeField]
        private float inputStepSize = 0.1f;
        [SerializeField]
        private StratusOrientation inputOrientation = StratusOrientation.Horizontal;

        private NumericType numericType { get; set; }
        private bool isPercentage { get; set; }
        private bool updatingValue { get; set; }

        public override Selectable drawerSelectable => slider;

        public override Color bodyColor
        {
            set => inputField.textComponent.color = value;
        }

        public override void Navigate(Vector2 dir)
        {
            switch (inputOrientation)
            {
                case StratusOrientation.Horizontal:
                    if (dir.x > 0)
                    {
                        slider.value += inputStepSize;
                    }
                    else if (dir.x < 0)
                    {
                        slider.value -= inputStepSize;
                    }
                    break;
                case StratusOrientation.Vertical:
                    if (dir.y > 0)
                    {
                        slider.value += inputStepSize;
                    }
                    else if (dir.y < 0)
                    {
                        slider.value -= inputStepSize;
                    }
                    break;
            }
        }

        protected override void OnSet(StratusRuntimeInspectorDrawerSettings settings)
        {
            if (settings.field.fieldType == StratusSerializedFieldType.Float)
            {
                numericType = NumericType.Float;
                slider.wholeNumbers = false;

            }
            else if (settings.field.fieldType == StratusSerializedFieldType.Integer)
            {
                numericType = NumericType.Integer;
                slider.wholeNumbers = true;
            }
            else
            {
                throw new Exception("Unsupported field type for numeric drawer...");
            }

            RangeAttribute range = (RangeAttribute)settings.field.attributesByType.GetValueOrDefault(typeof(RangeAttribute));
            if (range != null)
            {
                slider.minValue = range.min;
                slider.maxValue = range.max;
                if (numericType == NumericType.Float)
                {
                    isPercentage = range.min == 0 && range.max == 1;
                }
            }
            else
            {
                slider.gameObject.SetActive(false);
            }

            switch (numericType)
            {
                case NumericType.Integer:
                    inputField.contentType = TMP_InputField.ContentType.IntegerNumber;
                    break;
                case NumericType.Float:
                    inputField.contentType = TMP_InputField.ContentType.DecimalNumber;
                    break;
            }

            float currentValue = (float)settings.field.value;
            UpdateSliderValue(currentValue, false);
            UpdateTextValue(currentValue);

            inputField.onValueChanged.AddListener((value) =>
            {
                try
                {
                    switch (numericType)
                    {
                        case NumericType.Integer:
                            {
                                int parsedValue = int.Parse(value);
                                settings.field.value = value;
                                UpdateSliderValue(parsedValue, true);
                            }
                            break;
                        case NumericType.Float:
                            {
                                float parsedValue = float.Parse(value);
                                settings.field.value = value;
                                UpdateSliderValue(parsedValue, true);
                            }
                            break;
                    }
                }
                catch (Exception e)
                {
                    UpdateTextValue(slider.value);
                }
            });

            slider.onValueChanged.AddListener((value) =>
            {
                settings.field.value = value;
                UpdateTextValue(value);
            });
        }

        private void UpdateTextValue(float value)
        {
            if (updatingValue)
            {
                return;
            }
            updatingValue = true;
            inputField.text = isPercentage ? value.FromPercentRoundedString() 
                : value.ToString();
            updatingValue = false;
        }

        private void UpdateSliderValue(float value, bool fromText)
        {
            if (updatingValue)
            {
                return;
            }
            updatingValue = true;
            if (fromText && isPercentage)
            {
                value = value.ToPercent();
            }
            slider.SetValue(value);
            updatingValue = false;
        }
    }

}