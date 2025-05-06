using System;

namespace GeometryFigures
{
    public interface ILocation
    {
        double X { get; set; }
        double Y { get; set; }
        void SetPosition(double x, double y);
    }

    public interface IStylable
    {
        ConsoleColor Color { get; set; }
        bool IsVisible { get; set; }
    }

    public interface IBoundable
    {
        Clip GetClipBox();
    }

    public interface ICalculable
    {
        double Perimeter { get; }
        double Area { get; }
    }

    public class Location : ILocation
    {
        public double X { get; set; }
        public double Y { get; set; }

        public Location(double x = 0, double y = 0)
        {
            X = x;
            Y = y;
        }

        public void SetPosition(double x, double y)
        {
            X = Geometry.AccurateExtent(x);
            Y = Geometry.AccurateExtent(y);
        }
    }

    public static class Geometry
    {
        public const double Extent = 1e6;
        public const double Pi = Math.PI;

        public static double AccurateExtent(double value)
        {
            return Math.Clamp(value, -Extent, Extent);
        }
    }

    public class Clip
    {
        public Location Min { get; }
        public Location Max { get; }

        public Clip(Location min, Location max)
        {
            Min = new Location(min.X, min.Y);
            Max = new Location(max.X, max.Y);
        }

        public double SizeX => Max.X - Min.X;
        public double SizeY => Max.Y - Min.Y;
    }

    public class Primitive : ILocation, IStylable
    {
        private readonly Location _location = new Location();

        public double X
        {
            get => _location.X;
            set => _location.X = Geometry.AccurateExtent(value);
        }

        public double Y
        {
            get => _location.Y;
            set => _location.Y = Geometry.AccurateExtent(value);
        }

        public ConsoleColor Color { get; set; } = ConsoleColor.White;
        public bool IsVisible { get; set; } = true;

        public void SetPosition(double x, double y) => _location.SetPosition(x, y);
    }

    public enum RotationDirection { Clockwise, CounterClockwise }
    public class CircleSector : Primitive, IBoundable, ICalculable
    {
        public double Radius { get; private set; }
        public double StartAngle { get; private set; }
        public double EndAngle { get; private set; }
        public RotationDirection Direction { get; private set; }

        public CircleSector(double radius = 1, double startAngle = 0, double endAngle = 90, RotationDirection direction = RotationDirection.CounterClockwise)
        {
            SetRadius(radius);
            SetAngles(startAngle, endAngle);
            Direction = direction;
        }
        public void SetDirection(RotationDirection direction)
        {
            Direction = direction;
            UpdateAngles();
        }

        private void UpdateAngles()
        {
            // Корректировка углов при смене направления
            double temp = StartAngle;
            StartAngle = EndAngle;
            EndAngle = temp;
        }

        public void SetAngles(double start, double end)
        {
            StartAngle = start % 360;
            EndAngle = end % 360;

            if (Direction == RotationDirection.Clockwise && StartAngle < EndAngle)
                StartAngle += 360;
        }

        public void SetRadius(double radius)
        {
            if (radius <= 0) throw new ArgumentException("Radius must be positive");
            Radius = radius;
            ValidatePosition();
        }

        public void Resize(double newRadius, double newStartAngle, double newEndAngle)
        {
            // Валидация радиуса
            if (newRadius <= 0)
                throw new ArgumentException("Радиус должен быть положительным числом", nameof(newRadius));

            // Корректировка углов при смене направления
            if (Direction == RotationDirection.Clockwise && newStartAngle < newEndAngle)
                newStartAngle += 360;

            // Применение новых значений
            Radius = newRadius;
            StartAngle = newStartAngle % 360;
            EndAngle = newEndAngle % 360;

            // Проверка границ
            ValidatePosition();
            ValidateAngles();
        }

        private void ValidateAngles()
        {
            // Нормализация углов
            if (Direction == RotationDirection.CounterClockwise && StartAngle > EndAngle)
                EndAngle += 360;

            if (Direction == RotationDirection.Clockwise && StartAngle < EndAngle)
                StartAngle += 360;
        }

        public void Resize(double newRadius) => Resize(newRadius, StartAngle, EndAngle);

        public double SweepAngle
        {
            get
            {
                double angle = Direction == RotationDirection.CounterClockwise
                    ? EndAngle - StartAngle
                    : StartAngle - EndAngle;

                return (angle < 0) ? angle + 360 : angle;
            }
        }

        public double Perimeter =>
            (2 * Math.PI * Radius * SweepAngle / 360) + 2 * Radius;

        public double Area =>
            (Math.PI * Radius * Radius * SweepAngle) / 360;

        public double ArcLength =>
            (Math.PI * Radius * 2) * SweepAngle / 360;

        private void ValidatePosition()
        {
            X = Math.Clamp(X, -Geometry.Extent + Radius, Geometry.Extent - Radius);
            Y = Math.Clamp(Y, -Geometry.Extent + Radius, Geometry.Extent - Radius);
        }

        public Clip GetClipBox() => new Clip(
            new Location(X - Radius, Y - Radius),
            new Location(X + Radius, Y + Radius)
        );
    }

    public static class FigureModifier
    {
        public static void ModifyFigure(ref CircleSector sector)
        {
            while (true)
            {
                Console.WriteLine("\nТекущие параметры сектора:");
                Console.WriteLine($"1. X: {sector.X}");
                Console.WriteLine($"2. Y: {sector.Y}");
                Console.WriteLine($"3. Цвет: {sector.Color}");
                Console.WriteLine($"4. Видимость: {sector.IsVisible}");
                Console.WriteLine($"5. Радиус: {sector.Radius}");
                Console.WriteLine($"6. Начальный угол: {sector.StartAngle}°");
                Console.WriteLine($"7. Конечный угол: {sector.EndAngle}°");
                Console.WriteLine($"8. Направление: {sector.Direction}");
                Console.WriteLine($"9. Показать характеристики");
                Console.WriteLine("10. Выход");

                Console.Write("Выберите параметр для изменения: ");
                string choice = Console.ReadLine();

                try
                {
                    switch (choice)
                    {
                        case "1":
                            Console.Write("Новое значение X: ");
                            sector.X = double.Parse(Console.ReadLine());
                            break;
                        case "2":
                            Console.Write("Новое значение Y: ");
                            sector.Y = double.Parse(Console.ReadLine());
                            break;
                        case "3":
                            Console.WriteLine("Доступные цвета:");
                            foreach (var color in Enum.GetValues(typeof(ConsoleColor)))
                                Console.WriteLine(color);
                            Console.Write("Выберите цвет: ");
                            sector.Color = (ConsoleColor)Enum.Parse(
                                typeof(ConsoleColor), Console.ReadLine(), true);
                            break;
                        case "4":
                            sector.IsVisible = !sector.IsVisible;
                            break;
                        case "5":
                            Console.Write("Новый радиус: ");
                            double r = double.Parse(Console.ReadLine());
                            sector.Resize(r, sector.StartAngle, sector.EndAngle);
                            break;
                        case "6":
                            Console.Write("Новый начальный угол: ");
                            double sa = double.Parse(Console.ReadLine());
                            sector.Resize(sector.Radius, sa, sector.EndAngle);
                            break;
                        case "7":
                            Console.Write("Новый конечный угол: ");
                            double ea = double.Parse(Console.ReadLine());
                            sector.Resize(sector.Radius, sector.StartAngle, ea);
                            break;
                        case "8":
                            Console.WriteLine("Доступные направления:");
                            foreach (RotationDirection dir in Enum.GetValues(typeof(RotationDirection)))
                                Console.WriteLine($"- {dir}");

                            Console.Write("Введите направление: ");
                            if (Enum.TryParse(Console.ReadLine(), true, out RotationDirection newDir))
                                sector.SetDirection(newDir);
                            break;
                        case "9":
                            Console.WriteLine($"Периметр: {sector.Perimeter:F2}");
                            Console.WriteLine($"Площадь: {sector.Area:F2}");
                            Console.WriteLine($"Угол развертки: {sector.SweepAngle:F2}°");
                            Console.WriteLine($"Длина дуги: {sector.ArcLength:F2}");
                            break;
                        case "10":
                            return;
                        default:
                            Console.WriteLine("Неверный выбор!");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка: {ex.Message}");
                }
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            CircleSector sector = new CircleSector();
            FigureModifier.ModifyFigure(ref sector);
        }
    }
}