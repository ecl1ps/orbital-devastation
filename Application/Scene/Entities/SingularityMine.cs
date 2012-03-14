using System;

namespace Orbit.Scene.Entities {

	public class SingularityMine : SceneObject , ICollidable  {

		private int radius;

		public override bool IsOnScreen() {
			throw new Exception("Not implemented");
		}

		public override void Render() {
			throw new Exception("Not implemented");
		}

		public bool CollideWith(ICollidable other) {
			throw new Exception("Not implemented");
		}

		public void DoCollideWith(ICollidable other) {
			throw new Exception("Not implemented");
		}
	}

}
