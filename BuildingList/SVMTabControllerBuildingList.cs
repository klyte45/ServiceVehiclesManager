﻿using ColossalFramework;
using ColossalFramework.UI;
using Klyte.Commons.Extensors;
using Klyte.ServiceVehiclesManager.Extensors.VehicleExt;
using Klyte.ServiceVehiclesManager.Overrides;
using Klyte.ServiceVehiclesManager.Utils;
using System;
using System.Linq;
using UnityEngine;

namespace Klyte.ServiceVehiclesManager.UI
{
    internal abstract class SVMTabControllerBuildingHooks<T, V> : Redirector<T> where T : SVMTabControllerBuildingHooks<T, V> where V : SVMSysDef<V>
    {
        private static SVMTabControllerBuildingHooks<T, V> instance;

        public static void AfterCreateBuilding(bool __result, BuildingInfo info)
        {
            if (__result && SVMTabControllerBuildingList<V>.exists && (Singleton<V>.instance?.GetSSD()?.isFromSystem(info) ?? false))
            {
                SVMTabControllerBuildingList<V>.instance.m_LinesUpdated = false;
            }
        }
        public static void AfterRemoveBuilding(ushort building)
        {
            if (SVMTabControllerBuildingList<V>.exists && (Singleton<V>.instance?.GetSSD()?.isFromSystem(Singleton<BuildingManager>.instance?.m_buildings?.m_buffer?[building].Info) ?? false))
            {
                SVMTabControllerBuildingList<V>.instance.m_LinesUpdated = false;
            }
        }

        public override void AwakeBody()
        {
            instance = this;
            ServiceSystemDefinition def = Singleton<V>.instance.GetSSD();

            var from = typeof(BuildingManager).GetMethod("CreateBuilding", allFlags);
            var to = typeof(SVMTabControllerBuildingHooks<T, V>).GetMethod("AfterCreateBuilding", allFlags);
            var from2 = typeof(BuildingManager).GetMethod("ReleaseBuilding", allFlags);
            var to2 = typeof(SVMTabControllerBuildingHooks<T, V>).GetMethod("AfterRemoveBuilding", allFlags);
            SVMUtils.doLog("Loading After Hooks: {0} ({1}=>{2})", typeof(BuildingManager), from, to);
            SVMUtils.doLog("Loading After Hooks: {0} ({1}=>{2})", typeof(BuildingManager), from2, to2);
            AddRedirect(from, null, to);
            AddRedirect(from2, null, to2);
        }
        public override void doLog(string text, params object[] param)
        {
            SVMUtils.doLog(text, param);
        }
    }
    internal sealed class SVMTabControllerBuildingHooksDisCar : SVMTabControllerBuildingHooks<SVMTabControllerBuildingHooksDisCar, SVMSysDefDisCar> { }
    internal sealed class SVMTabControllerBuildingHooksDisHel : SVMTabControllerBuildingHooks<SVMTabControllerBuildingHooksDisHel, SVMSysDefDisHel> { }
    internal sealed class SVMTabControllerBuildingHooksFirCar : SVMTabControllerBuildingHooks<SVMTabControllerBuildingHooksFirCar, SVMSysDefFirCar> { }
    internal sealed class SVMTabControllerBuildingHooksFirHel : SVMTabControllerBuildingHooks<SVMTabControllerBuildingHooksFirHel, SVMSysDefFirHel> { }
    internal sealed class SVMTabControllerBuildingHooksGarCar : SVMTabControllerBuildingHooks<SVMTabControllerBuildingHooksGarCar, SVMSysDefGarCar> { }
    internal sealed class SVMTabControllerBuildingHooksGbcCar : SVMTabControllerBuildingHooks<SVMTabControllerBuildingHooksGbcCar, SVMSysDefGbcCar> { }
    internal sealed class SVMTabControllerBuildingHooksHcrCar : SVMTabControllerBuildingHooks<SVMTabControllerBuildingHooksHcrCar, SVMSysDefHcrCar> { }
    internal sealed class SVMTabControllerBuildingHooksHcrHel : SVMTabControllerBuildingHooks<SVMTabControllerBuildingHooksHcrHel, SVMSysDefHcrHel> { }
    internal sealed class SVMTabControllerBuildingHooksPolCar : SVMTabControllerBuildingHooks<SVMTabControllerBuildingHooksPolCar, SVMSysDefPolCar> { }
    internal sealed class SVMTabControllerBuildingHooksPolHel : SVMTabControllerBuildingHooks<SVMTabControllerBuildingHooksPolHel, SVMSysDefPolHel> { }
    internal sealed class SVMTabControllerBuildingHooksRoaCar : SVMTabControllerBuildingHooks<SVMTabControllerBuildingHooksRoaCar, SVMSysDefRoaCar> { }
    internal sealed class SVMTabControllerBuildingHooksWatCar : SVMTabControllerBuildingHooks<SVMTabControllerBuildingHooksWatCar, SVMSysDefWatCar> { }
    internal sealed class SVMTabControllerBuildingHooksPriCar : SVMTabControllerBuildingHooks<SVMTabControllerBuildingHooksPriCar, SVMSysDefPriCar> { }
    internal sealed class SVMTabControllerBuildingHooksDcrCar : SVMTabControllerBuildingHooks<SVMTabControllerBuildingHooksDcrCar, SVMSysDefDcrCar> { }
    internal sealed class SVMTabControllerBuildingHooksTaxCar : SVMTabControllerBuildingHooks<SVMTabControllerBuildingHooksTaxCar, SVMSysDefTaxCar> { }
    internal sealed class SVMTabControllerBuildingHooksCcrCcr : SVMTabControllerBuildingHooks<SVMTabControllerBuildingHooksCcrCcr, SVMSysDefCcrCcr> { }
    internal sealed class SVMTabControllerBuildingHooksSnwCar : SVMTabControllerBuildingHooks<SVMTabControllerBuildingHooksSnwCar, SVMSysDefSnwCar> { }
    internal sealed class SVMTabControllerBuildingHooksRegTra : SVMTabControllerBuildingHooks<SVMTabControllerBuildingHooksRegTra, SVMSysDefRegTra> { }
    internal sealed class SVMTabControllerBuildingHooksRegShp : SVMTabControllerBuildingHooks<SVMTabControllerBuildingHooksRegShp, SVMSysDefRegShp> { }
    internal sealed class SVMTabControllerBuildingHooksRegPln : SVMTabControllerBuildingHooks<SVMTabControllerBuildingHooksRegPln, SVMSysDefRegPln> { }
    internal sealed class SVMTabControllerBuildingHooksCrgTra : SVMTabControllerBuildingHooks<SVMTabControllerBuildingHooksCrgTra, SVMSysDefCrgTra> { }
    internal sealed class SVMTabControllerBuildingHooksCrgShp : SVMTabControllerBuildingHooks<SVMTabControllerBuildingHooksCrgShp, SVMSysDefCrgShp> { }
    internal sealed class SVMTabControllerBuildingHooksOutTra : SVMTabControllerBuildingHooks<SVMTabControllerBuildingHooksOutTra, SVMSysDefOutTra> { }
    internal sealed class SVMTabControllerBuildingHooksOutShp : SVMTabControllerBuildingHooks<SVMTabControllerBuildingHooksOutShp, SVMSysDefOutShp> { }
    internal sealed class SVMTabControllerBuildingHooksOutPln : SVMTabControllerBuildingHooks<SVMTabControllerBuildingHooksOutPln, SVMSysDefOutPln> { }
    internal sealed class SVMTabControllerBuildingHooksOutCar : SVMTabControllerBuildingHooks<SVMTabControllerBuildingHooksOutCar, SVMSysDefOutCar> { }
    internal sealed class SVMTabControllerBuildingHooksBeaCar : SVMTabControllerBuildingHooks<SVMTabControllerBuildingHooksBeaCar, SVMSysDefBeaCar> { }



    internal abstract class SVMTabControllerBuildingList<T> : UICustomControl where T : SVMSysDef<T>
    {
        public static SVMTabControllerBuildingList<T> instance { get; private set; }
        public static bool exists
        {
            get { return instance != null; }
        }

        private UIScrollablePanel mainPanel;
        private static readonly string kLineTemplate = "LineTemplate";
        public bool m_LinesUpdated = false;

        #region Awake
        private void Awake()
        {
            instance = this;
            mainPanel = GetComponentInChildren<UIScrollablePanel>();
            mainPanel.autoLayout = true;
            mainPanel.autoLayoutDirection = LayoutDirection.Vertical;
        }
        #endregion

        private void Update()
        {
            if (!mainPanel.isVisible) return;
            if (!this.m_LinesUpdated)
            {
                this.RefreshLines();
            }
        }

        private void AddToList(ushort buildingID, ref int count)
        {
            SVMBuildingInfoItem<T> buildingInfoItem;
            Type implClassBuildingLine = SVMUtils.GetImplementationForGenericType(typeof(SVMBuildingInfoItem<>), typeof(T));
            if (count >= mainPanel.components.Count)
            {
                var temp = UITemplateManager.Get<PublicTransportLineInfo>(kLineTemplate).gameObject;
                GameObject.Destroy(temp.GetComponent<PublicTransportLineInfo>());
                buildingInfoItem = (SVMBuildingInfoItem<T>)temp.AddComponent(implClassBuildingLine);
                mainPanel.AttachUIComponent(buildingInfoItem.gameObject);
            }
            else
            {
                buildingInfoItem = (SVMBuildingInfoItem<T>)mainPanel.components[count].GetComponent(implClassBuildingLine);
            }
            buildingInfoItem.buildingId = buildingID;
            buildingInfoItem.RefreshData();
            count++;
        }

        public void RefreshLines()
        {
            if (Singleton<BuildingManager>.exists)
            {
                int count = 0;
                var buildingList = SVMBuildingUtils.getAllBuildingsFromCity(Singleton<T>.instance.GetSSD());

                SVMUtils.doLog("{0} buildingList = [{1}] (s={2})", GetType(), string.Join(",", buildingList.Select(x => x.ToString()).ToArray()), buildingList.Count);
                foreach (ushort buildingID in buildingList)
                {
                    Building b = Singleton<BuildingManager>.instance.m_buildings.m_buffer[buildingID];
                    var ext = SVMBuildingAIOverrideUtils.getBuildingOverrideExtension(b.Info);
                    var maxCountField = ext.GetVehicleMaxCountField(SVMSysDef<T>.instance.GetSSD().vehicleType);
                    var maxVehicle = SVMUtils.GetPrivateField<int>(b.Info.GetAI(), maxCountField);
                    if (maxCountField == null || maxVehicle > 0)
                    {
                        AddToList(buildingID, ref count);
                    }

                }
                RemoveExtraLines(count);
                SVMUtils.doLog("{0} final count = {1}", GetType(), count);

                m_LinesUpdated = true;
            }
        }

        private void RemoveExtraLines(int linesCount)
        {
            while (mainPanel.components.Count > linesCount)
            {
                UIComponent uIComponent = mainPanel.components[linesCount];
                mainPanel.RemoveUIComponent(uIComponent);
                Destroy(uIComponent.gameObject);
            }
        }
    }
    internal sealed class SVMTabControllerBuildingListDisCar : SVMTabControllerBuildingList<SVMSysDefDisCar> { }
    internal sealed class SVMTabControllerBuildingListDisHel : SVMTabControllerBuildingList<SVMSysDefDisHel> { }
    internal sealed class SVMTabControllerBuildingListFirCar : SVMTabControllerBuildingList<SVMSysDefFirCar> { }
    internal sealed class SVMTabControllerBuildingListFirHel : SVMTabControllerBuildingList<SVMSysDefFirHel> { }
    internal sealed class SVMTabControllerBuildingListGarCar : SVMTabControllerBuildingList<SVMSysDefGarCar> { }
    internal sealed class SVMTabControllerBuildingListGbcCar : SVMTabControllerBuildingList<SVMSysDefGbcCar> { }
    internal sealed class SVMTabControllerBuildingListHcrCar : SVMTabControllerBuildingList<SVMSysDefHcrCar> { }
    internal sealed class SVMTabControllerBuildingListHcrHel : SVMTabControllerBuildingList<SVMSysDefHcrHel> { }
    internal sealed class SVMTabControllerBuildingListPolCar : SVMTabControllerBuildingList<SVMSysDefPolCar> { }
    internal sealed class SVMTabControllerBuildingListPolHel : SVMTabControllerBuildingList<SVMSysDefPolHel> { }
    internal sealed class SVMTabControllerBuildingListRoaCar : SVMTabControllerBuildingList<SVMSysDefRoaCar> { }
    internal sealed class SVMTabControllerBuildingListWatCar : SVMTabControllerBuildingList<SVMSysDefWatCar> { }
    internal sealed class SVMTabControllerBuildingListPriCar : SVMTabControllerBuildingList<SVMSysDefPriCar> { }
    internal sealed class SVMTabControllerBuildingListDcrCar : SVMTabControllerBuildingList<SVMSysDefDcrCar> { }
    internal sealed class SVMTabControllerBuildingListTaxCar : SVMTabControllerBuildingList<SVMSysDefTaxCar> { }
    internal sealed class SVMTabControllerBuildingListCcrCcr : SVMTabControllerBuildingList<SVMSysDefCcrCcr> { }
    internal sealed class SVMTabControllerBuildingListSnwCar : SVMTabControllerBuildingList<SVMSysDefSnwCar> { }
    internal sealed class SVMTabControllerBuildingListRegTra : SVMTabControllerBuildingList<SVMSysDefRegTra> { }
    internal sealed class SVMTabControllerBuildingListRegShp : SVMTabControllerBuildingList<SVMSysDefRegShp> { }
    internal sealed class SVMTabControllerBuildingListRegPln : SVMTabControllerBuildingList<SVMSysDefRegPln> { }
    internal sealed class SVMTabControllerBuildingListCrgTra : SVMTabControllerBuildingList<SVMSysDefCrgTra> { }
    internal sealed class SVMTabControllerBuildingListCrgShp : SVMTabControllerBuildingList<SVMSysDefCrgShp> { }
    internal sealed class SVMTabControllerBuildingListOutTra : SVMTabControllerBuildingList<SVMSysDefOutTra> { }
    internal sealed class SVMTabControllerBuildingListOutShp : SVMTabControllerBuildingList<SVMSysDefOutShp> { }
    internal sealed class SVMTabControllerBuildingListOutPln : SVMTabControllerBuildingList<SVMSysDefOutPln> { }
    internal sealed class SVMTabControllerBuildingListOutCar : SVMTabControllerBuildingList<SVMSysDefOutCar> { }
    internal sealed class SVMTabControllerBuildingListBeaCar : SVMTabControllerBuildingList<SVMSysDefBeaCar> { }

}
