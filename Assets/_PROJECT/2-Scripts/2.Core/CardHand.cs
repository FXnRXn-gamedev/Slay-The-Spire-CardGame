using Cysharp.Threading.Tasks;
using FXnRXn.Tweening;
using TriInspector;
using UnityEngine;
using UnityEngine.Splines;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FXnRXn
{
	/// <summary>
	/// Arranges cards in hand with smooth layout
	/// Supports both arc and spline-based layouts
	/// Uses custom tweening system (no DOTween dependency)
	/// </summary>
    public class CardHand : Singleton<CardHand>
    {
	    
	    // ------------------------------------------ Properties -------------------------------------------------------
	    
	    [Title("Spline Layout Settings")]
	    [SerializeField] private SplineContainer splineContainer;
	    [SerializeField] private int splineIndex = 0;
	    [SerializeField] private float splineToWorldScale = 0.01f;
	    
	    [Title("Refferences")]
	    [SerializeField] private Transform cardsParent;
	    
	    [Title("Animation")]
	    [SerializeField] private float animationDuration = 0.3f;
	    [SerializeField] private EaseType animationEase = EaseType.OutQuad;
	    [SerializeField] private float staggerDelay = 0.05f; // Delay between each card
	    
	    [Title("Draw Animation")]
	    [SerializeField] private Vector3 deckSpawnPosition = new Vector3(4f, -6f, 0f);
	    [SerializeField] private float baseCardScale = 0.32f;
	    [SerializeField] private float drawStartScaleMultiplier = 0.3f;
	    [field: SerializeField] private float cardSpacing = 0.1f; // Adjustable spacing between cards
	    [field: SerializeField] private float cardDepthOffset = 0.01f; // Z-offset for card layering
	    
	    [Title("Hover Effect")]
	    [SerializeField] private float hoverScale = 1.1f;
	    [SerializeField] private float hoverYOffset = 0.50f;
	    [SerializeField] private float hoverDuration = 0.2f;
	    
	    [Title("Debug")] 
	    [SerializeField] private bool useDebug = false;
	    [SerializeField] private bool useGizmos = false;
	    
	    
	    
	    public readonly List<GameObject> cards = new List<GameObject>();
	    


  	    // ---------------------------------------- Initialization -----------------------------------------------------

        
        // --------------------------------------- Card Arrangement ----------------------------------------------------
        
    	
	    /// <summary>
	    /// Add a card to the hand with animation
	    /// </summary>
	    public async UniTask AddCard(GameObject cardObject, bool hasParent = true)
	    {
		    if (hasParent)
		    {
			    cardObject.transform.SetParent(cardsParent);
		    }
		    else
		    {
			    cardObject.transform.SetParent(transform);
		    }
		    
		    cardObject.transform.localPosition = deckSpawnPosition;
		    cardObject.transform.localScale = Vector3.zero;
		    await cardObject.transform.TweenScale(Vector3.one * baseCardScale, 0.15f).SetEase(EaseType.InOutBounce).ToUniTask();

		    await UniTask.WaitUntil(() =>
		    {
			    cards.Add(cardObject);
			    return true;
		    });
		    
		    
	

		    await UpdateCardPositions(cardObject, 0.25f);
	    }

	    private async UniTask UpdateCardPositions(GameObject cardObject, float duration)
	    {
		    if (!ValidateSpline()) return;
		    if (cards.Count == 0) return;
		    
		    //~===========================================================================
		    Spline spline = splineContainer.Spline;
		    int cardCount = cardsParent.transform.childCount;
		    float centerPosition = 0.5f;
		    float totalSpacing = (cardCount - 1) * cardSpacing;
		    float startPosition = centerPosition - (totalSpacing / 2f);
			
		    List<UniTask> tweenTasks = new List<UniTask>();
		    for (int i = 0; i < cardCount; i++)
		    {
			    float normalizedPosition = startPosition + (i * cardSpacing);
			    normalizedPosition = Mathf.Clamp01(normalizedPosition);
			    Vector3 splinePosition = spline.EvaluatePosition(normalizedPosition);
			    Vector3 tangent = spline.EvaluateTangent(normalizedPosition);
			    Vector3 upVector = spline.EvaluateUpVector(normalizedPosition);
			    Quaternion rotation = Quaternion.LookRotation(-upVector, Vector3.Cross(-upVector, tangent).normalized);
			    Vector3 finalPosition = transform.position + splinePosition + (i * cardDepthOffset * Vector3.back);
			    tweenTasks.Add(cards[i].transform.TweenPosition(finalPosition, duration, false).ToUniTask().ContinueWith(() =>
			    {
				    cards[i].transform.TweenScale(Vector3.one * baseCardScale * drawStartScaleMultiplier, duration).ToUniTask();
			    }));
			    tweenTasks.Add(cards[i].transform.TweenRotation(rotation.eulerAngles, duration, false).ToUniTask());
			    // Also ensure proper scale
			    if (cards[i].transform.localScale != Vector3.one * baseCardScale)
			    {
				    tweenTasks.Add(cards[i].transform.TweenScale(Vector3.one * baseCardScale, duration).ToUniTask());
			    }
			    
			    await Task.Delay((int)(duration * 1000));
		    }
		    await UniTask.WhenAll(tweenTasks);
		    
		    
		    
		    
		    
		    //~===========================================================================
		    
		    
		    
		    // float cardSpacing = 1f / 10f;
		    // float firstCardPosition = 0.5f - (cards.Count - 1) * cardSpacing / 2f;
		    // Spline spline = splineContainer.Spline;
		    //
		    // for (int i = 0; i < cards.Count; i++)
		    // {
			   //  float p = firstCardPosition + i * cardSpacing;
			   //  Vector3 splinePosition = spline.EvaluatePosition(p);
			   //  Vector3 forward = spline.EvaluateTangent(p);
			   //  Vector3 up = spline.EvaluateUpVector(p);
			   //  Quaternion rotation = Quaternion.LookRotation(-up, Vector3.Cross(-up, forward).normalized);
      //           
			   //  cards[i].transform.TweenPosition(splinePosition - 0.01f * i * Vector3.back, duration, true);
			   //  cards[i].transform.TweenRotation(rotation.eulerAngles, duration, true);
			   //  cards[i].transform.TweenScale(Vector3.one * baseCardScale, duration);
			   //  
			   //  // Update sorting order: last card gets highest sorting order
			   //  SpriteRenderer spriteRenderer = cards[i].GetComponent<SpriteRenderer>();
			   //  if (spriteRenderer != null)
			   //  {
				  //   spriteRenderer.sortingOrder = i; // Later cards have higher sorting order
			   //  }
			   //  
			   //  
			   //  await Task.Delay((int)(duration * 1000));
		    // }

		    
	    }
	    
	    
	    
	    
	    
	    
	    
	    // --------------------------------------- Public Functionality ------------------------------------------------
	    
	    
	    /// <summary>
	    /// Remove a card from the hand
	    /// </summary>
	    public void RemoveCard(GameObject cardObject)
	    {
		    cardObject.transform.SetParent(null);
	    }
	    
	    
	    /// <summary>
	    /// Animate card hover effect
	    /// </summary>
	    public void OnCardHover(Transform cardTrans, bool isHovering)
	    {
		    if (isHovering)
		    {
			    cardTrans.TweenScale(Vector3.one * baseCardScale * hoverScale, hoverDuration)
				    .SetEase(EaseType.OutQuad);

			    Vector3 currentPos = cardTrans.localPosition;
			    cardTrans.TweenPosition(currentPos + Vector3.up * (hoverYOffset * splineToWorldScale), hoverDuration, true)
				    .SetEase(EaseType.OutQuad);
		    }
		    else
		    {
			    cardTrans.TweenScale(Vector3.one * baseCardScale, hoverDuration)
				    .SetEase(EaseType.OutQuad);
		    }
	    }
	    
	    /// <summary>
	    /// Get the number of cards in hand
	    /// </summary>
	    public int GetCardCount() => cardsParent != null ? cardsParent.childCount : transform.childCount - 1;
	    
	    /// <summary>
	    /// Set the spline container reference.
	    /// </summary>
	    public void SetSplineContainer(SplineContainer container, int index = 0)
	    {
		    splineContainer = container;
		    splineIndex = index;
	    }
	    
	    

	    // ---------------------------------------- private Properties -------------------------------------------------
	    /// <summary>
	    /// Validate that spline container and index are valid.
	    /// </summary>
	    private bool ValidateSpline()
	    {
		    if (splineContainer == null)
		    {
			    if(useDebug) DebugSystem.Custom("CardHand: SplineContainer is not assigned!", Color.brown);
			    return false;
		    }

		    if (splineIndex < 0 || splineIndex >= splineContainer.Splines.Count)
		    {
			    if(useDebug) DebugSystem.Custom($"CardHand: Invalid spline index {splineIndex}!", Color.brown);
			    return false;
		    }

		    return true;
	    }
	    
	    
	    /// <summary>
	    /// Animate card to target position and rotation.
	    /// </summary>
	    private void AnimateCardToPosition(Transform card, Vector3 position, Vector3 rotation, float delay)
	    {
		    card.TweenPosition(position, animationDuration, true)
			    .SetEase(animationEase)
			    .SetDelay(delay);

		    card.TweenRotation(rotation, animationDuration, true)
			    .SetEase(animationEase)
			    .SetDelay(delay);
	    }


    	// ------------------------------------------ Helper Method ----------------------------------------------------
	    
	    
	    
	    // -------------------------------------------- Gizmos ---------------------------------------------------------

	    
	    
	    // ------------------------------------------ Public Getters ---------------------------------------------------

	    public Transform GetCardsParent() => cardsParent != null ? cardsParent : transform;

    }
}