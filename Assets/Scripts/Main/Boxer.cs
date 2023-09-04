using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Main
{
    [RequireComponent(typeof(Animator))]
    public class Boxer : MonoBehaviour
    {
        [SerializeField] protected int maxHealth;
        [SerializeField] protected int maxStamina;
        [SerializeField] protected float koLiveTime = 15;
        [SerializeField] protected Animator AN;

        public int health { get; protected set; }
        public int previousMaxHealth { get; protected set; }
        public int stamina { get; protected set; }
        public bool knockedDown { get; protected set; }
        public float liveTime { get; protected set; }

        public virtual void SetUp(int newHealth)
        {
            health = newHealth;
            stamina = maxStamina;
            knockedDown = false;
            liveTime = 0;
        }

        protected virtual void FixedUpdate()
		{
            if (!knockedDown) liveTime += Time.deltaTime;
		}

        protected virtual bool OnDamage(int damage) //actually damage take
		{
            if (knockedDown) return false;

            health -= damage;
            //print("damage take");

            if (health <= 0)
			{
                OnDown();
                return false;
			} 
            else return true;
			
		}

        protected virtual void OnDown()
		{
            if (knockedDown) return;

            knockedDown = true;
		}

        protected virtual bool GetUp()
		{
            if (liveTime < koLiveTime)
			{
                print("getUpfailed: " + liveTime + " " + koLiveTime);
                KO();
                return false;
            }

            previousMaxHealth = (int)(previousMaxHealth * (2f / 3f));
            print("test: " + previousMaxHealth);
            if (previousMaxHealth > 20)
			{
                SetUp(previousMaxHealth);
                return true;
            }

            KO();

            return false;
		}

        protected virtual void KO()
		{
            //let gameManager know
		}
    }
}



