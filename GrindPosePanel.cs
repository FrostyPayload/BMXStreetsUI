using Il2Cpp;
using Il2CppInterop.Runtime.Injection;
using MelonLoader;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace BmxStreetsUI
{
    public class GrindPosePanel
    {
        public void SetupPanel()
        {

        }
        BMXFreeformPoseData[] poses;
        GameObject myBmx, ghost, bmx;
        Dictionary<GrindType, SmartDataContainerReferenceList> grindDataDict;
        void PopulateGrindPoses()
        {
            var list = new List<BMXFreeformPoseData>();
            foreach (var item in Resources.FindObjectsOfTypeAll<BMXFreeformPoseData>())
            {
                if (item.name.ToLower().Contains("grind"))
                {
                    list.Add(item);
                }
            }
            poses = list.ToArray();
        }
        void SpawnCharacter()
        {
            var character = GameObject.Find("Player Components");
            if (character == null || bmx == null) { Debug.Log("No character or bike found in FrostyUI.spawncharacter"); return; }
           // if (ghost == null) ghost = Instantiate(character);
            ghost.name = "FrostyGrindMenuGhostCharacter";

            var toDestroy = new List<GameObject>();
            foreach (var child in ghost.transform)
            {
                if (child is Transform)
                {
                    var childTrans = (Transform)child;
                    if (childTrans.gameObject.GetComponent<MGInputManager>() != null ||
                        childTrans.gameObject.GetComponent<SessionMarkerBasic>() != null ||
                        childTrans.gameObject.GetComponent<Camera>() != null ||
                        childTrans.gameObject.GetComponent<TimeInterpolator>() != null ||
                        childTrans.gameObject.GetComponent<PlayerAbilityDataBehaviour>() != null ||
                        childTrans.gameObject.GetComponent<VehicleSpawner>() != null
                        )
                    {
                        toDestroy.Add(childTrans.gameObject);
                    }
                    else if (childTrans.gameObject.GetComponent<Rigidbody>() != null && childTrans.gameObject.GetComponent<CapsuleCollider>() != null)
                    {
                        childTrans.position = bmx.transform.position + Vector3.up * 2;
                    }
                }
            }
            while (toDestroy.Count > 0)
            {
                //Destroy(toDestroy[0]);
            }

        }
        void SetupCharacter()
        {
            var body = GameObject.Find("BMX Showcase Body");
            if (body != null)
            {
                Debug.Log("Found bmx in open grinds tab");
                body.transform.Rotate(0, 90, 0);
                myBmx = body;
            }
        }
        public void SetupData(UIPanel panel)
        {
            var listSet = panel.listSet;
            if(listSet == null) { Debug.LogError($"Listset is null in GrindposePanel setup"); return; }
            grindDataDict = new Dictionary<GrindType, SmartDataContainerReferenceList>();
            foreach (var grindType in typeof(GrindType).GetEnumValues())
            {
                var grindEnum = (GrindType)grindType;
                if (!grindDataDict.ContainsKey(grindEnum))
                {
                    var grindName = grindType.ToString();
                    var grindListData = ScriptableObject.CreateInstance<SmartDataContainerReferenceList>();
                    grindListData.ListName = grindType.ToString();
                    grindListData.name = grindEnum.ToString() + "ReferenceList";
                    grindListData.OnSelected = new UnityEvent();
                    grindListData.OnValueChanged = new UnityEvent();
                    grindListData.OnDataChangeableChanged = new UnityEvent();
                    grindListData.OnValueChanged_DataValue = new SmartDataEvent();

                    var container = ScriptableObject.CreateInstance<SmartDataContainer>();
                    var name = new UnityEngine.Localization.LocalizedString();
                    container.name = "Grind poses config";
                    container._ContainerName = "GrindPoseConfig";
                    container._useStandardGameDataPanel = false;
                    container._description = "";
                    container.searchType = "";
                    container.SetDefaultValue();

                   // container.CanShowDataInUI = new SmartData.BlockFromDataListRule(canshow.GetFunctionPointer());
                    container.dataIdentifiers = ScriptableObject.CreateInstance<CategoryListScriptableObject>();
                    container.dataIdentifiers.categoryName = grindName + " Grind Pose";
                    container.dataIdentifiers.categories = new Il2CppSystem.Collections.Generic.List<string>();
                    container.dataIdentifiers.categories.Add("Bars Angle");
                    container._localizedLabel = name;
                    container._localizedDescription = name;
                    container.OnDataChangeableChanged = new UnityEvent();
                    container.filterSearchByName = false;
                    container.assetSearchFolder = "";
                    container.dataSaveExtension = "setting";
                    container.OnAnyValueChanged = new UnityEvent();
                    container._customSearchTag = "";
                    container._smartDatas = new Il2CppSystem.Collections.Generic.List<SmartData>();

                    CreateDatas(container, grindEnum, grindName ?? "Unknown");

                    grindListData.connectedContainer = container;
                    grindListData._dataContainer = container;
                    grindListData.dataGroup = container._smartDatas;
                    container.ValidateList();
                    grindDataDict.Add(grindEnum, grindListData);
                }

            }

            listSet.name = "GrindsReferenceSetObject";
            listSet.SetName = "GrindsReferenceSet";
            listSet._DataRefLists = new Il2CppSystem.Collections.Generic.List<SmartDataContainerReferenceList>();
            foreach (var item in grindDataDict.Keys)
            {
                listSet._DataRefLists.Add(grindDataDict[item]);
            }
            panel.TabName = "Grinds";
        }
        void CreateDatas(SmartDataContainer container, GrindType grindType, string grindName)
        {
            var BarsAngleSmartData = ScriptableObject.CreateInstance<SmartData_Float>();
            BarsAngleSmartData.OnDataChangeableChanged = new UnityEvent();
            BarsAngleSmartData.DataChangeableCallback = new BoolCallBackEvent();
            BarsAngleSmartData.EnableDataChangeable();
            var barsAngleSmartType = BarsAngleSmartData._mData;
            var BarsAngleSmartStruct = new SmartDataFloatStuct();
            BarsAngleSmartStruct.SetMin(-90);
            BarsAngleSmartStruct.SetMax(90);
            BarsAngleSmartStruct._dataStyle = SmartDataFloatStuct.DataStyle.Free;
            BarsAngleSmartStruct.displayDecimalAccuracy = 1;
            BarsAngleSmartStruct._wrapMinMax = false;

            barsAngleSmartType._value = BarsAngleSmartStruct;
            BarsAngleSmartData.name = grindType.ToString() + " Bars Angle";
            barsAngleSmartType._label = "Bars angle";
            barsAngleSmartType._identifyer = grindName + "PoseBarsAngle";
            BarsAngleSmartData._description = $"The angle you hold your bars at while in a {grindName} pose";

            BarsAngleSmartData.SetData(barsAngleSmartType);
            container._smartDatas.Add(BarsAngleSmartData);

            var CrankAngleSmartData = ScriptableObject.CreateInstance<SmartData_Float>();
            CrankAngleSmartData.OnDataChangeableChanged = new UnityEvent();
            CrankAngleSmartData.DataChangeableCallback = new BoolCallBackEvent();
            CrankAngleSmartData.EnableDataChangeable();
            var crankAngleSmartType = CrankAngleSmartData._mData;
            var crankSmartStruct = new SmartDataFloatStuct();
            crankSmartStruct.SetMin(-90);
            crankSmartStruct.SetMax(90);
            crankSmartStruct._dataStyle = SmartDataFloatStuct.DataStyle.Free;
            crankSmartStruct.displayDecimalAccuracy = 1;
            crankSmartStruct._wrapMinMax = false;

            crankAngleSmartType._value = crankSmartStruct;
            CrankAngleSmartData.name = grindType.ToString() + " Crank Angle";
            crankAngleSmartType._label = "Crank angle";
            crankAngleSmartType._identifyer = grindName + "PoseCrankAngle";
            CrankAngleSmartData._description = $"The angle you hold your crank at while in a {grindName} pose";

            CrankAngleSmartData.SetData(crankAngleSmartType);
            container._smartDatas.Add(CrankAngleSmartData);

            var LPedalAngleSmartData = ScriptableObject.CreateInstance<SmartData_Float>();
            LPedalAngleSmartData.OnDataChangeableChanged = new UnityEvent();
            LPedalAngleSmartData.DataChangeableCallback = new BoolCallBackEvent();
            LPedalAngleSmartData.EnableDataChangeable();
            var lPedalSmarttype = LPedalAngleSmartData._mData;
            var lPedalSmartstruct = new SmartDataFloatStuct();
            lPedalSmartstruct.SetMin(-90);
            lPedalSmartstruct.SetMax(90);
            lPedalSmartstruct._dataStyle = SmartDataFloatStuct.DataStyle.Free;
            lPedalSmartstruct.displayDecimalAccuracy = 1;
            lPedalSmartstruct._wrapMinMax = false;

            lPedalSmarttype._value = lPedalSmartstruct;
            LPedalAngleSmartData.name = grindType.ToString() + " Left Pedal Angle";
            lPedalSmarttype._label = "Left Pedal Angle";
            lPedalSmarttype._identifyer = grindName + "PoseLPedalAngle";
            LPedalAngleSmartData._description = $"The angle you hold your left pedal at while in a {grindName} pose";

            LPedalAngleSmartData.SetData(lPedalSmarttype);
            container._smartDatas.Add(LPedalAngleSmartData);

            var RPedalAngleSmartData = ScriptableObject.CreateInstance<SmartData_Float>();
            RPedalAngleSmartData.OnDataChangeableChanged = new UnityEvent();
            RPedalAngleSmartData.DataChangeableCallback = new BoolCallBackEvent();
            RPedalAngleSmartData.EnableDataChangeable();
            var RPedalSmartType = RPedalAngleSmartData._mData;
            var RPedalSmartstruct = new SmartDataFloatStuct();
            RPedalSmartstruct.SetMin(-90);
            RPedalSmartstruct.SetMax(90);
            RPedalSmartstruct._dataStyle = SmartDataFloatStuct.DataStyle.Free;
            RPedalSmartstruct.displayDecimalAccuracy = 1;
            RPedalSmartstruct._wrapMinMax = false;

            RPedalSmartType._value = RPedalSmartstruct;
            RPedalAngleSmartData.name = grindType.ToString() + " RPedal Angle";
            RPedalSmartType._label = "Right Pedal Angle";
            RPedalSmartType._identifyer = grindName + "PoseRPedalAngle";
            RPedalAngleSmartData._description = $"The angle you hold your right pedal at while in a {grindName} pose";

            RPedalAngleSmartData.SetData(RPedalSmartType);
            container._smartDatas.Add(RPedalAngleSmartData);

            var BodyX = ScriptableObject.CreateInstance<SmartData_Float>();
            BodyX.OnDataChangeableChanged = new UnityEvent();
            BodyX.DataChangeableCallback = new BoolCallBackEvent();
            BodyX.EnableDataChangeable();
            var bodyXSmartType = BodyX._mData;
            var bodyXSmartStruct = new SmartDataFloatStuct();
            bodyXSmartStruct.SetMin(-1);
            bodyXSmartStruct.SetMax(1);
            bodyXSmartStruct._dataStyle = SmartDataFloatStuct.DataStyle.Free;
            bodyXSmartStruct.displayDecimalAccuracy = 1;
            bodyXSmartStruct._wrapMinMax = false;

            bodyXSmartType._value = bodyXSmartStruct;
            BodyX.name = grindType.ToString() + " Lunge left right";
            bodyXSmartType._label = "Body Lunge Left Right";
            bodyXSmartType._identifyer = grindName + "LungeLeftRight";
            BodyX._description = $"The amount you lunge you body sideways towards or away from the grind while in a {grindName} pose";

            BodyX.SetData(bodyXSmartType);
            container._smartDatas.Add(BodyX);

            var BodyLungeZ = ScriptableObject.CreateInstance<SmartData_Float>();
            BodyLungeZ.OnDataChangeableChanged = new UnityEvent();
            BodyLungeZ.DataChangeableCallback = new BoolCallBackEvent();
            BodyLungeZ.EnableDataChangeable();
            var bodyZsmartType = BodyLungeZ._mData;
            var bodyZSmartStruct = new SmartDataFloatStuct();
            bodyZSmartStruct.SetMin(-1);
            bodyZSmartStruct.SetMax(1);
            bodyZSmartStruct._dataStyle = SmartDataFloatStuct.DataStyle.Free;
            bodyZSmartStruct.displayDecimalAccuracy = 1;
            bodyZSmartStruct._wrapMinMax = false;

            bodyZsmartType._value = bodyZSmartStruct;
            BodyLungeZ.name = grindType.ToString() + " LungeForwardBack";
            bodyZsmartType._label = "Body Lunge Forward Back";
            bodyZsmartType._identifyer = grindName + "LungeForwardBack";
            BodyLungeZ._description = $"The amount you lunge forward or back while in a {grindName} pose";

            BodyLungeZ.SetData(bodyZsmartType);
            container._smartDatas.Add(BodyLungeZ);

        }

        public enum GrindType
        {
            smith, feeble, pegs, crook, crank, luce, unluce, magiccarpet, ice, tooth
        }
    }
}
