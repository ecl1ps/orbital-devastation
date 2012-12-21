using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Client;
using Orbit.Core.SpecialActions;

namespace Orbit.Gui.ActionControllers
{
    /// <summary>
    /// ActionController je kontroller, ktery ovlada chovani BuyActionWindow okna, 
    /// controller bezi kompletne na scene threadu, takze neni treba synchronizovat
    /// </summary>
    public abstract class ActionController<T>
    {
        public ISpecialAction Action { get; set; }
        protected SceneMgr sceneMgr;

        public ActionController(ISpecialAction action, SceneMgr mgr)
        {
            sceneMgr = mgr;
            Action = action;
        }

        public void Enqueue(Action act)
        {
            sceneMgr.Enqueue(act);
        }

        public abstract void ActionClicked(T window);

        public abstract void CreateHeaderText(T window);

        public abstract void CreatePriceText(T window);

        public abstract void CreateImageUriString(T window);
    }
}
