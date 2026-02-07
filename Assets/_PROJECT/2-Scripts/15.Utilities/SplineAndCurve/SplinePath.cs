using TriInspector;
using UnityEngine;
using UnityEngine.Splines;
using Unity.Mathematics;


namespace FXnRXn
{
	/// <summary>
	/// Spline path for smooth card layout
	/// Uses Unity's Spline package for professional spline handling
	/// Requires: com.unity.splines package
	/// </summary>
	[System.Serializable]
    public class SplinePath
    {
	    // ------------------------------------------ Properties -------------------------------------------------------
	    
	    [Title("Spline Reference")]
	    public SplineContainer splineContainer;

	    [Title("Fallback Control Points (if no SplineContainer)")]
	    public Vector2 startPoint = new Vector2(-300f, -400f);
	    public Vector2 controlPoint1 = new Vector2(-150f, -200f);
	    public Vector2 controlPoint2 = new Vector2(150f, -200f);
	    public Vector2 endPoint = new Vector2(300f, -400f);
        
	    [Title("Settings")]
	    [SerializeField] private int resolution = 50;
	    [SerializeField] private int splineIndex = 0;
	    
	    // Cached spline reference
	    private Spline cachedSpline;
	    private bool useUnitySpline = false;

  	    // ---------------------------------------- Functionality ------------------------------------------------------
        
        /// <summary>
        /// Initialize and validate spline
        /// </summary>
        private void ValidateSpline()
        {
	        if (splineContainer != null && splineContainer.Splines.Count > splineIndex)
	        {
		        cachedSpline = splineContainer.Splines[splineIndex];
		        useUnitySpline = true;
	        }
	        else
	        {
		        useUnitySpline = false;
	        }
        }
        
        /// <summary>
        /// Get point at normalized position (0-1) along the curve
        /// </summary>
        public Vector2 GetPoint(float t)
        {
	        ValidateSpline();
	        t = Mathf.Clamp01(t);
            
	        if (useUnitySpline)
	        {
		        // Use Unity Spline
		        float3 position = cachedSpline.EvaluatePosition(t);
		        return new Vector2(position.x, position.y);
	        }
	        else
	        {
		        // Fallback to custom Bezier
		        return BezierCurve.GetCubicPoint(t, startPoint, controlPoint1, controlPoint2, endPoint);
	        }
        }
        
        /// <summary>
        /// Get tangent at normalized position (0-1) along the curve
        /// </summary>
        public Vector2 GetTangent(float t)
        {
	        ValidateSpline();
	        t = Mathf.Clamp01(t);
            
	        if (useUnitySpline)
	        {
		        // Use Unity Spline
		        float3 tangent = cachedSpline.EvaluateTangent(t);
		        return new Vector2(tangent.x, tangent.y).normalized;
	        }
	        else
	        {
		        // Fallback to custom Bezier
		        return BezierCurve.GetCubicTangent(t, startPoint, controlPoint1, controlPoint2, endPoint);
	        }
        }
        
        /// <summary>
        /// Get rotation angle at normalized position (0-1)
        /// </summary>
        public float GetRotation(float t)
        {
	        Vector2 tangent = GetTangent(t);
	        return Mathf.Atan2(tangent.y, tangent.x) * Mathf.Rad2Deg;
        }
        
        /// <summary>
        /// Get point at uniform distance along curve
        /// This ensures even spacing regardless of curve shape
        /// </summary>
        public Vector2 GetPointAtDistance(float normalizedDistance)
        {
	        ValidateSpline();
	        normalizedDistance = Mathf.Clamp01(normalizedDistance);
            
	        if (useUnitySpline)
	        {
		        // Unity Spline handles uniform distribution automatically
		        float length = cachedSpline.GetLength();
		        float distance = normalizedDistance * length;
                
		        // Convert distance to t parameter
		        float t = SplineUtility.GetNormalizedInterpolation(cachedSpline, distance, PathIndexUnit.Distance);
                
		        float3 position = cachedSpline.EvaluatePosition(t);
		        return new Vector2(position.x, position.y);
	        }
	        else
	        {
		        // Fallback: approximate uniform distribution
		        return GetPoint(normalizedDistance);
	        }
        }
        
        /// <summary>
        /// Get rotation at uniform distance along curve
        /// </summary>
        public float GetRotationAtDistance(float normalizedDistance)
        {
	        ValidateSpline();
	        normalizedDistance = Mathf.Clamp01(normalizedDistance);
            
	        if (useUnitySpline)
	        {
		        float length = cachedSpline.GetLength();
		        float distance = normalizedDistance * length;
		        float t = SplineUtility.GetNormalizedInterpolation(cachedSpline, distance, PathIndexUnit.Distance);
                
		        float3 tangent = cachedSpline.EvaluateTangent(t);
		        Vector2 tangent2D = new Vector2(tangent.x, tangent.y).normalized;
		        return Mathf.Atan2(tangent2D.y, tangent2D.x) * Mathf.Rad2Deg;
	        }
	        else
	        {
		        // Fallback
		        return GetRotation(normalizedDistance);
	        }
        }
        
        /// <summary>
        /// Mark the spline as dirty (needs recalculation)
        /// Call this when control points change
        /// </summary>
        public void MarkDirty()
        {
	        // Unity Spline handles this automatically
	        ValidateSpline();
        }
        
        /// <summary>
        /// Get total length of the curve
        /// </summary>
        public float GetLength()
        {
	        ValidateSpline();
            
	        if (useUnitySpline)
	        {
		        return cachedSpline.GetLength();
	        }
	        else
	        {
		        // Fallback: approximate length
		        return BezierCurve.GetCubicLength(startPoint, controlPoint1, controlPoint2, endPoint, resolution);
	        }
        }


    	// ------------------------------------------- Gizmos ----------------------------------------------------------
	    
	    /// <summary>
        /// Draw gizmos for visualization in editor
        /// </summary>
        public void DrawGizmos(Color color)
        {
            ValidateSpline();
            
            if (useUnitySpline)
            {
                // Draw Unity Spline
                Gizmos.color = color;
                
                Vector3 previousPoint = cachedSpline.EvaluatePosition(0f);
                for (int i = 1; i <= 50; i++)
                {
                    float t = (float)i / 50;
                    Vector3 currentPoint = cachedSpline.EvaluatePosition(t);
                    Gizmos.DrawLine(previousPoint, currentPoint);
                    previousPoint = currentPoint;
                }
                
                // Draw knots
                Gizmos.color = Color.yellow;
                foreach (var knot in cachedSpline.Knots)
                {
                    Gizmos.DrawWireSphere(knot.Position, 10f);
                }
            }
            else
            {
                // Draw fallback Bezier curve
                Gizmos.color = color;
                
                Vector2 previousPoint = startPoint;
                for (int i = 1; i <= 50; i++)
                {
                    float t = (float)i / 50;
                    Vector2 currentPoint = GetPoint(t);
                    Gizmos.DrawLine(previousPoint, currentPoint);
                    previousPoint = currentPoint;
                }
                
                // Draw control points
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(startPoint, 10f);
                Gizmos.DrawWireSphere(endPoint, 10f);
                
                Gizmos.color = Color.cyan;
                Gizmos.DrawWireSphere(controlPoint1, 8f);
                Gizmos.DrawWireSphere(controlPoint2, 8f);
                
                // Draw control lines
                Gizmos.color = Color.gray;
                Gizmos.DrawLine(startPoint, controlPoint1);
                Gizmos.DrawLine(controlPoint2, endPoint);
            }
        }
	    


    	// ------------------------------------------ Helper Method ----------------------------------------------------
	    
	    /// <summary>
	    /// Create a SplineContainer GameObject with default curve
	    /// Helper method for quick setup
	    /// </summary>
	    public static GameObject CreateSplineGameObject(string name = "CardHandSpline")
	    {
		    GameObject splineObj = new GameObject(name);
		    SplineContainer container = splineObj.AddComponent<SplineContainer>();
            
		    // Create default spline with 4 knots (similar to Bezier curve)
		    Spline spline = new Spline();
            
		    spline.Add(new BezierKnot(new float3(-300f, -400f, 0f)));
		    spline.Add(new BezierKnot(new float3(-150f, -200f, 0f)));
		    spline.Add(new BezierKnot(new float3(150f, -200f, 0f)));
		    spline.Add(new BezierKnot(new float3(300f, -400f, 0f)));
            
		    container.AddSpline(spline);
            
		    return splineObj;
	    }

    }
}