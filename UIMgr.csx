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
    public class TruckPanel : UIPanel
    {
        private static GameObject tp_gameObject;
        private static TruckPanel _instance;
        public static TruckPanel Instance
        {
            get
            {
                if (_instance == null)
                {
                    tp_gameObject = new GameObject("TruckPanel");
                    tp_gameObject.transform.parent = UIView.GetAView().transform;
                    tp_gameObject.AddComponent<TruckPanel>();
                }
                return _instance;
            }
        }

        public const float widthTruck = 60f;  //Width of truck column (coordinate with TruckListPanel)
        public const float widthAmount = 60f;
        public const float widthSource = 240f;
        public const float widthTarget = 240f;  // Sum to 600

        // Window Titlebar
        private UIDragHandle tp_dragHandle;                     // These are set from Start() which
        private UILabel tp_title;                               // is called from outside my
        private UIButton tp_close;                              // namescope, hence appear unused.

        // Line of tabs to change modes
        //private UIButton tp_trucksTab;
        //private UIButton tp_buildingsTab;
        //private UIButton tp_districtsTab;

        // Line of controls such as cargo selection (checkmarks or dropdown?)
        private UIDropDown tp_dropCargo;

        // Header of actual table
        private UIPanel tp_tableHeader;
        private UILabel tp_tableHeaderTruck;
        private UILabel tp_tableHeaderAmount;
        private UILabel tp_tableHeaderSourceB;
        private UILabel tp_tableHeaderTargetB;
        //private UILabel tp_headerStatus;  // Not sure whether feasible.

        //Parameters for the actual container panel
        private readonly List<UIPanel> tp_listLine = new List<UIPanel>();  // readonly applies to the list pointer,
        private readonly List<UILabel> tp_listTruck = new List<UILabel>(); // not the contents of the list.
        private readonly List<UILabel> tp_listAmount = new List<UILabel>();
        private readonly List<UILabel> tp_listSource = new List<UILabel>();
        private readonly List<UILabel> tp_listTarget = new List<UILabel>();
        private ushort tp_listCount = 1; // Actually, points to first free

        private float _timer;
        public float myRefreshInterval = 3.0f;


        public override void Awake()
        {
            _instance = this;
        }

        public override void Start()
        {
            base.Start();
            try
            {
                name = "TruckListPanel";
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

                //tp_trucksTab = CreateTruckButton(this);
                tp_dropCargo = CreateCargoDropDown(this);
                
                CreateTableHeader(this);  // Method stores references, as there are multiple

                // Probably let the data updater create the list items instead of attempting that here.
                //  The vars are static anyway.
            }
            catch (Exception e)
            {
                Debug.Log("CargoSpy - Exception during UIMgr.TruckPanel.Start():");
                Debug.LogException(e);
                //Destroy();
            }
        }
        //public static void Initialize()
        //{
        //    try
        //    {
        //        // Destroy the UI if already exists
        //        //tp_gameObject = GameObject.Find("CargoSpy");
        //        //if (tp_gameObject) DestroyAll();

        //        // Creating our own gameObect, helps finding the UI in ModTools
        //        //tp_gameObject = new GameObject("CargoSpy");
        //        //tp_gameObject.transform.parent = UIView.GetAView().transform;
        //        //Instance = tp_gameObject.AddComponent<CargoSpy.TruckPanel>();
        //    }
        //    catch (Exception e)
        //    {
        //        // Catching any exception to not block the loading process of other mods
        //        Debug.Log("CargoSpy - An exception occurred in TruckPanel.Initialize():");
        //        Debug.LogException(e);
        //    }
        //}
        public override void Update()
        {
            base.Update();
            if (isVisible)
            {
                _timer += Time.deltaTime;

                if (_timer > myRefreshInterval) // ModConfig.Instance.RefreshInterval)
                {
                    _timer -= myRefreshInterval; // ModConfig.Instance.RefreshInterval;
                    UpdateTruckTable();
                }

            }
        }

        public void UpdateTruckTable()  // Separated from Update() so it can be called at will.
        {
            ClearTruckTable();
            byte focusResource = 103;  //Glass
            uint ind = 0;
            string sourceB;
            string targetB;
            for (uint i = 0; i < 16384; i++)
            {
                Vehicle vehicle = VehicleManager.instance.m_vehicles.m_buffer[i];
                // Check if empty entry or actual vehicle?
                byte myType = vehicle.m_transferType;
                if (myType == focusResource) // && vehicle.m_transferSize > 0)
                {
                    ind++;
                    ushort sourceID = vehicle.m_sourceBuilding;
                    if (sourceID != 0)
                    {
                        sourceB = BuildingManager.instance.GetBuildingName(sourceID, InstanceID.Empty);
                    }
                    else
                    {
                        sourceB = "none";
                    }
                    ushort targetID = vehicle.m_targetBuilding;
                    if (targetID != 0)
                    {
                        targetB = BuildingManager.instance.GetBuildingName(targetID, InstanceID.Empty);
                    }
                    else
                    {
                        targetB = "none";
                    }
                    if (sourceB != "none" || targetB != "none")
                    {
                        AddTruckLine(i.ToString(), vehicle.m_transferSize.ToString(), sourceB, targetB);
                    }
                }

            }
        }
        public void ToggleVisible()
        {
            if (isVisible) { Hide(); }
            else { Show(true); }
        }
        public void AddTruckLine(string truck, string amount, string source, string target)
        {
            //UIPanel newLine = CreateLinePanel(this, tp_listCount, truck);
            UIPanel tp_liPanel = this.AddUIComponent<UIPanel>();
            Debug.Log("CargoSpy - Created tp_liPanel");
            tp_liPanel.anchor = UIAnchorStyle.Top | UIAnchorStyle.Left;
            tp_liPanel.height = 25f;
            tp_liPanel.width = 700f;
            tp_liPanel.relativePosition = new Vector3(25f, 105f + (25f * tp_listCount));
            tp_liPanel.backgroundSprite = "InfoviewPanel";
            tp_liPanel.color = tp_listCount % 2 != 0 ? new Color32(56, 61, 63, 255) : new Color32(49, 52, 58, 255);
            tp_liPanel.eventMouseEnter += (component, eventParam) =>
            {
                tp_liPanel.color = new Color32(73, 78, 87, 255);
            };
            tp_liPanel.eventMouseLeave += (component, eventParam) =>
            {
                tp_liPanel.color = tp_listCount % 2 != 0 ? new Color32(56, 61, 63, 255) : new Color32(49, 52, 58, 255);
            };
            tp_liPanel.eventMouseDown += (component, eventParam) =>
            {
                InstanceID inst = InstanceID.Empty;
                ushort truckn = ushort.Parse(truck);
                inst.Vehicle = truckn;
                Vector3 pos = VehicleManager.instance.m_vehicles.m_buffer[truckn].m_segment.a;
                InstanceManager.instance.SelectInstance(inst);
                InstanceManager.instance.FollowInstance(inst);
                ToolsModifierControl.cameraController.SetTarget(inst, pos, false);
                InfoManager.instance.SetCurrentMode(InfoManager.InfoMode.TrafficRoutes, InfoManager.SubInfoMode.Default);
            };
            // Data fields are label children of this
            tp_liPanel.name = "Pnl" + truck;
            tp_listLine.Add(tp_liPanel);
            Debug.Log("CargoSpy - Populated and stored tp_liPanel");

            UILabel tp_liTruck = tp_liPanel.AddUIComponent<UILabel>();
            Debug.Log("CargoSpy - Created tp_liTruck");
            tp_liTruck.textScale = 0.875f;
            tp_liTruck.textColor = new Color32(185, 221, 254, 255);
            tp_liTruck.textAlignment = UIHorizontalAlignment.Right;
            tp_liTruck.verticalAlignment = UIVerticalAlignment.Middle;
            tp_liTruck.autoSize = false;
            tp_liTruck.height = 23f;
            tp_liTruck.width = widthTruck;
            tp_liTruck.relativePosition = new Vector3(5f, 2f);
            tp_liTruck.name = "Truck" + truck;
            tp_liTruck.text = truck;
            tp_listTruck.Add(tp_liTruck);

            UILabel tp_liAmount = tp_liPanel.AddUIComponent<UILabel>();
            tp_liAmount.textScale = 0.875f;
            tp_liAmount.textColor = new Color32(185, 221, 254, 255);
            tp_liAmount.textAlignment = UIHorizontalAlignment.Right;
            tp_liAmount.verticalAlignment = UIVerticalAlignment.Middle;
            tp_liAmount.autoSize = false;
            tp_liAmount.height = 23f;
            tp_liAmount.width = widthAmount;
            tp_liAmount.relativePosition = new Vector3(15f + widthTruck, 2f);
            tp_liAmount.name = "Amount" + truck;
            tp_liAmount.text = amount;
            tp_listAmount.Add(tp_liAmount);

            UILabel tp_liSource = tp_liPanel.AddUIComponent<UILabel>();
            tp_liSource.textScale = 0.875f;
            tp_liSource.textColor = new Color32(185, 221, 254, 255);
            tp_liSource.textAlignment = UIHorizontalAlignment.Right;
            tp_liSource.verticalAlignment = UIVerticalAlignment.Middle;
            tp_liSource.autoSize = false;
            tp_liSource.height = 23f;
            tp_liSource.width = widthSource;
            tp_liSource.relativePosition = new Vector3(25f + widthTruck + widthAmount, 2f);
            tp_liSource.name = "Source" + truck;
            tp_liSource.text = source;
            tp_listSource.Add(tp_liSource);

            UILabel tp_liTarget = tp_liPanel.AddUIComponent<UILabel>();
            tp_liTarget.textScale = 0.875f;
            tp_liTarget.textColor = new Color32(185, 221, 254, 255);
            tp_liTarget.textAlignment = UIHorizontalAlignment.Right;
            tp_liTarget.verticalAlignment = UIVerticalAlignment.Middle;
            tp_liTarget.autoSize = false;
            tp_liTarget.height = 23f;
            tp_liTarget.width = widthTarget;
            tp_liTarget.relativePosition = new Vector3(25f + widthTruck + widthAmount + widthSource, 2f);
            tp_liTarget.name = "Target" + truck;
            tp_liTarget.text = target;
            tp_listTarget.Add(tp_liTarget);

            tp_listCount++;
        }
        public void ClearTruckTable()
        {
            if (tp_listCount == 1) return;  // Table already empty

            for (ushort i = 0; i < tp_listCount-1; i++)
            {
                tp_listTruck[i].parent.RemoveUIComponent(tp_listTruck[i]);
                Destroy(tp_listTruck[i].gameObject);     // Destroy the GameObject that contains this component
                tp_listAmount[i].parent.RemoveUIComponent(tp_listAmount[i]);
                Destroy(tp_listAmount[i].gameObject);
                tp_listSource[i].parent.RemoveUIComponent(tp_listSource[i]);
                Destroy(tp_listSource[i].gameObject);
                tp_listTarget[i].parent.RemoveUIComponent(tp_listTarget[i]);
                Destroy(tp_listTarget[i].gameObject);
                this.RemoveUIComponent(tp_listLine[i]);
                Destroy(tp_listLine[i].gameObject);
            }
            tp_listTarget.Clear();
            tp_listSource.Clear();
            tp_listAmount.Clear();
            tp_listTruck.Clear();
            tp_listLine.Clear();
            tp_listCount = 1;
        }
        private UIDropDown CreateCargoDropDown(UIComponent parent)
        {
            UIDropDown dropCargo = parent.AddUIComponent<UIDropDown>();
            dropCargo.anchor = UIAnchorStyle.Top | UIAnchorStyle.Left; 
            dropCargo.height = 30f;
            dropCargo.width = 250f;
            dropCargo.relativePosition = new Vector3(20f, 20f);
            dropCargo.size = new Vector2(250f, 30f);
            dropCargo.listBackground = "GenericPanelLight";
            dropCargo.itemHeight = 30;
            dropCargo.itemHover = "ListItemHover";
            dropCargo.itemHighlight = "ListItemHighlight";
            dropCargo.normalBgSprite = "ButtonMenu";
            dropCargo.disabledBgSprite = "ButtonMenuDisabled";
            dropCargo.hoveredBgSprite = "ButtonMenuHovered";
            dropCargo.focusedBgSprite = "ButtonMenu";
            dropCargo.listWidth = 90;
            dropCargo.listHeight = 500;
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

            Dictionary<string, byte> cargoTypes = new Dictionary<string, byte>
            {
                { "Animal Products", 97 },
                { "Flour", 98 },
                { "Paper", 99 },
                { "Planed Timber", 100 },
                { "Petroleum", 101 },
                { "Plastics", 102 },
                { "Glass", 103 },
                { "Metals", 104 },
                { "Unique Factory Products", 105 }
            };
            foreach (KeyValuePair<string, byte> kvp in cargoTypes)
            {
                dropCargo.AddItem(kvp.Key);
            }
            return dropCargo;
        }
        private void CreateTableHeader(UIPanel parent)
        {
            tp_tableHeader = parent.AddUIComponent<UIPanel>();
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
            tp_tableHeaderTruck.width = 60f;
            tp_tableHeaderTruck.relativePosition = new Vector3(5f, 2f);

            tp_tableHeaderAmount = tp_tableHeader.AddUIComponent<UILabel>();
            tp_tableHeaderAmount.name = "TP_TableHeaderAmount";
            tp_tableHeaderAmount.text = "Units";
            tp_tableHeaderAmount.textAlignment = UIHorizontalAlignment.Right;
            tp_tableHeaderAmount.verticalAlignment = UIVerticalAlignment.Middle;
            tp_tableHeaderAmount.autoSize = false;
            tp_tableHeaderAmount.height = 23f;
            tp_tableHeaderAmount.width = 60f;
            tp_tableHeaderAmount.relativePosition = new Vector3(15f + 60f, 2f);

            tp_tableHeaderSourceB = tp_tableHeader.AddUIComponent<UILabel>();
            tp_tableHeaderSourceB.name = "TP_TableHeaderSource";
            tp_tableHeaderSourceB.text = "Source";
            tp_tableHeaderSourceB.textAlignment = UIHorizontalAlignment.Left;
            tp_tableHeaderSourceB.verticalAlignment = UIVerticalAlignment.Middle;
            tp_tableHeaderSourceB.autoSize = false;
            tp_tableHeaderSourceB.height = 23f;
            tp_tableHeaderSourceB.width = 240f;
            tp_tableHeaderSourceB.relativePosition = new Vector3(25f + 60f + 60f, 2f);

            tp_tableHeaderTargetB = tp_tableHeader.AddUIComponent<UILabel>();
            tp_tableHeaderTargetB.name = "TP_TableHeaderTarget";
            tp_tableHeaderTargetB.text = "Target";
            tp_tableHeaderTargetB.textAlignment = UIHorizontalAlignment.Left;
            tp_tableHeaderTargetB.verticalAlignment = UIVerticalAlignment.Middle;
            tp_tableHeaderTargetB.autoSize = false;
            tp_tableHeaderTargetB.height = 23f;
            tp_tableHeaderTargetB.width = 240f;
            tp_tableHeaderTargetB.relativePosition = new Vector3(35f + 60f + 60f + 240f, 2f);
            //return tp_tableHeader;
        }
        public void DestroyAll()
        {
            // Destroy created components. Might need to add comps to list during creation, or store in vars
            // in case we get called half way through construction process.
            // For now assume we never need to destroy anything :)
            // Order:
            ClearTruckTable();

            Destroy(tp_dropCargo.gameObject);
            Destroy(tp_tableHeaderTargetB.gameObject);
            Destroy(tp_tableHeaderSourceB.gameObject);
            Destroy(tp_tableHeaderAmount.gameObject);
            Destroy(tp_tableHeaderTruck.gameObject);
            Destroy(tp_tableHeader.gameObject);
            Destroy(tp_dragHandle.gameObject);
            Destroy(tp_title.gameObject);
            Destroy(tp_close.gameObject);

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
        public TransferManager.TransferReason GetCargo()
        {
            byte selection = 103;
            return (TransferManager.TransferReason)selection;
        }
    }
    //public class UIMgr : MonoBehaviour
    //{
    //    // Singleton getter
    //    public static UIMgr Instance { get; private set; }  // Singleton pattern

    //    public static UIComponent m_truckPanel;
    //    //public static ExceptionPanel m_buildingPanel;

    //    public static void CreateGUI()      // Obsolete!!!
    //    {
    //        //if (!m_truckPanel) UIMgr.CreateTruckPanel();
    //        //UIView.library.ShowModal<ExceptionPanel>("ExceptionPanel");
    //        //if (!m_buildingPanel) UIMgr.CreateBuildingPanel();
    //        //Debug.Log("CargoSpy - CreateGUI() was executed");
    //    }
    //}
}
