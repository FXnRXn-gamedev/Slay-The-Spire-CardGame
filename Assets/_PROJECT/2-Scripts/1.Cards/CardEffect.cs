using TriInspector;
using UnityEngine;

namespace FXnRXn
{
	/// <summary>
	/// ScriptableObject defining a card's effect (damage, block, heal, etc.)
	/// Supports multiple effects per card
	/// </summary>
	[CreateAssetMenu(fileName = "New Card Effect", menuName = "Slay The Spire/Card Effect")]
	public class CardEffect : ScriptableObject
    {
	    // ------------------------------------------ Properties -------------------------------------------------------
	    [Title("Effect Type")]
	    public EffectType effectType;
        
	    [Title("Effect Values")]
	    public int baseValue;
	    public bool scalesWithStats = true;
        
	    [Title("Status Effect (if applicable)")]
	    public StatusEffectType statusEffect;
	    public int statusStacks = 1;
        
	    [Title("Additional Properties")]
	    public int repeatCount = 1;
	    public bool exhausts = false;

    }
    
    public enum EffectType
    {
	    Damage,          // Deal damage
	    Block,           // Gain block/armor
	    Heal,            // Restore HP
	    Draw,            // Draw cards
	    ApplyStatus,     // Apply buff/debuff
	    Energy,          // Gain energy
	    Discard,         // Discard cards
	    Exhaust          // Remove cards from combat
    }
    
    public enum StatusEffectType
    {
	    None,
	    Strength,        // Increase damage dealt
	    Dexterity,       // Increase block gained
	    Weak,            // Reduce damage dealt by 25%
	    Vulnerable,      // Take 50% more damage
	    Poison,          // Lose HP at turn start
	    Regeneration,    // Gain HP at turn start
	    Frail,           // Reduce block gained by 25%
	    Ritual,          // Gain Strength at turn end
	    Thorns           // Reflect damage
    }
}