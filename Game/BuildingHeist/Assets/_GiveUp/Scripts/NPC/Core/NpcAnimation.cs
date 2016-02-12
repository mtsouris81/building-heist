using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace GiveUp.Core
{
    public class NpcAnimation
    {



        Animator animator;
        MonoBehaviour owner;

        public NpcAnimation(MonoBehaviour owner)
        {
            this.owner = owner;
        }



        string CurrentAnimationName;

        public void PlayAnimation(string name)
        {
            PlayAnimation(name, false);
        }

        public void PlayAnimation(string name, bool forcePlay)
        {
            if (CurrentAnimationName == name && !forcePlay)
                return;

            CurrentAnimationName = name;

            if (animator != null)
            {
                animator.Play(name);
            }
        }

        public void FreezeAnimation()
        {
            if (animator != null)
            {
                animator.speed = 0;
            }
        }
        public void UnFreezeAnimation()
        {
            if (animator != null)
            {
                animator.speed = 1;
            }
        }

        public void Initialize()
        {
            animator = owner.GetComponentInChildren<Animator>();
        }

        internal void PlayAnimation(string name, float normalizedTime)
        {
            CurrentAnimationName = name;

            if (animator != null)
            {
                animator.Play(name, -1, normalizedTime);
            }
        }
    }
}