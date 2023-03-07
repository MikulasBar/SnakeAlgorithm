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
    public partial class MainWindow : Window
    {
        public static int rows = 20 , cols = 20;
        Direction Dir, pastDir;
        bool gameOver = true; Position foodPos;
        static Random random = new(); PathAlgorithm aS = new(); PrimsAlgorithm aP = new();
        LinkedList<Position> snakePositions = new(), AstarPath = new();
        Border[,] cells = new Border[rows,cols]; Direction[,] defaultDirs = new Direction[rows,cols];
        int[,] order = new int[rows,cols];
        SolidColorBrush empty = new(Color.FromRgb(49,44,64));
        void Setup()
        {
            snakePositions = new();
            for(int r = 0; r < rows; r++)
            {
                for(int c = 0; c < cols; c++)
                    cells[r,c].Background = empty;
            }
            for(int n = 0; n < 2; n++)
            {
                cells[0,n].Background = Brushes.Lime;
                snakePositions.AddFirst(new Position(0,n)); 
            }
            Dir = new(0,1); pastDir = new(0,1);
            (defaultDirs, order) = aP.HamiltonsCycle(rows, cols);
            AddFood();
        }
        int Order(Position pos)
        {
            return order[pos.Row,pos.Col];
        }
        List<Position> EmptyPositions()
        {
            List<Position> p = new();
            for(int r = 0; r < rows; r++)
            {
                for(int c = 0; c < cols; c++)
                {
                    if(cells[r,c].Background == empty)
                        p.Add(new(r,c));
                }
            }
            return p;
        }
        void AddFood()
        {
            List<Position> emptys = EmptyPositions();
            if(emptys.Count == 0)
                return;
            foodPos = emptys[random.Next(emptys.Count)];
            cells[foodPos.Row, foodPos.Col].Background = Brushes.Red;
        }
        void Move()
        {
            Position newpos = new(snakePositions.First.Value.Row + Dir.rowDir, snakePositions.First.Value.Col + Dir.colDir);
            if(aS.WillHit(cells, newpos,0 ,0 ))
            {
                gameOver = true;
                return;
            }
            if(cells[newpos.Row, newpos.Col].Background == Brushes.Red)
            {
                cells[newpos.Row, newpos.Col].Background = Brushes.Lime;
                snakePositions.AddFirst(new Position(newpos.Row, newpos.Col));
                AddFood();
            }
            else if(cells[newpos.Row, newpos.Col].Background == empty || newpos == snakePositions.Last.Value)
            {
                cells[snakePositions.Last.Value.Row, snakePositions.Last.Value.Col].Background = empty;
                cells[newpos.Row, newpos.Col].Background = Brushes.Lime;
                snakePositions.RemoveLast();
                snakePositions.AddFirst(new Position(newpos.Row, newpos.Col));
            }
            pastDir.rowDir = Dir.rowDir;
            pastDir.colDir = Dir.colDir;
        }
        bool Conditions()
        {
            bool a;
            if(Order(snakePositions.First.Value) > Order(snakePositions.Last.Value))
                a = Order(foodPos) > Order(snakePositions.First.Value);
            else
                a = Order(foodPos) > Order(snakePositions.First.Value) && Order(foodPos) < Order(snakePositions.Last.Value);
            return a;
        }
        void Path()
        {
            
            if(snakePositions.Count > rows*cols - rows*7.5)
                Dir = defaultDirs[snakePositions.First.Value.Row, snakePositions.First.Value.Col];
            else if(Order(snakePositions.First.Value) > Order(snakePositions.Last.Value) && Order(snakePositions.First.Value) != (rows)*(cols)-1 && Order(snakePositions.First.Value) > Order(foodPos))
            {
                AstarPath = aS.AStar(cells, snakePositions.First.Value, new(1,0), order);
                Dir = aS.NextMove(cells, snakePositions.First.Value, AstarPath.First.Value);
                AstarPath.RemoveFirst();
            } 
            else if(Conditions())
            {
                if(AstarPath.Count == 0)
                    AstarPath = aS.AStar(cells, snakePositions.First.Value, foodPos, order);
                Dir = aS.NextMove(cells, snakePositions.First.Value, AstarPath.First.Value);
                AstarPath.RemoveFirst();
            }
            else
                Dir = defaultDirs[snakePositions.First.Value.Row, snakePositions.First.Value.Col];
        }
        async Task Run()
        {
            await Task.Delay(5);
            if(gameOver)
                await Task.Delay(Timeout.Infinite);
            Path();
            Move();
            await Run();
        }
        async void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if(gameOver)
            { 
                gameOver = false;
                Setup();
                await Run();
            }
            else if(e.Key == Key.R)
            {
                gameOver = true;
                Setup();
            }
        }
        public MainWindow()
        {
            InitializeComponent();
            for(int r = 0; r < rows; r++)
                grid.RowDefinitions.Add(new RowDefinition {Height = new((int)800/rows)});
            for(int c = 0; c < cols; c++)
                grid.ColumnDefinitions.Add(new ColumnDefinition {Width = new((int)800/cols)});
            for(int r = 0; r < rows; r++)
            {
                for(int c = 0; c < cols; c++)
                {
                    cells[r,c] = new();
                    Grid.SetRow(cells[r,c], r);
                    Grid.SetColumn(cells[r,c], c);
                    grid.Children.Add(cells[r,c]);
                }
            }
            for(int r = 0; r < rows; r++)
            {
                for(int c = 0; c < cols; c++)
                    cells[r,c].Background = empty;
            }
        }
    }
}
