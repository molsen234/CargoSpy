using System.Collections.Generic;
using ICities;
using UnityEngine;

namespace CargoSpy
{
    public class CargoSpyThreading : ThreadingExtensionBase
    {
        public static bool hotkeyHandled = false;
        public static uint activations = 1;
        public static float myRefreshInterval = 3f;  // in seconds
        public static bool cstpExists = false;

        public static List<TruckItem> TruckList = new List<TruckItem>();

        private static float _timer;

        public override void OnUpdate(float realTimeDelta, float simulationTimeDelta)
        {
            // Called every frame or simulation tick
            // - Check if hotkey is pressed and react
            if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) && Input.GetKey(KeyCode.K))
            {
                if (!hotkeyHandled)
                {
                    // UnityEngine.Input.GetKey(key) checks if that key is currently activated on the keyboard.
                    // Thus this is not a keystroke event that needs to be processed and then sent off or discarded.
                    Debug.Log("CargoSpy - hotkey was pressed!");
                    CargoSpyPanel.Instance.ToggleVisible();
                    hotkeyHandled = true;
                    activations++;
                }
            }
            else if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) && Input.GetKey(KeyCode.D))
            {
                if(!hotkeyHandled)
                {
                    CargoSpyPanel.Instance.m_hideTransit = (!CargoSpyPanel.Instance.m_hideTransit);
                    CargoSpyPanel.Instance.m_forceRefresh = true;
                    hotkeyHandled = true;
                }
            }
            else
            {
                hotkeyHandled = false;
            }
            if (CargoSpyPanel.Instance.m_forceRefresh)
            {
                PerformUpdates();
                CargoSpyPanel.Instance.m_forceRefresh = false;
                _timer = 0f;
                return;
            }
            _timer += Time.deltaTime;

            if (_timer > myRefreshInterval)
            {
                _timer -= myRefreshInterval;
                if (cstpExists)
                {
                    PerformUpdates();
                }
                else
                {
                    Debug.Log("CargoSpy - Threading almost called CSPanel before it existed!");
                }
            }
        }
        private static void PerformUpdates()
        {
            CargoSpyPanel.Instance.m_selectedBuilding = InstanceManager.instance.GetSelectedInstance().Building;
            if (CargoSpyPanel.Instance.m_selectedBuilding != 0)
            {
                UpdateBuildingTable();
                CargoSpyPanel.Instance.RefreshTruckContainer();
            }
            else
            {
                UpdateTruckTable();  // In method so it can be called at will from other classes.
                CargoSpyPanel.Instance.RefreshTruckContainer();  // Have UI load new truck table
            }
        }

        public static void UpdateTruckTable()  // Separated from Update() so it can be called at will.
        {
            //byte focusResource = 103;  //Glass
            byte focusResource = CargoSpyPanel.Instance.m_selectedResource;
            byte focusResource2 = CargoSpyPanel.Instance.m_selectedResource2;

            if (focusResource == 255) return;

            uint ind = 0;
            string sourceB;
            string targetB;
            InstanceID emptyInst = InstanceID.Empty; //saves some instantiation?

            TruckList.Clear();
            bool hideDummy = false;

            for (ushort i = 0; i < VehicleManager.MAX_VEHICLE_COUNT; i++)  // 16384 items
            {
                Vehicle vehicle = VehicleManager.instance.m_vehicles.m_buffer[i];

                hideDummy = CargoSpyPanel.Instance.m_hideTransit && ((vehicle.m_flags & Vehicle.Flags.DummyTraffic) != 0);
                byte myType = vehicle.m_transferType;
                if (hideDummy == false && (myType == focusResource || (myType == focusResource2 && focusResource2 != 0))) // Should eventually recognize multiple resources (e.g ZI Ore = 14 & 19)
                {
                    ind++;
                    ushort sourceID = vehicle.m_sourceBuilding;
                    if (sourceID != 0)
                    {
                        sourceB = BuildingManager.instance.GetBuildingName(sourceID, emptyInst);
                    }
                    else
                    {
                        sourceB = "none";
                    }
                    ushort targetID = vehicle.m_targetBuilding;
                    if (targetID != 0)
                    {
                        targetB = BuildingManager.instance.GetBuildingName(targetID, emptyInst);
                    }
                    else
                    {
                        targetB = "none";
                    }
                    if (sourceB != "none" || targetB != "none")
                    {
                        TruckItem ti = new TruckItem(i.ToString(), vehicle.m_transferSize, vehicle.m_transferType.ToString(), sourceB, targetB);
                        TruckList.Add(ti);
                    }
                }
            }
        }
        public static void UpdateBuildingTable()
        {
            ushort focusBuilding = CargoSpyPanel.Instance.m_selectedBuilding;
            uint ind = 0;
            string sourceB;
            string targetB;
            InstanceID emptyInst = InstanceID.Empty; //saves some instantiation?

            TruckList.Clear();

            for (ushort i = 0; i < VehicleManager.MAX_VEHICLE_COUNT; i++)
            {
                Vehicle vehicle = VehicleManager.instance.m_vehicles.m_buffer[i];
                ushort sourceID = vehicle.m_sourceBuilding;
                ushort targetID = vehicle.m_targetBuilding;
                if (sourceID == focusBuilding || targetID == focusBuilding) // && vehicle.m_transferSize > 0)
                {
                    ind++;
                    if (sourceID != 0)
                    {
                        sourceB = BuildingManager.instance.GetBuildingName(sourceID, emptyInst);
                    }
                    else
                    {
                        sourceB = "none";
                    }
                    if (targetID != 0)
                    {
                        targetB = BuildingManager.instance.GetBuildingName(targetID, emptyInst);
                    }
                    else
                    {
                        targetB = "none";
                    }
                    if (sourceB != "none" || targetB != "none")
                    {
                        TruckItem ti = new TruckItem(i.ToString(), vehicle.m_transferSize, vehicle.m_transferType.ToString(), sourceB, targetB);
                        TruckList.Add(ti);
                    }
                }
            }
        }
    }
}
