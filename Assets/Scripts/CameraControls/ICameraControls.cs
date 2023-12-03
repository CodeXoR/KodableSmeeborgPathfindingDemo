using System.Threading.Tasks;

namespace CameraControls
{
    public interface ICameraControls
    {
        /// <summary>
        /// Positions and scales camera viewport centered to world bounds
        /// Note: this is set up as a Task for flexible game transition sequencing using async
        /// </summary>
        /// <param name="viewScaleModifier">
        /// optional view scale modifier for applying custom padding in view scale calculation
        /// </param>
        /// <returns></returns>
        Task CenterToWorldBounds(float viewScaleModifier = 1f);
    }
}