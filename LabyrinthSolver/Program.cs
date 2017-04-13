using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabyrinthSolver
{
    internal class Program
    {
        private const char path = '.';
        private const char start = 's';
        private const char wall = '#';

        private static readonly Direction[] directions = Enum.GetValues(typeof(Direction)).Cast<Direction>().ToArray();

        private static Direction determineStartDirection(Node node)
        {
            foreach (var direction in directions)
                if (!node.HasNeighborFor(direction))
                    return direction;

            throw new InvalidOperationException();
        }

        private static IEnumerable<Direction[]> findExit(Node node, Direction comingFrom)
        {
            var directionsTaken = new List<Direction>();

            Direction[] availableDirections;
            while ((availableDirections = getPathNeighbors(node).Where(d => d != comingFrom).ToArray()).Length > 0)
            {
                if (availableDirections.Length == 1)
                {
                    directionsTaken.Add(availableDirections[0]);
                    comingFrom = getOpposite(availableDirections[0]);
                    node = node[availableDirections[0]];

                    if (node.ElementType == ElementType.Exit)
                        break;

                    continue;
                }

                foreach (var direction in availableDirections)
                {
                    foreach (var exitPaths in findExit(node[direction], getOpposite(direction)))
                    {
                        var path = new List<Direction>(directionsTaken);
                        path.Add(direction);
                        path.AddRange(exitPaths);

                        yield return path.ToArray();
                    }
                }

                break;
            }

            if (node.ElementType == ElementType.Exit)
                yield return directionsTaken.ToArray();
        }

        private static Direction getOpposite(Direction direction)
        {
            switch (direction)
            {
                case Direction.Up:
                    return Direction.Down;

                case Direction.Right:
                    return Direction.Left;

                case Direction.Down:
                    return Direction.Up;

                case Direction.Left:
                    return Direction.Right;
            }

            throw new InvalidOperationException();
        }

        private static IEnumerable<Direction> getPathNeighbors(Node node)
        {
            foreach (var direction in directions)
                if (node[direction]?.ElementType == ElementType.Path || node[direction]?.ElementType == ElementType.Exit)
                    yield return direction;
        }

        private static void Main(string[] args)
        {
            // Parse input into node matrix
            Node startNode = null;
            var lines = File.ReadAllLines("input.txt").Select(line => line.ToCharArray()).ToArray();
            var xLength = lines[0].Length;
            var yLength = lines.Length;
            var grid = new Node[xLength, yLength];
            for (var x = 0; x < xLength; ++x)
                for (var y = 0; y < yLength; ++y)
                    switch (lines[y][x])
                    {
                        case wall:
                            grid[x, y] = new Node(ElementType.Wall);
                            break;

                        case start:
                            if (startNode != null)
                                throw new InvalidDataException("Labyrinth can only have one start!");

                            startNode = new Node(ElementType.Start);
                            grid[x, y] = startNode;
                            break;

                        case path:
                            if (x == 0 || x == lines[0].Length - 1 || y == 0 || y == lines.Length - 1)
                                grid[x, y] = new Node(ElementType.Exit);
                            else
                                grid[x, y] = new Node(ElementType.Path);
                            break;
                    }

            if (startNode == null)
                throw new InvalidDataException("Labyrinth must have a start!");

            // Link nodes with eachother
            for (var x = 0; x < xLength; ++x)
                for (var y = 0; y < yLength; ++y)
                {
                    if (y > 0)
                        grid[x, y][Direction.Up] = grid[x, y - 1];

                    if (y < yLength - 1)
                        grid[x, y][Direction.Down] = grid[x, y + 1];

                    if (x > 0)
                        grid[x, y][Direction.Left] = grid[x - 1, y];

                    if (x < xLength - 1)
                        grid[x, y][Direction.Right] = grid[x + 1, y];
                }

            var comingFrom = determineStartDirection(startNode);
            var exitPaths = findExit(startNode, comingFrom).ToArray();

            Console.WriteLine("Solutions:");
            foreach (var path in exitPaths)
            {
                Console.WriteLine(" + " + string.Join(", ", path.Select(d => d.ToString())));
            }

            Console.WriteLine();
            Console.WriteLine("Press any key to quit.");
            Console.ReadLine();
        }
    }
}