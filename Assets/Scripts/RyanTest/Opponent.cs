using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RyanTest
{
    public class Opponent : Boxer
    {
        [SerializeField] private Player player;
        [SerializeField] private float chanceToBlockBody = 0.75f;
        [SerializeField] private float chanceToBlockHead = 0.5f;
        [SerializeField] private float tendencyToAttackLarge = 0.5f;
        [SerializeField] private float attackThreshold = 0.75f;

        private float chanceToAttack;

        public enum State
        {
            idle,
            blockingBody, blockingHead,//opponent block head too
            windingUp,
            attackingLarge,
            attackingSmall,
            takingDamageBody, takingDamageHead
        }

        private State currentState;

        private void Awake()
        {
            ReturnToIdle();
            SetUp();
        }

        // Update is called once per frame
        void Update() {
            if (currentState != State.idle)
                return;

            if (chanceToAttack >= attackThreshold) {
                Debug.Log("attack");
                if (Random.Range(0f, 1f) <= tendencyToAttackLarge) {
                    WindUp();
                } else {
                    SmallAttack();
                }
                chanceToAttack = 0;
            }
        }

        public void OnHit(Player.State attackerState, int damage)
        {
            chanceToAttack += Random.Range(0.1f, 0.25f);

            PredictiveBlock(attackerState);

            switch (attackerState)
            {
                case Player.State.attackingLeftBody:
                    if (currentState == State.blockingBody)
                    {
                        Debug.Log("opponent block");
                        //TODO:player loses stamina
                    }
                    else
                    {
                        OnDamage(damage);
                    }
                    break;
                case Player.State.attackingLeftHead:
                    if (currentState == State.blockingHead)
                    {
                        Debug.Log("opponent block");
                    }
                    else
                    {
                        OnDamage(damage);
                    }
                    break;
                case Player.State.attackingRightBody:
                    if (currentState == State.blockingBody)
                    {
                        Debug.Log("opponent block");
                    }
                    else
                    {
                        OnDamage(damage);
                    }
                    break;
                case Player.State.attackingRightHead:
                    if (currentState == State.blockingHead)
                    {
                        Debug.Log("opponent block");
                    }
                    else
                    {
                        OnDamage(damage);
                    }
                    break;
            }
        }

        private void ReturnToIdle()
        {
            currentState = State.idle;
        }

        private void PredictiveBlock(Player.State attackerState)
        {
            if (attackerState == Player.State.attackingLeftBody || attackerState == Player.State.attackingRightBody) {
                if (Random.Range(0f, 1f) <= chanceToBlockBody) {
                    currentState = State.blockingBody;
                    AN.Play("OpponentBlockBody");
                }
            }
            else if (attackerState == Player.State.attackingLeftHead || attackerState == Player.State.attackingRightHead) {
                if (Random.Range(0f, 1f) <= chanceToBlockHead) {
                    currentState = State.blockingHead;
                    AN.Play("OpponentBlockHead");
                }
            }
        }

        private void WindUp() {
            currentState = State.windingUp;
            AN.Play("OpponentWindUp");
        }

        private void LargeAttack() {
            currentState = State.attackingLarge;
            if (player.OnHit(currentState, damage)) {
                chanceToAttack += Random.Range(0.5f, 1f);
            }
        }

        private void SmallAttack() {
            currentState = State.attackingSmall;
            if (player.OnHit(currentState, damage)) {
                chanceToAttack += Random.Range(0.5f, 1f);
            }
            AN.Play("OpponentSmallAttack");
        }

        protected override bool OnDamage(int damage) //from boxer class
        {
            if (base.OnDamage(damage))
            {
                //TODO hit sound
                print("opponent damaged");
                GameManager.Instance.OpponentHealthUpdate(health, maxHealth);
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

    }
}




