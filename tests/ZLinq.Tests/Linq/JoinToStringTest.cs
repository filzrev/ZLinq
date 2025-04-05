namespace ZLinq.Tests.Linq;

public class JoinToStringTest
{
    [Fact]
    public void JoinToString_EmptySequence_ReturnsEmptyString()
    {
        // Arrange
        var empty = Array.Empty<int>();

        // Act
        var result = empty.AsValueEnumerable().JoinToString(",");

        // Assert
        result.ShouldBe("");
    }

    [Fact]
    public void JoinToString_SingleItem_ReturnsItemAsString()
    {
        // Arrange
        var singleItem = new[] { 42 };

        // Act
        var result = singleItem.AsValueEnumerable().JoinToString(",");

        // Assert
        result.ShouldBe("42");
    }

    [Fact]
    public void JoinToString_EmptySequence_EmptySeparator()
    {
        // Arrange
        var empty = Array.Empty<int>();

        // Act
        var result = empty.AsValueEnumerable().JoinToString("");

        // Assert
        result.ShouldBe("");
    }

    [Fact]
    public void JoinToString_SingleItem_EmptySeparator()
    {
        // Arrange
        var singleItem = new[] { 42 };

        // Act
        var result = singleItem.AsValueEnumerable().JoinToString("");

        // Assert
        result.ShouldBe("42");
    }

    [Fact]
    public void JoinToString_MultipleItems_WithNonEmptySeparator()
    {
        // Arrange
        var items = new[] { 1, 2, 3, 4, 5 };
        var separator = ", ";

        // Act - ZLinq
        var actual = items.AsValueEnumerable().JoinToString(separator);

        // Act - Standard approach for comparison
        var expected = string.Join(separator, items);

        // Assert
        actual.ShouldBe(expected);
    }

    [Fact]
    public void JoinToString_MultipleItems_WithEmptySeparator()
    {
        // Arrange
        var items = new[] { 1, 2, 3, 4, 5 };
        var separator = "";

        // Act - ZLinq
        var actual = items.AsValueEnumerable().JoinToString(separator);

        // Act - Standard approach for comparison
        var expected = string.Join(separator, items);

        // Assert
        actual.ShouldBe(expected);
    }

    [Fact]
    public void JoinToString_WithNullValues_HandlesNullsCorrectly()
    {
        // Arrange
        var items = new string?[] { "a", null, "c", null, "e" };
        var separator = "-";

        // Act - ZLinq
        var actual = items.AsValueEnumerable().JoinToString(separator);

        // Act - Standard approach for comparison
        var expected = string.Join(separator, items);

        // Assert
        actual.ShouldBe(expected);
    }

    [Fact]
    public void JoinToString_WithComplexObjects_UsesToStringMethod()
    {
        // Arrange
        var items = new[]
        {
            new Person { Name = "Alice", Age = 30 },
            new Person { Name = "Bob", Age = 25 },
            new Person { Name = "Charlie", Age = 35 }
        };
        var separator = "; ";

        // Act - ZLinq
        var actual = items.AsValueEnumerable().JoinToString(separator);

        // Act - Standard approach for comparison
        var expected = string.Join(separator, items.AsEnumerable());

        // Assert
        actual.ShouldBe(expected);
    }

    [Fact]
    public void JoinToString_WithSpanOptimization_ReturnsCorrectResult()
    {
        // This test specifically targets the TryGetSpan optimization path
        // Arrange
        var items = new[] { 10, 20, 30, 40, 50 };
        var separator = ",";

        // Act - ZLinq
        var actual = items.AsValueEnumerable().JoinToString(separator);

        // Act - Standard approach for comparison
        var expected = string.Join(separator, items);

        // Assert
        actual.ShouldBe(expected);
    }

    [Fact]
    public void JoinToString_EnsuresDisposal()
    {
        // Arrange
        var disposed = false;

        IEnumerable<int> GetSequence()
        {
            try
            {
                yield return 1;
                yield return 2;
                yield return 3;
            }
            finally
            {
                disposed = true;
            }
        }

        // Act
        var result = GetSequence().AsValueEnumerable().JoinToString(",");

        // Assert
        disposed.ShouldBeTrue();
        result.ShouldBe("1,2,3");
    }

    [Fact]
    public void JoinToString_LargerThanStackallocLimit_WorksCorrectly()
    {
        // This test verifies that strings larger than the stackalloc buffer limit work correctly
        // Arrange
        var items = Enumerable.Range(1, 1000).ToArray(); // Much larger than the StackallocCharBufferSizeLimit
        var separator = ",";

        // Act - ZLinq
        var actual = items.AsValueEnumerable().JoinToString(separator);

        // Act - Standard approach for comparison
        var expected = string.Join(separator, items);

        // Assert
        actual.ShouldBe(expected);
    }

    [Fact]
    public void JoinToString_Enumerable_MultipleItem()
    {
        // Arrange
        var valueEnumerable = GetSource().AsValueEnumerable();

        // Char separator
        {
            // Act
            var actual = valueEnumerable.JoinToString(',');

            // Assert
            var expected = string.Join(",", GetSource());
            actual.ShouldBe(expected);
        }

        // Empty separator
        {
            // Act
            var actual = valueEnumerable.JoinToString("");

            // Assert
            var expected = string.Join("", GetSource());
            actual.ShouldBe(expected);
        }

        static IEnumerable<int> GetSource()
        {
            yield return 1;
            yield return 2;
            yield return 3;
        }
    }

    [Fact]
    public void JoinToString_Enumerable_SingleItem()
    {
        var valueEnumerable = GetSource().AsValueEnumerable();

        // Char separator
        {
            // Act
            var actual = valueEnumerable.JoinToString(',');

            // Assert
            var expected = string.Join(",", GetSource());
            actual.ShouldBe("1");
        }
        // Empty Separator
        {
            // Act
            var actual = valueEnumerable.JoinToString("");

            // Assert
            var expected = string.Join("", GetSource());
            actual.ShouldBe("1");
        }

        static IEnumerable<int> GetSource()
        {
            yield return 1;
        }
    }

    [Fact]
    public void JoinToString_Enumerable_EmptySource()
    {
        // Arrange
        var valueEnumerable = GetEmptySource().AsValueEnumerable();

        // Act
        var actual = valueEnumerable.JoinToString(',');

        // Assert
        actual.ShouldBe("");

        static IEnumerable<int> GetEmptySource()
        {
            yield break;
        }
    }

    private class Person
    {
        public string Name { get; set; } = "";
        public int Age { get; set; }

        public override string ToString() => $"{Name} ({Age})";
    }
}
