namespace PlayerAnimator.Interfaces
{
    public interface IPlayerAnimator
    {
        void StartAnimation();
        void UpdateAnimation(int x, int y);
        void EndAnimation();
    }
}