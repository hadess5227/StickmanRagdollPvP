using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace RagdollCreatures
{
	[CustomEditor(typeof(RagdollLimb))]
	[CanEditMultipleObjects]
	public class RagdollLimbEditor : Editor
	{
		ArcHandle arcHandle;

		void OnEnable()
		{
			arcHandle = new ArcHandle();
		}

		public override void OnInspectorGUI()
		{
			RagdollLimb limb = (RagdollLimb)target;
			GUILayout.Label("Mucles Rotation", EditorStyles.boldLabel);
			GUILayout.BeginHorizontal();
			if (GUILayout.Button("Reset muscle rotation"))
			{
				Undo.RecordObject(limb, "Reset muscle rotation");
				limb.ResetMuscleRotation();
			}
			if (GUILayout.Button("Sync muscle rotation"))
			{
				Undo.RecordObject(limb, "Sync muscle rotation");
				limb.SyncMuscleRotationWithGameObject();
			}
			GUILayout.EndHorizontal();
			GUILayout.Space(10);
			base.OnInspectorGUI();
		}

		void OnSceneGUI()
		{
			RagdollLimb limb = (RagdollLimb) target;

			if (limb.isMuscleRotationEditor)
			{
				Matrix4x4 matrix = Matrix4x4.TRS(
					limb.transform.position,
					Quaternion.Euler(90, 0, 0),
					limb.transform.localScale);

				arcHandle.SetColorWithRadiusHandle(Color.yellow, 0.25f);

				arcHandle.angle = limb.muscleRotation;
				arcHandle.radius = 1;

				Handles.color = Color.white;
				using (new Handles.DrawingScope(matrix))
				{
					arcHandle.DrawHandle();
				}
				Undo.RecordObject(limb, "Rotate muscle");
				limb.muscleRotation = arcHandle.angle;
			}

			if (limb.isGizmos)
			{
				Rigidbody2D rigidbody = limb.GetComponent<Rigidbody2D>();

				if (limb.useCustomCenterOfMass)
				{
					rigidbody.centerOfMass = limb.customCenterOfMass;
				}

				if (limb.useCustomCenterOfMass)
				{
					EditorGUI.BeginChangeCheck();
					Vector2 newCenterOfMass = Handles.PositionHandle(rigidbody.worldCenterOfMass, Quaternion.identity);
					if (EditorGUI.EndChangeCheck())
					{
						Undo.RecordObject(limb, "Change local center of mass");
						limb.customCenterOfMass = newCenterOfMass - rigidbody.position;
						rigidbody.centerOfMass = newCenterOfMass - rigidbody.position;
					}
				}
			}
		}
	}
}
