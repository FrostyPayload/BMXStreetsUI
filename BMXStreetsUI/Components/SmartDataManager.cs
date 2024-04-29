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
                    return SetupSlider(ScriptableObject.CreateInstance<SmartData_Float>(), option, SmartDataFloatStuct.DataStyle.Free, SmartData.DataUIStyle.Slider);
                case UIStyle.SteppedInt:
                    return SetupSlider(ScriptableObject.CreateInstance<SmartData_Float>(), option, SmartDataFloatStuct.DataStyle.Stepped, SmartData.DataUIStyle.Stepped);
                case UIStyle.Button:
                    return SetupButton(ScriptableObject.CreateInstance<SmartData_Button>(), option);
                case UIStyle.Toggle:
                    return SetupSlider(ScriptableObject.CreateInstance<SmartData_Float>(), option, SmartDataFloatStuct.DataStyle.Stepped, SmartData.DataUIStyle.Stepped);

            }
            return ScriptableObject.CreateInstance<SmartData_Float>();
        }
        static SmartData SetupSlider(SmartData_Float data, CustomMenuOption option, SmartDataFloatStuct.DataStyle style, SmartData.DataUIStyle uiStyle)
        {
            data._dataUIStyle = uiStyle;
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
            SmartType._identifyer = option.title; // the identifier is used in loading, when a container loads from disk, it takes the bytes and looks for a smartdata in its list whose id matches, then populates
            data._description = option.description;
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
    }
}
