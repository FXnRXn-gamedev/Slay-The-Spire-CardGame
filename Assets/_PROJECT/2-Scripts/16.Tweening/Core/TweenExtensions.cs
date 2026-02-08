using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;


namespace FXnRXn.Tweening
{
	/// <summary>
	/// Extension methods for easy tweening
	/// Provides DOTween-like API
	/// </summary>
    public static class TweenExtensions
    {
	    // --------------------------------------- TRANSFORM EXTENSIONS ------------------------------------------------

	    /// <summary>
        /// Tween Transform position
        /// </summary>
        public static Tween TweenPosition(this Transform transform, Vector3 endValue, float duration, bool isLocal = false)
        {
            return new PositionTweener(transform, endValue, duration, isLocal).Play();
        }
        
        /// <summary>
        /// Tween Transform rotation
        /// </summary>
        public static Tween TweenRotation(this Transform transform, Quaternion endValue, float duration, bool isLocal = false)
        {
            return new RotationTweener(transform, endValue, duration, isLocal).Play();
        }
        
        /// <summary>
        /// Tween Transform rotation (Euler angles)
        /// </summary>
        public static Tween TweenRotation(this Transform transform, Vector3 endEuler, float duration, bool isLocal = false)
        {
            return new RotationTweener(transform, endEuler, duration, isLocal).Play();
        }
        
        /// <summary>
        /// Tween Transform scale
        /// </summary>
        public static Tween TweenScale(this Transform transform, Vector3 endValue, float duration)
        {
            return new ScaleTweener(transform, endValue, duration).Play();
        }
        
        /// <summary>
        /// Tween Transform scale (uniform)
        /// </summary>
        public static Tween TweenScale(this Transform transform, float endValue, float duration)
        {
            return new ScaleTweener(transform, endValue, duration).Play();
        }
        
        // ===== RECTTRANSFORM EXTENSIONS =====
        
        /// <summary>
        /// Tween RectTransform anchored position
        /// </summary>
        public static Tween TweenAnchoredPosition(this RectTransform rectTransform, Vector2 endValue, float duration)
        {
            return new AnchoredPositionTweener(rectTransform, endValue, duration).Play();
        }
        
        // ===== SPRITERENDERER EXTENSIONS =====
        
        /// <summary>
        /// Tween SpriteRenderer color
        /// </summary>
        public static Tween TweenColor(this SpriteRenderer spriteRenderer, Color endValue, float duration)
        {
            return new ColorTweener(spriteRenderer, endValue, duration).Play();
        }
        
        /// <summary>
        /// Tween SpriteRenderer alpha
        /// </summary>
        public static Tween TweenAlpha(this SpriteRenderer spriteRenderer, float endAlpha, float duration)
        {
            Color endColor = spriteRenderer.color;
            endColor.a = endAlpha;
            return new ColorTweener(spriteRenderer, endColor, duration).Play();
        }
        
        // ===== IMAGE EXTENSIONS =====
        
        /// <summary>
        /// Tween Image color
        /// </summary>
        public static Tween TweenColor(this Image image, Color endValue, float duration)
        {
            return new ColorTweener(image, endValue, duration).Play();
        }
        
        /// <summary>
        /// Tween Image alpha
        /// </summary>
        public static Tween TweenAlpha(this Image image, float endAlpha, float duration)
        {
            Color endColor = image.color;
            endColor.a = endAlpha;
            return new ColorTweener(image, endColor, duration).Play();
        }
        
        // ===== GRAPHIC EXTENSIONS =====
        
        /// <summary>
        /// Tween Graphic color
        /// </summary>
        public static Tween TweenColor(this Graphic graphic, Color endValue, float duration)
        {
            return new ColorTweener(graphic, endValue, duration).Play();
        }
        
        /// <summary>
        /// Tween Graphic alpha
        /// </summary>
        public static Tween TweenAlpha(this Graphic graphic, float endAlpha, float duration)
        {
            Color endColor = graphic.color;
            endColor.a = endAlpha;
            return new ColorTweener(graphic, endColor, duration).Play();
        }
        
        // ===== CANVASGROUP EXTENSIONS =====
        
        /// <summary>
        /// Tween CanvasGroup alpha
        /// </summary>
        public static Tween TweenAlpha(this CanvasGroup canvasGroup, float endValue, float duration)
        {
            return new AlphaTweener(canvasGroup, endValue, duration).Play();
        }
        
        // ===== HELPER METHODS =====
        
        /// <summary>
        /// Kill all tweens on a GameObject
        /// </summary>
        public static void KillTweens(this GameObject gameObject)
        {
            // This would require tracking tweens by GameObject
            // For now, this is a placeholder
            Debug.LogWarning("KillTweens not fully implemented yet");
        }
        
        /// <summary>
        /// Kill all tweens on a Transform
        /// </summary>
        public static void KillTweens(this Transform transform)
        {
            transform.gameObject.KillTweens();
        }
        
        
        /// <summary>
        /// Convert Tween to awaitable UniTask
        /// </summary>
        public static UniTask ToUniTask(this Tween tween)
        {
            if (tween == null || tween.IsComplete)
                return UniTask.CompletedTask;
            
            var tcs = new UniTaskCompletionSource();
            tween.OnComplete(() => tcs.TrySetResult());
            return tcs.Task;
        }

    }
}