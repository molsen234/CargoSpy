using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
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
            else
            {
                hotkeyHandled = false;
            }
            _timer += Time.deltaTime;

            if (_timer > myRefreshInterval)
            {
                _timer -= myRefreshInterval;
                if (cstpExists)
                {
                    UpdateTruckTable();  // In method so it can be called at will from other classes.
                    CargoSpyPanel.Instance.RefreshTruckContainer();  // Have UI load new truck table
                }
                else
                {
                    Debug.Log("CargoSpy - Threading almost called CSPanel before it existed!");
                }
            }
        }
        public static void UpdateTruckTable()  // Separated from Update() so it can be called at will.
        {
            //byte focusResource = 103;  //Glass
            byte focusResource = CargoSpyPanel.Instance.m_selectedResource;
            uint ind = 0;
            string sourceB;
            string targetB;
            InstanceID emptyInst = InstanceID.Empty; //saves some instantiation?

            TruckList.Clear();

            for (ushort i = 0; i < VehicleManager.MAX_VEHICLE_COUNT; i++)
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
                        TruckItem ti = new TruckItem(i.ToString(), vehicle.m_transferSize.ToString(), sourceB, targetB);
                        TruckList.Add(ti);
                    }
                }

            }
        }

    }
}
