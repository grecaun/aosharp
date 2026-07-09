using System.Linq;
using AOSharp.Core;
using AOSharp.Core.Inventory;
using AOSharp.Core.Movement;
using AOSharp.Core.UI;
using SmokeLounge.AOtomation.Messaging.GameData;
using SmokeLounge.AOtomation.Messaging.Messages;
using SmokeLounge.AOtomation.Messaging.Messages.N3Messages;

namespace AutomatonCity
{
    public class CityControllerState : IState
    {
        private double Delay = 0;

        CityState CurrentCityState = new CityState();

        public void OnStateEnter()
        {
            Delay = 0;
            CurrentCityState = CityState.Open;
            Network.N3MessageReceived += CTWindowIsOpenBool;

            Chat.WriteLine("Checking city controller.");
        }

        public IState GetNextState()
        {
            if (Game.IsZoning) return null;

            if (!AutomatonCity._settings["Enable"].AsBool()) return AutomatonCity.Idle_State; ;

            if (!AutomatonCity.validPlayfields.Contains(Playfield.ModelIdentity.Instance)) return AutomatonCity.Idle_State; ;

            if (CurrentCityState != CityState.Done) return null;

            return AutomatonCity.City_Attack_State;
        }

        public void Tick()
        {
            if (Game.IsZoning) return;

            if (Time.AONormalTime < Delay) return;

            Delay = Time.AONormalTime + .25;

            var citycontroller = DynelManager.AllDynels.FirstOrDefault(c => c.Name == "City Controller");

            if (citycontroller == null) return;

            if (citycontroller.DistanceFrom(DynelManager.LocalPlayer) < 7f)
            {
                if (MovementController.Instance.IsNavigating)
                    MovementController.Instance.Halt();

                switch (CurrentCityState)
                {
                    case CityState.Open:

                        //Chat.WriteLine("Opening City Controller.");
                        CityController.Use();
                        CurrentCityState = CityState.Waiting;

                        return;
                    case CityState.Heal:

                        var cru = ControllerRecompilerUnit.Crus.Select(id => Inventory.Find(id, out var item) ? item : null).FirstOrDefault(item => item != null);

                        if (cru == null)
                        {
                            //Chat.WriteLine("No CRU found in inventory.");
                            return;
                        }

                        if (CityController.Charge <= 0.75f)
                            cru?.UseOn(citycontroller.Identity);

                        CurrentCityState = CityState.Waiting;
                        return;
                    case CityState.Cloak:
                        CityController.ToggleCloak();
                        CurrentCityState = CityState.Done;
                        return;
                }
            }
            else if (!MovementController.Instance.IsNavigating)
                MovementController.Instance.SetDestination(citycontroller.Position);
        }

        private enum CityState
        {
            Waiting, Open, Heal, Cloak, Close, Done
        }

        private void CTWindowIsOpenBool(object s, N3Message n3Msg)
        {
            if (n3Msg.N3MessageType != N3MessageType.AOTransportSignal) return;

            var sigMsg = (AOTransportSignalMessage)n3Msg;

            if (sigMsg.Action == AOSignalAction.ChargeInfo)
            {
                var cityCharge = sigMsg.TransportSignalMessage as CityCharge;

                if (cityCharge != null)
                {
                    //Chat.WriteLine($"CityCharge received. Charge={cityCharge.CityControllerCharge}");

                    if (cityCharge.CityControllerCharge < 0.75f)
                    {
                        CurrentCityState = CityState.Heal;
                        //Chat.WriteLine("City state set to Heal.");
                    }
                    else if (CityController.CanToggleCloak())
                    {
                        CurrentCityState = CityState.Cloak;
                        //Chat.WriteLine("City state set to Cloak.");
                    }
                    else
                    {
                        CurrentCityState = CityState.Done;
                        //Chat.WriteLine("City state set to Done.");
                    }
                }
                else
                {
                    //Chat.WriteLine("ChargeInfo received but TransportSignalMessage was not CityCharge.");
                }
            }

            if (sigMsg.Action == AOSignalAction.Close)
            {
                CurrentCityState = CityState.Done;
                //Chat.WriteLine("CT window closed.");
            }
        }

        public static class ControllerRecompilerUnit
        {
            public static readonly int[] Crus = {
                257110, 254364, 305225, 254367, 254359, 258522, 254350, 254329, 254328, 254327, 254326
            };
        }

        public void OnStateExit()
        {
            Network.N3MessageReceived -= CTWindowIsOpenBool;
            Delay = 0;
            CurrentCityState = CityState.Waiting;
        }
    }
}
