using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

public struct Weather
{
    public string Country { get; set; }
    public string Name { get; set; }
    public double Temp { get; set; }
    public string Description { get; set; }

    public Weather(string country, string name, double temp, string description)
    {
        Country = country;
        Name = name;
        Temp = temp;
        Description = description;
    }

    public override string ToString()
    {
        return $"Страна: {Country}, Город: {Name}, Температура: {Temp}, Описание: {Description}\n";
    }
}

class Program
{
    static void Main(string[] args)
    {
        string apiKey = "";
        string URL = $"https://api.openweathermap.org/data/2.5/weather";

        Weather[] weathers = new Weather[10];
        Random random = new Random(); //Генерируются случайные координаты
        int i = 0;
        while (i < weathers.Length)
        {
            {
                double minLat = -90;
                double maxLat = 90;
                double minLon = -180;
                double maxLon = 180;
                double latitude = random.NextDouble() * (maxLat - minLat) + minLat;
                double longtitude = random.NextDouble() * (maxLon - minLon) + minLon;

                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(URL); //Установка базового адреса для клиента
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json")); //Ожидаем получить ответ в формате JSON

                string urlParameters = $"?lat={latitude}&lon={longtitude}&appid={apiKey}"; //Параметры запроса
                HttpResponseMessage response = client.GetAsync(urlParameters).Result; //Запрос

                if (response.IsSuccessStatusCode) //Если запрос успешный
                {
                    var data = response.Content.ReadAsStringAsync().Result; //Чтение ответа
                    var json = JsonObject.Parse(data);
                    Weather res = new Weather();
                    res.Country = (string)json["sys"]["country"];
                    res.Name = (string)json["name"];
                    res.Temp = (double)json["main"]["temp"];
                    res.Description = (string)json["weather"][0]["main"];
                    if (res.Country == "" || res.Name == "") continue;
                    else
                    {
                        weathers[i] = res;
                        i += 1;
                    }

                    Console.WriteLine(i);
                }
                else
                {
                    Console.WriteLine("Не удалось получить данные.");
                }
                client.Dispose();
            }
            Console.WriteLine("Успешно получено.");
        }

        Console.WriteLine("Минимальная температура: ");
        Weather minTemp = (from w in weathers orderby w.Temp select w).First(); //Перебирает все элементы массива, сортирует элементы по свойству (Temp) и возвращает первый элемент
        Console.WriteLine(minTemp);

        Console.WriteLine("Максимальная температура: ");
        Weather maxTemp = weathers.OrderByDescending(w => w.Temp).First(); //Отсортированная температура в порядке убывания
        Console.WriteLine(maxTemp);

        Console.WriteLine("Средняя температура: ");
        double average = weathers.Average(w => w.Temp);
        Console.WriteLine(average);

        Console.WriteLine("Количество стран: ");
        int quantity = weathers.GroupBy(w => w.Country).Count();
        Console.WriteLine(quantity);

        var last = weathers.Where(w => w.Description == "Clear sky" || w.Description == "Rain" || w.Description == "Few clouds");

        if (last.Any())
        {
            Weather weather = last.First();
            Console.WriteLine("Точка с ясным небом, дождем или облаками: ");
            Console.WriteLine(weather);
        }
        else
        {
            Console.WriteLine("Нет таких мест.");
        }

        Console.WriteLine("Список показаний: ");
        for (int j = 0; j < weathers.Length; j++)
        {
            Console.WriteLine($"{j + 1}: {weathers[j]}");
            Console.WriteLine();
        }
    }
}