using System;

namespace Orbit.Scene.Entities {

	public interface ICollidable {

		bool CollideWith(ICollidable other);

		void DoCollideWith(ICollidable other);

	}
}
