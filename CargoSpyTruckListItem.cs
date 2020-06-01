using ColossalFramework.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICities;
using UnityEngine;
using System.Runtime.CompilerServices;

namespace CargoSpy
{
    public class TruckItem      //Simple DATA instance, NOT for UIFastList directly. See below.

    {
        public string vehicle;
        public string amount;
        public string source;
        public string target;
        
        public TruckItem(string v, string a, string s, string t)
        {
            vehicle = v;
            amount = a;
            source = s;
            target = t;
        }
    }
    public class CargoSpyTruckListItem : UIPanel, IUIFastListRow
    {
        public const float widthTruck = 60f;  //Width of truck column (coordinate with TruckListPanel)
        public const float widthAmount = 60f;
        public const float widthSource = 230f;
        public const float widthTarget = 230f;  // Sum to 580

        private TruckItem myTruck;

        //Parameters for the actual UI container panel
        UIPanel tp_listLine;
        UILabel tp_listTruck;
        UILabel tp_listAmount;
        UILabel tp_listSource;
        UILabel tp_listTarget;

        public void Display(object data, bool isRowOdd)  //Required by UIFastList
        {
            //CreateTruckList
            myTruck = data as TruckItem;

            // IF this is a new row, create UI elements.
            if (tp_listLine == null) BuildTruckLine(isRowOdd);

            // Populate the entry with the provided data.
            tp_listLine.name = "Pnl" + myTruck.vehicle;
            tp_listTruck.name = "Truck" + myTruck.vehicle;
            tp_listTruck.text = myTruck.vehicle;
            tp_listAmount.name = "Amount" + myTruck.vehicle;
            tp_listAmount.text = myTruck.amount;
            tp_listSource.name = "Source" + myTruck.vehicle;
            tp_listSource.text = myTruck.source;
            tp_listTarget.name = "Target" + myTruck.vehicle;
            tp_listTarget.text = myTruck.target;

            return;
        }
        public void Select(bool isRowOdd)  //Required by UIFastList
        {
            return;
        }
        public void Deselect(bool isRowOdd)  //Required by UIFastList
        {
            return;
        }
        public void BuildTruckLine(bool isOdd)
        {
            //UIPanel newLine = CreateLinePanel(this, tp_listCount, truck);
            UIPanel tp_liPanel = this.AddUIComponent<UIPanel>();
            tp_liPanel.anchor = UIAnchorStyle.Top | UIAnchorStyle.Left;
            tp_liPanel.height = 25f;
            tp_liPanel.width = 700f;
            tp_liPanel.relativePosition = new Vector3(0f, 0f);
            tp_liPanel.backgroundSprite = "InfoviewPanel";
            tp_liPanel.color = isOdd ? new Color32(56, 61, 63, 255) : new Color32(49, 52, 58, 255);
            tp_liPanel.eventMouseEnter += (component, eventParam) =>
            {
                tp_liPanel.color = new Color32(73, 78, 87, 255);
            };
            tp_liPanel.eventMouseLeave += (component, eventParam) =>
            {
                tp_liPanel.color = isOdd ? new Color32(56, 61, 63, 255) : new Color32(49, 52, 58, 255);
            };
            tp_liPanel.eventMouseDown += (component, eventParam) =>
            {
                InstanceID inst = InstanceID.Empty;
                ushort truckn = ushort.Parse(myTruck.vehicle);
                inst.Vehicle = truckn;
                Vector3 pos = VehicleManager.instance.m_vehicles.m_buffer[truckn].m_segment.a;
                InstanceManager.instance.SelectInstance(inst);
                InstanceManager.instance.FollowInstance(inst);
                ToolsModifierControl.cameraController.SetTarget(inst, pos, false);
                InfoManager.instance.SetCurrentMode(InfoManager.InfoMode.TrafficRoutes, InfoManager.SubInfoMode.Default);
            };
            // Data fields are label children of this
            tp_liPanel.name = "Pnl";
            tp_listLine = tp_liPanel;

            UILabel tp_liTruck = tp_liPanel.AddUIComponent<UILabel>();
            tp_liTruck.textScale = 0.875f;
            tp_liTruck.textColor = new Color32(185, 221, 254, 255);
            tp_liTruck.textAlignment = UIHorizontalAlignment.Right;
            tp_liTruck.verticalAlignment = UIVerticalAlignment.Middle;
            tp_liTruck.autoSize = false;
            tp_liTruck.height = 23f;
            tp_liTruck.width = widthTruck;
            tp_liTruck.relativePosition = new Vector3(5f, 2f);
            tp_liTruck.name = "Truck";
            tp_liTruck.text = "";
            tp_listTruck = tp_liTruck;

            UILabel tp_liAmount = tp_liPanel.AddUIComponent<UILabel>();
            tp_liAmount.textScale = 0.875f;
            tp_liAmount.textColor = new Color32(185, 221, 254, 255);
            tp_liAmount.textAlignment = UIHorizontalAlignment.Right;
            tp_liAmount.verticalAlignment = UIVerticalAlignment.Middle;
            tp_liAmount.autoSize = false;
            tp_liAmount.height = 23f;
            tp_liAmount.width = widthAmount;
            tp_liAmount.relativePosition = new Vector3(15f + widthTruck, 2f);
            tp_liAmount.name = "Amount";
            tp_liAmount.text = "";
            tp_listAmount = tp_liAmount;

            UILabel tp_liSource = tp_liPanel.AddUIComponent<UILabel>();
            tp_liSource.textScale = 0.875f;
            tp_liSource.textColor = new Color32(185, 221, 254, 255);
            tp_liSource.textAlignment = UIHorizontalAlignment.Left;
            tp_liSource.verticalAlignment = UIVerticalAlignment.Middle;
            tp_liSource.autoSize = false;
            tp_liSource.height = 23f;
            tp_liSource.width = widthSource;
            tp_liSource.relativePosition = new Vector3(25f + widthTruck + widthAmount, 2f);
            tp_liSource.name = "Source";
            tp_liSource.text = "";
            tp_listSource = tp_liSource;

            UILabel tp_liTarget = tp_liPanel.AddUIComponent<UILabel>();
            tp_liTarget.textScale = 0.875f;
            tp_liTarget.textColor = new Color32(185, 221, 254, 255);
            tp_liTarget.textAlignment = UIHorizontalAlignment.Left;
            tp_liTarget.verticalAlignment = UIVerticalAlignment.Middle;
            tp_liTarget.autoSize = false;
            tp_liTarget.height = 23f;
            tp_liTarget.width = widthTarget;
            tp_liTarget.relativePosition = new Vector3(25f + widthTruck + widthAmount + widthSource, 2f);
            tp_liTarget.name = "Target";
            tp_liTarget.text = "";
            tp_listTarget = tp_liTarget;

        }

    }
}
