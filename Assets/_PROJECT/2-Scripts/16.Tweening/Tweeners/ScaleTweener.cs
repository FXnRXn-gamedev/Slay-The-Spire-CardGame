using UnityEngine;

namespace FXnRXn.Tweening
{
	/// <summary>
	/// Tweens a Transform's scale
	/// </summary>
    public class ScaleTweener : Tween
    {
	    // ------------------------------------------ Properties -------------------------------------------------------
	    private Transform transform;
	    private Vector3 startValue;
	    private Vector3 endValue;


  	    // ---------------------------------------- Functionality ------------------------------------------------------
        public ScaleTweener(Transform transform, Vector3 endValue, float duration) : base(duration)
        {
	        this.transform = transform;
	        this.endValue = endValue;
	        this.startValue = transform.localScale;
        }
        
        public ScaleTweener(Transform transform, float endValue, float duration) : base(duration)
        {
	        this.transform = transform;
	        this.endValue = Vector3.one * endValue;
	        this.startValue = transform.localScale;
        }
        
        protected override void UpdateValue(float t)
        {
	        if (transform == null) return;
            
	        transform.localScale = Vector3.Lerp(startValue, endValue, t);
        }
        
        
        
    	// ------------------------------------------ Helper Method ----------------------------------------------------

    }
}