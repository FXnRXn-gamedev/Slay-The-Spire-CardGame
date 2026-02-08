using System;
using UnityEngine;
using System.Collections.Generic;
using TriInspector;

namespace FXnRXn
{
    public class TestCardGenerate : MonoBehaviour
    {
	    // ------------------------------------------ Properties -------------------------------------------------------
	    
	    [Header("References")]
	    [SerializeField] private DeckManager deckManager;
	    [SerializeField] private CardHand cardHand;
	    [SerializeField] private GameObject cardPrefab;
	    
	    
	    // Track visual card GameObjects
	    private Dictionary<CardData, GameObject> cardDataToGameObject = new Dictionary<CardData, GameObject>();


  	    // ---------------------------------------- Unity Callback -----------------------------------------------------

        private void Start()
        {
	        if (deckManager == null) deckManager = DeckManager.Instance;
	        if (cardHand == null) cardHand = CardHand.Instance;
	        if (cardPrefab == null) cardPrefab = Resources.Load<GameObject>("Prefab/Card");

	        if (deckManager == null || cardHand == null || cardPrefab == null)
	        {
		        DebugSystem.Custom("Missing refferences !", Color.brown);
		        return;
	        }
	        
	        if (deckManager != null)
	        {
		        deckManager.OnCardDrawn.AddListener(OnCardDrawn);
		        deckManager.OnCardDiscarded.AddListener(OnCardDiscarded);
	        }
	        

        }

        private void OnDestroy()
        {
	        // Unsubscribe from events
	        if (deckManager != null)
	        {
		        deckManager.OnCardDrawn.RemoveListener(OnCardDrawn);
		        deckManager.OnCardDiscarded.RemoveListener(OnCardDiscarded);
	        }
        }


        // ---------------------------------------- Public Properties --------------------------------------------------
        
        [Button(ButtonSizes.Large)]
        public void DrawStartingHand(int startingHandSize)
        {
	        if (deckManager != null)
	        {
		        deckManager.DrawCards(startingHandSize);
	        }
        }


    	// ---------------------------------------- Private Properties -------------------------------------------------

	    private void OnCardDrawn(CardData cardData)
	    {
		    if (cardData == null)
		    {
			    DebugSystem.Custom("HandManager: Tried to draw null CardData!", Color.brown);
			    return;
		    }
            
		    // Create visual card GameObject
		    GameObject cardObject = Instantiate(cardPrefab);
		    
            
		    // Initialize the Card component with data
		    Card cardComponent = cardObject.GetComponent<Card>();
		    if (cardComponent != null)
		    {
			    cardComponent.Initialize(cardData);
		    }
		    else
		    {
			    DebugSystem.Custom($"HandManager: Card prefab is missing Card component!", Color.brown);
		    }
            
		    // Track the relationship
		    cardDataToGameObject[cardData] = cardObject;
            
		    // Add to hand layout (this will parent it and animate it in)
		    if (cardHand != null)
		    {
			   cardHand.AddCard(cardObject);
		    }
            
		    DebugSystem.Info($"HandManager: Created visual card for {cardData.cardName}");
	    }

	    private void OnCardDiscarded(CardData cardData)
	    {
		    if (cardData == null) return;
            
		    // Find the visual GameObject
		    if (cardDataToGameObject.TryGetValue(cardData, out GameObject cardObject))
		    {
			    // Remove from hand layout
			    if (cardHand != null)
			    {
				    cardHand.RemoveCard(cardObject);
			    }
                
			    // Get Card component and play discard animation
			    Card cardComponent = cardObject.GetComponent<Card>();
			    if (cardComponent != null)
			    {
				    cardComponent.DiscardCard(); // This will destroy the GameObject after animation
			    }
			    else
			    {
				    // No Card component, just destroy immediately
				    Destroy(cardObject);
			    }
                
			    // Remove from tracking
			    cardDataToGameObject.Remove(cardData);
                
			    DebugSystem.Info($"HandManager: Discarded visual card for {cardData.cardName}");
		    }
		    else
		    {
			    DebugSystem.Custom($"HandManager: Tried to discard card {cardData.cardName} but no visual GameObject found!", Color.brown);
		    }
	    }

	    [Button(ButtonSizes.Large)]
	    public void PlayCard()
	    {
		    PlayCard(deckManager.GetRandomCard());
	    }
	    
	    
	    /// <summary>
	    /// Manually play a card (removes from hand and plays animation)
	    /// This should be called by the Card component when it's played
	    /// </summary>
	    public void PlayCard(CardData cardData)
	    {
		    if (cardData == null) return;
            
		    // Find the visual GameObject
		    if (cardDataToGameObject.TryGetValue(cardData, out GameObject cardObject))
		    {
			    // Remove from hand layout
			    if (cardHand != null)
			    {
				    cardHand.RemoveCard(cardObject);
			    }
                
			    // The Card component will handle the play animation and destruction
			    // We just need to remove from tracking
			    cardDataToGameObject.Remove(cardData);
                
			    DebugSystem.Info($"HandManager: Played card {cardData.cardName}");
		    }
	    }
        
	    /// <summary>
	    /// Get the visual GameObject for a CardData
	    /// </summary>
	    public GameObject GetCardObject(CardData cardData)
	    {
		    cardDataToGameObject.TryGetValue(cardData, out GameObject cardObject);
		    return cardObject;
	    }
        
	    /// <summary>
	    /// Clear all cards from hand (useful for end of combat)
	    /// </summary>
	    [Button(ButtonSizes.Large)]
	    public void ClearHand()
	    {
		    foreach (var kvp in cardDataToGameObject)
		    {
			    if (kvp.Value != null)
			    {
				    Destroy(kvp.Value);
			    }
		    }
            
		    cardDataToGameObject.Clear();
	    }


    	// ------------------------------------------ Helper Method ----------------------------------------------------

    }
}