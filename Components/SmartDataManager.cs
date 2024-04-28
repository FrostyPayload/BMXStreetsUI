using Il2Cpp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.Events;

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

        public static SmartDataContainer GetNewContainer(string saveName, string folder = "")
        {
            var container = ScriptableObject.CreateInstance<SmartDataContainer>();
            var name = new UnityEngine.Localization.LocalizedString();
            container._ContainerName = saveName + "SmartContainer";
            container.name = container._ContainerName + "Object";
            container._useStandardGameDataPanel = false;
            container._description = "";
            container.searchType = "";
            container.SetDefaultValue();
            // container.CanShowDataInUI = new SmartData.BlockFromDataListRule(canshow.GetFunctionPointer());
            container.dataIdentifiers = ScriptableObject.CreateInstance<CategoryListScriptableObject>();
            container.dataIdentifiers.categoryName = saveName;
            container.dataIdentifiers.categories = new Il2CppSystem.Collections.Generic.List<string>();
            //container.dataIdentifiers.categories.Add("StreetsUI");
            container._localizedLabel = name;
            container._localizedDescription = name;
            container.OnDataChangeableChanged = new UnityEvent();
            container.filterSearchByName = false;
            container.assetSearchFolder = "";
            container.dataSaveExtension = "streetsui";
            container.OnAnyValueChanged = new UnityEvent();
            container._customSearchTag = "";
            container._smartDatas = new Il2CppSystem.Collections.Generic.List<SmartData>();

            return container;
        }
    }
}
