using GTA;
using GTA.Native;
using System;

namespace Enable_Radio_for_All_Vehicles
{
    public class Main : Script
    {
        public Main()
        {
            Tick += OnTick;
        }

        private void OnTick(object sender, EventArgs e)
        {
            if (Game.Player.Character.IsInVehicle() && !Game.Player.IsDead)
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
    }
}