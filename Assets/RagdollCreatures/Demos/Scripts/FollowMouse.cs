using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using Photon.Pun;
using RagdollCreatures;


	/// <summary>
	/// Simple script to let GameObject follow the mouse position.
	/// Uses the new Input system.
	/// </summary>
public class FollowMouse : MonoBehaviour, IInputSystem
{
	public int direction = 1;

	#region Input System
	[Header("Input System")]
	public bool useNewInputSystem;
	#endregion

	#region Internal
	private Vector2 position;
#endregion

	Vector2 inputPos = Vector2.zero;
	public bool valuesReceived = false;
	void Awake()
	{
		useNewInputSystem = InputSystemSwitcher.UseNewInputSystem;
	}

	void Update()
	{
		if(transform.root.GetComponent<RagdollCreature>().aiCont == false)
			transform.localPosition = 5 * GetMouseInput();
		else
        {
			transform.localPosition = 5 * (new Vector2(2, 0) + new Vector2(-1, 0.4f));
        }
	}

	private Vector2 GetMouseInput()
	{
		Vector2 newPosition;
		if (useNewInputSystem)
		{
			newPosition = position;
			//Debug.Log(newPosition);
		}
		else
		{
			//newPosition = Input.mousePosition;

			if(GamePlay.Instance.AttackController.movementVector.x > 0.0f)
            {
				direction = 1;
            }
			else if(GamePlay.Instance.AttackController.movementVector.x < 0.0f)
            {
				direction = -1;
            }

			if ((GamePlay.Instance.AttackController.movementVector.magnitude) > 0.3f)
			{
				newPosition = (GamePlay.Instance.AttackController.movementVector) + new Vector2(-1, 0.4f);
				
			}
			else
			{
				if (GamePlay.Instance.MoveController.movementVector.y > 0.2f)
				{
					newPosition = (GamePlay.Instance.MoveController.movementVector) + new Vector2(-1, 0.4f);
					if(GamePlay.Instance.MoveController.movementVector.x > 0.0f)
                    {
						direction = 1;
                    }
					else if(GamePlay.Instance.MoveController.movementVector.x < 0.0f)
                    {
						direction = -1;
                    }
				}
				else
				{

					if (GamePlay.Instance.localPlayer && GamePlay.Instance.localPlayer.interact.currentInteractable.activeInHierarchy && (GamePlay.Instance.localPlayer.interact.currentInteractable.GetComponent<Sword>() || GamePlay.Instance.localPlayer.interact.currentInteractable.name.ToLower().Contains("grenade")))
					{
						newPosition = (GamePlay.Instance.AttackController.movementVector) + new Vector2(-1, 0.4f);
					}
					else
					{
						newPosition = new Vector2(0.01f * direction, 0.4f);
					}
				}
			}
		}

		return newPosition;
	}

	public void OnMouseMove(InputAction.CallbackContext context)
	{
		if (GamePlay.Instance.AttackController.movementVector.magnitude > 0.2f)
			position = GamePlay.Instance.AttackController.movementVector;
		else
		{
			if (GamePlay.Instance.MoveController.movementVector.y > 0.2f)
				position = GamePlay.Instance.MoveController.movementVector;
			else
			{
				if (GamePlay.Instance.localPlayer.interact.currentInteractable.activeInHierarchy && (GamePlay.Instance.localPlayer.interact.currentInteractable.GetComponent<Sword>() || GamePlay.Instance.localPlayer.interact.currentInteractable.name.ToLower().Contains("grenade")))
				{
					position = GamePlay.Instance.AttackController.movementVector;
				}
				else
				{
					position = Vector2.zero;
				}
			}
		}
	}

	public bool UseNewInputSystem()
	{
		return useNewInputSystem;
	}

	public void SetUseNewInputSystem(bool useNewInputSystem)
	{
		this.useNewInputSystem = useNewInputSystem;
	}
}
