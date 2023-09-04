using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Main
{
    
    public class Opponent : Boxer
    {
        [SerializeField] private SpriteRenderer SR;
        [SerializeField] private ParticleSystem hitBodyPS, hitHeadPS;
        [SerializeField] private Player player;
        [SerializeField] private float chanceToBlockBody = 0.75f;
        [SerializeField] private float chanceToBlockHead = 0.5f;
        [SerializeField] private float chanceToBlockBodyUnExpected = 0.3f; //this is for head or body changed;
        [SerializeField] private float chanceToBlockHeadUnExpected = 0.15f;
        [SerializeField] private float chanceToAttackLarge = 0.6f;
        [SerializeField] private float chanceToAttackVeryLarge = 0;
        [SerializeField] private int smallAttackDamage = 10;
        [SerializeField] private int largeAttackDamage = 20;
        [SerializeField] private float unbotheredTimeToAttack = 5;
        [SerializeField] private int maxComboHit = 3; //this is not the old one  
        [SerializeField] private float endLag = 0.5f;
        [SerializeField] private float starEndlag = 0.25f;
        //[SerializeField] private State[] combo;

        int currentComboHit;

        //private float chanceToAttack;
        //private float chanceToCombo;
        //private bool isInCombo = false;
        //private int currentComboIndex = 0;

        private bool lastAttackBody;

        private float timeToNextAttack;


        private bool isTryingGetUp = false;

        public enum State
        {
            idle,
            blockingBody, blockingHead,//opponent block head too
            windingUpVunerable, windingUpBlockable, windingUpInvincible,
            attackingLarge,
            attackingSmall, attackingLeft,
            takingDamageBody, takingDamageHead
        }

        private State currentState;

        /////////////////////////////////////////////////////////////////////

        private void Start()
        {
            SetUp(maxHealth);
            previousMaxHealth = maxHealth;
        }

        public override void SetUp(int newHealth)
        {
            base.SetUp(newHealth);
            
            ReturnToIdle();
            //chanceToAttack = 0;
            timeToNextAttack = unbotheredTimeToAttack;
            isTryingGetUp = false;

            GameManager.Instance.OpponentHealthUpdate(newHealth, maxHealth);
        }


        //////////////////////////////////////////////////////////////
        void Update()
        {
#if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.E)) //for debugging purposes
            {
                OnDamage(25);
            }
#endif

            if (currentState != State.idle || GameManager.Instance.gameState != GameManager.GameState.Round)
                return;
            
            if (player.stamina <= 0)
			{
                timeToNextAttack = 0;
            }

            if (timeToNextAttack > 0) timeToNextAttack -= Time.deltaTime;
            else
			{
                if (Random.Range(0f, 1f) <= chanceToAttackVeryLarge)
                {
                    AN.Play("OpponentVeryLargeAttack");
                    SR.flipX = false;
                }
                else if (Random.Range(0f, 1f) <= chanceToAttackLarge)
                {
                    //WindUp();
                    AN.Play("OpponentLargeAttack");
                    SR.flipX = false;
                }
                else
                {
                    //SmallAttack();
                    //currentState = State.windingUp;
                    AN.Play("OpponentSmallAttack");
                    SR.flipX = false;
                }

                timeToNextAttack = unbotheredTimeToAttack;
                currentComboHit = 0;
            }
        }

        protected override void FixedUpdate()
		{
			base.FixedUpdate();

            if (knockedDown && !isTryingGetUp)
			{
                print("FixedUpdate..isTryingGetUp!!");
                StartCoroutine(TryGetUp());
            }
		}

        public void OnHit(Player.State attackerState, int damage)
        {
            //chanceToAttack += Random.Range(0.25f, 0.5f);
            //if (timeToNextAttack )
            timeToNextAttack = Random.Range(0.1f, 2.5f);

            if (currentState == State.windingUpVunerable)
			{
                player.GainStar();
			} 
            else
			{
                PredictiveBlock(attackerState);
            }

            switch (attackerState)
            {
                case Player.State.attackingLeftBody:
                    lastAttackBody = true;
                    if (currentState == State.blockingBody)
                    {
                        Debug.Log("opponent block");
                        AppManager.Instance.sfxManager.PlaySound("Block1");
                        //TODO:player loses stamina
                    }
                    else
                    {
                        OnDamage(damage);
                        hitBodyPS.Play();
                        AN.speed = endLag;
                        AppManager.Instance.sfxManager.PlaySound("Hurt2");
                        AN.Play("OpponentHitBody");
                    }
                    break;
                case Player.State.attackingLeftHead:
                    lastAttackBody = false;
                    if (currentState == State.blockingHead)
                    {
                        Debug.Log("opponent block");
                        AppManager.Instance.sfxManager.PlaySound("Block1");
                    }
                    else
                    {
                        OnDamage(damage);
                        hitHeadPS.Play();
                        AN.speed = endLag;
                        AN.Play("OpponentHitHeadLeft");
                        AppManager.Instance.sfxManager.PlaySound("Hurt2");
                    }
                    break;
                case Player.State.attackingRightBody:
                    lastAttackBody = true;
                    if (currentState == State.blockingBody)
                    {
                        Debug.Log("opponent block");
                        AppManager.Instance.sfxManager.PlaySound("Block1");
                    }
                    else
                    {
                        OnDamage(damage);
                        hitBodyPS.Play();
                        AN.speed = endLag;
                        AN.Play("OpponentHitBody");
                        AppManager.Instance.sfxManager.PlaySound("Hurt2");
                    }
                    break;
                case Player.State.attackingRightHead:
                    lastAttackBody = false;
                    if (currentState == State.blockingHead)
                    {
                        Debug.Log("opponent block");
                        AppManager.Instance.sfxManager.PlaySound("Block1");
                    }
                    else
                    {
                        OnDamage(damage);
                        hitHeadPS.Play();
                        AN.speed = endLag;
                        AN.Play("OpponentHitHeadRight");
                        AppManager.Instance.sfxManager.PlaySound("Hurt2");
                        SR.flipX = true;
                    }
                    break;
                case Player.State.attackingStarPunch:
                    
                    OnDamage(damage);
                    hitHeadPS.Play();

                    if (currentState != State.idle && !knockedDown)
                        AN.speed = starEndlag;

                    AppManager.Instance.sfxManager.PlaySound("Hurt2");
                    AppManager.Instance.sfxManager.PlaySound("CrowdCheer");
                    GameManager.Instance.StarPunchCheer();
                    break;
            }
        }

        private void ReturnToIdle()
        {
            currentState = State.idle;
            SR.flipX = false;
            AN.speed = 1;
        }

        private void PredictiveBlock(Player.State attackerState)
        {
            currentComboHit++;

            // TODO: if opponent taking damage, check whether they are 50% or more in their animation, if so, they can block, otherwise they can't
            if (currentState == State.attackingLarge || currentState == State.attackingSmall
                || currentState == State.windingUpVunerable)
                return;

            if (attackerState == Player.State.attackingLeftBody || attackerState == Player.State.attackingRightBody)
            {
                if (currentComboHit >= maxComboHit ? true 
                    : (Random.Range(0f, 1f) <= (!lastAttackBody ? chanceToBlockBodyUnExpected : chanceToBlockBody)))
                {
                    currentState = State.blockingBody;
                    AN.Play("OpponentBlockBody");
                    AppManager.Instance.sfxManager.PlaySound("Block1");
                }
                print(!lastAttackBody ? chanceToBlockBodyUnExpected : chanceToBlockBody);
            }
            else if (currentComboHit >= maxComboHit ? true 
                : (attackerState == Player.State.attackingLeftHead || attackerState == Player.State.attackingRightHead))
            {
                if (Random.Range(0f, 1f) <= (lastAttackBody ? chanceToBlockHeadUnExpected : chanceToBlockHead))
                {
                    currentState = State.blockingHead;
                    AN.Play("OpponentBlockHead");
                    AppManager.Instance.sfxManager.PlaySound("Block1");
                }
                print(lastAttackBody ? chanceToBlockHeadUnExpected : chanceToBlockHead);
            }
        }

        private void WindUpVunerable()
		{
            currentState = State.windingUpVunerable;
		}

        private void WindUpInvincible()
        {
            currentState = State.windingUpInvincible;
        }

        private void BlockHead() {
            currentState = State.blockingHead;
        }

        private void BlockBody() {
            currentState = State.blockingBody;
        }

        private void WindUpBlockable()
		{
            currentState= State.windingUpBlockable;
		}

        private void LargeAttack()
        {
            currentState = State.attackingLarge;
        }

        private void SmallAttack()
        {
            currentState = State.attackingSmall;
        }

        private void LeftAttack()
        {
            currentState = State.attackingLeft;
        }

        private void Contact() {
            switch (currentState) {
                case State.attackingLarge:
                    if (player.OnHit(currentState, largeAttackDamage))
                    {
                        Debug.Log("player took damage");
                    } 
                    else
					{
                        AN.speed = endLag;
                    }
                    break;
                case State.attackingSmall:
                    if (player.OnHit(currentState, smallAttackDamage))
                    {
                        Debug.Log("player took damage");
                    }
                    else
                    {
                        AN.speed = endLag;
                    }
                    break;
            }
        }

        /////////////////////////////////////////////////////////////////////////////////////
        protected override bool OnDamage(int damage) //from boxer class
        {
            if (base.OnDamage(damage))
            {
                GameManager.Instance.AddScore(10);
                GameManager.Instance.OpponentHealthUpdate(health, maxHealth);
                AppManager.Instance.sfxManager.PlaySound("Hurt1");

                if (!isInOnDamageFeedbackRoutine)
				{
                    //StartCoroutine(OnDamageFeedback());
				}

                return true;
            }

            return false; //return value don't matter 
        }

        bool isInOnDamageFeedbackRoutine;

        private IEnumerator OnDamageFeedback()
		{
            isInOnDamageFeedbackRoutine = true;

            SR.color = Color.black;
            yield return new WaitForSeconds(0.14f);
            SR.color = Color.white;

            isInOnDamageFeedbackRoutine = false;
        }

        protected override void OnDown() //from boxer class
        {
            if (knockedDown) return;

            GameManager.Instance.OpponentHealthUpdate(0, maxHealth);
            base.OnDown();
            //print("knockddown..!!");
            //AN.Play("OpponentKnockedDown");
            AN.SetTrigger("Knockdown");
            AppManager.Instance.sfxManager.PlaySound("KO");
            AppManager.Instance.sfxManager.PlaySound("CheerShort");
            GameManager.Instance.AddScore(1000);
        }

        IEnumerator TryGetUp()
		{
            isTryingGetUp = true;
            print("trying.... get up!!");

            if (liveTime > 50)
            {
                GameManager.Instance.StartCountDown(true, 2);
                yield return new WaitForSeconds(2); //TODO more randomize

            }
            else if (liveTime > 40)
            {
                GameManager.Instance.StartCountDown(true, 4);
                yield return new WaitForSeconds(4);//TODO more randomize

            }
            else if (liveTime > 30)
            {
                GameManager.Instance.StartCountDown(true, 5);
                yield return new WaitForSeconds(5);

            }
            else 
            {
                GameManager.Instance.StartCountDown(true, 4);
			    yield return new WaitForSeconds(4); 
            }

            if (GetUp()) isTryingGetUp = false;
        }

        protected override bool GetUp()
        {
             //TODO move place

            if (base.GetUp()) //TODO: in base, set health based on previous health
            {
                //place for sound
                print("getUP!");
                GameManager.Instance.EndCountDown(true, true);
                AN.Play("OpponentKnockedDownGetUp");
                return true;
            }
            else
            {
                //let GameManager know
                print("CANT getUP!");
                GameManager.Instance.EndCountDown(true, false);
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
    }
}




