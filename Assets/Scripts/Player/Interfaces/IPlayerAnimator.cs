namespace Player.Interfaces
{
    public interface IPlayerAnimator
    {
        void StartAnimation();
        void UpdateAnimation(float playerMoveSpeed, int x, int y);
        void EndAnimation();
    }
}