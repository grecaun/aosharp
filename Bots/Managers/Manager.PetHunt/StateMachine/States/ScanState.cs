using AOSharp.Common.GameData;
using AOSharp.Core;
using AOSharp.Core.UI;
using System.Collections.Generic;
using System.Linq;

namespace ManagerPetHunt
{
    public class ScanState : IState
    {
        private SimpleChar _target;

        public IState GetNextState()
        {

            if (ManagerPetHunt._settings["Enable"].AsBool())
            {
                if (_target != null)
                {
                    return new PetAttackState(_target);
                }
            }
            else
            {
                return new IdleState();
            }

            return null;
        }

        public void OnStateEnter()
        {
            //Chat.WriteLine("Scanning");
        }

        public void Tick()
        {
            if (ManagerPetHunt._mob.Count >= 1)
            {
                if (ManagerPetHunt._mob.FirstOrDefault().Health == 0) { return; }

                _target = ManagerPetHunt._mob.FirstOrDefault();
                //Chat.WriteLine($"Found _target: {_target.Name}.");
            }
            else if (ManagerPetHunt._bossMob.Count >= 1)
            {
                if (ManagerPetHunt._bossMob.FirstOrDefault().Health == 0) { return; }

                _target = ManagerPetHunt._bossMob.FirstOrDefault();
                //Chat.WriteLine($"Found _target: {_target.Name}.");
            }
        }

        public void OnStateExit()
        {
        }
    }
}
