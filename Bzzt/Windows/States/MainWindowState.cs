namespace CatboyEngineering.Bzzt.Windows.States
{
    public class MainWindowState
    {
        public Plugin Plugin { get; set; }
        public bool HasError { get; set; }
        public string ErrorText { get; set; }

        public string stringBuffer;
        public bool isRequestInFlight;

        public MainWindowState(Plugin plugin)
        {
            Plugin = plugin;

            SetDefauts();
        }

        public void SetDefauts()
        {
            isRequestInFlight = false;

            ClearErrors();
            ResetBuffers();
        }

        public void ResetBuffers()
        {
            stringBuffer = "";
        }

        public void OnError(string error)
        {
            HasError = true;
            ErrorText = error;
        }

        public void ClearErrors()
        {
            HasError = false;
            ErrorText = string.Empty;
        }
    }
}