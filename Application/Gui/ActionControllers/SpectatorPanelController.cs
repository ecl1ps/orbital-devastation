using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Gui.InteractivePanel;
using Orbit.Core.SpecialActions.Spectator;
using Orbit.Core;
using Orbit.Core.Helpers;
using Orbit.Core.Client;

namespace Orbit.Gui.ActionControllers
{
    class SpectatorPanelController : IGameState
    {
        private int size;

        private IInteractivePanel<ActionOverview> rootPanel;
        private List<ISpectatorAction> actions;
        private SceneMgr mgr;

        public SpectatorPanelController(SceneMgr mgr, IInteractivePanel<ActionOverview> panel, List<ISpectatorAction> actions, int size)
        {
            this.rootPanel = panel;
            this.actions = actions;
            this.size = size;
            this.mgr = mgr;

            ISpectatorAction action = null;
            for (int i = 0; i < size; i++)
            {
                if (i < actions.Count)
                    action = actions[i];
                else
                    action = null;
                
                GuiObjectFactory.CreateAndAddActionOverview(mgr, rootPanel, action);
            }
        }

        public void Update(float tpf)
        {
            mgr.BeginInvoke(new Action(() =>
            {
                rootPanel.ClearAll();

                actions.Sort(Compare);

                int count = actions.Count;
                if (count > size)
                    count = size;
           
                for (int i = 0; i < count; i++)
                    if (rootPanel.getItem(i) != null && actions[i].Percentage != 1)
                        rootPanel.getItem(i).RenderAction(actions[i]);
            }));
        }

        public int Compare(ISpectatorAction a1, ISpectatorAction a2)
        {
            int c = a1.Percentage.CompareTo(a2.Percentage);
            if(c == 0)
                c = a1.Name.CompareTo(a2.Name);

            return c; 
        }
    }
}
