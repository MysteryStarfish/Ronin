using Ronin.Gameplay;
using UnityEditor;
using UnityEngine;

namespace MyTools
{
    [CustomEditor(typeof(PlayerController))]
    public class DrawEnemyDetectorGizmo : Editor
    {
        [DrawGizmo(GizmoType.Selected | GizmoType.Active)]
        static void DrawDetectorGizmo(PlayerController player, GizmoType gizmoType)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(player.transform.position, player.InnerRadius);
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(player.transform.position, player.DetectorRadius);
        }
        [DrawGizmo(GizmoType.Active)]
        static void DrawAimGizmo(PlayerController player, GizmoType gizmoType)
        {
            if (!player.AimTarget) return;
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(player.AimTarget.position, 0.5f);
        }
    }
}