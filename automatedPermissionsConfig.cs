using Rocket.API;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace automatedPermissions
{
    public class Message__
    {
        [XmlElement(ElementName = "Message")]
        public string Color;
        public string Text;
        public Message__(string text, string color)
        {
            Color = color;
            Text = text;
        }
    }

    public class automatedPermissionsConfig : IRocketPluginConfiguration
    {

        public int intervalUntilPromotionSeconds = 900;
        public string targetGroupId = "member";

        //[XmlArray]
       // public List<Message__> Messages = new List<Message__>()
        //{
       //     new Message__("Congratulations! You were automatically added to group Member!", "blue"),
       //     new Message__("Type /p to see your current Permissions :)", "blue")
      //  };

        public void LoadDefaults() {
        }
    }
}