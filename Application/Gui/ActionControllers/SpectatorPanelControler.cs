using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Gui.InteractivePanel;
using Orbit.Core.SpecialActions.Spectator;
using Orbit.Core;

namespace Orbit.Gui.ActionControllers
{
    class SpectatorPanelControler : IGameState
    {
        private int size;

        private IInteractivePanel rootPanel;
        private List<ISpectatorAction> actions;

        public SpectatorPanelControler(IInteractivePanel panel, List<ISpectatorAction> actions, int size)
        {
            this.rootPanel = panel;
            this.actions = actions;
            this.size = size;
        }

        public void Update(float tpf)
        {
            actions.Sort(Compare);
            rootPanel.ClearAll();

            for (int i = 0; i < size; i++)
                rootPanel.AddItem(createActionOverview(actions[i]));
        }

        public int Compare(ISpectatorAction a1, ISpectatorAction a2)
        {
            return a1.Percentage.CompareTo(a2.Percentage);
        }

        private ActionOverview createActionOverview(ISpectatorAction action)
        {
            ActionOverview a = new ActionOverview();
            a.RenderAction(action);
            return a;
        }
    }
}
