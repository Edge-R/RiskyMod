﻿namespace EntityStates.RiskyMod.Bandit2.Revolver
{
    public class PrepRackEmUp : BasePrepState
    {
        protected override EntityState GetNextState()
        {
            return new FireRackEmUp();
        }
    }
}
