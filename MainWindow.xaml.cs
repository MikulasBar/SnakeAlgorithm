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
        // Variables
        public static int rows = 20 , cols = 20;                  
        Direction Dir, pastDir; // current direction, past direction
        bool gameOver = true; Position foodPos; string AstarType = ""; // Is Game Over, position of food, type of path: to the food / to the end of cycle
        static Random random = new(); PathAlgorithm aS = new(); PrimsAlgorithm aP = new(); // Instances of classes and structures
        LinkedList<Position> snakePositions = new(), AstarPath = new(); // All positions of snakes body, list of nodes in path to target
        Border[,] cells = new Border[rows,cols]; Direction[,] defaultDirs = new Direction[rows,cols]; // Cells of grid with color, cycle directions
        int[,] order = new int[rows,cols]; // Order of positions in cycle
        SolidColorBrush empty = new(Color.FromRgb(49,44,64)); // Color of empty node

        // Shorthand functions
        Position Head() => snakePositions.First.Value;              
        Position Tail() => snakePositions.Last.Value;
        Border Cell(Position pos) => cells[pos.Row, pos.Col];
        int Order(Position pos) => order[pos.Row, pos.Col];

        // Functions
        void Setup() // Setup for start of game
        {
            snakePositions = new();
            for(int r = 0; r < rows; r++) // Set grid to empty 
            {
                for(int c = 0; c < cols; c++)
                    cells[r,c].Background = empty;
            }
            for(int n = 0; n < 2; n++) // Set snake
            {
                cells[0,n].Background = Brushes.Lime;
                snakePositions.AddFirst(new Position(0,n)); 
            }
            Dir = new(0,1); pastDir = new(0,1); // Set direction
            (defaultDirs, order) = aP.HamiltonsCycle(rows, cols); // Set cycle
            AddFood();
        }
        List<Position> EmptyPositions() // Get all empty positions
        {
            List<Position> p = new();
            for(int r = 0; r < rows; r++)
            {
                for(int c = 0; c < cols; c++)
                {
                    if(Cell(new(r,c)).Background == empty)
                        p.Add(new(r,c));
                }
            }
            return p;
        }
        void AddFood() // Add food to random empty position
        {
            List<Position> emptys = EmptyPositions();
            if(emptys.Count == 0)
                return;
            foodPos = emptys[random.Next(emptys.Count)];
            Cell(foodPos).Background = Brushes.Red;
        }
        void Move() // Move snake by 1 position in direction Dir, all possibilities
        {
            Position newpos = new(Head().Row + Dir.rowDir, Head().Col + Dir.colDir);
            if(aS.WillHit(cells, newpos,0 ,0 ))
            {
                gameOver = true;
                return;
            }
            if(Cell(newpos).Background == Brushes.Red)
            {
                Cell(newpos).Background = Brushes.Lime;
                snakePositions.AddFirst(newpos);
                AddFood();
            }
            else if(Cell(newpos).Background == empty || newpos == Tail())
            {
                Cell(Tail()).Background = empty;
                Cell(newpos).Background = Brushes.Lime;
                snakePositions.RemoveLast();
                snakePositions.AddFirst(newpos);
            }
            pastDir.rowDir = Dir.rowDir;
            pastDir.colDir = Dir.colDir;
        }
        bool Conditions() // State when the snake can go for food / end of cycle
        {
            return (Order(Head()) > Order(Tail())) 
            ? Order(foodPos) > Order(Head()) 
            : Order(foodPos) > Order(Head()) && Order(foodPos) < Order(Tail());
        }
        void Path() // Controling gamestate, pick optimal next direction for the snake 
        {
            if(snakePositions.Count > rows*cols - rows*7.5) // After a while, its best to go around the cycle
                Dir = defaultDirs[Head().Row, Head().Col];
            else if(Order(Head()) > Order(Tail()) && Order(Head()) != (rows)*(cols)-1 && Order(Head()) > Order(foodPos))
            {
                if(AstarPath.Count == 0 || AstarType != "End")  // The snake follow the path to end of cycle
                    AstarPath = aS.AStar(cells, Head(), new(1,0), order); AstarType = "End";
                Dir = aS.NextMove(cells, Head(), AstarPath.First.Value);
                AstarPath.RemoveFirst();
            }
            else if(Conditions()) // the snake follows the path to food
            {
                if(AstarPath.Count == 0 || AstarType != "Food")
                    AstarPath = aS.AStar(cells, Head(), foodPos, order); AstarType = "Food";
                Dir = aS.NextMove(cells, Head(), AstarPath.First.Value);
                AstarPath.RemoveFirst();
            }
            else // The snake just follow cycle
                Dir = defaultDirs[Head().Row, Head().Col];
        }
        async Task Run() // Combining game and algorithm for controling the snake
        {
            await Task.Delay(5); // Delay
            if(gameOver)
                await Task.Delay(Timeout.Infinite);
            Path();
            Move();
            await Run();
        }
        async void Window_PreviewKeyDown(object sender, KeyEventArgs e) // Inputs
        {
            if(gameOver) // Gameover and any key pressed
            { 
                gameOver = false;
                Setup();
                await Run();
            }
            else if(e.Key == Key.R) // Reset game with R key
            {
                gameOver = true;
                Setup();
            }
        }
        public MainWindow() // Set grid
        {
            InitializeComponent();
            for(int r = 0; r < rows; r++)
                grid.RowDefinitions.Add(new RowDefinition {Height = new((int)800/rows)}); // Set rows
            for(int c = 0; c < cols; c++)
                grid.ColumnDefinitions.Add(new ColumnDefinition {Width = new((int)800/cols)}); // Set columns
            for(int r = 0; r < rows; r++)
            {
                for(int c = 0; c < cols; c++) // Connect grid with rows and columns
                {
                    cells[r,c] = new();
                    Grid.SetRow(cells[r,c], r);
                    Grid.SetColumn(cells[r,c], c);
                    grid.Children.Add(cells[r,c]);
                }
            }
            for(int r = 0; r < rows; r++) // Set grid to empty
            {
                for(int c = 0; c < cols; c++)
                    cells[r,c].Background = empty;
            }
        }
    }
}
