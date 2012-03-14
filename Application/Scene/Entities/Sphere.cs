using System;
using System.Drawing;
using System.Windows;

namespace Orbit.Scene.Entities {

	public class Sphere : SceneObject , IMovable , ICollidable  {

		private Color color;
		private Vector direction;
		private uint radius;

		public Vector GetDirection() {
			return direction;
		}

		public bool CollideWith(ICollidable other) {
			throw new Exception("Not implemented");
		}

		public void DoCollideWith(ICollidable other) {
			throw new Exception("Not implemented");
		}

		public override void Render() {
			throw new Exception("Not implemented");
		}

		public override bool IsOnScreen() {
			throw new Exception("Not implemented");
		}
	}

}
