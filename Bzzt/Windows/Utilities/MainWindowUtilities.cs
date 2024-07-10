using CatboyEngineering.Bzzt.Windows.States;
using System.Threading.Tasks;

namespace CatboyEngineering.Bzzt.Windows.Utilities
{
    public class MainWindowUtilities
    {
        public static async Task HandleWithIndicator(MainWindowState state, Task task, int delay = 0)
        {
            state.isRequestInFlight = true;

            await task.ContinueWith(t =>
            {
                Task.Delay(delay).ContinueWith(t =>
                {
                    state.isRequestInFlight = false;
                });
            });
        }
    }
}
