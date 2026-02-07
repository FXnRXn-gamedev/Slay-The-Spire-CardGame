using UnityEngine;
using System;

namespace FXnRXn.Tweening
{
	/// <summary>
	/// Easing function types for tweening animations
	/// </summary>
	public enum EaseType
	{
		Linear,
		InQuad, OutQuad, InOutQuad,
		InCubic, OutCubic, InOutCubic,
		InQuart, OutQuart, InOutQuart,
		InSine, OutSine, InOutSine,
		InExpo, OutExpo, InOutExpo,
		InBack, OutBack, InOutBack,
		InElastic, OutElastic, InOutElastic,
		InBounce, OutBounce, InOutBounce
	}
	
	
	/// <summary>
	/// Collection of easing functions for smooth animations
	/// All functions take t (0-1) and return eased value (0-1)
	/// </summary>
    public class Easing
    {
	    // ------------------------------------------ Const Properties -------------------------------------------------
	    
	    private const float PI = 3.14159265359f;
	    private const float HALF_PI = 1.57079632679f;


	    /// <summary>
	    /// Get easing function by type
	    /// </summary>
	    public static Func<float, float> GetEasingFunction(EaseType easeType)
	    {
		    switch (easeType)
		    {
			    case EaseType.Linear: return Linear;
                
			    case EaseType.InQuad: return InQuad;
			    case EaseType.OutQuad: return OutQuad;
			    case EaseType.InOutQuad: return InOutQuad;
                
			    case EaseType.InCubic: return InCubic;
			    case EaseType.OutCubic: return OutCubic;
			    case EaseType.InOutCubic: return InOutCubic;
                
			    case EaseType.InQuart: return InQuart;
			    case EaseType.OutQuart: return OutQuart;
			    case EaseType.InOutQuart: return InOutQuart;
                
			    case EaseType.InSine: return InSine;
			    case EaseType.OutSine: return OutSine;
			    case EaseType.InOutSine: return InOutSine;
                
			    case EaseType.InExpo: return InExpo;
			    case EaseType.OutExpo: return OutExpo;
			    case EaseType.InOutExpo: return InOutExpo;
                
			    case EaseType.InBack: return InBack;
			    case EaseType.OutBack: return OutBack;
			    case EaseType.InOutBack: return InOutBack;
                
			    case EaseType.InElastic: return InElastic;
			    case EaseType.OutElastic: return OutElastic;
			    case EaseType.InOutElastic: return InOutElastic;
                
			    case EaseType.InBounce: return InBounce;
			    case EaseType.OutBounce: return OutBounce;
			    case EaseType.InOutBounce: return InOutBounce;
                
			    default: return Linear;
		    }
	    }
	    


    	// ------------------------------------------ Helper Method ----------------------------------------------------
	    
	    // Linear
        public static float Linear(float t) => t;
        
        // Quadratic
        public static float InQuad(float t) => t * t;
        public static float OutQuad(float t) => t * (2f - t);
        public static float InOutQuad(float t) => t < 0.5f ? 2f * t * t : -1f + (4f - 2f * t) * t;
        
        // Cubic
        public static float InCubic(float t) => t * t * t;
        public static float OutCubic(float t) => (--t) * t * t + 1f;
        public static float InOutCubic(float t) => t < 0.5f ? 4f * t * t * t : (t - 1f) * (2f * t - 2f) * (2f * t - 2f) + 1f;
        
        // Quartic
        public static float InQuart(float t) => t * t * t * t;
        public static float OutQuart(float t) => 1f - (--t) * t * t * t;
        public static float InOutQuart(float t) => t < 0.5f ? 8f * t * t * t * t : 1f - 8f * (--t) * t * t * t;
        
        // Sine
        public static float InSine(float t) => 1f - (float)Math.Cos(t * HALF_PI);
        public static float OutSine(float t) => (float)Math.Sin(t * HALF_PI);
        public static float InOutSine(float t) => 0.5f * (1f - (float)Math.Cos(PI * t));
        
        // Exponential
        public static float InExpo(float t) => t == 0f ? 0f : (float)Math.Pow(2f, 10f * (t - 1f));
        public static float OutExpo(float t) => t == 1f ? 1f : 1f - (float)Math.Pow(2f, -10f * t);
        public static float InOutExpo(float t)
        {
            if (t == 0f) return 0f;
            if (t == 1f) return 1f;
            if (t < 0.5f) return 0.5f * (float)Math.Pow(2f, 20f * t - 10f);
            return 0.5f * (2f - (float)Math.Pow(2f, -20f * t + 10f));
        }
        
        // Back
        public static float InBack(float t)
        {
            const float s = 1.70158f;
            return t * t * ((s + 1f) * t - s);
        }
        
        public static float OutBack(float t)
        {
            const float s = 1.70158f;
            return --t * t * ((s + 1f) * t + s) + 1f;
        }
        
        public static float InOutBack(float t)
        {
            const float s = 1.70158f * 1.525f;
            if ((t *= 2f) < 1f) return 0.5f * (t * t * ((s + 1f) * t - s));
            return 0.5f * ((t -= 2f) * t * ((s + 1f) * t + s) + 2f);
        }
        
        // Elastic
        public static float InElastic(float t)
        {
            if (t == 0f) return 0f;
            if (t == 1f) return 1f;
            return -(float)Math.Pow(2f, 10f * (t -= 1f)) * (float)Math.Sin((t - 0.1f) * (2f * PI) / 0.4f);
        }
        
        public static float OutElastic(float t)
        {
            if (t == 0f) return 0f;
            if (t == 1f) return 1f;
            return (float)Math.Pow(2f, -10f * t) * (float)Math.Sin((t - 0.1f) * (2f * PI) / 0.4f) + 1f;
        }
        
        public static float InOutElastic(float t)
        {
            if (t == 0f) return 0f;
            if (t == 1f) return 1f;
            
            t *= 2f;
            if (t < 1f)
            {
                return -0.5f * (float)Math.Pow(2f, 10f * (t -= 1f)) * (float)Math.Sin((t - 0.1f) * (2f * PI) / 0.4f);
            }
            return (float)Math.Pow(2f, -10f * (t -= 1f)) * (float)Math.Sin((t - 0.1f) * (2f * PI) / 0.4f) * 0.5f + 1f;
        }
        
        // Bounce
        public static float InBounce(float t) => 1f - OutBounce(1f - t);
        
        public static float OutBounce(float t)
        {
            if (t < 1f / 2.75f)
            {
                return 7.5625f * t * t;
            }
            else if (t < 2f / 2.75f)
            {
                return 7.5625f * (t -= 1.5f / 2.75f) * t + 0.75f;
            }
            else if (t < 2.5f / 2.75f)
            {
                return 7.5625f * (t -= 2.25f / 2.75f) * t + 0.9375f;
            }
            else
            {
                return 7.5625f * (t -= 2.625f / 2.75f) * t + 0.984375f;
            }
        }
        
        public static float InOutBounce(float t)
        {
            if (t < 0.5f) return InBounce(t * 2f) * 0.5f;
            return OutBounce(t * 2f - 1f) * 0.5f + 0.5f;
        }

    }
}
