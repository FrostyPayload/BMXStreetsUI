using Il2Cpp;
using Il2CppMG_Core.MG_SmartData.SaveLoad;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace BmxStreetsUI.Components
{
    internal class SmartDataManager
    {
        public static SmartData CreateSmartData(CustomMenuOption option)
        {
            switch (option.UIStyle)
            {
                case UIStyle.Slider:
                    return SetupSlider(CreateDefaultSmartData<SmartData_Float>(option.title), option, SmartDataFloatStuct.DataStyle.Free, SmartData.DataUIStyle.Slider);
                case UIStyle.SteppedInt:
                    return SetupSlider(CreateDefaultSmartData<SmartData_Float>(option.title), option, SmartDataFloatStuct.DataStyle.Stepped, SmartData.DataUIStyle.Stepped);
                case UIStyle.Button:
                    return SetupButton(CreateDefaultSmartData<SmartData_Button>(option.title), option);
                case UIStyle.Toggle:
                    return SetupSlider(CreateDefaultSmartData<SmartData_Float>(option.title), option, SmartDataFloatStuct.DataStyle.Stepped, SmartData.DataUIStyle.Stepped);

            }
            return ScriptableObject.CreateInstance<SmartData_Float>();
        }
        static SmartData SetupSlider(SmartData_Float data, CustomMenuOption option, SmartDataFloatStuct.DataStyle style, SmartData.DataUIStyle uiStyle)
        {
            data._description = option.description;
            data._dataUIStyle = uiStyle;
            data._defaultValue = option.defaultValue;
            data.OnBelowSignificantValue = new UnityEvent();
            data.OnSignificantValueCrossed = new UnityEvent();
            data.OnSteppedLabelListChanged = new UnityEvent();
            var SmartType = data._mData;
            var FloatData = new SmartDataFloatStuct();
            FloatData.SetMin(option.GetMin());
            FloatData.SetMax(option.GetMax());
            FloatData._dataStyle = style;
            FloatData.displayDecimalAccuracy = 0;
            FloatData.Value = 0;
            FloatData._clampMinMax = true;
            FloatData._wrapMinMax = false;
            SmartType._value = FloatData;
            SmartType._label = option.title;// seen in UI
            SmartType._identifyer = option.title; // used in data matching

            data.SetData(SmartType);

            if(uiStyle == SmartData.DataUIStyle.Stepped)
            {
                data.steppedLabelList = ScriptableObject.CreateInstance<CategoryListScriptableObject>();
                data.steppedLabelList.categoryName = option.title;
                var LabelList = option.GetLabels();
                data.OnSteppedLabelListChanged = new UnityEvent();
                data.SetSteppedLabelListData(LabelList);
                data.MatchMinMaxToSteppedList();
            }

            return data;
        }
        static SmartData SetupButton(SmartData_Button data, CustomMenuOption option)
        {
            var SmartType = data._mData;
            var btnData = new SmartDataButtonStuct();
            data._mData._value = btnData;
            data._dataUIStyle = SmartData.DataUIStyle.Button;
            SmartType._value = btnData;
            SmartType._label = option.title; // seen in UI
            SmartType._identifyer = option.title; // used in data matching
            data._description = option.description; // seen in UI
            data.SetData(SmartType);
            return data;
        }

        public static SmartDataContainer CreateNewContainer(string saveName, string folder = "")
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
            container.dataIdentifiers.categoryName = folder; // folderName in locallow/mash/containers/
            container.dataIdentifiers.categories = new Il2CppSystem.Collections.Generic.List<string>();
            container.dataIdentifiers.categories.Add(saveName);
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
        public static SmartDataContainerReferenceListSet CreateNewSet(string name)
        {
            var listSet = ScriptableObject.CreateInstance<SmartDataContainerReferenceListSet>();
            listSet.SetName = name + "ListSet";
            listSet.name = name + "Object";

            return listSet;
        }
        public static SmartDataContainerReferenceList CreateNewList(string name)
        {
            var list = ScriptableObject.CreateInstance<SmartDataContainerReferenceList>();
            list.ListName = name;
            list.name = name + "ReferenceList";
            list.OnSelected = new UnityEvent();
            list.OnValueChanged = new UnityEvent();
            list.OnDataChangeableChanged = new UnityEvent();
            list.OnValueChanged_DataValue = new SmartDataEvent();
            return list;
        }

        static T CreateDefaultSmartData<T>(string name) where T : SmartData
        {
            var data = ScriptableObject.CreateInstance<T>();
            data.name = name + "_SmartObject";
            data.OnDataChangeableChanged = new UnityEvent();
            data.DataChangeableCallback = new BoolCallBackEvent();
            data.OnValueChanged_DataValue = new SmartDataEvent();
            data.OnValueChanged = new UnityEvent();
            data._visibilityCompare = SmartData.VisibilityCompare.EqualTo;
            data.referencedVisibilityValue = 0;
            

            return data;
        }
    }
}
