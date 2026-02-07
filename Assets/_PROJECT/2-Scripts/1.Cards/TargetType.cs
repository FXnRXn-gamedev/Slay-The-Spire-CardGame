namespace FXnRXn
{
	/// <summary>
	/// Defines what the card can target
	/// </summary>
	public enum TargetType
	{
		SingleEnemy,   // Target one enemy
		AllEnemies,    // Hits all enemies
		Self,          // Affects only the player
		None           // No targeting required (e.g., draw cards)
	}
}