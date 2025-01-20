using Microsoft.AspNetCore.Mvc;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Projekt.Models;

public class WeatherController : Controller
{
    private static readonly HttpClient client = new HttpClient();

    public async Task<IActionResult> WeatherPartial()
    {
        var weatherData = await GetWeatherData();
        if (weatherData == null)
        {
            ViewBag.Error = "Error";
        }
        else
        {
            ViewBag.RoundedTemperature = Math.Round(weatherData.CurrentTemperature);
            ViewBag.City = "Kraków";
        }
        return PartialView("WeatherPartial", weatherData);
    }

    private async Task<WeatherModel> GetWeatherData()
    {
        try
        {
            var apiUrl = "https://api.openweathermap.org/data/2.5/weather?q=Krakow&appid=2b4d742cf9d3a4f1333986d050282f06&units=metric";
            var response = await client.GetStringAsync(apiUrl);
            var data = JsonConvert.DeserializeObject<OpenWeatherResponse>(response);
            return new WeatherModel
            {
                CurrentTemperature = data.Main?.Temp ?? 0,
                WeatherDescription = data.Weather?[0]?.Description ?? "No data",
                CurrentTime = DateTime.Now.ToString("F")
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Błąd: {ex.Message}");
            return null;
        }
    }
}

public class OpenWeatherResponse
{
    public MainInfo Main { get; set; }
    public WeatherInfo[] Weather { get; set; }
}

public class MainInfo
{
    public double Temp { get; set; }
}

public class WeatherInfo
{
    public string Description { get; set; }
}
