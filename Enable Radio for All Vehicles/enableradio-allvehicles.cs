﻿using GTA;
using GTA.Native;
using System;
using System.Collections.Generic;

namespace Enable_Radio_for_All_Vehicles
{
    public class Main : Script
    {
        // Settings
        private List<VehicleClass> disableClasses = new List<VehicleClass>();

        private List<VehicleHash> disableHashes = new List<VehicleHash>();

        public Main()
        {
            GetSettings();

            Tick += OnTick;
        }

        private void OnTick(object sender, EventArgs e)
        {
            // Check the conditions of the conditional branch
            var currentVehicle = Game.Player.Character.CurrentVehicle;

            if (currentVehicle == null)
            {
                Function.Call(Hash.SET_MOBILE_RADIO_ENABLED_DURING_GAMEPLAY, false);
                return;
            }

            currentVehicle.IsRadioEnabled = false;

            var isEnable = true;
            var isEngineRunning = currentVehicle.IsEngineRunning;

            var isDisableClass = disableClasses.Contains(currentVehicle.ClassType);

            var isDisbaleHash = disableHashes.Contains((VehicleHash)Game.GenerateHash(currentVehicle.DisplayName));

            if (isDisableClass || isDisbaleHash) isEnable = false;

            // Turn on/off the radio
            if (isEnable && isEngineRunning)
            {
                var prevRadioStation = Game.RadioStation;
                Function.Call(Hash.SET_MOBILE_RADIO_ENABLED_DURING_GAMEPLAY, true);
                Game.RadioStation = prevRadioStation;
            }
            else
            {
                Function.Call(Hash.SET_MOBILE_RADIO_ENABLED_DURING_GAMEPLAY, false);
            }
        }

        private void GetSettings()
        {
            // Get the ini configuration
            var ini = ScriptSettings.Load(@"scripts\enableradio-allvehicles.ini");

            var classes = ini.GetValue<string>("Settings", "DisableClasses", "").Split(',');
            var hashes = ini.GetValue<string>("Settings", "DisableHashes", "").Split(',');

            // Cut out the value and add it to the list
            foreach (var value in classes)
                if (Enum.TryParse(value, out VehicleClass addClass)) disableClasses.Add(addClass);

            foreach (var value in hashes)
                if (Enum.TryParse(value, out VehicleHash addHash)) disableHashes.Add(addHash);
        }
    }
}