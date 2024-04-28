using Il2Cpp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BmxStreetsUI.Components
{
    internal class SmartDataManager
    {
        public static SmartData CreateSmartDatas(CustomMenuOption option)
        {
            switch (option.uiStyle)
            {
                case SmartData.DataUIStyle.Slider:
                    return SetupSlider(ScriptableObject.CreateInstance<SmartData_Float>(), option);
                case SmartData.DataUIStyle.Stepped:
                    return SetupSlider(ScriptableObject.CreateInstance<SmartData_Float>(), option, SmartDataFloatStuct.DataStyle.Stepped);
                case SmartData.DataUIStyle.Button:
                    return SetupButton(ScriptableObject.CreateInstance<SmartData_Button>(), option);
                case SmartData.DataUIStyle.Stat:
                    return ScriptableObject.CreateInstance<Stat>();
                case SmartData.DataUIStyle.Custom:
                    return ScriptableObject.CreateInstance<SmartData_Button>();

            }
            return ScriptableObject.CreateInstance<SmartData_Float>();
        }
        static SmartData SetupSlider(SmartData_Float data, CustomMenuOption option, SmartDataFloatStuct.DataStyle style = SmartDataFloatStuct.DataStyle.Free)
        {
            var SmartType = data._mData;
            var FloatData = new SmartDataFloatStuct();
            FloatData.SetMin(option.GetMin());
            FloatData.SetMax(option.GetMax());
            FloatData._dataStyle = style;
            FloatData.displayDecimalAccuracy = 1;
            FloatData._wrapMinMax = false;
            data._mData._value = FloatData;

            SmartType._value = FloatData;
            data.name = option.title + "Object";
            SmartType._label = option.title;
            SmartType._identifyer = option.title;
            data._description = option.description;
            data.SetData(SmartType);
            return data;
        }
        static SmartData SetupButton(SmartData_Button data, CustomMenuOption option)
        {
            var SmartType = data._mData;
            var btnData = new SmartDataButtonStuct();
            data._mData._value = btnData;

            SmartType._value = btnData;
            data.name = option.title + "Object";
            SmartType._label = option.title;
            SmartType._identifyer = option.title;
            data._description = option.description;
            data.SetData(SmartType);
            return data;
        }

    }
}
