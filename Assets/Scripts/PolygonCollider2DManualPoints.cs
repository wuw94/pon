using UnityEngine;
//using UnityEditor;
/*
[RequireComponent(typeof(PolygonCollider2D))]
public class PolygonCollider2DManualPoints : MonoBehaviour { }

[UnityEditor.CustomEditor(typeof(PolygonCollider2DManualPoints))]
public class PolygonCollider2DManualPointsEditor : UnityEditor.Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var collider = ((PolygonCollider2DManualPoints)target).GetComponent<PolygonCollider2D>();
        var points = collider.points;
        for (int i = 0; i < points.Length; i++)
        {
            points[i] = UnityEditor.EditorGUILayout.Vector2Field(i.ToString(), points[i]);
        }
        collider.points = points;
        UnityEditor.EditorUtility.SetDirty(collider);
    }
}
*/