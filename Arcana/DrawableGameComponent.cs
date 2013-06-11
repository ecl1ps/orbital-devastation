using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Arcane.Xna.Presentation
{
    public class DrawableGameComponent : GameComponent, IDrawable
    {
        // Fields
        private IGraphicsDeviceService deviceService;
        private int drawOrder;
        private bool initialized;
        private bool visible;

        // Events
        public event EventHandler<EventArgs> DrawOrderChanged;

        public event EventHandler<EventArgs> VisibleChanged;

        // Methods
        public DrawableGameComponent(Game game)
            : base(game)
        {
            this.visible = true;
        }

        private void DeviceCreated(object sender, EventArgs e)
        {
            this.LoadContent();
        }

        private void DeviceDisposing(object sender, EventArgs e)
        {
            this.UnloadContent();
        }

        private void DeviceReset(object sender, EventArgs e)
        {
        }

        private void DeviceResetting(object sender, EventArgs e)
        {
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.UnloadContent();
                if (this.deviceService != null)
                {
                    this.deviceService.DeviceCreated -= new EventHandler<EventArgs>(this.DeviceCreated);
                    this.deviceService.DeviceResetting -= new EventHandler<EventArgs>(this.DeviceResetting);
                    this.deviceService.DeviceReset -= new EventHandler<EventArgs>(this.DeviceReset);
                    this.deviceService.DeviceDisposing -= new EventHandler<EventArgs>(this.DeviceDisposing);
                }
            }
            base.Dispose(disposing);
        }

        public virtual void Draw(GameTime gameTime)
        {
        }

        public override void Initialize()
        {
            base.Initialize();
            if (!this.initialized)
            {
                this.deviceService = base.Game.Services.GetService(typeof(IGraphicsDeviceService)) as IGraphicsDeviceService;
                if (this.deviceService == null)
                {
                    throw new InvalidOperationException(Properties.Resources.MissingGraphicsDeviceService);
                }
                this.deviceService.DeviceCreated += new EventHandler<EventArgs>(this.DeviceCreated);
                this.deviceService.DeviceResetting += new EventHandler<EventArgs>(this.DeviceResetting);
                this.deviceService.DeviceReset += new EventHandler<EventArgs>(this.DeviceReset);
                this.deviceService.DeviceDisposing += new EventHandler<EventArgs>(this.DeviceDisposing);
                if (this.deviceService.GraphicsDevice != null)
                {
                    this.LoadContent();
                }
            }
            this.initialized = true;
        }

        protected virtual void LoadContent()
        {
        }

        protected virtual void OnDrawOrderChanged(object sender, EventArgs args)
        {
            if (this.DrawOrderChanged != null)
            {
                this.DrawOrderChanged(this, args);
            }
        }

        protected virtual void OnVisibleChanged(object sender, EventArgs args)
        {
            if (this.VisibleChanged != null)
            {
                this.VisibleChanged(this, args);
            }
        }

        protected virtual void UnloadContent()
        {
        }

        // Properties
        public int DrawOrder
        {
            get
            {
                return this.drawOrder;
            }
            set
            {
                if (this.drawOrder != value)
                {
                    this.drawOrder = value;
                    this.OnDrawOrderChanged(this, EventArgs.Empty);
                }
            }
        }

        public GraphicsDevice GraphicsDevice
        {
            get
            {
                if (this.deviceService == null)
                {
                    throw new InvalidOperationException(Properties.Resources.PropertyCannotBeCalledBeforeInitialize);
                }
                return this.deviceService.GraphicsDevice;
            }
        }

        public bool Visible
        {
            get
            {
                return this.visible;
            }
            set
            {
                if (this.visible != value)
                {
                    this.visible = value;
                    this.OnVisibleChanged(this, EventArgs.Empty);
                }
            }
        }
    }



}
