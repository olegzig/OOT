using System;

namespace GeometryFigures
{
    // Класс для описания положения
    public class Location
    {
        public double X { get; set; }
        public double Y { get; set; }

        public Location() : this(0, 0) { }

        public Location(double x, double y)
        {
            X = x;
            Y = y;
        }

        public Location(Location other) : this(other.X, other.Y) { }
    }

    // Класс для ограничивающей области
    public class Clip
    {
        public double MinX { get; set; }
        public double MaxX { get; set; }
        public double MinY { get; set; }
        public double MaxY { get; set; }

        public Clip() : this(0, 0, 0, 0) { }

        public Clip(double minX, double maxX, double minY, double maxY)
        {
            MinX = minX;
            MaxX = maxX;
            MinY = minY;
            MaxY = maxY;
        }

        public Clip(Clip other) : this(other.MinX, other.MaxX, other.MinY, other.MaxY) { }
    }

    // Статический класс с методами проверки
    public static class Geometry
    {
        public static Clip GlobalClip { get; set; } = new Clip(-100, 100, -100, 100);
        public const double Epsilon = 1e-5;

        public static bool IsWithinBounds(Location location)
        {
            return location.X >= GlobalClip.MinX - Epsilon &&
                   location.X <= GlobalClip.MaxX + Epsilon &&
                   location.Y >= GlobalClip.MinY - Epsilon &&
                   location.Y <= GlobalClip.MaxY + Epsilon;
        }
    }

    // Класс с оформительскими свойствами (наследование от Location)
    public class Primitive : Location
    {
        public ConsoleColor Color { get; set; }
        public bool IsVisible { get; set; }

        public Primitive() : base()
        {
            Color = ConsoleColor.White;
            IsVisible = true;
        }

        public Primitive(double x, double y, ConsoleColor color, bool isVisible) : base(x, y)
        {
            Color = color;
            IsVisible = isVisible;
        }

        public Primitive(Primitive other) : base(other.X, other.Y)
        {
            Color = other.Color;
            IsVisible = other.IsVisible;
        }
    }

    // Класс точки (наследуем от Primitive)
    public class Point : Primitive
    {
        public Point() : base() { }

        public Point(double x, double y, ConsoleColor color, bool isVisible) 
            : base(x, y, color, isVisible) { }

        public Point(Point other) : base(other) { }
    }

    // Пример фигуры - прямоугольник (ваш вариант может отличаться)
    public class Rectangle : Point
    {
        public double Width { get; private set; }
        public double Height { get; private set; }

        public Rectangle() : base()
        {
            Width = 0;
            Height = 0;
        }

        public Rectangle(double x, double y, ConsoleColor color, bool isVisible, 
                        double width, double height) : base(x, y, color, isVisible)
        {
            Width = width;
            Height = height;
            ValidateBounds();
        }

        public Rectangle(Rectangle other) : base(other)
        {
            Width = other.Width;
            Height = other.Height;
        }

        public double Perimeter => 2 * (Width + Height);
        public double Area => Width * Height;

        public Clip GetBoundingClip()
        {
            return new Clip
            {
                MinX = X - Width / 2,
                MaxX = X + Width / 2,
                MinY = Y - Height / 2,
                MaxY = Y + Height / 2
            };
        }

        public void Resize(double width, double height)
        {
            Width = width;
            Height = height;
            ValidateBounds();
        }

        private void ValidateBounds()
        {
            Clip clip = GetBoundingClip();
            if (!Geometry.IsWithinBounds(new Location(clip.MinX, clip.MinY)) ||
                !Geometry.IsWithinBounds(new Location(clip.MaxX, clip.MaxY)))
                throw new ArgumentException("Фигура выходит за границы!");
        }
    }

    public static class FigureModifier
    {
        public static void ModifyFigure(ref Rectangle figure)
        {
            while (true)
            {
                Console.WriteLine("\nТекущие параметры фигуры:");
                Console.WriteLine($"1. X: {figure.X}");
                Console.WriteLine($"2. Y: {figure.Y}");
                Console.WriteLine($"3. Цвет: {figure.Color}");
                Console.WriteLine($"4. Видимость: {figure.IsVisible}");
                Console.WriteLine($"5. Ширина: {figure.Width}");
                Console.WriteLine($"6. Высота: {figure.Height}");
                Console.WriteLine("7. Выход");

                Console.Write("Выберите параметр для изменения: ");
                string choice = Console.ReadLine();

                try
                {
                    switch (choice)
                    {
                        case "1":
                            Console.Write("Новое значение X: ");
                            figure.X = double.Parse(Console.ReadLine());
                            break;
                        case "2":
                            Console.Write("Новое значение Y: ");
                            figure.Y = double.Parse(Console.ReadLine());
                            break;
                        case "3":
                            Console.WriteLine("Доступные цвета:");
                            foreach (var color in Enum.GetValues(typeof(ConsoleColor)))
                                Console.WriteLine(color);
                            Console.Write("Выберите цвет: ");
                            figure.Color = (ConsoleColor)Enum.Parse(
                                typeof(ConsoleColor), Console.ReadLine(), true);
                            break;
                        case "4":
                            figure.IsVisible = !figure.IsVisible;
                            break;
                        case "5":
                            Console.Write("Новая ширина: ");
                            double w = double.Parse(Console.ReadLine());
                            Console.Write("Новая высота: ");
                            double h = double.Parse(Console.ReadLine());
                            figure.Resize(w, h);
                            break;
                        case "6":
                            Console.Write("Новая высота: ");
                            double height = double.Parse(Console.ReadLine());
                            figure.Resize(figure.Width, height);
                            break;
                        case "7":
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

    // Пример использования
    class Program
    {
        static void Main(string[] args)
        {
            Rectangle rect = new Rectangle(0, 0, ConsoleColor.Red, true, 20, 10);
            FigureModifier.ModifyFigure(ref rect);
        }
    }
}