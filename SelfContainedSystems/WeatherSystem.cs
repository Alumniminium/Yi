using System;
using System.Linq;
using System.Net;
using Newtonsoft.Json;
using YiX.Database.JsonObjects;
using YiX.Entities;
using YiX.Enums;
using YiX.Network.Packets.Conquer;
using YiX.Scheduler;
using YiX.World;

namespace YiX.SelfContainedSystems
{
    public static class WeatherSystem
    {
        private const string QueryString = "http://api.openweathermap.org/data/2.5/weather?q=Vienna,aut&units=metric&appid=6b3c25a5664c00e2ebd7222fd559c7df";
        private static readonly WebClient WebClient = new WebClient();
        private static WeatherJson _currentWeather = new WeatherJson();
        public static WeatherType CurrentWeatherType = WeatherType.None;
        public static void Start()
        {
            YiScheduler.Instance.Do(TimeSpan.FromMinutes(60), Start);
            try
            {
                var json = WebClient.DownloadString(QueryString);
                _currentWeather = JsonConvert.DeserializeObject<WeatherJson>(json);

                switch (_currentWeather.weather.First().main)
                {
                    case "Rain":
                        CurrentWeatherType = WeatherType.Rain;
                        SetRaining(150, (int)_currentWeather.wind.deg);
                        break;
                    case "Snow":
                        CurrentWeatherType = WeatherType.Snow;
                        SetSnowing(150, (int)_currentWeather.wind.deg);
                        break;
                    default:
                        {
                            SetClear(150, (int)_currentWeather.wind.deg);
                            Output.WriteLine("Default Weather: " + _currentWeather.weather.First().main);
                            break;
                        }
                }
            }
            catch (Exception e)
            {
                Output.WriteLine(e);
            }
        }
        public static void SetWeatherFor(Player player)
        {
            if (_currentWeather?.weather?.First() == null)
                return;

            switch (_currentWeather.weather.First().main)
            {
                case "Rain":
                    player.Send(MsgWeather.Create(WeatherType.Rain, 150, (int)_currentWeather.wind.deg, 0));
                    break;
                case "Snow":
                    player.Send(MsgWeather.Create(WeatherType.Snow, 150, (int)_currentWeather.wind.deg, 0));
                    break;
                default:
                    {
                        player.Send(MsgWeather.Create(WeatherType.Atoms, 150, (int)_currentWeather.wind.deg, 0));
                        break;
                    }
            }
            player.Send(MsgText.Create("WeatherSystem", player.Name, $"Weather set to: {_currentWeather.weather.First().main} - Intensity: {150} - Angel: {_currentWeather.wind.deg}", MsgTextType.Talk));
        }

        private static void SetSnowing(int intensity, int angel)
        {
            foreach (var player in GameWorld.Maps.Values.SelectMany(map => map.Entities.Values.OfType<Player>()))
            {
                player.Send(MsgWeather.Create(WeatherType.Snow, intensity, angel, 0));
            }
        }

        private static void SetClear(int intensity, int angel)
        {
            foreach (var player in GameWorld.Maps.Values.SelectMany(map => map.Entities.Values.OfType<Player>()))
            {
                player.Send(MsgWeather.Create(WeatherType.Atoms, intensity, angel, 0));
            }
        }

        private static void SetRaining(int intensity, int angel)
        {
            foreach (var player in GameWorld.Maps.Values.SelectMany(map => map.Entities.Values.OfType<Player>()))
            {
                player.Send(MsgWeather.Create(WeatherType.Rain, intensity, angel, 0));
            }
        }
    }
}
