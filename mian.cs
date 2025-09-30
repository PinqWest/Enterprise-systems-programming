
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.Json;

class Program
{
    static string eventsFile = "events.json";
    static Dictionary<DateTime, List<string>> events = new Dictionary<DateTime, List<string>>();

    static void Main()
    {
        LoadEvents();

        while (true)
        {
            Console.Clear();
            ShowMonth(DateTime.Now.Year, DateTime.Now.Month);

            Console.WriteLine("\nКоманды:");
            Console.WriteLine(" add [дд] [мм] [гггг] [заметка] – добавить заметку");
            Console.WriteLine(" show [дд] [мм] [гггг] – показать заметки дня");
            Console.WriteLine(" show year – показать все месяцы года (3x4)");
            Console.WriteLine(" left month – дни до конца месяца");
            Console.WriteLine(" left year – дни до конца года");
            Console.WriteLine(" exit – выйти");

            Console.Write("\nВведите команду: ");
            string input = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(input)) continue;

            string[] parts = input.Split(' ', 5);
            string command = parts[0].ToLower();

            if (command == "exit") break;

            switch (command)
            {
                case "add":
                    if (parts.Length < 5)
                    {
                        Console.WriteLine("Использование: add [дд] [мм] [гггг] [текст заметки]");
                        Console.ReadKey();
                        break;
                    }

                    if (int.TryParse(parts[1], out int day) &&
                        int.TryParse(parts[2], out int month) &&
                        int.TryParse(parts[3], out int year))
                    {
                        string note = parts[4];
                        for (int i = 5; i < parts.Length; i++) note += " " + parts[i];

                        DateTime date;
                        try
                        {
                            date = new DateTime(year, month, day);
                        }
                        catch
                        {
                            Console.WriteLine("Неверная дата.");
                            Console.ReadKey();
                            break;
                        }

                        if (!events.ContainsKey(date)) events[date] = new List<string>();
                        events[date].Add(note);
                        SaveEvents();
                    }
                    else
                    {
                        Console.WriteLine("Неверный формат даты.");
                        Console.ReadKey();
                    }
                    break;

                case "show":
                    if (parts.Length == 2 && parts[1] == "year")
                    {
                        Console.Clear();
                        ShowYear(DateTime.Now.Year);
                        Console.ReadKey();
                    }
                    else if (parts.Length >= 4 &&
                             int.TryParse(parts[1], out int d) &&
                             int.TryParse(parts[2], out int m) &&
                             int.TryParse(parts[3], out int y))
                    {
                        DateTime dateShow;
                        try
                        {
                            dateShow = new DateTime(y, m, d);
                        }
                        catch
                        {
                            Console.WriteLine("Неверная дата.");
                            Console.ReadKey();
                            break;
                        }

                        if (events.ContainsKey(dateShow))
                        {
                            Console.WriteLine($"\nЗаметки на {dateShow:dd.MM.yyyy}:");
                            foreach (var note in events[dateShow])
                                Console.WriteLine($" - {note}");
                        }
                        else Console.WriteLine("Нет заметок.");
                        Console.ReadKey();
                    }
                    break;

                case "left":
                    if (parts.Length < 2) break;
                    if (parts[1] == "month")
                    {
                        int daysLeft = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month) - DateTime.Now.Day;
                        Console.WriteLine($"До конца месяца осталось {daysLeft} дней.");
                        Console.ReadKey();
                    }
                    else if (parts[1] == "year")
                    {
                        DateTime endYear = new DateTime(DateTime.Now.Year, 12, 31);
                        int daysLeft = (endYear - DateTime.Now).Days;
                        Console.WriteLine($"До конца года осталось {daysLeft} дней.");
                        Console.ReadKey();
                    }
                    break;
            }
        }
    }

    // === КАЛЕНДАРЬ МЕСЯЦА ===
    static string[] RenderMonth(int year, int month)
    {
        List<string> lines = new List<string>();

        // Заголовок
        string title = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month) + " " + year;
        lines.Add(title.PadLeft((22 + title.Length) / 2).PadRight(22));

        // Дни недели
        string[] days = { "Пн", "Вт", "Ср", "Чт", "Пт", "Сб", "Вс" };
        lines.Add(string.Join(" ", days));

        // Первая неделя — отступ
        DateTime firstDay = new DateTime(year, month, 1);
        int offset = ((int)firstDay.DayOfWeek + 6) % 7;
        string week = new string(' ', offset * 3);

        int daysInMonth = DateTime.DaysInMonth(year, month);
        for (int day = 1; day <= daysInMonth; day++)
        {
            DateTime current = new DateTime(year, month, day);
            string mark = events.ContainsKey(current) ? "*" : " ";

            week += day.ToString().PadLeft(2) + mark;

            if (((offset + day) % 7) == 0 || day == daysInMonth)
            {
                lines.Add(week.PadRight(22));
                week = "";
            }
        }

        return lines.ToArray();
    }

    static void ShowMonth(int year, int month)
    {
        var lines = RenderMonth(year, month);
        DateTime today = DateTime.Today;

        // Заголовок
        Console.WriteLine(lines[0]);
        // Дни недели
        Console.WriteLine(lines[1]);

        // Недели
        for (int i = 2; i < lines.Length; i++)
        {
            string line = lines[i];
            int pos = 0;

            while (pos < line.Length)
            {
                if (char.IsDigit(line[pos]))
                {
                    string dayStr = line.Substring(pos, 2);
                    int.TryParse(dayStr, out int day);
                    DateTime current = new DateTime(year, month, day);

                    if (current == today)
                        Console.ForegroundColor = ConsoleColor.Green;
                    else if (current.DayOfWeek == DayOfWeek.Saturday || current.DayOfWeek == DayOfWeek.Sunday)
                        Console.ForegroundColor = ConsoleColor.Red;
                    else
                        Console.ForegroundColor = ConsoleColor.Gray;

                    Console.Write(dayStr);
                    pos += 2;

                    if (pos < line.Length && line[pos] == '*')
                    {
                        Console.Write('*');
                        pos++;
                    }
                    else
                        Console.Write(' ');

                    Console.ResetColor();
                }
                else
                {
                    Console.Write(line[pos]);
                    pos++;
                }
            }
            Console.WriteLine();
        }
    }

    // === КАЛЕНДАРЬ ГОДА С ЦВЕТАМИ И ЗАМЕТКАМИ ===
    static void ShowYear(int year)
    {
        DateTime today = DateTime.Today;

        for (int row = 0; row < 4; row++)
        {
            string[][] months = new string[3][];
            for (int col = 0; col < 3; col++)
            {
                int month = row * 3 + col + 1;
                months[col] = RenderMonth(year, month);
            }

            int maxLines = Math.Max(months[0].Length, Math.Max(months[1].Length, months[2].Length));

            for (int i = 0; i < maxLines; i++)
            {
                for (int col = 0; col < 3; col++)
                {
                    if (i < months[col].Length)
                    {
                        string line = months[col][i];
                        int pos = 0;

                        while (pos < line.Length)
                        {
                            if (char.IsDigit(line[pos]))
                            {
                                string dayStr = line.Substring(pos, 2);
                                int.TryParse(dayStr, out int day);
                                int monthNum = row * 3 + col + 1;
                                DateTime current = new DateTime(year, monthNum, day);

                                if (current == today)
                                    Console.ForegroundColor = ConsoleColor.Green;
                                else if (current.DayOfWeek == DayOfWeek.Saturday || current.DayOfWeek == DayOfWeek.Sunday)
                                    Console.ForegroundColor = ConsoleColor.Red;
                                else
                                    Console.ForegroundColor = ConsoleColor.Gray;

                                Console.Write(dayStr);
                                pos += 2;

                                if (pos < line.Length && line[pos] == '*')
                                {
                                    Console.Write('*');
                                    pos++;
                                }
                                else
                                    Console.Write(' ');

                                Console.ResetColor();
                            }
                            else
                            {
                                Console.Write(line[pos]);
                                pos++;
                            }
                        }

                        Console.Write("   ");
                    }
                    else
                    {
                        Console.Write(new string(' ', 22) + "   ");
                    }
                }
                Console.WriteLine();
            }

            Console.WriteLine();
        }
    }

    // === СОХРАНЕНИЕ/ЗАГРУЗКА ===
    static void SaveEvents()
    {
        var options = new JsonSerializerOptions { WriteIndented = true };
        File.WriteAllText(eventsFile, JsonSerializer.Serialize(events, options));
    }

    static void LoadEvents()
    {
        if (File.Exists(eventsFile))
        {
            string json = File.ReadAllText(eventsFile);
            events = JsonSerializer.Deserialize<Dictionary<DateTime, List<string>>>(json);
        }
    }
}
