using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class EAMathUtil
{
    public const float HALF_PI = Mathf.PI * 0.5f;
    public const float TWO_PI = Mathf.PI * 2f;

    static System.Random rand = new System.Random();

    public static Vector2 ToVec2(Vector3 v)
    {
        return new Vector2(v.x, v.z);
    }

    public static Vector3 ToVector3(Vector2 v,float y)
    {
        return new Vector3(v.x, y, v.y);
    }

    public static float RandInRange(float min,float max)
    {
        if (min >= max) return min;
        return min + (float)rand.NextDouble() * (max - min);
    }

    // returns a random float in the range -1 < n < 1
    public static float RandomClamped()
    {
        return ((float)rand.Next(0, 200)) / 100.0f - 1.0f;
    }

    // returns a random integer between x and y
    public static int RandInt(int x,int y)
    {
        Debug.Assert(y >= x, "<RandInt>: y is less than x");
        return rand.Next(int.MaxValue - x) % (y - x + 1) + x;
    }

    // returns a random double between zero and 1
    public static double RandFloat()
    {
        return rand.NextDouble();
    }

    private static Vector2 AccumulateNormal(Vector2 a,Vector2 b,Vector2 heading)
    {
        Vector2 normal = Vector2.Perpendicular((a-b).normalized);
        heading = (heading + normal).normalized;
        return heading;
    }
    
    public static float GetCrossProductScalar(Vector2 a,Vector2 b)
    {
        return a.x * b.y - a.y * b.x;
    }

    public static Vector3 PointToWorldSpace(Vector3 point,Vector3 agentHeading,Vector3 agentSide,Vector3 agentPosition)
    {
        Vector3 up = Vector3.Cross(agentHeading, agentSide);

        //create a transformation matrix
        Matrix4x4 matTransform = Matrix4x4.TRS(agentPosition, Quaternion.LookRotation(agentHeading, up), Vector3.one);
        Vector3 transPoint = matTransform.MultiplyPoint(point);
        return transPoint;
    }

    public static Vector3 VectorToWorldSpace(Vector3 vector,Vector3 agentHeading,Vector3 agentSide)
    {
        Vector3 up = Vector3.Cross(agentHeading, agentSide);

        //create a transformation matrix
        Matrix4x4 matTransform = Matrix4x4.TRS(Vector3.zero, Quaternion.LookRotation(agentHeading, up), Vector3.one);
        Vector3 transPoint = matTransform.MultiplyVector(vector);
        return transPoint;
    }

    public static Vector3 PointToLocalSpace(Vector3 point,
                                           Vector3 agentHeading,
                                           Vector3 agentSide,
                                           Vector3 agentPosition)
    {
        Vector3 up = Vector3.Cross(agentHeading, agentSide);

        //create a transformation matrix
        Matrix4x4 matTransform = Matrix4x4.TRS(agentPosition, Quaternion.LookRotation(agentHeading, up), Vector3.one);
        Vector3 transPoint = matTransform.inverse.MultiplyPoint(point);
        return transPoint;
    }

    public static Vector3 Truncate(Vector3 v,float mag)
    {
        if(v.magnitude > mag)
        {
            v.Normalize();
            v *= mag;
        }
        return v;
    }

    public static KeyValuePair<bool,Vector2> LineIntersection2D(Vector2 A, Vector2 B, Vector2 C, Vector2 D, out float dist)
    {
        // Line AB represented as a1x + b1y = c1  
        float a1 = B.y - A.y;
        float b1 = A.x - B.x;
        float c1 = a1 * (A.x) + b1 * (A.y);

        // Line CD represented as a2x + b2y = c2  
        float a2 = D.y - C.y;
        float b2 = C.x - D.x;
        float c2 = a2 * (C.x) + b2 * (C.y);

        float determinant = a1 * b2 - a2 * b1;

        if (determinant == 0)
        {
            dist = float.MaxValue;
            // The lines are parallel. This is simplified  
            // by returning a pair of FLT_MAX  
            return new KeyValuePair<bool, Vector2>(false, new Vector2(float.MaxValue, float.MaxValue));
        }
        else
        {

            float x = (b2 * c1 - b1 * c2) / determinant;
            float y = (a1 * c2 - a2 * c1) / determinant;

            Vector2 crossPoint = new Vector2(x, y);

            dist = (crossPoint - A).magnitude;

            return new KeyValuePair<bool, Vector2>(true, crossPoint);
        }
    }
    public static bool LineIntersectionXZ(Vector3 A,Vector3 B,Vector3 C,Vector3 D,out float dist)
    {
        Vector2 a = ToVec2(A);
        Vector2 b = ToVec2(B);
        Vector2 c = ToVec2(C);
        Vector2 d = ToVec2(D);

        KeyValuePair<bool, Vector2> result = LineIntersection2D(a, b, c, d, out dist);

        return result.Key;
    }
}