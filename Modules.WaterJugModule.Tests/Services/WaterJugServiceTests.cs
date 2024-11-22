using Modules.WaterJugModule.Services;
using System;
using System.Collections.Generic;
using Core.Domain;
using Microsoft.Extensions.Caching.Memory;
using Moq;

namespace Modules.WaterJugModule.Tests.Services
{
    public class WaterJugServiceTests
    {
        private readonly IMemoryCache _memoryCache;
        private readonly WaterJugService _service;

        public WaterJugServiceTests()
        {
            _memoryCache = new MemoryCache(new MemoryCacheOptions());
            _service = new WaterJugService(_memoryCache);
        }

        [Fact]
        public void Solve_ValidInput_ReturnsCorrectSolution()
        {
            // Arrange
            int x = 3, y = 5, z = 4;

            // Act
            var result = _service.Solve(x, y, z);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Solved", result[^1].Status);
            Assert.Contains(result, state => state.BucketX == 4 || state.BucketY == 4);
        }

        [Fact]
        public void Solve_InvalidInput_ThrowsException()
        {
            // Arrange
            int x = 2, y = 6, z = 5;

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => _service.Solve(x, y, z));
        }

        [Fact]
        public void Solve_ValidInput_CachesResult()
        {
            // Arrange
            int x = 3, y = 5, z = 4;

            // Act
            var firstResult = _service.Solve(x, y, z);
            var isCached = _memoryCache.TryGetValue($"{x}-{y}-{z}", out List<BucketState>? cachedResult);

            // Assert
            Assert.True(isCached);
            Assert.Equal(firstResult, cachedResult);
        }

        [Fact]
        public void Solve_NegativeInput_ThrowsException()
        {
            // Arrange
            int x = -1, y = 3, z = 2;

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => _service.Solve(x, y, z));
        }

        [Fact]
        public void Solve_UnsolvableInput_ThrowsException()
        {
            // Arrange
            int x = 1, y = 2, z = 5;

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => _service.Solve(x, y, z));
        }

        [Theory]
        [InlineData(3, 5, 4, true)]
        [InlineData(2, 6, 5, false)]
        [InlineData(1, 2, 5, false)]
        public void Solve_ParameterizedTests(int x, int y, int z, bool hasSolution)
        {
            // Act & Assert
            if (hasSolution)
            {
                var result = _service.Solve(x, y, z);
                Assert.NotNull(result);
                Assert.Equal("Solved", result[^1].Status);
                Assert.Contains(result, state => state.BucketX == z || state.BucketY == z);
            }
            else
            {
                Assert.Throws<InvalidOperationException>(() => _service.Solve(x, y, z));
            }
        }
    }
}
