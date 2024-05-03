using Il2Cpp;
using UnityEngine;
using UnityEngine.Events;

namespace BmxStreetsUI.Components
{
    public class SmartDataManager
    {
        public static SmartData OptionToSmartUI(MenuOptionBase option,string identifyer)
        {
            switch (option.UIStyle)
            {
                case UIStyle.Slider:
                    return SetupSmartUI(CreateDefaultSmartFloat(option.title),identifyer, option, SmartDataFloatStuct.DataStyle.Free, SmartData.DataUIStyle.Slider);
                case UIStyle.SteppedInt:
                    return SetupSmartUI(CreateDefaultSmartFloat(option.title), identifyer, option, SmartDataFloatStuct.DataStyle.Stepped, SmartData.DataUIStyle.Stepped);
                case UIStyle.Button:
                    return SetupButton(CreateDefaultSmartData<SmartData_Button>(option.title), option);
                case UIStyle.Toggle:
                    return SetupSmartUI(CreateDefaultSmartFloat(option.title), identifyer, option, SmartDataFloatStuct.DataStyle.Stepped, SmartData.DataUIStyle.Stepped);

            }
            return ScriptableObject.CreateInstance<SmartData_Float>();
        }
        public static SmartData SetupSmartUI(SmartData_Float data, string identifyer, MenuOptionBase option, SmartDataFloatStuct.DataStyle style, SmartData.DataUIStyle uiStyle)
        {
            data._description = option.description;
            data._dataUIStyle = uiStyle;
            data._defaultValue = option.defaultValue;
            data.OnBelowSignificantValue = new UnityEvent();
            data.OnSignificantValueCrossed = new UnityEvent();
            data.OnSteppedLabelListChanged = new UnityEvent();
            var SmartType = data._mData;
            var FloatData = SmartType._value;
            FloatData.SetMin(option.GetMin());
            FloatData.SetMax(option.GetMax());
            FloatData._dataStyle = style;
            SmartType._dataUnit = option.DataUnit;
            //FloatData.displayDecimalAccuracy = 5;
            //FloatData.decimalVal = option.decimalPlaces;
            //data._needsDecimalAccuracy = option.decimalPlaces > 0 ? true : false;
            SmartType._identifyer = identifyer;
            SmartType._label = option.title;
            FloatData.Value = 0;
            FloatData._clampMinMax = true;
            FloatData._wrapMinMax = false;
            SmartType._value = FloatData;
            data.OnValueChanged_DataValue.AddListener(option.ValueCallBack);
            data.OnValueChanged.AddListener(option.VoidCallBack);
            data.EnableDataChangeable();

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
        public static SmartData SetupButton(SmartData_Button data, MenuOptionBase option)
        {
            var SmartType = data._mData;
            var btnData = SmartType._value;
            SmartType._value = btnData;
            data._dataUIStyle = SmartData.DataUIStyle.Button;
            SmartType._value = btnData;
            SmartType._label = option.title; // seen in UI
            SmartType._identifyer = option.title; // used in data matching
            data._description = option.description; // seen in UI
            data._mData = SmartType;
            data._OnButtonEvent = new GameEvent();
            data._OnButtonEvent.OnRaise = new UnityEvent();
            data._OnButtonEvent.OnRaise.AddListener(option.VoidCallBack);
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
            listSet._DataRefLists = new Il2CppSystem.Collections.Generic.List<SmartDataContainerReferenceList>();

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
        public static SmartData_Float CreateDefaultSmartFloat(string name)
        {
            var smartData = CreateDefaultSmartData<SmartData_Float>(name);
            return smartData;
        }
        public static T CreateDefaultSmartData<T>(string name) where T : SmartData
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
