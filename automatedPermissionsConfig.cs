using Rocket.API;

namespace automatedPermissions
{
    public class automatedPermissionsConfig : IRocketPluginConfiguration
    {

        public int intervalUntilPromotion = 900;

        public void LoadDefaults() { }
    }
}