using FXnRXn.Tweening;
using TriInspector;
using UnityEngine;

namespace FXnRXn
{
	/// <summary>
	/// Arranges cards in hand with smooth layout
	/// Supports both arc and spline-based layouts
	/// Uses custom tweening system (no DOTween dependency)
	/// </summary>
    public class CardHand : Singleton<CardHand>
    {
	    public enum LayoutMode
	    {
		    Arc,
		    Spline,
		    Linear
	    }
	    
	    
	    
	    // ------------------------------------------ Properties -------------------------------------------------------
	    [Title("Mode")]
	    [SerializeField] private LayoutMode layoutMode = LayoutMode.Spline;
	    
	    [Title("Arc Layout Settings")]
	    [ShowIf(nameof(layoutMode), LayoutMode.Arc)] [SerializeField] private float arcRadius = 800f;
	    [ShowIf(nameof(layoutMode), LayoutMode.Arc)] [SerializeField] private float maxArcAngle = 30f;
	    [ShowIf(nameof(layoutMode), LayoutMode.Arc)] [SerializeField] private float cardSpacing = 150f;
	    [ShowIf(nameof(layoutMode), LayoutMode.Arc)] [SerializeField] private float verticalOffset = -400f;
	    
	    [Title("Spline Layout Settings")]
	    [ShowIf(nameof(layoutMode), LayoutMode.Spline)] [SerializeField] private SplinePath splinePath = new SplinePath();
	    [ShowIf(nameof(layoutMode), LayoutMode.Spline)] [SerializeField] private bool useUniformDistribution = true;
	    
	    [Title("Refferences")]
	    [SerializeField] private Transform cardsParent;
	    
	    
	    [Title("Animation")]
	    [SerializeField] private float animationDuration = 0.3f;
	    [SerializeField] private EaseType animationEase = EaseType.OutQuad;
	    [SerializeField] private float staggerDelay = 0.05f; // Delay between each card
        
	    [Title("Card Rotation")]
	    [SerializeField] private bool rotateCards = true;
	    [SerializeField] private float maxRotation = 15f;
	    [SerializeField] private bool useTangentRotation = true; // For spline mode
        
	    [Title("Hover Effect")]
	    [SerializeField] private float hoverScale = 1.1f;
	    [SerializeField] private float hoverYOffset = 0.50f;
	    [SerializeField] private float hoverDuration = 0.2f;
	    
	    [Title("Debug")] 
	    [SerializeField] private bool useDebug = false;
	    [SerializeField] private bool useGizmos = false;

  	    // ---------------------------------------- Unity Callback -----------------------------------------------------
        


    	// --------------------------------------- Public Functionality ------------------------------------------------
	    /// <summary>
	    /// Arrange all child cards based on selected layout mode
	    /// </summary>
	    public void ArrangeCards()
	    {
		    int cardCount = cardsParent != null ? cardsParent.childCount : transform.childCount - 1;
		    if (cardCount == 0) return;
		    
		    switch (layoutMode)
		    {
			    case LayoutMode.Arc:
				    ArrangeCardsArc(cardCount);
				    break;
			    case LayoutMode.Spline:
				    ArrangeCardsSpline(cardCount);
				    break;
			    case LayoutMode.Linear:
				    ArrangeCardsLinear(cardCount);
				    break;
		    }
	    }
	    
	    // --------------------------------------- Card Arrangement ----------------------------------------------------
	    /// <summary>
	    /// Arrange cards in arc layout (original method)
	    /// </summary>
	    private void ArrangeCardsArc(int cardCount)
	    {
		    float angleStep = cardCount > 1 ? maxArcAngle / (cardCount - 1) : 0;
		    float startAngle = -maxArcAngle / 2f;
		    
		    for (int i = 0; i < cardCount; i++)
		    {
			    Transform card = cardsParent.GetChild(i);
			    Transform cardTrans = card;
                
			    if (cardTrans == null) continue;
                
			    // Calculate position on arc
			    float angle = startAngle + (angleStep * i);
			    float angleRad = angle * Mathf.Deg2Rad;
                
			    // Calculate arc position
			    float x = Mathf.Sin(angleRad) * arcRadius;
			    float y = verticalOffset - (Mathf.Cos(angleRad) * arcRadius - arcRadius);
                
			    Vector2 targetPosition = new Vector2(x, y);
                
			    // Calculate rotation
			    Vector3 targetRotation = Vector3.zero;
			    if (rotateCards)
			    {
				    float rotationAngle = Mathf.Lerp(-maxRotation, maxRotation, (float)i / Mathf.Max(1, cardCount - 1));
				    targetRotation = new Vector3(0, 0, rotationAngle);
			    }
                
			    // Animate to position with stagger
			    float delay = i * staggerDelay;
			    cardTrans.TweenPosition(targetPosition, animationDuration)
				    .SetEase(animationEase)
				    .SetDelay(delay);
                    
			    cardTrans.TweenRotation(targetRotation, animationDuration, true)
				    .SetEase(animationEase)
				    .SetDelay(delay);
		    }
	    }

	    /// <summary>
	    /// Arrange cards along spline path (new method)
	    /// </summary>
	    private void ArrangeCardsSpline(int cardCount)
	    {
		    // Mark spline as dirty to recalculate if needed
            splinePath.MarkDirty();
            
            for (int i = 0; i < cardCount; i++)
            {
                Transform card = cardsParent.GetChild(i);
                Transform cardTrans = card;
                
                if (cardTrans == null) continue;
                
                // Calculate normalized position (0-1)
                float t = cardCount > 1 ? (float)i / (cardCount - 1) : 0.5f;
                
                // Get position and rotation from spline
                Vector2 targetPosition;
                float targetRotationAngle;
                
                if (useUniformDistribution)
                {
                    // Evenly distributed along curve length
                    targetPosition = splinePath.GetPointAtDistance(t);
                    targetRotationAngle = splinePath.GetRotationAtDistance(t);
                }
                else
                {
                    // Distributed by parameter t
                    targetPosition = splinePath.GetPoint(t);
                    targetRotationAngle = splinePath.GetRotation(t);
                }
                
                // Apply rotation
                Vector3 targetRotation = Vector3.zero;
                if (rotateCards)
                {
                    if (useTangentRotation)
                    {
                        // Use tangent-based rotation (follows curve)
                        targetRotation = new Vector3(0, 0, targetRotationAngle - 90f); // -90 to align card upright
                    }
                    else
                    {
                        // Use linear interpolation
                        float rotationAngle = Mathf.Lerp(-maxRotation, maxRotation, t);
                        targetRotation = new Vector3(0, 0, rotationAngle);
                    }
                }
                
                // Animate to position with stagger
                float delay = i * staggerDelay;
                cardTrans.TweenPosition(targetPosition, animationDuration)
                    .SetEase(animationEase)
                    .SetDelay(delay);
                    
                cardTrans.TweenRotation(targetRotation, animationDuration, true)
                    .SetEase(animationEase)
                    .SetDelay(delay);
            }
	    }

	    /// <summary>
	    /// Arrange cards in linear layout (simple fallback)
	    /// </summary>
	    private void ArrangeCardsLinear(int cardCount)
	    {
		    float totalWidth = (cardCount - 1) * cardSpacing;
		    float startX = -totalWidth / 2f;
            
		    for (int i = 0; i < cardCount; i++)
		    {
			    Transform card = cardsParent.GetChild(i);
			    Transform cardTrans = card;
                
			    if (cardTrans == null) continue;
                
			    float x = startX + (i * cardSpacing);
			    Vector2 targetPosition = new Vector2(x, verticalOffset);
                
			    // Animate to position with stagger
			    float delay = i * staggerDelay;
			    cardTrans.TweenPosition(targetPosition, animationDuration)
				    .SetEase(animationEase)
				    .SetDelay(delay);
                    
			    // Reset rotation
			    cardTrans.TweenRotation(Vector3.zero, animationDuration, true)
				    .SetEase(animationEase)
				    .SetDelay(delay);
		    }
	    }


    	// ---------------------------------------- public Properties --------------------------------------------------
	    /// <summary>
	    /// Add a card to the hand with animation
	    /// </summary>
	    public void AddCard(GameObject cardObject, bool hasParent = true)
	    {
		    if (hasParent)
		    {
			    cardsParent.transform.SetParent(transform);
		    }
		    else
		    {
			    cardObject.transform.SetParent(transform);
		    }
            
		    // Start from deck position (off-screen bottom)
		    Transform cardTrans = cardObject.transform;
		    cardTrans.localPosition = new Vector2(0, -1000);
		    cardTrans.localScale = Vector3.zero;
            
		    // Animate in with bounce
		    cardTrans.TweenScale(Vector3.one, 0.3f)
			    .SetEase(EaseType.OutBack);
            
		    // Rearrange all cards
		    ArrangeCards();
	    }
	    
	    
	    /// <summary>
	    /// Remove a card from the hand
	    /// </summary>
	    public void RemoveCard(GameObject cardObject, bool hasParent = true)
	    {
		    if (hasParent)
		    {
			    cardsParent.transform.SetParent(null);
		    }
		    else
		    {
			    cardObject.transform.SetParent(null);
		    }
		    ArrangeCards();
	    }
	    
	    
	    /// <summary>
	    /// Animate card hover effect
	    /// </summary>
	    public void OnCardHover(Transform cardTrans, bool isHovering)
	    {
		    if (isHovering)
		    {
			    // Scale up and move up
			    cardTrans.TweenScale(Vector3.one * hoverScale, hoverDuration)
				    .SetEase(EaseType.OutQuad);
                    
			    Vector2 currentPos = cardTrans.localPosition;
			    cardTrans.TweenPosition(currentPos + Vector2.up * hoverYOffset, hoverDuration)
				    .SetEase(EaseType.OutQuad);
		    }
		    else
		    {
			    // Return to normal - rearrange will handle position
			    cardTrans.TweenScale(Vector3.one, hoverDuration)
				    .SetEase(EaseType.OutQuad);
                    
			    ArrangeCards();
		    }
	    }
	    
	    /// <summary>
	    /// Get the number of cards in hand
	    /// </summary>
	    public int GetCardCount()
	    {
		    return cardsParent != null ? cardsParent.childCount : transform.childCount - 1;
	    }
	    
	    
	    /// <summary>
	    /// Set layout mode
	    /// </summary>
	    public void SetLayoutMode(LayoutMode mode)
	    {
		    layoutMode = mode;
		    ArrangeCards();
	    }
	    
	    /// <summary>
	    /// Update spline control points
	    /// </summary>
	    public void UpdateSplinePoints(Vector2 start, Vector2 control1, Vector2 control2, Vector2 end)
	    {
		    splinePath.startPoint = start;
		    splinePath.controlPoint1 = control1;
		    splinePath.controlPoint2 = control2;
		    splinePath.endPoint = end;
		    splinePath.MarkDirty();
            
		    if (layoutMode == LayoutMode.Spline)
		    {
			    ArrangeCards();
		    }
	    }


    	// ------------------------------------------ Helper Method ----------------------------------------------------
	    
	    
	    
	    // -------------------------------------------- Gizmos ---------------------------------------------------------
	    /// <summary>
	    /// Draw gizmos for visualization
	    /// </summary>
	    private void OnDrawGizmos()
	    {
		    if (layoutMode == LayoutMode.Spline)
		    {
			   if(useGizmos) splinePath.DrawGizmos(Color.cadetBlue);
		    }
	    }
	    
	    
	    
	    // ------------------------------------------ Public Getters ---------------------------------------------------

	    public Transform GetCardsParent() => cardsParent != null ? cardsParent : transform;

    }
}