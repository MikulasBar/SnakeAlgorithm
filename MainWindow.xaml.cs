﻿using System;
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
        public static readonly int rows = 16 , cols = 16;
        Direction Dir = new Direction(0,1);
        Direction pastDir = new Direction(0,1);
        bool gameOver = true;
        static Random random = new Random(); Algorithm al = new Algorithm();
        LinkedList<Position> snakePositions = new LinkedList<Position>(); Position foodPos;
        LinkedList<Position> AstarPath = new LinkedList<Position>();
        Border[,] cells = new Border[rows,cols]; Direction[,] defaultDirs = new Direction[rows,cols];
        int[,] order = new int[rows,cols];
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
        void SetDefaultDirs()
        {
            for(int r = 1; r < rows-1; r++)
            {
                if(r%2 == 1)
                {
                    defaultDirs[r,rows -2] = new Direction(1,0);
                    for(int c = 1; c < cols -2; c++)
                        defaultDirs[r,c] = new Direction(0,1);
                }
                if(r%2 == 0)
                {
                    
                    for(int c = 2; c < cols -1; c++)
                        defaultDirs[r,c] = new Direction(0,-1);
                } 
            }
            for(int r = 2; r < rows -1; r++)
                defaultDirs[r,1] = new Direction(-1, 0);
            for(int r = 2; r < rows -2; r++)
            {
                if(r%2 == 0)
                    defaultDirs[r,2] = new Direction(1,0);
            }
        }
        void SetOrder()
        {
            Position pos = new Position(1,1);
            for(int i = 0; i < (rows-2) * (cols-2); i++)
            {
                order[pos.Row,pos.Col] = i;
                pos = new Position(pos.Row + defaultDirs[pos.Row,pos.Col].rowDir, pos.Col + defaultDirs[pos.Row, pos.Col].colDir);
            }
        }
        int Order(Position pos)
        {
            return order[pos.Row,pos.Col];
        }
        Position ReverseOrder(int i)
        {
            for(int r = 1; r < rows-1; r++)
            {
                for(int c = 1; c < cols-1; c++)
                {
                    if(order[r,c] == i)
                        return new Position(r,c);
                }
            }
            return new Position(0,0);
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
        bool Conditions()
        {
            bool a;
            if(Order(snakePositions.First.Value) > Order(snakePositions.Last.Value))
            {
                a = Order(foodPos) > Order(snakePositions.First.Value);
            }
            else
            {
                a = Order(foodPos) > Order(snakePositions.First.Value) && Order(foodPos) < Order(snakePositions.Last.Value);
            }
            return a;
        }
        void Path()
        {
            if(snakePositions.Count > (rows-2)*(cols-2) - (rows-2)*4)
                Dir = defaultDirs[snakePositions.First.Value.Row, snakePositions.First.Value.Col];
            else if(Conditions())
            {
                if(AstarPath.Count == 0)
                    AstarPath = al.AStar(cells, snakePositions.First.Value, foodPos, order);
                Dir = al.NextMove(cells, snakePositions.First.Value, AstarPath.First.Value);
                AstarPath.RemoveFirst();
            }
            else if(Order(snakePositions.First.Value) > Order(snakePositions.Last.Value) && Order(snakePositions.First.Value) != (rows-2)*(cols-2)-1)
            {
                AstarPath = al.AStar(cells, snakePositions.First.Value, ReverseOrder((rows-2)*(cols-2)-1), order);
                Dir = al.NextMove(cells, snakePositions.First.Value, AstarPath.First.Value);
                AstarPath.RemoveFirst();
            }
            else
                Dir = defaultDirs[snakePositions.First.Value.Row, snakePositions.First.Value.Col];
        }
        async Task Run()
        {
            if(gameOver)
                await Task.Delay(Timeout.Infinite);
            Path();
            if(pastDir.rowDir == -1*Dir.rowDir && Dir.rowDir != 0)
                Dir.rowDir = pastDir.rowDir;
            if(pastDir.colDir == -1*Dir.colDir && Dir.colDir != 0)
                Dir.colDir = pastDir.colDir;
                await Task.Delay(1);
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
            SetDefaultDirs();
            SetOrder();
            for(int r = 0; r < rows; r++)
                grid.RowDefinitions.Add(new RowDefinition {Height = new GridLength((int)800/rows)});
            for(int c = 0; c < cols; c++)
                grid.ColumnDefinitions.Add(new ColumnDefinition {Width = new GridLength((int)800/cols)});
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
