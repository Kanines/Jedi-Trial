using System.Collections;
using UnityEngine;

public static class Utils
{
    public static Color hitColor = new Color(1.0f, 0.7f, 0.1f, 1.0f);

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

    public static IEnumerator HitFxRoutine(SpriteRenderer sprite, float duration = 0.25f)
    {
        sprite.color = hitColor;
        yield return new WaitForSeconds(duration);
        sprite.color = Color.white;
    }
}
