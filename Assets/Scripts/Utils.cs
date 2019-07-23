using UnityEngine;

public static class Utils
{
    public static bool isNear(Vector3 obj, Vector3 target, float xThreshold = 0.5f, float yThreshold = 1.5f)
    {
        if (Mathf.Abs(obj.x - target.x) > xThreshold)
        {
            return false;
        }

        if (Mathf.Abs(obj.y - target.y) > yThreshold)
        {
            return false;
        }

        return true;
    }
}
