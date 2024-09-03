using Common;
using FrameWork;
using WorldServer.Managers;

namespace WorldServer.Services.World
{
    [Service]
    public class MailService : ServiceBase
    {
        public static bool MailItem(uint characterId, uint itemId, ushort count)
        {
            var character = CharMgr.GetCharacter(characterId, true);
            if (character == null) return false;
            var characterName = character?.Name;

            characters_mails mail = new characters_mails
            {
                Guid = CharMgr.GenerateMailGuid(),
                CharacterId = characterId, //CharacterId
                SenderName = "Ikthaleon",
                ReceiverName = characterName,
                SendDate = (uint)TCPManager.GetTimeStamp(),
                Title = "",
                Content = "",
                Money = 0,
                Opened = false,
                CharacterIdSender = 283
            };

            mail_item item = new mail_item(itemId, count);
            if (item != null)
            {
                mail.Items.Add(item);
                CharMgr.AddMail(mail);
            }

            return true;
        }
    }
}