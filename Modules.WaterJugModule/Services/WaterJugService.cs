using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Domain;
using Core.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace Modules.WaterJugModule.Services
{
    public class WaterJugService(IMemoryCache cache) : IWaterJugService
    {
        public List<BucketState>? Solve(int x, int y, int z)
        {
            // Create a unique cache key for the given inputs
            string cacheKey = $"{x}-{y}-{z}";

            // Check if the result is already cached
            if (cache.TryGetValue(cacheKey, out List<BucketState>? cachedResult))
            {
                return cachedResult;
            }

            if (z < 0 || x < 0 || y < 0)
                throw new InvalidOperationException("Invalid input values");

            // Ensure the problem is solvable: z must be less than the largest bucket and divisible by GCD(x, y)
            if (z > Math.Max(x, y) || z % Gcd(x, y) != 0)
                throw new InvalidOperationException("No solution possible");

            var queue = new Queue<(int, int, List<BucketState>?)>();
            var visited = new HashSet<(int, int)>();

            queue.Enqueue((0, 0, new List<BucketState>()));
            visited.Add((0, 0));

            while (queue.Count > 0)
            {
                var (currX, currY, currSteps) = queue.Dequeue();

                // Return if the target is reached in either bucket
                if (currX == z || currY == z)
                {
                    if (currSteps != null && currSteps.Any())
                    {
                        currSteps[^1].Status = "Solved";
                    }

                    // Cache the result for 10 minutes
                    cache.Set(cacheKey, currSteps, TimeSpan.FromMinutes(10));
                    return currSteps;
                }

                // Generate and explore next states
                foreach (var (nextX, nextY, action) in GetNextStates(currX, currY, x, y))
                {
                    if (!visited.Add((nextX, nextY))) continue;
                    if (currSteps == null) continue;
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

            throw new InvalidOperationException("No solution possible");
        }

        private IEnumerable<(int, int, string)> GetNextStates(int currX, int currY, int maxX, int maxY)
        {
            // Return all possible transitions from the current state
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

        // Compute the GCD using the Euclidean algorithm
        private int Gcd(int a, int b) => b == 0 ? a : Gcd(b, a % b);
    }
}
