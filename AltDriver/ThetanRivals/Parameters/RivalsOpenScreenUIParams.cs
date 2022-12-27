using Altom.AltDriver.Commands;

namespace AltDriver.ThetanRivals.Parameters
{
    [Command("rivals_open_screen_ui_by_name")]
    public class RivalsOpenScreenUIParams : CommandParams
    {
        public string screenName;
        public float timeout;
        public float interval;

        public RivalsOpenScreenUIParams(string screenName, float timeout = 10, float interval = 0.5f)
        {
            this.screenName = screenName;
            this.timeout = timeout;
            this.interval = interval;
        }
    }
}