#pragma warning disable 0649
#pragma warning disable 0168
#pragma warning disable 0414

using UnityEngine;
using System.Collections.Generic;

namespace FXnRXn.Tweening
{
	/// <summary>
	/// Singleton manager that updates all active tweens
	/// </summary>
    public class TweenManager : MonoBehaviour
    {
	    // ------------------------------------------ Singleton --------------------------------------------------------
	    public static TweenManager Instance { get; private set; }
	    
	    // ------------------------------------------ Properties -------------------------------------------------------
	    
	    private List<Tween> activeTweens = new List<Tween>();
	    private List<Tween> tweensToAdd = new List<Tween>();
	    private List<Tween> tweensToRemove = new List<Tween>();


  	    // ---------------------------------------- Unity Callback -----------------------------------------------------
        
        private void Awake()
        {
	        if (Instance == null)
	        {
		        Instance = this;
		        DontDestroyOnLoad(gameObject);
	        }
	        else
	        {
		        Destroy(gameObject);
	        }
        }
        
        private void Update()
        {
	        float deltaTime = Time.deltaTime;
            
	        // Add pending tweens
	        if (tweensToAdd.Count > 0)
	        {
		        activeTweens.AddRange(tweensToAdd);
		        tweensToAdd.Clear();
	        }
            
	        // Update all active tweens
	        for (int i = activeTweens.Count - 1; i >= 0; i--)
	        {
		        if (i >= activeTweens.Count) continue;
                
		        Tween tween = activeTweens[i];
                
		        if (tween == null || !tween.Update(deltaTime))
		        {
			        activeTweens.RemoveAt(i);
		        }
	        }
            
	        // Remove pending tweens
	        if (tweensToRemove.Count > 0)
	        {
		        foreach (var tween in tweensToRemove)
		        {
			        activeTweens.Remove(tween);
		        }
		        tweensToRemove.Clear();
	        }
        }


    	// ------------------------------------------ Helper Method ----------------------------------------------------
	    
	    /// <summary>
	    /// Add a tween to be managed
	    /// </summary>
	    public void AddTween(Tween tween)
	    {
		    if (tween == null) return;
            
		    tweensToAdd.Add(tween);
	    }
	    
	    /// <summary>
	    /// Remove a tween from management
	    /// </summary>
	    public void RemoveTween(Tween tween)
	    {
		    if (tween == null) return;
            
		    tweensToRemove.Add(tween);
	    }
	    
	    /// <summary>
	    /// Kill all active tweens
	    /// </summary>
	    public void KillAll()
	    {
		    foreach (var tween in activeTweens)
		    {
			    tween?.Kill();
		    }
            
		    activeTweens.Clear();
		    tweensToAdd.Clear();
		    tweensToRemove.Clear();
	    }
	    
	    /// <summary>
	    /// Pause all active tweens
	    /// </summary>
	    public void PauseAll()
	    {
		    foreach (var tween in activeTweens)
		    {
			    tween?.Pause();
		    }
	    }
        
	    /// <summary>
	    /// Resume all paused tweens
	    /// </summary>
	    public void ResumeAll()
	    {
		    foreach (var tween in activeTweens)
		    {
			    tween?.Resume();
		    }
	    }
	    
	    
	    /// <summary>
	    /// Get count of active tweens
	    /// </summary>
	    public int GetActiveTweenCount()
	    {
		    return activeTweens.Count;
	    }
	    
	    /// <summary>
	    /// Initialize the TweenManager (call this at game start)
	    /// </summary>
	    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	    private static void Initialize()
	    {
		    if (Instance == null)
		    {
			    GameObject managerObject = new GameObject("[TweenManager]");
			    managerObject.AddComponent<TweenManager>();
		    }
	    }

    }
}
