using Altom.AltDriver.Commands;

namespace AltDriver.ThetanRivals.Commands
{
    /// <summary>
    /// Wait for the Home menu to appear, ready to start a new game.
    /// </summary>
    public class RivalsWaiForUIElementTarget : AltBaseCommand
    {
        public RivalsWaiForUIElementTarget(IDriverCommunication commHandler) : base(commHandler)
        {
        }
    }
}