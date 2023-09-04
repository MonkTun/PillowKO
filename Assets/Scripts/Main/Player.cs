using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Main
{
	public class Player : Boxer
	{
		// Test comment

		#region fields

		[SerializeField] private SpriteRenderer SR;
		[SerializeField] private Opponent opponent;
		[SerializeField] private int damage, starPunchDamage;
		public enum State
		{
			idle,
			dodgingLeft, dodgingRight, dodgingVulnerable,
			blocking,
			attackingLeftBody, attackingRightBody,
			attackingLeftHead, attackingRightHead, attackingStarPunch,
			takingDamageBody, takingDamageHead,
		}

		private Keyboard KB;

		private State currentState;

		//these are used for coyote time feels
		float leftDodgeSpamPreventor, rightDodgeSpamPreventor; //TODO spamPreventorforBlock?

		int attackCoyote; //0 null //1 rightBody, 2 rightHead, 3 leftBody, 4 leftHead, 
		bool dodgeCoyoteTime;
		int tryGetUpCount;
		//bool isTryingGetUp;

		int star;
		int originalStarPunchDamage;

		#endregion

		#region initialization

		private void Start()////////////////////////////////////////
		{
			KB = Keyboard.current;
			previousMaxHealth = maxHealth;
			//AN = GetComponent<Animator>();
			SetUp(maxHealth);
			GameManager.Instance.UpdateStar(star);
			stamina = 20;
			GameManager.Instance.UpdateStamina(stamina);
			originalStarPunchDamage = starPunchDamage;
		}

		public override void SetUp(int newHealth)////////////////
		{
			base.SetUp(newHealth);
			attackCoyote = 0;
			leftDodgeSpamPreventor = 0;
			rightDodgeSpamPreventor = 0;
			dodgeCoyoteTime = false;
            star = 0;
            stamina = 15;
            ReturnToIdle();
			GameManager.Instance.PlayerHealthUpdate(health, maxHealth);
            GameManager.Instance.UpdateStamina(stamina);
			GameManager.Instance.UpdateStar(star);
            tryGetUpCount = 0;
		}

		#endregion

		#region Update

		// Update is called once per frame
		void Update() /////////////////////////////////////////////
		{
			if (!knockedDown)
				Control();
			else
			{
				TryGetUp();
			}
        }

		#endregion

		#region Movement

		private void Control()
		{
            if (GameManager.Instance.gameState != GameManager.GameState.Round) return;

			if (rightDodgeSpamPreventor >= 0)
				rightDodgeSpamPreventor -= Time.deltaTime;
			if (leftDodgeSpamPreventor >= 0)
				leftDodgeSpamPreventor -= Time.deltaTime;

			if (currentState != State.idle)
			{
				switch (currentState)
				{
					case State.blocking:
						if (KB.downArrowKey.wasReleasedThisFrame)
						{
							BlockEnd();
							dodgeCoyoteTime = true;
						}
						break;
					case State.dodgingLeft:
						if (KB.rightArrowKey.wasPressedThisFrame)
						{
							leftDodgeSpamPreventor = float.MaxValue;
							if (stamina >= 0) AN.speed = 2;
						}
						break;
					case State.dodgingRight:
						if (KB.leftArrowKey.wasPressedThisFrame)
						{
							rightDodgeSpamPreventor = float.MaxValue;
							if (stamina >= 0) AN.speed = 2;
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

			if (KB.enterKey.wasPressedThisFrame)
			{
				SpecialAttack();
			}
			else if (KB.xKey.wasPressedThisFrame)
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
            if (stamina <= 0 && GameManager.Instance.gameState == GameManager.GameState.Round && !knockedDown)
            {
                AN.Play("PlayerTired");
                print("player tired");
            }
            currentState = State.idle;
            SR.flipX = false;
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
            Debug.Log(stamina);

			if (stamina <= 0) return;

			SR.flipX = true;
			currentState = State.attackingLeftBody;
			AN.Play("PlayerAttackLeftBody");
			leftDodgeSpamPreventor = 0;
			rightDodgeSpamPreventor = 0;

			UseStamina(1);
		}

		void AttackLeftHead()
		{
			Debug.Log(stamina);

			if (stamina <= 0) return;

			SR.flipX = true;
			currentState = State.attackingLeftHead;
			AN.Play("PlayerAttackLeftHead");
			leftDodgeSpamPreventor = 0;
			rightDodgeSpamPreventor = 0;

			UseStamina(1);
		}

		void AttackRightBody()
		{
            Debug.Log(stamina);

			if (stamina <= 0) return;

			SR.flipX = false;
			currentState = State.attackingRightBody;
			AN.Play("PlayerAttackRightBody");
			leftDodgeSpamPreventor = 0;
			rightDodgeSpamPreventor = 0;

			UseStamina(1);
		}

		void AttackRightHead()
		{
            Debug.Log(stamina);

            if (stamina <= 0) return;

			SR.flipX = false;
			currentState = State.attackingRightHead;
			AN.Play("PlayerAttackRightHead");
			leftDodgeSpamPreventor = 0;
			rightDodgeSpamPreventor = 0;

			UseStamina(1);
		}

		void DodgeLeft()
		{
			if (stamina <= 0) AN.speed = 0.75f;

			currentState = State.dodgingVulnerable;
			leftDodgeSpamPreventor = 0.2f;
			rightDodgeSpamPreventor = 0;
			AN.Play("PlayerDodgeLeft");
			SR.flipX = true;
		}

		void DodgeRight()
		{
			if (stamina <= 0) AN.speed = 0.75f;

			currentState = State.dodgingVulnerable;
			rightDodgeSpamPreventor = 0.2f;
			leftDodgeSpamPreventor = 0;
			AN.Play("PlayerDodgeRight");
			SR.flipX = false;
		}

		public void DodgeRegisterLeft() //callfrom animator
		{
			currentState = State.dodgingLeft;
		}

		public void DodgeRegisterRight()
		{
			currentState = State.dodgingRight;
		}

		public void DodgeUnRegisterLeft() //callfrom animator
		{
			currentState = State.dodgingVulnerable;
        }

		public void DodgeUnRegisterRight()
		{
			currentState = State.dodgingVulnerable;
		}

		void BlockStart()
		{
			currentState = State.blocking;
			AN.Play("PlayerBlockStart");
		}

		void BlockEnd()
		{
			AN.SetTrigger("ReleaseBlock");
			leftDodgeSpamPreventor = 0;
			rightDodgeSpamPreventor = 0;
		}

		void SpecialAttack()
		{
			if (UseStar())
			{
				currentState = State.attackingStarPunch;
				AN.Play("PlayerAttackStarPunch");
			}
		}

		private void Contact()
		{
			if (currentState == State.attackingStarPunch)
			{
				opponent.OnHit(currentState, starPunchDamage);
			} 
			else
			{
				opponent.OnHit(currentState, damage);
			}
		}

		#endregion

		#region Health

		public bool OnHit(Opponent.State attackerState, int damage)
		{
			if (GameManager.Instance.gameState != GameManager.GameState.Round || knockedDown) return false;

			switch (attackerState)
			{
				case Opponent.State.attackingLarge:
					if (currentState == State.dodgingLeft)
					{
						GainStamina();
					}
					else if (currentState == State.blocking)
					{
                        OnDamage(damage / 3);
						UseStamina(1);
						AppManager.Instance.sfxManager.PlaySound("Block1");
					}
					else
					{
                        OnDamage(damage);
						AN.Play("PlayerTakingDamage");
						UseStamina(4);
						AppManager.Instance.sfxManager.PlaySound("Hurt1");
						return true;
					}
					break;
				case Opponent.State.attackingSmall:
					if (currentState == State.dodgingLeft || currentState == State.dodgingRight)
					{
						GainStamina();
					}
					else if (currentState == State.blocking)
					{
                        OnDamage(damage / 3);
						UseStamina(1);
						AppManager.Instance.sfxManager.PlaySound("Block1");
					}
					else
					{
                        SR.flipX = true;
                        AN.Play("PlayerTakingDamage");
                        AppManager.Instance.sfxManager.PlaySound("Hurt1");
                        OnDamage(damage);
						UseStamina(4);
						return true;
					}
					break;
				case Opponent.State.attackingLeft:
					if (currentState == State.dodgingRight)
					{
						GainStamina();
					}
					else if (currentState == State.blocking)
					{
                        AppManager.Instance.sfxManager.PlaySound("Block1");
						OnDamage(damage / 3);
						UseStamina(1);
					}
					else
					{
                        AppManager.Instance.sfxManager.PlaySound("Hurt1");
                        OnDamage(damage);
						UseStamina(4);
						return true;
					}
					break;
			}

			return false;
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////

		protected override bool OnDamage(int damage) //from boxer class
		{
			if (base.OnDamage(damage))
			{
				GameManager.Instance.PlayerHealthUpdate(health, maxHealth);
				return true;
			} 

			return false; //return value don't matter 
		}

		protected override void OnDown() //from boxer class
		{
            //print("player down");
            if (knockedDown) return;

			GameManager.Instance.PlayerHealthUpdate(0, maxHealth);
			base.OnDown();
			print("player down anima");
            //AN.Play("PlayerGettingKnockedOut");
            AN.SetTrigger("KnockDown");
            print("player down anim called");

			AppManager.Instance.sfxManager.PlaySound("KO3");
            AppManager.Instance.sfxManager.PlaySound("CheerShort");
            GameManager.Instance.StartCountDown(false, 10);
		}

		private void TryGetUp() //TODO upgrade with animation
		{
			if (tryGetUpCount >= 10)
			{
				GetUp();
				return;
			}

			if (KB.xKey.wasPressedThisFrame || KB.zKey.wasPressedThisFrame)
			{
				tryGetUpCount++;
				AN.SetTrigger("GetUpAttempt");
			}
		}

		protected override bool GetUp()
		{			
			if (base.GetUp()) //TODO: in base, set health based on previous health
			{
				GameManager.Instance.EndCountDown(false, true);
				AN.Play("PlayerGetUp");
				return true;
			}
			else
			{
				GameManager.Instance.EndCountDown(false, false);
                return false;
			}
		}

		//base.GetUp will call KO.
		protected override void KO()
		{
			base.KO();
			//letGameManagerKnow
			print("KO");
		}

		#endregion

		#region specials

		public void GainStamina()
		{
			if (stamina <= 0)
			{
				stamina += 15;
                AppManager.Instance.sfxManager.PlaySound("RecoverStamina");
            }
			GameManager.Instance.UpdateStamina(stamina);
		}

		public void UseStamina(int amount)
		{
			stamina -= amount;
            if (stamina < 0)
            {
                AN.Play("PlayerTired");
                stamina = 0;
            }
            GameManager.Instance.UpdateStamina(stamina);
		}

		public void GainStar()
		{
			if (star >= 3)
			{
				return;
			}
			star++;
			GameManager.Instance.UpdateStar(star);
			AppManager.Instance.sfxManager.PlaySound("Powerup1");
		}

		public bool UseStar()
		{
			if (star > 0)
			{
				starPunchDamage = originalStarPunchDamage;
				starPunchDamage += star * 15;
				star = 0;
				GameManager.Instance.UpdateStar(star);
                AppManager.Instance.sfxManager.PlaySound("Star");
                return true;
			}
			return false;
		}

		#endregion
	}
}




