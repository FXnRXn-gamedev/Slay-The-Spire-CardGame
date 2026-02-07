using UnityEngine;

namespace FXnRXn.Tweening
{
	/// <summary>
	/// Tweens a Transform's rotation
	/// </summary>
    public class RotationTweener : Tween
    {
	    // ------------------------------------------ Properties -------------------------------------------------------
	    private Transform transform;
	    private Quaternion startValue;
	    private Quaternion endValue;
	    private bool isLocal;


  	    // ---------------------------------------- Functionality ------------------------------------------------------
        
        public RotationTweener(Transform transform, Quaternion endValue, float duration, bool isLocal = false) : base(duration)
        {
	        this.transform = transform;
	        this.endValue = endValue;
	        this.isLocal = isLocal;
	        this.startValue = isLocal ? transform.localRotation : transform.rotation;
        }
        
        public RotationTweener(Transform transform, Vector3 endEuler, float duration, bool isLocal = false) : base(duration)
        {
	        this.transform = transform;
	        this.endValue = Quaternion.Euler(endEuler);
	        this.isLocal = isLocal;
	        this.startValue = isLocal ? transform.localRotation : transform.rotation;
        }
        
        protected override void UpdateValue(float t)
        {
	        if (transform == null) return;
            
	        Quaternion newValue = Quaternion.Slerp(startValue, endValue, t);
            
	        if (isLocal)
	        {
		        transform.localRotation = newValue;
	        }
	        else
	        {
		        transform.rotation = newValue;
	        }
        }


    	// ------------------------------------------ Helper Method ----------------------------------------------------

    }
}