using Newtonsoft.Json;
using System.Globalization;
using System.Text;
using System.Web;
using WeatherConsoleApp;

Console.SetWindowSize(Console.WindowWidth + 30, Console.WindowHeight);
Console.InputEncoding = Encoding.Unicode;
Console.WriteLine("Добро пожаловать в прогноз погоды!");
Console.Write("Введите интересующий город: ");
var city = Console.ReadLine();

var apiKey = "СЮДА НУЖНО ПОДСТАВИТЬ СВОЙ КЛЮЧ";
var units = "metric";
var language = "ru";

var uri = new Uri($"https://api.openweathermap.org/data/2.5/weather?q={HttpUtility.UrlEncode(city)}&appid={apiKey}&units={units}&lang={language}");
var http = new HttpClient();
var response = await http.GetAsync(uri);

//обработка ошибки
if (!response.IsSuccessStatusCode)
{
    var jsonResponse = await response.Content.ReadAsStringAsync();
    var errorModel = JsonConvert.DeserializeObject<ErrorModel>(jsonResponse);
    Console.WriteLine($"Ошибка: {errorModel.Message}");
    return;
}

//Обработка и вывод текущей погоды
var jsonResult = await response.Content.ReadAsStringAsync();
var model = JsonConvert.DeserializeObject<WeatherModel>(jsonResult);
Console.WriteLine();
Console.WriteLine($"Город: {model.Name}");
Console.WriteLine($"Температура: {((Math.Round(model.Main.Temp) < 0) ? Math.Round(model.Main.Temp) : $"+{Math.Round(model.Main.Temp)}")}°");
Console.WriteLine($"На улице {model.Weather[0].Description}");
Console.WriteLine($"Ощущается как: {Math.Round(model.Main.FeelsLike)}°");
Console.WriteLine($"Ветер: {Math.Round(model.Wind.Speed, 1)} м/с, {GetDirection(model.Wind.Deg)}");
Console.WriteLine($"Влажность: {model.Main.Humidity}%");
Console.WriteLine($"Давление: {model.Main.Pressure} мм рт.ст.");
Console.WriteLine();

//Обработка и вывод погоды на 4 дня вперёд
uri = new Uri($"https://api.openweathermap.org/data/2.5/forecast?q={HttpUtility.UrlEncode(city)}&appid={apiKey}&units={units}&lang={language}");
response = await http.GetAsync(uri);
jsonResult = await response.Content.ReadAsStringAsync();
var modelDays = JsonConvert.DeserializeObject<WeatherModelDays>(jsonResult);

Console.WriteLine("Прогноз погоды на следующие 4 дня\n");
var firstPositionCursor = Console.GetCursorPosition();
firstPositionCursor.Left = 18;
Console.WriteLine($"Дата:");
var secondtPositionCursor = Console.GetCursorPosition();
Console.WriteLine($"День недели:");
var thirtPositionCursor = Console.GetCursorPosition();
Console.WriteLine($"Min, max °:");
var fourthPositionCursor = Console.GetCursorPosition();
Console.WriteLine($"На улице будет");

var p = 0;
for (; p < modelDays.List.Length; p++)
{
    DateTime.TryParse(modelDays.List[p].DtTxt, out var date);
    if (date.ToShortDateString() != DateTime.Today.ToShortDateString())
        break;
}

for (int i = p; i < modelDays.List.Length; i += 8)
{
    var leftCursorPosition = firstPositionCursor.Left + (i - 1) / 8 * 25;
    DateTime.TryParse(modelDays.List[i].DtTxt, out var date);
    Console.SetCursorPosition(leftCursorPosition, firstPositionCursor.Top);
    Console.WriteLine($"{date.ToShortDateString()}");
    Console.SetCursorPosition(leftCursorPosition, secondtPositionCursor.Top);
    Console.WriteLine($"{CultureInfo.GetCultureInfo("ru-RU").DateTimeFormat.GetDayName(date.DayOfWeek)}");


    var minTemp = modelDays.List[i].Main.TempMin;
    var maxTemp = modelDays.List[i].Main.TempMax;

    for (int j = i; j < i + 8 && j < modelDays.List.Length; j++)
    {
        if (modelDays.List[j].Main.TempMin < minTemp)
        {
            minTemp = modelDays.List[j].Main.TempMin;
        }
        if (modelDays.List[j].Main.TempMax > maxTemp)
        {
            maxTemp = modelDays.List[j].Main.TempMax;
        }
    }

    Console.SetCursorPosition(leftCursorPosition, thirtPositionCursor.Top);
    Console.WriteLine($"{Math.Round(minTemp)}°, {Math.Round(maxTemp)}°");
    Console.SetCursorPosition(leftCursorPosition, fourthPositionCursor.Top);
    Console.WriteLine($"{modelDays.List[i].Weather[0].Description}");
}

Console.ReadKey();

string GetDirection(int deg)
{
    if (deg >= 0 && deg < 22.5 || deg >= 337.5 && deg <= 360)
        return "C";
    if (deg >= 22.5 && deg < 67.5)
        return "СВ";
    if (deg >= 67.5 && deg < 112.5)
        return "В";
    if (deg >= 112.5 && deg < 157.5)
        return "ЮВ";
    if (deg >= 157.5 && deg < 202.5)
        return "Ю";
    if (deg >= 202.5 && deg < 247.5)
        return "ЮЗ";
    if (deg >= 247.5 && deg < 292.5)
        return "З";
    if (deg >= 292.5 && deg < 337.5)
        return "СЗ";

    return string.Empty;
}