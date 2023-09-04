using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RyanTest
{
    [RequireComponent(typeof(Animator))]
    public class Boxer : MonoBehaviour
    {
        [SerializeField] protected int maxHealth;
        [SerializeField] protected int maxStamina;
        [SerializeField] protected int damage;

        public int health { get; protected set; }
        public int stamina { get; protected set; }
        public bool knockedDown { get; protected set; }

        public Animator AN { get; private set; }

        public virtual void SetUp()
        {
            health = maxHealth;
            stamina = maxStamina;
            knockedDown = false;
            AN = GetComponent<Animator>();
        }

        protected virtual bool OnDamage(int damage) //actually damage take
		{
            if (knockedDown) return false;

            health -= damage;
            //print("damage take");

            if (health < 0)
			{
                OnDeath();
                return false;
			} 
            else return true;
			
		}

        protected virtual void OnDeath()
		{
            if (knockedDown) return;

            knockedDown = false;
		}
    }
}



