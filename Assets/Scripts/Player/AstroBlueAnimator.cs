using Player.Interfaces;
using UnityEngine;

namespace Player
{
    public class AstroBlueAnimator : IPlayerAnimator
    {
        private const float PLAYER_RUN_THRESHOLD = 4f;
        
        // caching animator state hashes for better memory allocation
        
        // idle animation for the character has no animation, it's not looping seamlessly so it got removed
        private readonly int _idleHash = Animator.StringToHash("Idle");
        private readonly int _walkSideHash = Animator.StringToHash("Walk_Side");
        private readonly int _walkDownHash = Animator.StringToHash("Walk_Down");
        private readonly int _walkUpHash = Animator.StringToHash("Walk_Up");
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

        public void UpdateAnimation(float playerMoveSpeed, int x, int y)
        {
            _spriteRenderer.flipX = x < 0;

            if (x != 0)
            {
                _animator.Play(playerMoveSpeed >= PLAYER_RUN_THRESHOLD ? _runSideHash : _walkSideHash);
            }
            else
            {
                switch (y)
                {
                    case < 0:
                        _animator.Play(playerMoveSpeed >= PLAYER_RUN_THRESHOLD ? _runDownHash : _walkDownHash);
                        break;
                    case > 0:
                        _animator.Play(playerMoveSpeed >= PLAYER_RUN_THRESHOLD ? _runUpHash : _walkUpHash);
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