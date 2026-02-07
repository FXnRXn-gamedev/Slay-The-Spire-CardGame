using UnityEngine;
using UnityEngine.UI;


namespace FXnRXn.Tweening
{
	/// <summary>
	/// Tweens a SpriteRenderer's color
	/// </summary>
    public class ColorTweener : Tween
    {
	    // ------------------------------------------ Properties -------------------------------------------------------
	    
	    private SpriteRenderer spriteRenderer;
	    private Image image;
	    private Graphic graphic;
	    private Color startValue;
	    private Color endValue;


  	    // ---------------------------------------- Functionality-------------------------------------------------------
        public ColorTweener(SpriteRenderer spriteRenderer, Color endValue, float duration) : base(duration)
        {
	        this.spriteRenderer = spriteRenderer;
	        this.endValue = endValue;
	        this.startValue = spriteRenderer.color;
        }
        
        public ColorTweener(Image image, Color endValue, float duration) : base(duration)
        {
	        this.image = image;
	        this.endValue = endValue;
	        this.startValue = image.color;
        }
        
        public ColorTweener(Graphic graphic, Color endValue, float duration) : base(duration)
        {
	        this.graphic = graphic;
	        this.endValue = endValue;
	        this.startValue = graphic.color;
        }
        
        protected override void UpdateValue(float t)
        {
	        Color newValue = Color.Lerp(startValue, endValue, t);
            
	        if (spriteRenderer != null)
	        {
		        spriteRenderer.color = newValue;
	        }
	        else if (image != null)
	        {
		        image.color = newValue;
	        }
	        else if (graphic != null)
	        {
		        graphic.color = newValue;
	        }
        }


    	// ------------------------------------------ Helper Method ----------------------------------------------------

    }
	
	
	/// <summary>
	/// Tweens a CanvasGroup's alpha
	/// </summary>
	public class AlphaTweener : Tween
	{
		private CanvasGroup canvasGroup;
		private float startValue;
		private float endValue;
        
		public AlphaTweener(CanvasGroup canvasGroup, float endValue, float duration) : base(duration)
		{
			this.canvasGroup = canvasGroup;
			this.endValue = endValue;
			this.startValue = canvasGroup.alpha;
		}
        
		protected override void UpdateValue(float t)
		{
			if (canvasGroup == null) return;
            
			canvasGroup.alpha = Mathf.Lerp(startValue, endValue, t);
		}
	}
}