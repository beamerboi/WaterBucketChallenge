using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Domain;
using Core.Interfaces;

namespace Modules.WaterJugModule.Services
{
    public class WaterJugService : IWaterJugService
    {
        public List<BucketState> Solve(int x, int y, int z)
        {
            if (z > Math.Max(x, y) || z % Gcd(x, y) != 0)
                throw new InvalidOperationException("No solution possible");

            var queue = new Queue<(int, int, List<BucketState>)>();
            var visited = new HashSet<(int, int)>();

            queue.Enqueue((0, 0, new List<BucketState>()));
            visited.Add((0, 0));

            while (queue.Count > 0)
            {
                var (currX, currY, currSteps) = queue.Dequeue();

                if (currX == z || currY == z)
                {
                    if (currSteps.Any())
                    {
                        currSteps[^1].Status = "Solved";
                    }
                    return currSteps;
                }

                foreach (var (nextX, nextY, action) in GetNextStates(currX, currY, x, y))
                {
                    if (!visited.Contains((nextX, nextY)))
                    {
                        visited.Add((nextX, nextY));
                        var newSteps = new List<BucketState>(currSteps)
                        {
                            new BucketState
                            {
                                Step = currSteps.Count + 1,
                                BucketX = nextX,
                                BucketY = nextY,
                                Action = action
                            }
                        };
                        queue.Enqueue((nextX, nextY, newSteps));
                    }
                }
            }

            throw new InvalidOperationException("No solution possible");
        }

        private IEnumerable<(int, int, string)> GetNextStates(int currX, int currY, int maxX, int maxY)
        {
            return new List<(int, int, string)>
            {
                (maxX, currY, "Fill Bucket X"),
                (currX, maxY, "Fill Bucket Y"),
                (0, currY, "Empty Bucket X"),
                (currX, 0, "Empty Bucket Y"),
                (Math.Min(currX + currY, maxX), Math.Max(0, currY - (maxX - currX)), "Transfer Y to X"),
                (Math.Max(0, currX - (maxY - currY)), Math.Min(currX + currY, maxY), "Transfer X to Y")
            };
        }

        private int Gcd(int a, int b) => b == 0 ? a : Gcd(b, a % b);
    }
}
