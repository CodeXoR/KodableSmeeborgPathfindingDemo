using PlayerAnimator.Interfaces;
using UnityEngine;

namespace PlayerAnimator
{
    public class AstroBlueAnimator : IPlayerAnimator
    {
        private readonly int _idleHash = Animator.StringToHash("Idle");
        private readonly int _runSideHash = Animator.StringToHash("Run_Side");
        private readonly int _runDownHash = Animator.StringToHash("Run_Down");
        private readonly int _runUpHash = Animator.StringToHash("Run_Up");
        private readonly int _vanishHash = Animator.StringToHash("Vanish");
        
        private readonly Animator _animator;
        private readonly SpriteRenderer _spriteRenderer;
        public AstroBlueAnimator(Animator animator, SpriteRenderer spriteRenderer)
        {
            _animator = animator;
            _spriteRenderer = spriteRenderer;
        }

        public void StartAnimation()
        {
            _animator.Play(_idleHash);
        }

        public void UpdateAnimation(int x, int y)
        {
            _spriteRenderer.flipX = x < 0;

            if (x != 0)
            {
                _animator.Play(_runSideHash);
            }
            else
            {
                switch (y)
                {
                    case < 0:
                        _animator.Play(_runDownHash);
                        break;
                    case > 0:
                        _animator.Play(_runUpHash);
                        break;
                }
            }
        }

        public void EndAnimation()
        {
            _animator.Play(_vanishHash);
        }
    }
}