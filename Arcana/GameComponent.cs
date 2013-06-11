using System;
using Microsoft.Xna.Framework;

namespace Arcane.Xna.Presentation
{

    public class GameComponent : IGameComponent, IUpdateable, IDisposable
    {
        // Fields
        private bool enabled = true;
        private Game game;
        private int updateOrder;

        // Events
        public event EventHandler<EventArgs> Disposed;

        public event EventHandler<EventArgs> EnabledChanged;

        public event EventHandler<EventArgs> UpdateOrderChanged;

        // Methods
        public GameComponent(Game game)
        {
            this.game = game;
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                lock (this)
                {
                    if (this.Game != null)
                    {
                        this.Game.Components.Remove(this);
                    }
                    if (this.Disposed != null)
                    {
                        this.Disposed(this, EventArgs.Empty);
                    }
                }
            }
        }

        ~GameComponent()
        {
            this.Dispose(false);
        }

        public virtual void Initialize()
        {
        }

        protected virtual void OnEnabledChanged(object sender, EventArgs args)
        {
            if (this.EnabledChanged != null)
            {
                this.EnabledChanged(this, args);
            }
        }

        protected virtual void OnUpdateOrderChanged(object sender, EventArgs args)
        {
            if (this.UpdateOrderChanged != null)
            {
                this.UpdateOrderChanged(this, args);
            }
        }

        public virtual void Update(GameTime gameTime)
        {
        }

        // Properties
        public bool Enabled
        {
            get
            {
                return this.enabled;
            }
            set
            {
                if (this.enabled != value)
                {
                    this.enabled = value;
                    this.OnEnabledChanged(this, EventArgs.Empty);
                }
            }
        }

        public Game Game
        {
            get
            {
                return this.game;
            }
        }

        public int UpdateOrder
        {
            get
            {
                return this.updateOrder;
            }
            set
            {
                if (this.updateOrder != value)
                {
                    this.updateOrder = value;
                    this.OnUpdateOrderChanged(this, EventArgs.Empty);
                }
            }
        }
    }
}