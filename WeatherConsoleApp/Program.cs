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
var jsonResult = await response.Content.ReadAsStringAsync();

//обработка ошибки
if (!response.IsSuccessStatusCode)
{
    var errorModel = JsonConvert.DeserializeObject<ErrorModel>(jsonResult);
    Console.WriteLine($"Ошибка: {errorModel.Message}");
    return;
}

//Обработка и вывод текущей погоды
var model = JsonConvert.DeserializeObject<WeatherModel>(jsonResult);
Console.WriteLine();
Console.WriteLine($"Город: {model.Name}");
Console.WriteLine($"Температура: {Math.Round(model.Main.Temp):+#;-#;0}°");
Console.WriteLine($"На улице {model.Weather[0].Description}");
Console.WriteLine($"Ощущается как: {Math.Round(model.Main.FeelsLike):+#;-#;0}°");
Console.WriteLine($"Ветер: {Math.Round(model.Wind.Speed, 1)} м/с, {GetDirection(model.Wind.Deg)}");
Console.WriteLine($"Влажность: {model.Main.Humidity}%");
Console.WriteLine($"Давление: {model.Main.Pressure} мм рт.ст.");
Console.WriteLine();

//Обработка и вывод погоды на 4 дня вперёд
Console.WriteLine("Прогноз погоды на следующие 4 дня\n");

uri = new Uri($"https://api.openweathermap.org/data/2.5/forecast?q={HttpUtility.UrlEncode(city)}&appid={apiKey}&units={units}&lang={language}");
response = await http.GetAsync(uri);
jsonResult = await response.Content.ReadAsStringAsync();

//обработка ошибки
if (!response.IsSuccessStatusCode)
{
    var errorModel = JsonConvert.DeserializeObject<ErrorModel>(jsonResult);
    Console.WriteLine($"Ошибка: {errorModel.Message}");
    return;
}

var modelDays = JsonConvert.DeserializeObject<WeatherModelDays>(jsonResult);

var firstPositionCursor = Console.GetCursorPosition();
firstPositionCursor.Left = 18;
Console.WriteLine("Дата:");
var secondPositionCursor = Console.GetCursorPosition();
Console.WriteLine("День недели:");
var thirdPositionCursor = Console.GetCursorPosition();
Console.WriteLine("Min, max °:");
var fourthPositionCursor = Console.GetCursorPosition();
Console.WriteLine("На улице будет");

var p = 0;
for (; p < modelDays.List.Length; p++)
{
    DateTime.TryParse(modelDays.List[p].DtTxt, out var date);
    date = date.AddSeconds(modelDays.City.TimeZone);
    if (date.ToShortDateString() != DateTime.Today.ToShortDateString())
        break;
}

for (var i = p; i < modelDays.List.Length; i += 8)
{
    var leftCursorPosition = firstPositionCursor.Left + (i - 1) / 8 * 25;
    DateTime.TryParse(modelDays.List[i].DtTxt, out var date);
    Console.SetCursorPosition(leftCursorPosition, firstPositionCursor.Top);
    Console.WriteLine($"{date.ToShortDateString()}");
    Console.SetCursorPosition(leftCursorPosition, secondPositionCursor.Top);
    Console.WriteLine($"{CultureInfo.GetCultureInfo("ru-RU").DateTimeFormat.GetDayName(date.DayOfWeek)}");


    var minTemp = modelDays.List[i].Main.TempMin;
    var maxTemp = modelDays.List[i].Main.TempMax;

    for (var j = i; j < i + 8 && j < modelDays.List.Length; j++)
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

    Console.SetCursorPosition(leftCursorPosition, thirdPositionCursor.Top);
    Console.WriteLine($" {Math.Round(minTemp):+#;-#;0}°, {Math.Round(maxTemp):+#;-#;0}°");
    Console.SetCursorPosition(leftCursorPosition, fourthPositionCursor.Top);
    Console.WriteLine($"{modelDays.List[i].Weather[0].Description}");
}

Console.ReadKey();

string GetDirection(int deg) =>
    deg switch
    {
        >= 0 and < 23 or >= 338 and <= 360 => "C",
        >= 23 and < 68 => "СВ",
        >= 68 and < 113 => "В",
        >= 113 and < 158 => "ЮВ",
        >= 158 and < 203 => "Ю",
        >= 203 and < 248 => "ЮЗ",
        >= 248 and < 292 => "З",
        >= 292 and < 338 => "CЗ",
        _ => "",
    };
