#pragma warning disable 0649
#pragma warning disable 0168
#pragma warning disable 0414

using FXnRXn.Tweening;
using UnityEngine;
using TMPro;
using TriInspector;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;



namespace FXnRXn
{
	/// <summary>
	/// MonoBehaviour for the card prefab
	/// Handles visual display and user interaction
	/// </summary>
    public class Card : MonoBehaviour
    {
	    // ------------------------------------------ Properties -------------------------------------------------------
	    
	    [Title("References")]
	    [SerializeField] private SpriteRenderer cardArtImage;
	    [SerializeField] private SpriteRenderer cardBackgroundImage;
	    [SerializeField] private TMP_Text cardNameText;
	    [SerializeField] private TMP_Text descriptionText;
	    [SerializeField] private TMP_Text energyCostText;
	    [SerializeField] private SpriteRenderer energyCostBackground;
	    [SerializeField] private TMP_Text cardTypeText;
        
	    [Title("Visual Settings")]
	    [SerializeField] private float hoverScale = 1.2f;
	    [SerializeField] private float hoverYOffset = 1f;
	    [SerializeField] private float hoverDuration = 0.2f;

	    [Title("Debug")] 
	    [SerializeField] private bool useDebug = false;
	    
	    // Card data
	    private CardData cardData;

	    // State
	    private bool isHovered = false;
	    private bool isDragging = false;
	    private bool isPlayable = true;
        
	    // Original transform data
	    private Vector3 originalPosition;
	    private Vector3 originalScale;
	    private int originalSiblingIndex;
	    private Transform originalParent;
        
	    // Tweens
	    private Tween hoverTween;
	    private Tween scaleTween;
	    
	    // Camera reference
	    private Camera mainCamera;
	    
	    
	    
	    
	    // ---------------------------------------- Initialization -----------------------------------------------------
	    private void Awake()
	    {
		    mainCamera = Camera.main;
            
		    // Validate required components
		    if (GetComponent<Collider2D>() == null)
		    {
			    DebugSystem.Error($"Card {gameObject.name} is missing a Collider2D component! OnMouse events won't work.");
		    }
	    }
	    
	    /// <summary>
	    /// Initialize the card with data
	    /// </summary>
	    public void Initialize(CardData data)
	    {
		    cardData = data;
		    UpdateVisuals();
	    }
	    
	    
  	    // ---------------------------------------- Functionality ------------------------------------------------------
        
        /// <summary>
        /// Update all visual elements based on card data
        /// </summary>
        private void UpdateVisuals()
        {
	        if (cardData == null) return;
            
	        if(cardNameText) cardNameText.text = cardData.cardName;
	        if(descriptionText) descriptionText.text = cardData.GetFormattedDescription();
	        if(energyCostText) energyCostText.text = cardData.energyCost.ToString();
	        if(cardTypeText) cardTypeText.text = cardData.cardType.ToString();
            
	        if (cardData.cardArt != null)
	        {
		        cardArtImage.sprite = cardData.cardArt;
	        }
            
	        // Set card color based on type
	        if(cardBackgroundImage) cardBackgroundImage.color = cardData.cardColor;
            
	        // Color energy cost based on playability
	        UpdatePlayability();
        }
        
        /// <summary>
        /// Update whether this card can be played
        /// </summary>
        public void UpdatePlayability()
        {
	        // This will be connected to EnergyManager later
	        // For now, just update visual feedback
	        if(energyCostBackground) energyCostBackground.color = isPlayable ? Color.white : Color.gray;
        }
        
        /// <summary>
        /// Set whether this card can be played
        /// </summary>
        public void SetPlayable(bool playable)
        {
	        isPlayable = playable;
	        UpdatePlayability();
        }


    	// ------------------------------------------ Hover Effects ----------------------------------------------------
	    
	    public void OnMouseEnter()
	    {
		    if (isDragging) return;
            
		    isHovered = true;
		    
		    // Store original values
		    originalPosition = transform.localPosition;
		    originalScale = transform.localScale;
		    originalSiblingIndex = transform.GetSiblingIndex();
		    
		    // Move to front
		    transform.SetAsLastSibling();
            
		    // Animate hover
		    hoverTween?.Kill();
		    scaleTween?.Kill();
		    
		    Vector3 targetPos = originalPosition;
		    targetPos.y += hoverYOffset;
		    hoverTween = transform.TweenPosition(targetPos, hoverDuration, true)
			    .SetEase(EaseType.OutQuad);
            
		    scaleTween = transform.TweenScale(originalScale * hoverScale, hoverDuration)
			    .SetEase(EaseType.OutQuad);
		    
		    
		    if (useDebug) DebugSystem.Info($"MouseEnter");
	    }

	    public void OnMouseExit()
	    {
		    if (isDragging) return;
		    
		    isHovered = false;
		    ReturnToOriginalState();
		    
		    if (useDebug) DebugSystem.Info($"MouseExit");
	    }
	    
	    
	    
	    // ------------------------------------------- Drag Handling ---------------------------------------------------

	    public void OnMouseDown()
	    {
		    if (!isPlayable  || cardData == null) return;
		    
		    isDragging = true;
            
		    // Store parent
		    originalParent = transform.parent;
		    
		    // // Move to canvas root for proper rendering
		    // transform.SetParent(transform.root);
		    // transform.SetAsLastSibling();
		    
		    // Cancel hover animations
		    hoverTween?.Kill();
		    scaleTween?.Kill();
		    
		    // Scale up slightly
		    transform.TweenScale(originalScale * 1.1f, 0.1f);
		    
		    // //TODO: Start targeting system
		    // // if (Targeting.TargetingManager.Instance != null)
		    // // {
			   // //  Targeting.TargetingManager.Instance.BeginTargeting(transform.position, cardData);
		    // // }

		    if (useDebug) DebugSystem.Info($"Started dragging: {cardData.cardName}");
	    }
	    

	    public void OnMouseDrag()
	    {
		    if (!isDragging || mainCamera == null) return;
            
		    // Convert mouse position to world position
		    Vector3 mousePos = Mouse.current.delta.ReadValue();
		    mousePos.z = Mathf.Abs(mainCamera.transform.position.z); // Distance from camera
		    Vector3 worldPos = mainCamera.ScreenToWorldPoint(mousePos);
     
		    // Follow mouse
		    transform.position = worldPos;
            
		    // Targeting arrow updates automatically in TargetingManager
	    }

	    public void OnMouseUp()
	    {
		    if (!isDragging) return;
            
		    isDragging = false;
		    
		    // Check if we're over a valid target
		    bool cardPlayed = CheckAndPlayCard();
		    
		    // if (!cardPlayed)
		    // {
			   //  //TODO: Cancel targeting
			   //  // if (Targeting.TargetingManager.Instance != null)
			   //  // {
				  //  //  Targeting.TargetingManager.Instance.CancelTargeting();
			   //  // }
      //           
			   //  // Return to hand
			   //  ReturnToHand();
		    // }
	    }


    	// ---------------------------------------- Private Properties -------------------------------------------------

	    private void ReturnToOriginalState()
	    {
		    hoverTween?.Kill();
		    scaleTween?.Kill();
		    
		    hoverTween = transform.TweenPosition(originalPosition, hoverDuration, true)
			    .SetEase(EaseType.OutQuad);
		    
		    scaleTween = transform.TweenScale(originalScale, hoverDuration)
			    .SetEase(EaseType.OutQuad)
			    .OnComplete(() =>
			    {
				    if (!isHovered && !isDragging)
				    {
					    transform.SetSiblingIndex(originalSiblingIndex);
				    }
			    });
	    }


	    private bool CheckAndPlayCard()
	    {
		    //TODO: Get target from targeting system
		    // if (Targeting.TargetingManager.Instance != null)
		    // {
			   //  Targeting.ITargetable target = Targeting.TargetingManager.Instance.EndTargeting();
      //           
			   //  if (target != null)
			   //  {
				  //   // Use CardPlayer to execute card effects
				  //   if (Combat.CardPlayer.Instance != null)
				  //   {
					 //    bool success = Combat.CardPlayer.Instance.PlayCard(cardData, target);
      //                   
					 //    if (success)
					 //    {
						//     Debug.Log($"Card {cardData.cardName} played successfully!");
						//     PlayCard();
						//     return true;
					 //    }
				  //   }
			   //  }
		    // }
            
		    if(useDebug) DebugSystem.Info($"No valid target for card: {cardData.cardName}");
		    return false;
	    }

	    private void ReturnToHand()
	    {
		    if (originalParent != null)
		    {
			    // Return to original parent
			    transform.SetParent(originalParent);
			    transform.SetSiblingIndex(originalSiblingIndex);
		    }
		    
            
		    // Animate back to position
		    transform.TweenPosition(originalPosition, 0.3f, true).SetEase(EaseType.OutQuad);
		    transform.TweenScale(originalScale, 0.3f).SetEase(EaseType.OutQuad);
	    }
	    
	    // ------------------------------------------ Public Helper ----------------------------------------------------
	    /// <summary>
	    /// Play this card (called by CardPlayer)
	    /// </summary>
	    public void PlayCard()
	    {
		    // Animate card being played
		    transform.TweenScale(Vector3.zero, 0.3f).SetEase(EaseType.InBack);
		    Vector3 playTargetPos = transform.localPosition;
		    playTargetPos.y += 2f;
		    transform.TweenPosition(playTargetPos, 0.3f, true)
			    .OnComplete(() =>
			    {
				    Destroy(gameObject);
			    });
	    }
	    
	    /// <summary>
	    /// Discard this card with animation
	    /// </summary>
	    public void DiscardCard()
	    {
		    transform.TweenScale(Vector3.zero, 0.2f).SetEase(EaseType.InQuad);
		    Vector3 discardTargetPos = transform.localPosition;
		    discardTargetPos.y -= 1f;
		    transform.TweenPosition(discardTargetPos, 0.2f, true)
			    .OnComplete(() =>
			    {
				    Destroy(gameObject);
			    });
	    }


    	// ------------------------------------------- Getters ---------------------------------------------------------
	    
	    public CardData GetCardData() => cardData;
	    public bool IsPlayable() => isPlayable;
	    public bool IsDragging() => isDragging;

	    
    }
}