using UnityEngine;
using System.Collections;

public static class Physics2DExtension
{
    public static bool Raycast(this Collider2D collider, Ray2D ray, out RaycastHit2D hitInfo, float maxDistance)
    {
        var oriLayer = collider.gameObject.layer;
        const int tempLayer = 31;
        collider.gameObject.layer = tempLayer;
        hitInfo = Physics2D.Raycast(ray.origin, ray.direction, maxDistance, 1 << tempLayer);
        collider.gameObject.layer = oriLayer;
        if (hitInfo.collider && hitInfo.collider != collider)
        {
            Debug.LogError("Collider2D.Raycast() need a unique temp layer to work! Make sure Layer #" + tempLayer + " is unused!");
            return false;
        }
        return hitInfo.collider != null;
    }
}