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
        private readonly WaterJugService _waterJugService;
        private readonly Mock<IMemoryCache> _memoryCacheMock;

        public WaterJugServiceTests()
        {
            // Create a mock for IMemoryCache
            _memoryCacheMock = new Mock<IMemoryCache>();

            // Setup the mock to always return null for simplicity (no cache hits in tests)
            _memoryCacheMock
                .Setup(x => x.TryGetValue(It.IsAny<object>(), out It.Ref<object>.IsAny))
                .Returns(false);

            // Pass the mock to WaterJugService
            _waterJugService = new WaterJugService(_memoryCacheMock.Object);
        }

        [Fact]
        public void Solve_WithValidInput_ReturnsSolution()
        {
            // Arrange
            int bucketX = 3;
            int bucketY = 5;
            int target = 4;

            // Act
            var result = _waterJugService.Solve(bucketX, bucketY, target);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.Equal(4, result.Last().BucketY); // Final state should have target amount in one of the buckets
            Assert.All(result, state =>
            {
                Assert.InRange(state.BucketX, 0, bucketX); // X bucket should never exceed its capacity
                Assert.InRange(state.BucketY, 0, bucketY); // Y bucket should never exceed its capacity
                Assert.NotNull(state.Action); // Each state should have an action
            });
        }

        [Theory]
        [InlineData(2, 6, 4)]
        [InlineData(3, 5, 4)]
        public void Solve_WithValidInputs_ReturnsValidSteps(int x, int y, int z)
        {
            // Act
            var result = _waterJugService.Solve(x, y, z);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.True(result.Last().BucketX == z || result.Last().BucketY == z);
            Assert.Equal("Solved", result.Last().Status);
            Assert.Equal(result.Count, result.Last().Step);
            Assert.All(result, state =>
            {
                Assert.True(state.Step > 0 && state.Step <= result.Count);
                if (state != result.Last())
                {
                    Assert.Null(state.Status);
                }
            });
        }

        [Theory]
        [InlineData(2, 3, 7)] // Target larger than both buckets
        [InlineData(4, 6, 5)] // Target not possible with GCD
        public void Solve_WithImpossibleTarget_ThrowsInvalidOperationException(int x, int y, int z)
        {
            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => _waterJugService.Solve(x, y, z));
            Assert.Equal("No solution possible", exception.Message);
        }

        [Fact]
        public void Solve_VerifySpecificSolutionPath()
        {
            // Arrange
            int bucketX = 3;
            int bucketY = 5;
            int target = 4;

            // Act
            var result = _waterJugService.Solve(bucketX, bucketY, target);

            // Assert
            Assert.Collection(result,
                step => AssertBucketState(step, 0, 5, "Fill Bucket Y", 1, null),
                step => AssertBucketState(step, 3, 2, "Transfer Y to X", 2, null),
                step => AssertBucketState(step, 0, 2, "Empty Bucket X", 3, null),
                step => AssertBucketState(step, 2, 0, "Transfer Y to X", 4, null),
                step => AssertBucketState(step, 2, 5, "Fill Bucket Y", 5, null),
                step => AssertBucketState(step, 3, 4, "Transfer Y to X", 6, "Solved")
            );
        }

        private void AssertBucketState(BucketState state, int expectedX, int expectedY, string expectedAction, int expectedStep, string? expectedStatus)
        {
            Assert.Equal(expectedX, state.BucketX);
            Assert.Equal(expectedY, state.BucketY);
            Assert.Equal(expectedAction, state.Action);
            Assert.Equal(expectedStep, state.Step);
            Assert.Equal(expectedStatus, state.Status);
        }
    }
}
