using System.Linq;
using System.Windows.Forms;

namespace FactorioSupervisor.Helpers
{
    public static class ScreenHelpers
    {
        public static double GetWindowPosLeft(Screen primaryScreen, double windowWidth)
        {
            return (primaryScreen.WorkingArea.Width / 2.0) - (windowWidth / 2);
        }

        public static double GetWindowPosTop(Screen primaryScreen, double windowHeight)
        {
            return (primaryScreen.WorkingArea.Height / 2.0) - (windowHeight / 2);
        }

        public static int GetScreensWorkingAreaWidth()
        {
            return Screen.AllScreens.Sum(screen => screen.WorkingArea.Width);
        }

        public static int GetScreensWorkingAreaHeight()
        {
            return Screen.AllScreens.Sum(screen => screen.WorkingArea.Height);
        }
    }
}
