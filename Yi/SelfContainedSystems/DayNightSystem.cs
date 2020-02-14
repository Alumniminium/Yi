using System;
using System.Linq;
using Yi.Entities;
using Yi.Enums;
using Yi.Network.Packets.Conquer;
using Yi.Scheduler;
using Yi.World;

namespace Yi.SelfContainedSystems
{
    public static class DayNightSystem
    {
        public static int LastColor;
        public static TimeSpan TimeNow = TimeSpan.Zero;
        public static void Start()
        {
            YiScheduler.Instance.Do(TimeSpan.FromSeconds(2), Start);

            TimeNow = DateTime.UtcNow.Subtract(new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day));
            
            var currentColor = (int)TimeNow.TotalSeconds/ 192;
            if (currentColor < 30)
                currentColor = 30;
            if (currentColor > 255)
                currentColor = 255 - -(255 - currentColor);

            if (currentColor == LastColor)
                return;

            LastColor = currentColor;
            foreach (var player in GameWorld.Maps.Values.SelectMany(map => map.Entities.Values.OfType<Player>()))
                SetTimeFor(player);
        }

        public static void SetTimeFor(Player player)
        {
            if (TimeNow.Hours != DateTime.UtcNow.Hour)
                return;

            var color = System.Drawing.Color.FromArgb(255, LastColor, LastColor, LastColor);
            player.Send(MsgColor.Create(player, color));
            player.Send(MsgText.Create("Server", player.Name, $"Color: {LastColor}| SenderUniqueId: {DateTime.UtcNow.ToShortTimeString()} | TimeNow.Seconds / 192 = ({TimeNow.TotalSeconds / 192})", MsgTextType.Talk));
        }

        public static bool IsDay()
        {
            //Later than 6am and earlier than 6pm
            return TimeNow.Hours > 6 && TimeNow.Hours < 18;
        }
        public static bool IsNight()
        {
            //earlier than 6am or later than 6pm
            return TimeNow.Hours < 6 || TimeNow.Hours > 18;
        }
    }
}