using System;
using Orbit.Scene.Entities;

namespace Orbit.Scene.Controls {

	public class SingularityControl : Control  {

		private float strength;
		private float speed;

		private void Grow() {
			throw new Exception("Not implemented");
		}

		public override void InitControl(ISceneObject me) {
			throw new Exception("Not implemented");
		}

		public override void UpdateControl(float tpf) {
			throw new Exception("Not implemented");
		}
	}
}
