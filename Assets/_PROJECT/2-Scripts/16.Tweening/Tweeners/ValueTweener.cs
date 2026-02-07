using UnityEngine;
using System;


namespace FXnRXn.Tweening
{
	/// <summary>
	/// Tweens a generic float value
	/// </summary>
    public class ValueTweener : Tween
    {
	    // ------------------------------------------ Properties -------------------------------------------------------
	    private float startValue;
	    private float endValue;
	    private Action<float> onValueChanged;


  	    // ---------------------------------------- Functionality ------------------------------------------------------
        
        public ValueTweener(float startValue, float endValue, float duration, Action<float> onValueChanged) : base(duration)
        {
	        this.startValue = startValue;
	        this.endValue = endValue;
	        this.onValueChanged = onValueChanged;
        }
        
        protected override void UpdateValue(float t)
        {
	        float newValue = startValue + (endValue - startValue) * t;
	        onValueChanged?.Invoke(newValue);
        }
        
    	// ------------------------------------------ Helper Method ----------------------------------------------------

    }
}