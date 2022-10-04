using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RagdollCreatures
{
	public class InputSystemSwitcher : MonoBehaviour
	{
		public static bool UseNewInputSystem;
		private bool actualUseNewInputSystem;

		private void Awake()
		{
			if (!UseNewInputSystem)
			{
				try
				{
					Input.GetKeyDown(KeyCode.T);
				}
				catch (Exception e)
				{
					Debug.Log(e);
					Debug.Log("The old input system is not activated, so the InputSystemSwitcher switches all assets to the new system");
					UseNewInputSystem = true;
					SwitchInputSystem();
				}
			}
		}

		private void Update()
		{
			SwitchInputSystem();
		}

		private void SwitchInputSystem()
		{
			if (UseNewInputSystem != actualUseNewInputSystem)
			{
				foreach (IInputSystem inputSystem in Find<IInputSystem>())
				{
					inputSystem.SetUseNewInputSystem(UseNewInputSystem);
					actualUseNewInputSystem = UseNewInputSystem;
				}
			}
		}

		public static List<T> Find<T>()
		{
			List<T> interfaces = new List<T>();
			GameObject[] rootGameObjects = SceneManager.GetActiveScene().GetRootGameObjects();
			foreach (var rootGameObject in rootGameObjects)
			{
				T[] childrenInterfaces = rootGameObject.GetComponentsInChildren<T>();
				foreach (var childInterface in childrenInterfaces)
				{
					interfaces.Add(childInterface);
				}
			}
			return interfaces;
		}
	}
}
