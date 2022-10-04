using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Realtime;
using ExitGames.Client.Photon;
using Photon.Pun;
using RagdollCreatures;

public class AbstractRagdollCreatureController : MonoBehaviour, IRagdollCreatureController
{
    #region Internal
    public RagdollCreature ragdollCreature;
    public RagdollCreatureMovement movement;
    private float lastJumpTime;
    private Vector2 moveVector;
    #endregion

    Vector2 inputHorPos = Vector2.zero;
    bool valuesReceived = false;
    Vector2 inputPos = Vector2.zero;
    float inputJumpPos = 0;
    public Vector2 horizontalMove = Vector2.zero;
    Vector2 beforeMove = Vector2.zero;
    public AbstractRagdollCreatureController(RagdollCreature ragdollCreature, RagdollCreatureMovement movement)
    {
        this.ragdollCreature = ragdollCreature;
        this.movement = movement;
        if (null == movement)
        {
            this.movement = ScriptableObject.CreateInstance(typeof(RagdollCreatureMovement)) as RagdollCreatureMovement;
        }
        this.movement.useNewInputSystem = InputSystemSwitcher.UseNewInputSystem;

        if (this.ragdollCreature.aiCont)
        {
            beforeMove = horizontalMove;
            horizontalMove = new Vector2(2.0f, 0);
        }
    }

    public void Update()
    {
        if (ragdollCreature.isDead)
        {
            return;
        }

        // Animations
        UpdateAnimations();
    }
    /*
	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.IsWriting)
		{
			stream.SendNext(GetHorizontalMovementInput());
			stream.SendNext(GetJumpMovementInput());
			stream.SendNext(GetMovementInput());
		}
		else
		{
			inputHorPos = (Vector2)stream.ReceiveNext();
			inputJumpPos = (float)stream.ReceiveNext();
			inputPos = (Vector2)stream.ReceiveNext();
			valuesReceived = true;
		}
	}
	*/

    public void FixedUpdate()
    {
        if (ragdollCreature.isDead)
        {
            foreach (RagdollLimb limb in ragdollCreature.ragdollLimbs)
            {
                limb.rigidbody.gravityScale = 1;
            }
            return;
        }


        //if (GetComponent<PhotonView>().IsMine)


        if (this.ragdollCreature.aiCont == false)
        {
            horizontalMove = GetHorizontalMovementInput();
        }


        //else if(valuesReceived)
        //{
        //	horizontalMove = inputHorPos;
        //}
        if (this.ragdollCreature.aiCont == true)
        {
            movement.movementSpeed = 1;
            movement.jumpMovementSpeed = 1;
        }
        else
        {
            movement.movementSpeed = 10;
            movement.jumpMovementSpeed = 5;
        }

        float speed = ragdollCreature.isGrounded ? movement.movementSpeed : movement.jumpMovementSpeed;


        Rigidbody2D centerOfMass = ragdollCreature.centerOfMass?.rigidbody;
        if (null != centerOfMass)
        {
            // Reduce Y velocity of limbs for smoother walking
            // To achieve a similar effect you could also reduce the friction from the floor.
            // You could also adjust the groundGravityScale.
            if (movement.isSmootherWalking && ragdollCreature.isGrounded)
            {
                foreach (RagdollLimb limb in ragdollCreature.ragdollLimbs)
                {
                    if (limb.isMuscleActive && limb.isActiveGroundDetection)
                    {
                        Rigidbody2D limbRigidbody = limb.rigidbody;
                        float velocityY = 0.0f;
                        float smoothY = Mathf.SmoothDamp(limbRigidbody.velocity.y, 0, ref velocityY, Time.fixedDeltaTime);
                        limbRigidbody.velocity = new Vector2(limbRigidbody.velocity.x, smoothY);
                    }
                }
            }

            // Adjust gravity scaling.
            // Jump with normal gravity (gravityScale == 1) is more real but less fun :)
            foreach (RagdollLimb limb in ragdollCreature.ragdollLimbs)
            {
                if (!ragdollCreature.isGrounded)
                {
                    if (limb.rigidbody.velocity.y < 0)
                    {
                        limb.rigidbody.gravityScale = movement.fallGravityScale;
                    }
                    else if (limb.rigidbody.velocity.y > 0)
                    {
                        limb.rigidbody.gravityScale = movement.jumpGravityScale;
                    }
                }
                else
                {
                    limb.rigidbody.gravityScale = movement.groundGravityScale;
                }
            }

            bool directionSwitch = centerOfMass.velocity.x > 0 && horizontalMove.x < 0
                || centerOfMass.velocity.x < 0 && horizontalMove.x > 0;

            if (movement.isRemoveXVelocityBeforeDirectionSwitch && directionSwitch)
            {
                foreach (RagdollLimb limb in ragdollCreature.ragdollLimbs)
                {
                    limb.rigidbody.velocity = new Vector2(0, centerOfMass.velocity.y);
                }
            }

            // Move

            if (this.ragdollCreature.aiCont)
            {
                if (this.ragdollCreature.centerObj.transform.position.x > 22)
                    horizontalMove = new Vector2(-2, 0);
                else if (this.ragdollCreature.centerObj.transform.position.x < -22)
                    horizontalMove = new Vector2(2, 0);
                else
                {
                    GameObject enemy = null;
                    if (this.ragdollCreature.centerObj.GetComponent<AIController>().enemy)
                        enemy = this.ragdollCreature.centerObj.GetComponent<AIController>().enemy;

                    if (enemy)
                    {
                        if (enemy.GetComponent<RagdollCreature>().centerObj.transform.position.x > this.ragdollCreature.centerObj.transform.position.x)
                        {
                            horizontalMove = new Vector2(2, 0);
                        }
                        else
                        {
                            horizontalMove = new Vector2(-2, 0);
                        }
                    }
                    else
                    {
                        if (GamePlay.Instance.localPlayer.GetComponent<RagdollCreature>().centerObj.transform.position.x > this.ragdollCreature.centerObj.transform.position.x)
                        {
                            horizontalMove = new Vector2(2, 0);
                        }
                        else
                        {
                            horizontalMove = new Vector2(-2, 0);
                        }
                    }
                }
            }

            if (horizontalMove != Vector2.zero)
            {
                beforeMove = horizontalMove;
            }
            if (this.ragdollCreature.aiCont == true && this.ragdollCreature.centerObj.GetComponent<AIController>().EnemyInAttackRange())
            {
                horizontalMove = Vector2.zero;
                this.ragdollCreature.GetComponent<PlayerController>().interact.OnShot();
            }
            else
            {
                if (this.ragdollCreature.aiCont == true)
                {
                    horizontalMove = beforeMove;
                }
            }
            
            centerOfMass.velocity = Vector2.Lerp(
                centerOfMass.velocity,
                new Vector2(horizontalMove.x * speed, centerOfMass.velocity.y),
                Time.deltaTime * movement.movementLerpFactor);
            
            if (directionSwitch && ragdollCreature.isGrounded)
            {
                // TODO: PlayWalkEffect();
            }

            // Only jump if Creature is grounded and the jump delay is over

            float jumpMovement = 0;
            /*****************************************/
            if (this.ragdollCreature.aiCont == false)
            {
                if (((RoomManager.Instance && ragdollCreature.GetComponent<PhotonView>().IsMine) || (RoomManager.Instance == null && ragdollCreature.aiCont == false)) && GamePlay.Instance.jumpClicked)
                {
                    ragdollCreature.deactivateMusclesInAir = false;
                    jumpMovement = GetJumpMovementInput();
                }
            }
            

            /*****************************************/

            //else if(valuesReceived)
            //	jumpMovement = inputJumpPos;

            if (jumpMovement > 0 && ragdollCreature.isGrounded
                && Time.time >= lastJumpTime + movement.jumpDelay)
            {
                if (movement.isRemoveYVelocityBeforeJumping)
                {
                    foreach (RagdollLimb limb in ragdollCreature.ragdollLimbs)
                    {
                        limb.rigidbody.velocity = new Vector2(centerOfMass.velocity.x, 0);
                    }
                }

                // The actual jump
                centerOfMass.AddForce(new Vector2(0, Vector2.up.y) * movement.highForce, ForceMode2D.Impulse);

                lastJumpTime = Time.time;
                // TODO: PlayJumpEffect();
            }
        }
    }

    protected virtual float GetJumpMovementInput()
    {
        float jumpMovement;
        if (movement.useNewInputSystem)
        {
            jumpMovement = moveVector.y;
        }
        else
        {
            jumpMovement = UnityEngine.Random.RandomRange(40, 60f);
            //jumpMovement = Input.GetAxis("Vertical");
        }

        return jumpMovement;
    }

    protected virtual Vector2 GetMovementInput()
    {
        Vector2 move;
        if (movement.useNewInputSystem)
        {
            move = moveVector;
        }
        else
        {
            //move = new Vector2(	Input.GetAxis("Horizontal"),Input.GetAxis("Vertical"));
            if (this.ragdollCreature.aiCont == false)
                move = GamePlay.Instance.MoveController.movementVector;
            else
                move = horizontalMove;
        }

        return move;
    }

    protected virtual Vector2 GetHorizontalMovementInput()
    {
        return new Vector2(GetMovementInput().x, 0);
        //return Vector2.zero;
    }

    protected virtual Vector2 GetVertivalMovementInput()
    {
        return new Vector2(0, GetMovementInput().y);
    }

    void UpdateAnimations()
    {
        Vector2 move = Vector2.zero;
        //if (GetComponent<PhotonView>().IsMine)


        move = GetMovementInput();

        //else if (valuesReceived)
        //	move = inputPos;
        /*
        if (ragdollCreature.gameObject.GetComponent<PhotonView>() && ragdollCreature.gameObject.GetComponent<PhotonView>().IsMine)
            ragdollCreature.gameObject.GetComponent<PhotonView>().RPC("RPC_UpdateAnimations", RpcTarget.All, move);
        */
        RPC_UpdateAnimations(move);
    }
    //[PunRPC]
    void RPC_UpdateAnimations(Vector2 move)
    {

        ragdollCreature.PlayWalkAnimation(move);
    }

    //void PlayWalkEffect(RagdollCreatureMovement movement)
    //{
    //	if (null != movement.walkParticleSystem && playWalkEffect && !walkParticleSystem.isPlaying)
    //	{
    //		walkParticleSystem.Play();
    //	}
    //}

    //void PlayJumpEffect()
    //{
    //	if (null != jumpParticleSystem && playJumpEffect && !jumpParticleSystem.isPlaying)
    //	{
    //		jumpParticleSystem.Play();
    //	}
    //}

    /// <summary>
    /// Get the move vector from the InputActions.
    /// </summary>
    /// <param name="context"></param>
    public void OnMove(InputAction.CallbackContext context)
    {
        moveVector = context.ReadValue<Vector2>();
    }

    public void OnRagdollLimbCollisionEnter2D(object sender, Collision2D col)
    {
        Debug.Log("OnRagdollLimbCollisionEnter2D: " + col.ToString());
    }

    public void OnRagdollLimbCollisionExit2D(object sender, Collision2D col)
    {
        Debug.Log("OnRagdollLimbCollisionExit2D: " + col.ToString());
    }

    public void OnTriggerEnter2D(Collider2D col)
    {
        Debug.Log("OnTriggerEnter2D: " + col.ToString());
    }

    public void OnTriggerExit2D(Collider2D col)
    {
        Debug.Log("OnTriggerExit2D: " + col.ToString());
    }

    public bool UseNewInputSystem()
    {
        return movement.useNewInputSystem;
    }

    public void SetUseNewInputSystem(bool useNewInputSystem)
    {
        movement.useNewInputSystem = useNewInputSystem;
    }
}

