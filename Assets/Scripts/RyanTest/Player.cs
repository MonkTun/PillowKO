using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RyanTest
{
	public class Player : Boxer
	{
		#region fields

		[SerializeField] private Opponent opponent;

		public enum State
		{
			idle,
			dodgingLeft, dodgingRight,
			blocking,
			attackingLeftBody, attackingRightBody,
			attackingLeftHead, attackingRightHead,
			takingDamageBody, takingDamageHead
		}

		private State currentState;

		private Keyboard KB;
		//these are used for coyote time feels
		float leftDodgeSpamPreventor, rightDodgeSpamPreventor; //TODO spamPreventorforBlock?

		int attackCoyote; //0 null //1 rightBody, 2 rightHead, 3 leftBody, 4 leftHead, 
		bool dodgeCoyoteTime;

		#endregion

		#region initialization

		private void Awake()
		{
			KB = Keyboard.current;			
			SetUp();
		}

		public override void SetUp()
		{
			base.SetUp();
			attackCoyote = 0;
			leftDodgeSpamPreventor = 0;
			rightDodgeSpamPreventor = 0;
			dodgeCoyoteTime = false;
			ReturnToIdle();
		}

		#endregion

		#region Update

		// Update is called once per frame
		void Update()
		{
			Control();
		}

		#endregion

		#region Movement

		private void Control()
		{
			if (rightDodgeSpamPreventor > 0)
				rightDodgeSpamPreventor -= Time.deltaTime;
			if (leftDodgeSpamPreventor > 0)
				leftDodgeSpamPreventor -= Time.deltaTime;

			if (currentState != State.idle)
			{
				switch (currentState)
				{
					case State.blocking:
						if (KB.downArrowKey.wasReleasedThisFrame)
						{
							BlockEnd();
						}
						break;
					case State.dodgingLeft:
						if (KB.rightArrowKey.wasPressedThisFrame)
						{
							leftDodgeSpamPreventor = float.MaxValue;
							AN.speed = 2;
						}
						break;
					case State.dodgingRight:
						if (KB.leftArrowKey.wasPressedThisFrame)
						{
							rightDodgeSpamPreventor = float.MaxValue;
							AN.speed = 2;
						}
						break;
					default:
						dodgeCoyoteTime = true;
						break;
				}

				if ((currentState != State.attackingLeftHead && currentState != State.attackingLeftBody
					&& currentState != State.attackingRightHead && currentState != State.attackingRightBody)
					&& AN.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.5f)
				{
					if (KB.xKey.wasPressedThisFrame)
					{
						if (KB.upArrowKey.isPressed) attackCoyote = 2;
						else attackCoyote = 1;

					}
					else if (KB.zKey.wasPressedThisFrame)
					{
						if (KB.upArrowKey.isPressed) attackCoyote = 4;
						else attackCoyote = 3;
					}
				}

				return;
			}

			if (KB.xKey.wasPressedThisFrame)
			{
				if (KB.upArrowKey.isPressed)
				{
					AttackRightHead();
				}
				else
				{
					AttackRightBody();
				}
			}
			else if (KB.zKey.wasPressedThisFrame)
			{
				if (KB.upArrowKey.isPressed)
				{
					AttackLeftHead();
				}
				else
				{
					AttackLeftBody();
				}
			}
			else if (KB.rightArrowKey.wasPressedThisFrame)
			{
				DodgeRight();
			}
			else if (KB.leftArrowKey.wasPressedThisFrame)
			{
				DodgeLeft();
			}
			else if (KB.downArrowKey.wasPressedThisFrame)
			{
				BlockStart();
			}
		}

		private void ReturnToIdle()
		{
			currentState = State.idle;
			AN.speed = 1;

			switch (attackCoyote)
			{
				case 1:
					AttackRightBody();
					attackCoyote = 0;
					return;
				case 2:
					AttackRightHead();
					attackCoyote = 0;
					return;
				case 3:
					AttackLeftBody();
					attackCoyote = 0;
					return;
				case 4:
					AttackLeftHead();
					attackCoyote = 0;
					return;
			}

			if (KB.downArrowKey.isPressed)
			{
				BlockStart();
			} 
			else if (KB.rightArrowKey.isPressed && dodgeCoyoteTime)
			{
				DodgeRight();
			}
			else if (KB.leftArrowKey.isPressed && dodgeCoyoteTime)
			{
				DodgeLeft();
			}

			dodgeCoyoteTime = false;
		}

		#endregion

		#region Action

		void AttackLeftBody()
		{
			currentState = State.attackingLeftBody;
			AN.Play("PlayerAttackRightBody");//dont confuse this with attack left
			opponent.OnHit(currentState, damage);
			leftDodgeSpamPreventor = 0;
			rightDodgeSpamPreventor = 0;
		}

		void AttackLeftHead()
		{
			currentState = State.attackingLeftHead;
			AN.Play("PlayerAttackRightHead"); //dont confuse this with attack left head
			opponent.OnHit(currentState, damage);
			leftDodgeSpamPreventor = 0;
			rightDodgeSpamPreventor = 0;
		}

		void AttackRightBody()
		{
			currentState = State.attackingRightBody;
			AN.Play("PlayerAttackLeftBody"); //dont confuse this with attack right
			opponent.OnHit(currentState, damage);
			leftDodgeSpamPreventor = 0;
			rightDodgeSpamPreventor = 0;
		}

		private void AttackRightHead()
		{
			currentState = State.attackingRightHead;
			AN.Play("PlayerAttackLeftHead"); //dont confuse this with attack right head
			opponent.OnHit(currentState, damage);
			leftDodgeSpamPreventor = 0;
			rightDodgeSpamPreventor = 0;
		}

		private void DodgeLeft()
		{
			currentState = State.dodgingLeft;
			leftDodgeSpamPreventor = 0.2f;
			rightDodgeSpamPreventor = 0;
			AN.Play("PlayerDodgeLeft");
		}

		private void DodgeRight()
		{
			currentState = State.dodgingRight;
			rightDodgeSpamPreventor = 0.2f;
			leftDodgeSpamPreventor = 0;
			AN.Play("PlayerDodgeRight");
		}

		private void BlockStart()
		{
			currentState = State.blocking;
			AN.Play("PlayerBlockStart");
		}

		private void BlockEnd()
		{
			AN.SetTrigger("ReleaseBlock");
			leftDodgeSpamPreventor = 0;
			rightDodgeSpamPreventor = 0;
		}

		#endregion

		#region Health

		public bool OnHit(Opponent.State attackerState, int damage)
		{
			switch (attackerState)
			{
				case Opponent.State.attackingLarge:
					if (currentState == State.dodgingLeft || currentState == State.dodgingRight)
					{
						Debug.Log("player dodge");
					}
					else if (currentState == State.blocking)
					{
						Debug.Log("player block");
					}
					else
					{
						OnDamage(damage);
                        return true;
                    }
					break;
				case Opponent.State.attackingSmall:
					if (currentState == State.dodgingLeft || currentState == State.dodgingRight)
					{
						Debug.Log("player dodge");
					}
					else if (currentState == State.blocking)
					{
						Debug.Log("player block");
					}
					else
					{
						OnDamage(damage);
                        return true;
                    }
					break;
            }

            return false;
        }

		protected override bool OnDamage(int damage) //from boxer class
		{
			if (base.OnDamage(damage))
			{
				//TODO hit sound
				GameManager.Instance.PlayerHealthUpdate(health, maxHealth);
				return true;
			} 

			return false; //return value don't matter 
		}

		protected override void OnDeath() //from boxer class
		{
			if (knockedDown) return;

			base.OnDeath();

			//let gameManager know
		}

		#endregion
	}
}



