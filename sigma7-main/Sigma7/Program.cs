using System;
using System.Globalization;
using System.Text;

namespace Sigma7
{
    internal static class Program
    {
        private static int Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            if (args.Length == 0 || (args.Length == 1 && args[0] == "help"))
            {
                PrintHelp();
                return 0;
            }

            string command = args[0];
            if (command == "crew")
            {
                if (args.Length != 2 || string.IsNullOrWhiteSpace(args[1]))
                    return InvalidInput();

                CrewMember member = new CrewMember(args[1]);
                Console.WriteLine("crew=" + member.Name);
                return 0;
            }

            if (command != "average" && command != "alarms" && command != "minimum"
                && command != "refuel" && command != "access")
                return InvalidInput();

            int[] numbers = new int[args.Length - 1];
            for (int i = 1; i < args.Length; i++)
            {
                if (!int.TryParse(args[i], out int value) || value < -100 || value > 100)
                    return InvalidInput();
                numbers[i - 1] = value;
            }

            switch (command)
            {
                case "average":
                    if (numbers.Length < 1 || numbers.Length > 20)
                        return InvalidInput();
                    Console.WriteLine("average=" + Telemetry.Average(numbers)
                        .ToString("0.###############", CultureInfo.InvariantCulture));
                    return 0;
                case "minimum":
                    if (numbers.Length < 1 || numbers.Length > 20)
                        return InvalidInput();
                    Console.WriteLine("minimum=" + Telemetry.Minimum(numbers));
                    return 0;
                case "alarms":
                    if (numbers.Length < 2 || numbers.Length > 21)
                        return InvalidInput();
                    int[] readings = new int[numbers.Length - 1];
                    for (int i = 1; i < numbers.Length; i++)
                        readings[i - 1] = numbers[i];
                    Console.WriteLine("alarms=" + Telemetry.CountAlarms(readings, numbers[0]));
                    return 0;
                case "refuel":
                    if (numbers.Length != 2 || numbers[0] < 0 || numbers[1] < 0
                        || numbers[0] + numbers[1] > 100)
                        return InvalidInput();
                    FuelTank tank = new FuelTank(numbers[0]);
                    tank.Refuel(numbers[1]);
                    Console.WriteLine("fuel=" + tank.Fuel);
                    return 0;
                case "access":
                    if (numbers.Length != 2 || numbers[0] < 0 || numbers[0] > 1
                        || numbers[1] < 0 || numbers[1] > 1)
                        return InvalidInput();
                    Console.WriteLine("access=" + (Airlock.CanOpen(numbers[0] == 1,
                        numbers[1] == 1) ? "OPEN" : "CLOSED"));
                    return 0;
                default:
                    return InvalidInput();
            }
        }

        private static int InvalidInput()
        {
            Console.Error.WriteLine("Некорректные аргументы. Справка: help");
            return 2;
        }

        private static void PrintHelp()
        {
            Console.WriteLine("СИГМА-7: пульт диагностики орбитальной станции");
            Console.WriteLine("average <показания...>          Среднее показаний");
            Console.WriteLine("alarms <порог> <показания...>    Количество тревог");
            Console.WriteLine("minimum <показания...>          Минимум показаний");
            Console.WriteLine("refuel <запас> <добавка>         Заправка бака");
            Console.WriteLine("crew <имя>                      Регистрация члена экипажа");
            Console.WriteLine("access <пропуск:0|1> <PIN:0|1>   Управление шлюзом");
            Console.WriteLine("Правила и допустимые данные: README.md");
        }
    }
}
