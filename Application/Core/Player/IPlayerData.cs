using System;

namespace Orbit.Core.Player {

	public interface IPlayerData {

		int GetScore();

		void UpdateScore(int amount);

		int GetBaseIntegrity();

		void UpdateBaseIntegrity(int amount);

		PlayerPosition GetPosition();

	}
}
