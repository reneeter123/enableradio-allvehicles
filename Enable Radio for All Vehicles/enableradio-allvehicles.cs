using GTA;
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
            var player = Game.Player;
            var currentVehicle = player.Character.CurrentVehicle;

            if (currentVehicle == null)
            {
                Function.Call(Hash.SET_MOBILE_RADIO_ENABLED_DURING_GAMEPLAY, false);
                return;
            }

            currentVehicle.IsRadioEnabled = false;

            // Check the conditions of the conditional branch
            var isLive = !player.IsDead;
            var isEnable = true;
            var isEngineRunning = currentVehicle.IsEngineRunning;

            var currentClass = currentVehicle.ClassType;
            var isDisableClass = disableClasses.Contains(currentClass);

            var currentHash = (VehicleHash)Game.GenerateHash(currentVehicle.DisplayName);
            var isDisbaleHash = disableHashes.Contains(currentHash);

            if (isDisableClass || isDisbaleHash) isEnable = false;

            // Turn on/off the radio
            if (isLive && isEnable && isEngineRunning)
            {
                var prevRadio = Game.RadioStation;
                Function.Call(Hash.SET_MOBILE_RADIO_ENABLED_DURING_GAMEPLAY, true);
                Game.RadioStation = prevRadio;
            }
            else
            {
                Function.Call(Hash.SET_MOBILE_RADIO_ENABLED_DURING_GAMEPLAY, false);
            }
        }

        private void GetSettings()
        {
            // Get the ini configuration
            var settingsIni = ScriptSettings.Load(@"scripts\enableradio-allvehicles.ini");

            var origDisableClasses = settingsIni.GetValue<string>("Settings", "DisableClasses", "");
            var origDisableHashes = settingsIni.GetValue<string>("Settings", "DisableHashes", "");

            // If no value can be obtained, exit
            var isNullOrEmptyClass = string.IsNullOrEmpty(origDisableClasses);
            var isNullOrEmptyHash = string.IsNullOrEmpty(origDisableHashes);

            if (isNullOrEmptyClass || isNullOrEmptyHash) return;

            // Cut out the value and add it to the list
            var oDCSepIndex = origDisableClasses.LastIndexOf(", ");
            var oDHSepIndex = origDisableHashes.LastIndexOf(", ");

            while (oDCSepIndex != -1)
            {
                var disableClassString = origDisableClasses.Substring(oDCSepIndex + 2);
                VehicleClass addClass;
                var isClassParsed = Enum.TryParse(disableClassString, out addClass);
                if (isClassParsed)
                {
                    disableClasses.Add(addClass);
                }
                origDisableClasses = origDisableClasses.Remove(oDCSepIndex);

                oDCSepIndex = origDisableClasses.LastIndexOf(", ");
            }

            while (oDHSepIndex != -1)
            {
                var disableHashString = origDisableHashes.Substring(oDHSepIndex + 2);
                VehicleHash addHash;
                var isHashParsed = Enum.TryParse(disableHashString, out addHash);
                if (isHashParsed)
                {
                    disableHashes.Add(addHash);
                }
                origDisableHashes = origDisableHashes.Remove(oDHSepIndex);

                oDHSepIndex = origDisableHashes.LastIndexOf(", ");
            }

            VehicleClass _addClass;
            var _isClassParsed = Enum.TryParse(origDisableClasses, out _addClass);
            if (_isClassParsed)
            {
                disableClasses.Add(_addClass);
            }
            VehicleHash _addHash;
            var _isHashParsed = Enum.TryParse(origDisableHashes, out _addHash);
            if (_isHashParsed)
            {
                disableHashes.Add(_addHash);
            }
        }
    }
}