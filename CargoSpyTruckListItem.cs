using ColossalFramework.UI;
using UnityEngine;

namespace CargoSpy
{
    public class TruckItem      //Simple DATA instance, NOT for UIFastList directly. See below.
    {
        public string vehicle;
        public uint amount;
        public string resource;
        public string source;
        public string target;
        
        public TruckItem(string v, uint a, string r, string s, string t)
        {
            vehicle = v;
            amount = a;
            resource = r;
            source = s;
            target = t;
        }
    }
    public class CargoSpyTruckListItem : UIPanel, IUIFastListRow
    {

        private TruckItem myTruck;

        //Parameters for the actual UI container panel
        UIPanel tp_listLine;
        UILabel tp_listTruck;
        UILabel tp_listAmount;
        UILabel tp_listResource;
        UILabel tp_listSource;
        UILabel tp_listTarget;

        public void Display(object data, bool isRowOdd)  //Required by UIFastList
        {
            myTruck = data as TruckItem;

            if (tp_listLine == null) BuildTruckLine(isRowOdd);

            tp_listLine.name = "Pnl" + myTruck.vehicle;
            tp_listTruck.name = "Truck" + myTruck.vehicle;
            tp_listTruck.text = myTruck.vehicle;
            tp_listAmount.name = "Amount" + myTruck.vehicle;
            tp_listAmount.text = myTruck.amount.ToString();
            tp_listResource.name = "Resource" + myTruck.vehicle;
            tp_listResource.text = myTruck.resource;
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
            //tp_liPanel.eventMouseDown += (component, eventParam) =>
            //{
            //    InstanceID inst = InstanceID.Empty;
            //    ushort truckn = ushort.Parse(myTruck.vehicle);
            //    inst.Vehicle = truckn;
            //    Vector3 pos = VehicleManager.instance.m_vehicles.m_buffer[truckn].m_segment.a;
            //    InstanceManager.instance.SelectInstance(inst);
            //    InstanceManager.instance.FollowInstance(inst);
            //    ToolsModifierControl.cameraController.SetTarget(inst, pos, false);
            //    InfoManager.instance.SetCurrentMode(InfoManager.InfoMode.TrafficRoutes, InfoManager.SubInfoMode.Default);
            //};
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
            tp_liTruck.width = CargoSpyPanel.widthTruck;
            tp_liTruck.relativePosition = new Vector3(CargoSpyPanel.innerMargin, 2f);
            tp_liTruck.name = "Truck";
            tp_liTruck.text = "";
            tp_liTruck.eventMouseDown += (component, eventParam) =>
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
            tp_listTruck = tp_liTruck;

            UILabel tp_liAmount = tp_liPanel.AddUIComponent<UILabel>();
            tp_liAmount.textScale = 0.875f;
            tp_liAmount.textColor = new Color32(185, 221, 254, 255);
            tp_liAmount.textAlignment = UIHorizontalAlignment.Right;
            tp_liAmount.verticalAlignment = UIVerticalAlignment.Middle;
            tp_liAmount.autoSize = false;
            tp_liAmount.height = 23f;
            tp_liAmount.width = CargoSpyPanel.widthAmount;
            tp_liAmount.relativePosition = new Vector3(CargoSpyPanel.innerMargin*3 + CargoSpyPanel.widthTruck, 2f);
            tp_liAmount.name = "Amount";
            tp_liAmount.text = "";
            tp_listAmount = tp_liAmount;

            UILabel tp_liResource = tp_liPanel.AddUIComponent<UILabel>();
            tp_liResource.textScale = 0.875f;
            tp_liResource.textColor = new Color32(185, 221, 254, 255);
            tp_liResource.textAlignment = UIHorizontalAlignment.Right;
            tp_liResource.verticalAlignment = UIVerticalAlignment.Middle;
            tp_liResource.autoSize = false;
            tp_liResource.height = 23f;
            tp_liResource.width = CargoSpyPanel.widthResource;
            tp_liResource.relativePosition = new Vector3(CargoSpyPanel.innerMargin * 5 + CargoSpyPanel.widthTruck + CargoSpyPanel.widthAmount, 2f);
            tp_liResource.name = "Resource";
            tp_liResource.text = "";
            tp_listResource = tp_liResource;

            UILabel tp_liSource = tp_liPanel.AddUIComponent<UILabel>();
            tp_liSource.textScale = 0.875f;
            tp_liSource.textColor = new Color32(185, 221, 254, 255);
            tp_liSource.textAlignment = UIHorizontalAlignment.Left;
            tp_liSource.verticalAlignment = UIVerticalAlignment.Middle;
            tp_liSource.autoSize = false;
            tp_liSource.height = 23f;
            tp_liSource.width = CargoSpyPanel.widthSource;
            tp_liSource.relativePosition = new Vector3(CargoSpyPanel.innerMargin * 7 + CargoSpyPanel.widthTruck + CargoSpyPanel.widthAmount + CargoSpyPanel.widthResource, 2f);
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
            tp_liTarget.width = CargoSpyPanel.widthTarget;
            tp_liTarget.relativePosition = new Vector3(CargoSpyPanel.innerMargin * 9 + CargoSpyPanel.widthTruck + CargoSpyPanel.widthAmount + CargoSpyPanel.widthSource + CargoSpyPanel.widthResource, 2f);
            tp_liTarget.name = "Target";
            tp_liTarget.text = "";
            tp_listTarget = tp_liTarget;
        }
    }
}
