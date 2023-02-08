using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Threading;

namespace SnakeAl
{
    class Direction
    {
        public int rowDir;
        public int colDir;
        public Direction(int rowDir, int colDir)
        {
            this.rowDir = rowDir;
            this.colDir = colDir;
        }
    }
    public partial class MainWindow : Window
    {
        public static readonly int rows = 40 , cols = 40;
        Direction Dir = new Direction(0,1);
        Direction pastDir = new Direction(0,1);
        bool gameOver = true;
        static Random random = new Random(); Algorithm al = new Algorithm();
        LinkedList<Position> snakePositions = new LinkedList<Position>(); Position foodPos;
        LinkedList<Position> keyPositions = new LinkedList<Position>();
        static int[,] Fcost = new int[rows, cols];
        Border[,] cells = new Border[rows,cols];
        SolidColorBrush empty = new SolidColorBrush(Color.FromRgb(49,44,64));
        void Setup()
        {
            snakePositions = new LinkedList<Position>();
            for(int r = 0; r < rows; r++)
            {
                for(int c = 0; c < cols; c++)
                {
                    if(r == 0 || c == 0 || r == rows-1 || c == cols-1)
                        cells[r,c].Background = Brushes.Black;
                    else
                        cells[r,c].Background = empty;
                }
            }
            for(int n = 1; n < 4; n++)
            {
                cells[1,n].Background = Brushes.Lime;
                snakePositions.AddFirst(new Position(1, n)); 
            }
            Dir.rowDir = 0; Dir.colDir = 1; pastDir.rowDir = 0; pastDir.colDir = 1;
            AddFood();
        }
        IEnumerable<Position> EmptyPositions()
        {
            for(int r = 0; r < rows; r++)
            {
                for(int c = 0; c < cols; c++)
                {
                    if(cells[r,c].Background == empty)
                        yield return new Position(r,c);
                }
            }
        }
        void AddFood()
        {
            List<Position> emptys = new List<Position>(EmptyPositions());
            if(emptys.Count == 0)
                return;
            foodPos = emptys[random.Next(emptys.Count)];
            cells[foodPos.Row, foodPos.Col].Background = Brushes.Red;
            keyPositions = al.AStar(cells, snakePositions.First.Value, foodPos);
        }
        void Move()
        {
            Position newpos = new Position(snakePositions.First.Value.Row + Dir.rowDir, snakePositions.First.Value.Col + Dir.colDir);
            if(cells[newpos.Row, newpos.Col].Background == Brushes.Lime || cells[newpos.Row, newpos.Col].Background == Brushes.Black)
            {
                Task.Delay(50);
                gameOver = true;
                return;
            }
            if(cells[newpos.Row, newpos.Col].Background == Brushes.Red)
            {
                cells[newpos.Row, newpos.Col].Background = Brushes.Lime;
                snakePositions.AddFirst(new Position(newpos.Row, newpos.Col));
                AddFood();
            }
            else if(cells[newpos.Row, newpos.Col].Background == empty)
            {
                cells[newpos.Row, newpos.Col].Background = Brushes.Lime;
                cells[snakePositions.Last.Value.Row, snakePositions.Last.Value.Col].Background = empty;
                snakePositions.AddFirst(new Position(newpos.Row, newpos.Col));
                snakePositions.RemoveLast();
            }
            pastDir.rowDir = Dir.rowDir;
            pastDir.colDir = Dir.colDir;
        }
        async Task Run()
        {
            if(gameOver)
                await Task.Delay(Timeout.Infinite);
            if(keyPositions.Count != 0)
            {
                Dir = al.FindPath(cells, snakePositions.First.Value, keyPositions.First.Value);
                keyPositions.RemoveFirst();
            }
            else
            {
                Dir = al.FindPath(cells, snakePositions.First.Value, foodPos);
            }
            await Task.Delay(35);
            if(pastDir.rowDir == -1*Dir.rowDir && Dir.rowDir != 0)
                Dir.rowDir = pastDir.rowDir;
            if(pastDir.colDir == -1*Dir.colDir && Dir.colDir != 0)
                Dir.colDir = pastDir.colDir;
            Move();
            await Run();
        }
        void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if(!gameOver)
            {
                if(e.Key == Key.Up)
                    Dir = new Direction(-1,0);
                else if(e.Key == Key.Down)
                    Dir = new Direction(1,0);
                else if(e.Key == Key.Right)
                    Dir = new Direction(0,1);
                else if(e.Key == Key.Left)
                    Dir = new Direction(0,-1);
            }
        }
        async void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if(gameOver)
            { 
                gameOver = false;
                Setup();
                await Run();
            }
        }
        public MainWindow()
        {
            InitializeComponent();
            for(int r = 0; r < rows; r++)
                grid.RowDefinitions.Add(new RowDefinition {Height = new GridLength(20)});
            for(int c = 0; c < cols; c++)
                grid.ColumnDefinitions.Add(new ColumnDefinition {Width = new GridLength(20)});
            for(int r = 0; r < rows; r++)
            {
                for(int c = 0; c < cols; c++)
                {
                    cells[r,c] = new Border();
                    Grid.SetRow(cells[r,c], r);
                    Grid.SetColumn(cells[r,c], c);
                    grid.Children.Add(cells[r,c]);
                }
            }
            for(int r = 0; r < rows; r++)
            {
                for(int c = 0; c < cols; c++)
                {
                    if(r == 0 || c == 0 || r == rows-1 || c == cols-1)
                        cells[r,c].Background = Brushes.Black;
                    else
                        cells[r,c].Background = empty;
                }
            }
        }
    }
}
