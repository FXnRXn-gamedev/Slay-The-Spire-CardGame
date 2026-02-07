#pragma warning disable 0649
#pragma warning disable 0168
#pragma warning disable 0414


using UnityEngine;
using System;



namespace FXnRXn.Tweening
{
	/// <summary>
	/// Base class for all tweens
	/// </summary>
	public abstract class Tween
	{
		// ------------------------------------------ Properties -------------------------------------------------------
		
		// Core properties
		public float Duration { get; protected set; }
		public float Delay { get; protected set; }
		public EaseType EaseType { get; protected set; }
		public bool IsPlaying { get; protected set; }
		public bool IsComplete { get; protected set; }
        
		// Timing
		protected float elapsedTime;
		protected float delayTimer;
		protected Func<float, float> easingFunction;
        
		// Callbacks
		protected Action onStart;
		protected Action<float> onUpdate;
		protected Action onComplete;
        
		// Loop settings
		protected int loopCount = 0; // 0 = no loop, -1 = infinite
		protected bool isPingPong = false;
		protected int currentLoop = 0;
		protected bool isReversing = false;


		// ---------------------------------------- Functionality ------------------------------------------------------
		
		protected Tween(float duration)
		{
			Duration = duration;
			EaseType = EaseType.Linear;
			easingFunction = Easing.Linear;
		}
		
		
		/// <summary>
		/// Update the tween (called by TweenManager)
		/// </summary>
		public virtual bool Update(float deltaTime)
		{
			if (!IsPlaying || IsComplete) return false;
            
			// Handle delay
			if (delayTimer < Delay)
			{
				delayTimer += deltaTime;
				if (delayTimer >= Delay)
				{
					onStart?.Invoke();
				}
				return true;
			}
            
			// Update tween
			elapsedTime += deltaTime;
			float t = Mathf.Clamp01(elapsedTime / Duration);
            
			// Apply easing
			float easedT = easingFunction(t);
            
			// Reverse if ping-pong
			if (isReversing)
			{
				easedT = 1f - easedT;
			}
            
			// Update value
			UpdateValue(easedT);
			onUpdate?.Invoke(easedT);
            
			// Check completion
			if (t >= 1f)
			{
				HandleCompletion();
			}
            
			return true;
		}


		
		protected virtual void HandleCompletion()
		{
			currentLoop++;
            
			// Check if should loop
			if (loopCount == -1 || currentLoop < loopCount)
			{
				if (isPingPong)
				{
					isReversing = !isReversing;
				}
				else
				{
					elapsedTime = 0f;
				}
				elapsedTime = 0f;
			}
			else
			{
				IsComplete = true;
				IsPlaying = false;
				onComplete?.Invoke();
			}
		}

		// ------------------------------------------ Helper Method ----------------------------------------------------
		
		/// <summary>
		/// Update the actual value (implemented by subclasses)
		/// </summary>
		protected abstract void UpdateValue(float t);
		
		/// <summary>
		/// Start the tween
		/// </summary>
		public virtual Tween Play()
		{
			IsPlaying = true;
			IsComplete = false;
			elapsedTime = 0f;
			delayTimer = 0f;
			currentLoop = 0;
			isReversing = false;
            
			TweenManager.Instance.AddTween(this);
			return this;
		}
		
		
		/// <summary>
		/// Pause the tween
		/// </summary>
		public virtual Tween Pause()
		{
			IsPlaying = false;
			return this;
		}
        
		/// <summary>
		/// Resume the tween
		/// </summary>
		public virtual Tween Resume()
		{
			IsPlaying = true;
			return this;
		}
		
		/// <summary>
		/// Stop and remove the tween
		/// </summary>
		public virtual void Kill()
		{
			IsPlaying = false;
			IsComplete = true;
			TweenManager.Instance.RemoveTween(this);
		}
		
		/// <summary>
		/// Set easing function
		/// </summary>
		public Tween SetEase(EaseType easeType)
		{
			EaseType = easeType;
			easingFunction = Easing.GetEasingFunction(easeType);
			return this;
		}
		
		/// <summary>
		/// Set delay before tween starts
		/// </summary>
		public Tween SetDelay(float delay)
		{
			Delay = delay;
			return this;
		}
		
		/// <summary>
		/// Set loop count (-1 for infinite)
		/// </summary>
		public Tween SetLoops(int loops, bool pingPong = false)
		{
			loopCount = loops;
			isPingPong = pingPong;
			return this;
		}
		
		/// <summary>
		/// Set callback when tween starts
		/// </summary>
		public Tween OnStart(Action callback)
		{
			onStart = callback;
			return this;
		}
		
		/// <summary>
		/// Set callback on each update (receives eased t value)
		/// </summary>
		public Tween OnUpdate(Action<float> callback)
		{
			onUpdate = callback;
			return this;
		}
		
		/// <summary>
		/// Set callback when tween completes
		/// </summary>
		public Tween OnComplete(Action callback)
		{
			onComplete = callback;
			return this;
		}
		
	}
}
