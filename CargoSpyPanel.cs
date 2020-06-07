using ColossalFramework.UI;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace CargoSpy
{
    public class CargoSpyPanel : UIPanel
    {
        private static GameObject tp_gameObject;
        private static CargoSpyPanel _instance;
        public static CargoSpyPanel Instance
        {
            get
            {
                if (_instance == null)
                {
                    tp_gameObject = new GameObject("CargoSpyPanel");
                    tp_gameObject.transform.parent = UIView.GetAView().transform;
                    tp_gameObject.AddComponent<CargoSpyPanel>();
                }
                return _instance;
            }
        }

        public const float widthTruck = 60f;
        public const float widthAmount = 60f;
        public const float widthResource = 40f;
        public const float widthSource = 205f;
        public const float widthTarget = 205f;
        public const float innerMargin = 5f;

        public byte m_selectedResource = 103;
        public byte m_selectedResource2 = 0;
        public ushort m_selectedBuilding = 0;
        public bool m_hideTransit = false;
        public string m_sortBy = "Truck";
        public bool m_forceRefresh = false;

        private UIDragHandle tp_dragHandle;
        private UILabel tp_title;
        private UIButton tp_close;
        private UIDropDown tp_dropCargo;
        private UIPanel tp_tableHeader;
        private UILabel tp_tableHeaderTruck;
        private UILabel tp_tableHeaderAmount;
        private UILabel tp_tableHeaderResource;
        private UILabel tp_tableHeaderSourceB;
        private UILabel tp_tableHeaderTargetB;
        private Color32 sortColor = new Color32(185, 221, 254, 255);
        private Color32 headerColor = new Color32(255, 255, 255, 255);

        private UIFastList tp_truckList;

        public override void Awake()
        {
            _instance = this;
            CargoSpyThreading.cstpExists = true;
        }

        public override void Start()
        {
            base.Start();
            try
            {
                name = "CargoSpyPanel";
                backgroundSprite = "UnlockingPanel2"; // or "MenuPanel2"
                isVisible = false;
                canFocus = true;
                isInteractive = true;
                width = 750f;
                height = 650f;
                relativePosition = new Vector3(Mathf.Floor((GetUIView().fixedWidth - width) / 2f), Mathf.Floor((GetUIView().fixedHeight - height) / 2f));

                tp_dragHandle = CreateDragHandle(this);
                tp_title = CreateTitle(this, "Cargo Spy");
                tp_close = CreateCloseButton(this);
                tp_dropCargo = CreateCargoDropDown(this);
                CreateTableHeader(this);

                tp_truckList = UIFastList.Create<CargoSpyTruckListItem>(this);
                tp_truckList.width = 700f;
                tp_truckList.rowHeight = 25f;
                tp_truckList.relativePosition = new Vector3(26f, 125f);
                tp_truckList.autoHideScrollbar = true;

                CargoSpyThreading.UpdateTruckTable();
                RefreshTruckContainer();
            }
            catch (Exception e)
            {
                Debug.Log("CargoSpy - Exception during CargoSpyPanel.Start():");
                Debug.LogException(e);
                //Destroy();
            }
        }
        public void ToggleVisible()
        {
            if (isVisible) { Hide(); }
            else { Show(true); }
        }
        public void RefreshTruckContainer()
        {
            if (m_sortBy == "Truck") CargoSpyThreading.TruckList.Sort((TruckItem x, TruckItem y) => x.vehicle.CompareTo(y.vehicle));
            if (m_sortBy == "Amount") CargoSpyThreading.TruckList.Sort((TruckItem x, TruckItem y) => y.amount.CompareTo(x.amount));
            if (m_sortBy == "Resource") CargoSpyThreading.TruckList.Sort((TruckItem x, TruckItem y) => x.resource.CompareTo(y.resource));
            if (m_sortBy == "Source") CargoSpyThreading.TruckList.Sort((TruckItem x, TruckItem y) => x.source.CompareTo(y.source));
            if (m_sortBy == "Target") CargoSpyThreading.TruckList.Sort((TruckItem x, TruckItem y) => x.target.CompareTo(y.target));

            tp_truckList.rowsData.m_buffer = CargoSpyThreading.TruckList.ToArray();
            tp_truckList.rowsData.m_size = tp_truckList.rowsData.m_buffer.Length;
            tp_truckList.height = Mathf.Min(tp_truckList.rowsData.m_size * tp_truckList.rowHeight, 500f);
            tp_truckList.Refresh();
        }
        private UIDropDown CreateCargoDropDown(UIComponent parent)
        {
            UIDropDown dropCargo = parent.AddUIComponent<UIDropDown>();
            dropCargo.anchor = UIAnchorStyle.Top | UIAnchorStyle.Left;
            dropCargo.height = 30f;
            dropCargo.width = 250f;
            dropCargo.relativePosition = new Vector3(25f, 59f);
            dropCargo.size = new Vector2(250f, 30f);
            dropCargo.listBackground = "GenericPanelLight";
            dropCargo.itemHeight = 30;
            dropCargo.itemHover = "ListItemHover";
            dropCargo.itemHighlight = "ListItemHighlight";
            dropCargo.normalBgSprite = "ButtonMenu";
            dropCargo.disabledBgSprite = "ButtonMenuDisabled";
            dropCargo.hoveredBgSprite = "ButtonMenuHovered";
            dropCargo.focusedBgSprite = "ButtonMenu";
            dropCargo.listWidth = 250;
            dropCargo.listHeight = 720;
            dropCargo.foregroundSpriteMode = UIForegroundSpriteMode.Stretch;
            dropCargo.popupColor = new Color32(45, 52, 61, 255);
            dropCargo.popupTextColor = new Color32(170, 170, 170, 255);
            dropCargo.zOrder = 1;
            dropCargo.textScale = 0.8f;
            dropCargo.verticalAlignment = UIVerticalAlignment.Middle;
            dropCargo.horizontalAlignment = UIHorizontalAlignment.Left;
            dropCargo.selectedIndex = 0;
            dropCargo.textFieldPadding = new RectOffset(8, 0, 8, 0);
            dropCargo.itemPadding = new RectOffset(14, 0, 8, 0);

            UIButton button = dropCargo.AddUIComponent<UIButton>();
            dropCargo.triggerButton = button;
            button.text = "";
            button.size = dropCargo.size;
            button.relativePosition = new Vector3(0f, 0f);
            button.textVerticalAlignment = UIVerticalAlignment.Middle;
            button.textHorizontalAlignment = UIHorizontalAlignment.Left;
            button.normalFgSprite = "IconDownArrow";
            button.hoveredFgSprite = "IconDownArrowHovered";
            button.pressedFgSprite = "IconDownArrowPressed";
            button.focusedFgSprite = "IconDownArrowFocused";
            button.disabledFgSprite = "IconDownArrowDisabled";
            button.foregroundSpriteMode = UIForegroundSpriteMode.Fill;
            button.horizontalAlignment = UIHorizontalAlignment.Right;
            button.verticalAlignment = UIVerticalAlignment.Middle;
            button.zOrder = 0;
            button.textScale = 0.8f;
            dropCargo.eventSizeChanged += new PropertyChangedEventHandler<Vector2>((c, t) =>
            {
                button.size = t; dropCargo.listWidth = (int)t.x;
            });

            Dictionary<string, ushort> cargoTypes = new Dictionary<string, ushort>
            {
                {"none", 255},
                {"Oil", 13},
                {"Ore", 14},
                {"Logs", 15},
                {"Crops", 16},     // Looks like it was called Grain before Industries DLC, but never displayed to user?
                {"Goods", 17},
                {"Coal", 19},
                {"Petrol", 31},
                {"Food", 32},
                {"Lumber", 37},
                {"AnimalProducts", 97},
                {"Flours", 98},
                {"Paper", 99},
                {"PlanedTimber", 100},
                {"Petroleum", 101},
                {"Plastics", 102},
                {"Glass", 103},
                {"Metals", 104},
                {"LuxuryProducts", 105},
                {"Fish", 108},
                {"ZI-Oil", (13 << 8) + 31},      // 3359
                {"ZI-Ore", (14 << 8) + 19},      // 3603
                {"ZI-Forestry", (15 << 8) + 37}, // 3877
                {"ZI-Farming", (16 << 8) + 32}   // 4128
            };
            foreach (KeyValuePair<string, ushort> kvp in cargoTypes)
            {
                dropCargo.AddItem(kvp.Key);
            }
            dropCargo.eventSelectedIndexChanged += (c, e) =>
            {
                cargoTypes.TryGetValue(dropCargo.selectedValue, out ushort myResource);
                if (myResource > 255)  // if top byte present
                {
                    m_selectedResource2 = (byte) ((myResource & 0xFF00)>>8); // Extract top byte
                }
                else
                {
                    m_selectedResource2 = 0;
                }
                m_selectedResource = (byte) (myResource & 0xFF); // Extract low byte
                m_selectedBuilding = 0;
                m_forceRefresh = true;
            };
            dropCargo.selectedIndex = 3;
            dropCargo.selectedValue = "Grain";
            return dropCargo;
        }
        private void CreateTableHeader(UIPanel parent)
        {
            tp_tableHeader = parent.AddUIComponent<UIPanel>();
            tp_tableHeader.name = "TP_TableHeaderPanel";
            tp_tableHeader.anchor = UIAnchorStyle.Top | UIAnchorStyle.Left;
            tp_tableHeader.height = 25f;
            tp_tableHeader.width = parent.width - 50f;
            tp_tableHeader.relativePosition = new Vector3(25f, 100f);

            tp_tableHeaderTruck = tp_tableHeader.AddUIComponent<UILabel>();
            tp_tableHeaderTruck.name = "TP_TableHeaderTruck";
            tp_tableHeaderTruck.text = "Truck";
            tp_tableHeaderTruck.textAlignment = UIHorizontalAlignment.Right;
            tp_tableHeaderTruck.verticalAlignment = UIVerticalAlignment.Middle;
            tp_tableHeaderTruck.autoSize = false;
            tp_tableHeaderTruck.height = 23f;
            tp_tableHeaderTruck.width = widthTruck;
            tp_tableHeaderTruck.relativePosition = new Vector3(innerMargin, 2f);
            tp_tableHeaderTruck.eventMouseDown += (component, eventParam) =>
            {
                m_sortBy = "Truck";
                tp_tableHeaderTruck.textColor = sortColor;
                tp_tableHeaderAmount.textColor = headerColor;
                tp_tableHeaderResource.textColor = headerColor;
                tp_tableHeaderSourceB.textColor = headerColor;
                tp_tableHeaderTargetB.textColor = headerColor;
                RefreshTruckContainer();
            };

            tp_tableHeaderAmount = tp_tableHeader.AddUIComponent<UILabel>();
            tp_tableHeaderAmount.name = "TP_TableHeaderAmount";
            tp_tableHeaderAmount.text = "Units";
            tp_tableHeaderAmount.textAlignment = UIHorizontalAlignment.Right;
            tp_tableHeaderAmount.verticalAlignment = UIVerticalAlignment.Middle;
            tp_tableHeaderAmount.autoSize = false;
            tp_tableHeaderAmount.height = 23f;
            tp_tableHeaderAmount.width = widthAmount;
            tp_tableHeaderAmount.relativePosition = new Vector3(innerMargin*3 + widthTruck, 2f);
            tp_tableHeaderAmount.eventMouseDown += (component, eventParam) =>
            {
                m_sortBy = "Amount";
                tp_tableHeaderTruck.textColor = headerColor;
                tp_tableHeaderAmount.textColor = sortColor;
                tp_tableHeaderResource.textColor = headerColor;
                tp_tableHeaderSourceB.textColor = headerColor;
                tp_tableHeaderTargetB.textColor = headerColor;
                RefreshTruckContainer();
            };

            tp_tableHeaderResource = tp_tableHeader.AddUIComponent<UILabel>();
            tp_tableHeaderResource.name = "TP_TableHeaderResource";
            tp_tableHeaderResource.text = "Res.";
            tp_tableHeaderResource.textAlignment = UIHorizontalAlignment.Right;
            tp_tableHeaderResource.verticalAlignment = UIVerticalAlignment.Middle;
            tp_tableHeaderResource.autoSize = false;
            tp_tableHeaderResource.height = 23f;
            tp_tableHeaderResource.width = widthResource;
            tp_tableHeaderResource.relativePosition = new Vector3(innerMargin*5 + widthTruck + widthAmount, 2f);
            tp_tableHeaderResource.eventMouseDown += (component, eventParam) =>
            {
                m_sortBy = "Resource";
                tp_tableHeaderTruck.textColor = headerColor;
                tp_tableHeaderAmount.textColor = headerColor;
                tp_tableHeaderResource.textColor = sortColor;
                tp_tableHeaderSourceB.textColor = headerColor;
                tp_tableHeaderTargetB.textColor = headerColor;
                RefreshTruckContainer();
            };

            tp_tableHeaderSourceB = tp_tableHeader.AddUIComponent<UILabel>();
            tp_tableHeaderSourceB.name = "TP_TableHeaderSource";
            tp_tableHeaderSourceB.text = "Source";
            tp_tableHeaderSourceB.textAlignment = UIHorizontalAlignment.Left;
            tp_tableHeaderSourceB.verticalAlignment = UIVerticalAlignment.Middle;
            tp_tableHeaderSourceB.autoSize = false;
            tp_tableHeaderSourceB.height = 23f;
            tp_tableHeaderSourceB.width = widthSource;
            tp_tableHeaderSourceB.relativePosition = new Vector3(innerMargin*7 + widthTruck + widthAmount + widthResource, 2f);
            tp_tableHeaderSourceB.eventMouseDown += (component, eventParam) =>
            {
                m_sortBy = "Source";
                tp_tableHeaderTruck.textColor = headerColor;
                tp_tableHeaderAmount.textColor = headerColor;
                tp_tableHeaderResource.textColor = headerColor;
                tp_tableHeaderSourceB.textColor = sortColor;
                tp_tableHeaderTargetB.textColor = headerColor;
                RefreshTruckContainer();
            };

            tp_tableHeaderTargetB = tp_tableHeader.AddUIComponent<UILabel>();
            tp_tableHeaderTargetB.name = "TP_TableHeaderTarget";
            tp_tableHeaderTargetB.text = "Target";
            tp_tableHeaderTargetB.textAlignment = UIHorizontalAlignment.Left;
            tp_tableHeaderTargetB.verticalAlignment = UIVerticalAlignment.Middle;
            tp_tableHeaderTargetB.autoSize = false;
            tp_tableHeaderTargetB.height = 23f;
            tp_tableHeaderTargetB.width = widthTarget;
            tp_tableHeaderTargetB.relativePosition = new Vector3(innerMargin*9 + widthTruck + widthAmount + widthResource + widthSource, 2f);
            tp_tableHeaderTargetB.eventMouseDown += (component, eventParam) =>
            {
                m_sortBy = "Target";
                tp_tableHeaderTruck.textColor = headerColor;
                tp_tableHeaderAmount.textColor = headerColor;
                tp_tableHeaderResource.textColor = headerColor;
                tp_tableHeaderSourceB.textColor = headerColor;
                tp_tableHeaderTargetB.textColor = sortColor;
                RefreshTruckContainer();
            };
            // Add panel here to contain the list. 
        }

        private UIDragHandle CreateDragHandle(UIPanel parent)
        {
            UIDragHandle dragHandle = parent.AddUIComponent<UIDragHandle>();
            dragHandle.name = "DragHandle";
            dragHandle.width = parent.width - 40f; // Make room for close button?
            dragHandle.height = 40f;
            dragHandle.relativePosition = Vector3.zero;
            dragHandle.target = parent;
            return dragHandle;
        }
        private UILabel CreateTitle(UIComponent parent, string title)
        {
            UILabel label = parent.AddUIComponent<UILabel>();
            label.name = "Title";
            label.text = title;
            label.textAlignment = UIHorizontalAlignment.Center;
            label.relativePosition = new Vector3(parent.width / 2f - label.width / 2f, 11f);
            return label;
        }
        private UIButton CreateCloseButton(UIComponent parent)
        {
            UIButton button = parent.AddUIComponent<UIButton>();
            button.name = "CloseButton";
            button.relativePosition = new Vector3(parent.width - 37f, 2f);
            button.normalBgSprite = "buttonclose";
            button.hoveredBgSprite = "buttonclosehover";
            button.pressedBgSprite = "buttonclosepressed";
            button.eventClick += (component, eventParam) =>
            {
                parent.Hide();
            };
            return button;
        }
        public void DestroyAll()
        {
            Destroy(tp_tableHeaderTargetB.gameObject);
            Destroy(tp_tableHeaderSourceB.gameObject);
            Destroy(tp_tableHeaderResource.gameObject);
            Destroy(tp_tableHeaderAmount.gameObject);
            Destroy(tp_tableHeaderTruck.gameObject);
            Destroy(tp_tableHeader.gameObject);
            Destroy(tp_dropCargo.gameObject);
            Destroy(tp_dragHandle.gameObject);
            Destroy(tp_title.gameObject);
            Destroy(tp_close.gameObject);
        }
    }
}
