using TriInspector;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

namespace FXnRXn
{
	/// <summary>
	/// Manages the three card piles: Draw Pile, Hand, and Discard Pile
	/// Handles shuffling, drawing, and discarding cards
	/// </summary>
    public class DeckManager : Singleton<DeckManager>
    {
	    // ------------------------------------------ Properties -------------------------------------------------------
	    
	    [Title("Deck Configuration")]
	    [SerializeField] private List<CardData> starterDeck = new List<CardData>();
	    [SerializeField] private int maxHandSize = 10;
	    
	    [Title("Card Piles")]
	    private List<CardData> drawPile = new List<CardData>();
	    private List<CardData> hand = new List<CardData>();
	    private List<CardData> discardPile = new List<CardData>();
	    private List<CardData> exhaustPile = new List<CardData>();
        
	    [Title("Events")]
	    public UnityEvent<CardData> OnCardDrawn;
	    public UnityEvent<CardData> OnCardDiscarded;
	    public UnityEvent OnDeckShuffled;
	    public UnityEvent<int> OnDrawPileChanged;
	    public UnityEvent<int> OnDiscardPileChanged;
	    
	    [Title("Debug")] 
	    [SerializeField] private bool useDebug = false;
	    [SerializeField] private bool useGizmos = false;


  	    // ---------------------------------------- Unity Callback -----------------------------------------------------

        protected override void Awake()
        {
	        base.Awake();
	        InitializeDeck();
        }

        // ---------------------------------------- Initialization -----------------------------------------------------
        /// <summary>
        /// Initialize the deck with starter cards
        /// </summary>
        public void InitializeDeck()
        {
	        drawPile.Clear();
	        hand.Clear();
	        discardPile.Clear();
	        exhaustPile.Clear();
            
	        // Create instances of starter cards
	        foreach (var cardData in starterDeck)
	        {
		        drawPile.Add(cardData.CreateInstance());
	        }
            
	        ShuffleDeck();
        }


    	// ---------------------------------------- Public Properties --------------------------------------------------
	    
	    /// <summary>
	    /// Shuffle the draw pile using Fisher-Yates algorithm
	    /// </summary>
	    public void ShuffleDeck()
	    {
		    for (int i = drawPile.Count - 1; i > 0; i--)
		    {
			    int randomIndex = Random.Range(0, i + 1);
			    CardData temp = drawPile[i];
			    drawPile[i] = drawPile[randomIndex];
			    drawPile[randomIndex] = temp;
		    }
            
		    OnDeckShuffled?.Invoke();
		    OnDrawPileChanged?.Invoke(drawPile.Count);
            
		    if(useDebug) DebugSystem.Info($"Deck shuffled! Draw pile: {drawPile.Count} cards");
	    }


    	// ---------------------------------------- Private Properties -------------------------------------------------


    	// ------------------------------------------ Helper Method ----------------------------------------------------
	    
	    
	    /// <summary>
	    /// Get Random card 
	    /// </summary>
	    public CardData GetRandomCard()
	    {
		    if (drawPile.Count == 0) return null;
		    int randomIndex = Random.Range(0, drawPile.Count);
		    return drawPile[randomIndex];
	    }

    }
}