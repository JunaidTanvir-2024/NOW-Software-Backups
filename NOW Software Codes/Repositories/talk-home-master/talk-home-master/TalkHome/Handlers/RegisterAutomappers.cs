using TalkHome.Automappers;

namespace TalkHome.Handlers
{
    /// <summary>
    /// Initialises Automappers.
    /// </summary>
    public static class RegisterAutomappers
    {
        /// <summary>
        /// Registers map class.
        /// </summary>
        public static void Register()
        {
            ConfigureAutomappers.Maps();
        }
    }
}
