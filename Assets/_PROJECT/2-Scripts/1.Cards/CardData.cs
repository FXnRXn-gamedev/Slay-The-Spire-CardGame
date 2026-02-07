using TriInspector;
using UnityEngine;
using System.Collections.Generic;

namespace FXnRXn
{
	/// <summary>
	/// ScriptableObject defining all properties of a card
	/// This is the data-driven approach used in the Bilibili course
	/// </summary>
	[CreateAssetMenu(fileName = "New Card", menuName = "Slay The Spire/Card Data")]
    public class CardData : ScriptableObject
    {
	    // ------------------------------------------ Properties -------------------------------------------------------
	    [Title("Basic Info")]
	    public string cardName;
	    [TextArea(3, 5)] public string description;
	    public Sprite cardArt;
        
	    [Title("Card Properties")]
	    public CardType cardType;
	    public int energyCost;
	    public TargetType targetType;
        
	    [Title("Effects")]
	    public List<CardEffect> effects = new List<CardEffect>();
        
	    [Title("Upgrade Properties")]
	    public bool isUpgraded = false;
	    public CardData upgradedVersion;
        
	    [Title("Visual")]
	    public Color cardColor = Color.white;
	    public GameObject playVFX;
        
	    [Title("Audio")]
	    public AudioClip playSound;


  	    // --------------------------------------------- Func ----------------------------------------------------------
        
        /// <summary>
        /// Get the formatted description with actual values
        /// </summary>
        public string GetFormattedDescription()
        {
	        string result = description;
            
	        // Replace placeholders with actual values
	        foreach (var effect in effects)
	        {
		        if (effect.effectType == EffectType.Damage)
		        {
			        result = result.Replace("{damage}", effect.baseValue.ToString());
		        }
		        else if (effect.effectType == EffectType.Block)
		        {
			        result = result.Replace("{block}", effect.baseValue.ToString());
		        }
		        else if (effect.effectType == EffectType.Heal)
		        {
			        result = result.Replace("{heal}", effect.baseValue.ToString());
		        }
		        else if (effect.effectType == EffectType.Draw)
		        {
			        result = result.Replace("{draw}", effect.baseValue.ToString());
		        }
	        }
            
	        return result;
        }
        
        /// <summary>
        /// Create a runtime copy of this card data
        /// </summary>
        public CardData CreateInstance()
        {
	        return Instantiate(this);
        }
        

    }
}