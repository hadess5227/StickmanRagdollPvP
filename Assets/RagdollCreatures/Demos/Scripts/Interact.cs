using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Pun;
using RagdollCreatures;

	[RequireComponent(typeof(Collider2D))]
	public class Interact : MonoBehaviour, IInputSystem
	{
		public Interact Instance;
		public Vector2 dir = Vector2.zero;
		#region Settings
		[Header("Settings")]
		public GameObject root;
		public Rigidbody2D parent;
		public Transform position;
		#endregion

		#region Input System
		[Header("Input System")]
		public bool useNewInputSystem = InputSystemSwitcher.UseNewInputSystem;
		#endregion

		#region Internal
		private GameObject nearestInteractable;
		public GameObject currentInteractable;

		private Vector2 aimPosition;
		#endregion

		void Awake()
		{
			Instance = this;
			useNewInputSystem = InputSystemSwitcher.UseNewInputSystem;
		}

		void Update()
		{
			if (!useNewInputSystem)
			{
				if (Input.GetKeyDown(KeyCode.E))
				{
					GetComponent<PhotonView>().RPC("RPC_OnInteract", RpcTarget.All);
					//OnInteract();
				}

				
				//if (Input.GetMouseButtonDown(0))
				{
					//GetComponent<PhotonView>().RPC("RPC_OnAttack", RpcTarget.All);
					
					//OnAttack();
				}
				
			}
		}

		public void OnAttack(InputAction.CallbackContext context)
		{
			if (context.started && useNewInputSystem)
			{
				//OnAttack();
				//GetComponent<PhotonView>().RPC("RPC_OnAttack", RpcTarget.All);
				//0OnAttack();
			}
		}

		public void OnShot()
		{
			RagdollCreature ragdollCreature = transform.root.GetComponent<RagdollCreature>();

			if (ragdollCreature.isDead == false)
			{
				if (RoomManager.Instance && transform.root.GetComponent<PhotonView>().IsMine)
				{
					transform.root.GetComponent<PhotonView>().RPC("RPC_OnAttack", RpcTarget.All, dir);
				}
				else if (RoomManager.Instance == null)
				{
					transform.root.GetComponent<PlayerController>().OnAttack();
				}
			}
		}

		public void OnAim(InputAction.CallbackContext context)
		{
			if (useNewInputSystem)
			{
				aimPosition = Camera.main.ScreenToWorldPoint(context.ReadValue<Vector2>());
			}
		}

		public void OnInteract(InputAction.CallbackContext context)
		{
			if (context.started && useNewInputSystem)
			{
				GetComponent<PhotonView>().RPC("RPC_OnInteract", RpcTarget.All);
			}
		}

		[PunRPC]
		private void RPC_OnInteract()
		{
			Reset();

			if (null != nearestInteractable && null == nearestInteractable.transform.parent && null == currentInteractable)
			{
				foreach (Collider2D collider in root.GetComponentsInChildren<Collider2D>())
				{
					Physics2D.IgnoreCollision(nearestInteractable.GetComponent<Collider2D>(), collider);
				}

				nearestInteractable.transform.SetParent(position, false);
				nearestInteractable.transform.position = position.position;

				Rigidbody2D rb = nearestInteractable.GetComponent<Rigidbody2D>();
				if (null != rb)
				{
					rb.isKinematic = true;
				}

				Equipable equipable = nearestInteractable.GetComponent<Equipable>();
				if (null != equipable)
				{
					nearestInteractable.transform.rotation = Quaternion.Euler(0.0f, 0.0f, equipable.rotationOffset + position.rotation.eulerAngles.z);
				}

				RotationFlipper rotationFlipper = nearestInteractable.GetComponent<RotationFlipper>();
				if (null != rotationFlipper)
				{
					rotationFlipper.activeRotationFlip = true;
				}

				currentInteractable = nearestInteractable;
			}
		}

		public void Reset()
		{
			if (null != currentInteractable)
			{
				foreach (Collider2D collider in root.GetComponentsInChildren<Collider2D>())
				{
					Physics2D.IgnoreCollision(currentInteractable.GetComponent<Collider2D>(), collider, false);
				}

				Rigidbody2D rb = currentInteractable.GetComponent<Rigidbody2D>();
				if (null != rb)
				{
					rb.isKinematic = false;
				}

				RotationFlipper rotationFlipper = currentInteractable.GetComponent<RotationFlipper>();
				if (null != rotationFlipper)
				{
					rotationFlipper.activeRotationFlip = false;
				}

				currentInteractable.transform.parent = null;
				currentInteractable = null;
			}
		}

		public void OnTriggerEnter2D(Collider2D col)
		{
			if (col.CompareTag("Equipable"))
			{
				nearestInteractable = col.gameObject;
			}
		}

		public void OnTriggerExit2D(Collider2D col)
		{
			if (col.CompareTag("Equipable") && col.gameObject == nearestInteractable)
			{
				nearestInteractable = null;
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
