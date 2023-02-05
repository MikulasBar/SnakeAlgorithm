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
     class Position
    {
        public int Row;
        public int Col;
        public Position(int row, int col)
        {
            Row = row;
            Col = col;
        }
    }
    enum Value
    {
        Snake, Empty, Food, Border
    }
    public partial class MainWindow : Window
    {
        static readonly int rows = 40, cols = 40;
        Direction Dir = new Direction(0,1);
        Direction pastDir = new Direction(0,1);
        int dirRow, dirCol, dirPastR, dirPastC;
        bool gameOver = true;
        static Random random = new Random();
        LinkedList<Position> snakePositions = new LinkedList<Position>();
        Value[,] Grid0 = new Value[rows,cols];
        Border[,] cells = new Border[rows,cols];
        Dictionary<Value, SolidColorBrush> dict = new() 
        {
            {Value.Empty, new SolidColorBrush(Color.FromRgb(49,44,64))},
            {Value.Snake, Brushes.Lime}, 
            {Value.Food, Brushes.Red},
            {Value.Border, Brushes.Black}
        };
        void Setup()
        {
            snakePositions = new LinkedList<Position>();
            for(int r = 0; r < rows; r++)
            {
                for(int c = 0; c < cols; c++)
                {
                    if(r == 0 || c == 0 || r == rows-1 || c == cols-1)
                        Grid0[r,c] = Value.Border;
                    else
                        Grid0[r,c] = Value.Empty;
                }
            }
            for(int n = 1; n < 4; n++)
            {
                Grid0[1,n] = Value.Snake;
                snakePositions.AddFirst(new Position(1, n)); 
            }
            Dir.rowDir = 0; Dir.colDir = 1; pastDir.rowDir = 0; pastDir.colDir = 1;
            AddFood();
        }
        void Draw()
        {
            for(int r = 0; r < rows; r++)
            {
                for(int c = 0; c < cols; c++)
                {
                    Value val = Grid0[r,c];
                    if(val == Value.Empty && r%2 == c%2)
                        cells[r,c].Background = new SolidColorBrush(Color.FromRgb(45, 40, 60));
                    else
                        cells[r,c].Background = dict[val];
                    
                }
            }
        }
        IEnumerable<Position> EmptyPositions()
        {
            for(int r = 0; r < rows; r++)
            {
                for(int c = 0; c < cols; c++)
                {
                    if(Grid0[r,c] == Value.Empty)
                        yield return new Position(r,c);
                }
            }
        }
        void AddFood()
        {
            List<Position> empty = new List<Position>(EmptyPositions());
            if(empty.Count == 0)
                return;
            Position pos = empty[random.Next(empty.Count)];
            Grid0[pos.Row, pos.Col] = Value.Food;
        }
        void Move()
        {
            Position newpos = new Position(snakePositions.First.Value.Row + Dir.rowDir, snakePositions.First.Value.Col + Dir.colDir);
            if(Grid0[newpos.Row, newpos.Col] == Value.Snake || Grid0[newpos.Row, newpos.Col] == Value.Border)
            {
                Task.Delay(50);
                gameOver = true;
                return;
            }
            if(Grid0[newpos.Row, newpos.Col] == Value.Food)
            {
                Grid0[newpos.Row, newpos.Col] = Value.Snake;
                snakePositions.AddFirst(new Position(newpos.Row, newpos.Col));
                AddFood();
            }
            else if(Grid0[newpos.Row, newpos.Col] == Value.Empty)
            {
                Grid0[newpos.Row, newpos.Col] = Value.Snake;
                Grid0[snakePositions.Last.Value.Row, snakePositions.Last.Value.Col] = Value.Empty;
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
            await Task.Delay(35);
            if(pastDir.rowDir == -1*Dir.rowDir && Dir.rowDir != 0)
                Dir.rowDir = pastDir.rowDir;
            if(pastDir.colDir == -1*Dir.colDir && Dir.colDir != 0)
                Dir.colDir = pastDir.colDir;
            Draw();
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
            {
                for(int c = 0; c < cols; c++)
                {
                    if(r == 0 || c == 0 || r == rows-1 || c == cols-1)
                        Grid0[r,c] = Value.Border;
                    else
                        Grid0[r,c] = Value.Empty;
                }
            }
            for(int r = 0; r < rows; r++)
                grid.RowDefinitions.Add(new RowDefinition {Height = new GridLength(20)});
            for(int c = 0; c < cols; c++)
                grid.ColumnDefinitions.Add(new ColumnDefinition {Width = new GridLength(20)});
            for(int r = 0; r < rows; r++)
            {
                for(int c = 0; c < cols; c++)
                {
                    Value val = Grid0[r,c];
                    cells[r,c] = new Border {Background = dict[val]};
                    Grid.SetRow(cells[r,c], r);
                    Grid.SetColumn(cells[r,c], c);
                    grid.Children.Add(cells[r,c]);
                }
            }
        }
    }
}
