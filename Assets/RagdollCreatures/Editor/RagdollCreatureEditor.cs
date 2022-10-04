using UnityEditor;
using UnityEngine;

namespace RagdollCreatures
{
	/// <summary>
	/// Extension for the RagdollCreature editor for easy switching between ragdoll modes.
	/// </summary>
	[CustomEditor(typeof(RagdollCreature), true)]
	public class RagdollCreatureEditor : Editor
	{
		private Color color = Color.white;

		public override void OnInspectorGUI()
		{
			// Ragdoll
			RagdollCreature creature = (RagdollCreature)target;
			GUILayout.Label("Ragdoll", EditorStyles.boldLabel);
			GUILayout.BeginHorizontal();
			if (GUILayout.Button("Switch to active ragdoll"))
			{
				creature.ActivateAllMuscles();
			}
			if (GUILayout.Button("Switch to ragdoll"))
			{
				creature.DeactivateAllMuscles();
			}
			GUILayout.EndHorizontal();
			GUILayout.Space(10);

			// Ragdoll limb Muscles
			GUILayout.Label("Mucles Rotations", EditorStyles.boldLabel);
			GUILayout.BeginHorizontal();
			if (GUILayout.Button("Reset all muscle rotations"))
			{
				creature.ResetAllMuscleRotations();
			}
			if (GUILayout.Button("Sync all muscle rotations"))
			{
				creature.SyncAllMuscleRotationsWithGameObject();
			}
			GUILayout.EndHorizontal();
			GUILayout.Space(10);

			// Color
			GUILayout.Label("Ragdoll Color", EditorStyles.boldLabel);
			GUILayout.Label("Be careful: Sets the color in all SpriteRenderer!!");
			GUILayout.BeginHorizontal();
			color = EditorGUILayout.ColorField(color);
			if (GUILayout.Button("Change Color!"))
			{
				foreach (SpriteRenderer renderer in creature.GetComponentsInChildren<SpriteRenderer>())
				{
					if (null != renderer && null != renderer.GetComponent<RagdollLimb>())
					{
						renderer.color = color;
					}
				}

				foreach (LineRenderer renderer in creature.GetComponentsInChildren<LineRenderer>())
				{
					if (null != renderer && null != renderer.GetComponent<RagdollLimb>())
					{
						renderer.startColor = color;
						renderer.endColor = color;
					}
				}
			}
			GUILayout.EndHorizontal();
			GUILayout.Space(10);
			base.OnInspectorGUI();
		}
	}
}
