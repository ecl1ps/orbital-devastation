using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Client;

namespace Orbit.Gui.ActionControllers
{
    /// <summary>
    /// ActionController je kontroller, ktery ovlada chovani BuyActionWindow okna, 
    /// controller bezi kompletne na scene threadu, takze neni treba synchronizovat
    /// </summary>
    public abstract class ActionController
    {
        protected SceneMgr sceneMgr;

        public ActionController(SceneMgr mgr)
        {
            sceneMgr = mgr;
        }

        public void Enqueue(Action act)
        {
            sceneMgr.Enqueue(act);
        }

        public abstract void ActionClicked(BuyActionWindow window);

        public abstract void CreateHeaderText(BuyActionWindow window);

        public abstract void CreatePriceText(BuyActionWindow window);

        public abstract void CreateImageUriString(BuyActionWindow window);
    }
}
