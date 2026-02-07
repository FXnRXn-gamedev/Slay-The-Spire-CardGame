using UnityEngine;

namespace FXnRXn
{
	/// <summary>
	/// Bezier curve implementation for smooth curves
	/// Supports quadratic (3 points) and cubic (4 points) Bezier curves
	/// </summary>
    public static class BezierCurve
    {
	    /// <summary>
	    /// Get point on quadratic Bezier curve (3 control points)
	    /// </summary>
	    /// <param name="t">Position along curve (0-1)</param>
	    /// <param name="p0">Start point</param>
	    /// <param name="p1">Control point</param>
	    /// <param name="p2">End point</param>
	    public static Vector2 GetQuadraticPoint(float t, Vector2 p0, Vector2 p1, Vector2 p2)
	    {
		    float u = 1f - t;
		    float tt = t * t;
		    float uu = u * u;
            
		    Vector2 point = uu * p0; // (1-t)^2 * P0
		    point += 2f * u * t * p1; // 2(1-t)t * P1
		    point += tt * p2; // t^2 * P2
            
		    return point;
	    }


	    /// <summary>
	    /// Get point on cubic Bezier curve (4 control points)
	    /// </summary>
	    /// <param name="t">Position along curve (0-1)</param>
	    /// <param name="p0">Start point</param>
	    /// <param name="p1">First control point</param>
	    /// <param name="p2">Second control point</param>
	    /// <param name="p3">End point</param>
	    public static Vector2 GetCubicPoint(float t, Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3)
	    {
		    float u = 1f - t;
		    float tt = t * t;
		    float uu = u * u;
		    float uuu = uu * u;
		    float ttt = tt * t;
            
		    Vector2 point = uuu * p0; // (1-t)^3 * P0
		    point += 3f * uu * t * p1; // 3(1-t)^2 * t * P1
		    point += 3f * u * tt * p2; // 3(1-t) * t^2 * P2
		    point += ttt * p3; // t^3 * P3
            
		    return point;
	    }


	    /// <summary>
	    /// Get tangent (derivative) on quadratic Bezier curve
	    /// </summary>
	    public static Vector2 GetQuadraticTangent(float t, Vector2 p0, Vector2 p1, Vector2 p2)
	    {
		    float u = 1f - t;
            
		    Vector2 tangent = 2f * u * (p1 - p0);
		    tangent += 2f * t * (p2 - p1);
            
		    return tangent.normalized;
	    }


	    /// <summary>
	    /// Get tangent (derivative) on cubic Bezier curve
	    /// </summary>
	    public static Vector2 GetCubicTangent(float t, Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3)
	    {
		    float u = 1f - t;
		    float uu = u * u;
		    float tt = t * t;
            
		    Vector2 tangent = 3f * uu * (p1 - p0);
		    tangent += 6f * u * t * (p2 - p1);
		    tangent += 3f * tt * (p3 - p2);
            
		    return tangent.normalized;
	    }


	    /// <summary>
	    /// Approximate length of cubic Bezier curve
	    /// Uses linear approximation with segments
	    /// </summary>
	    public static float GetCubicLength(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, int segments = 20)
	    {
		    float length = 0f;
		    Vector2 previousPoint = p0;
            
		    for (int i = 1; i <= segments; i++)
		    {
			    float t = (float)i / segments;
			    Vector2 currentPoint = GetCubicPoint(t, p0, p1, p2, p3);
			    length += Vector2.Distance(previousPoint, currentPoint);
			    previousPoint = currentPoint;
		    }
            
		    return length;
	    }
        
	    /// <summary>
	    /// Get rotation angle from tangent
	    /// </summary>
	    public static float GetRotationFromTangent(Vector2 tangent)
	    {
		    return Mathf.Atan2(tangent.y, tangent.x) * Mathf.Rad2Deg;
	    }

    }
}