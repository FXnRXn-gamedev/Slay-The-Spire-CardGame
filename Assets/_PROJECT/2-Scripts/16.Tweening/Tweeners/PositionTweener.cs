using UnityEngine;

namespace FXnRXn.Tweening
{
	/// <summary>
	/// Tweens a Transform's position
	/// </summary>
    public class PositionTweener : Tween
    {
	    // ------------------------------------------ Properties -------------------------------------------------------
	    
	    private Transform transform;
	    private Vector3 startValue;
	    private Vector3 endValue;
	    private bool isLocal;


  	    // ---------------------------------------- Functionality ------------------------------------------------------
        public PositionTweener(Transform transform, Vector3 endValue, float duration, bool isLocal = false) : base(duration)
        {
	        this.transform = transform;
	        this.endValue = endValue;
	        this.isLocal = isLocal;
	        this.startValue = isLocal ? transform.localPosition : transform.position;
        }
        
        protected override void UpdateValue(float t)
        {
	        if (transform == null) return;
            
	        Vector3 newValue = Vector3.Lerp(startValue, endValue, t);
            
	        if (isLocal)
	        {
		        transform.localPosition = newValue;
	        }
	        else
	        {
		        transform.position = newValue;
	        }
        }


    	// ------------------------------------------ Helper Method ----------------------------------------------------

    }
	
	/// <summary>
	/// Tweens a RectTransform's anchored position
	/// </summary>
	public class AnchoredPositionTweener : Tween
	{
		private RectTransform rectTransform;
		private Vector2 startValue;
		private Vector2 endValue;
        
		public AnchoredPositionTweener(RectTransform rectTransform, Vector2 endValue, float duration) : base(duration)
		{
			this.rectTransform = rectTransform;
			this.endValue = endValue;
			this.startValue = rectTransform.anchoredPosition;
		}
        
		protected override void UpdateValue(float t)
		{
			if (rectTransform == null) return;
            
			rectTransform.anchoredPosition = Vector2.Lerp(startValue, endValue, t);
		}
	}
}