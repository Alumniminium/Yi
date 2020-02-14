using System.Linq;
using Yi.Calculations;
using Yi.Entities;
using Yi.Enums;
using Yi.World;

namespace Yi.Helpers
{
    public static class Message
    {
        public static bool SendTo(string from, string to, string message, MsgTextType type)
        {
            if (GameWorld.Find(to, out YiObj target))
                target.GetMessage(from, to, message, type);

            return target != null;
        }

        public static bool SendTo(string to, string message, MsgTextType type)
        {
            if (GameWorld.Find(to, out YiObj target))
                target.GetMessage(Constants.System, to, message, type);

            return target != null;
        }

        public static void SendTo(YiObj player, string message, MsgTextType action)
        {
            if(string.IsNullOrEmpty(message))
                return;
            player.GetMessage(Constants.System, player.Name, message, action);
        }

        public static void Broadcast(string from, string message, MsgTextType channel)
        {
            foreach (var value in GameWorld.Maps.Values)
            {
                foreach (var entity in value.Entities.Values.OfType<Player>())
                {
                    entity.GetMessage(from,entity.Name,message,channel);
                }
            }
        }
    }
}