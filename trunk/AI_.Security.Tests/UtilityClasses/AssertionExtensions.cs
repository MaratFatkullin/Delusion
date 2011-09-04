using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace AI_.Security.Tests.UtilityClasses
{
    public static class AssertionExtensions
    {
        public static void Empty<T>(this ICollection<T> collection)
        {
            Assert.Equal(0, collection.Count);
        }

        public static void NotEmpty<T>(this ICollection<T> collection)
        {
            Assert.NotEqual(0, collection.Count);
        }

        public static void ShouldContainExactlyOneItem<T>(this ICollection<T> collection, T item)
        {
            Assert.Equal(1, collection.Count);
            Assert.Equal(collection.First(), item);
        }

        public static void ShouldContainExactlyCountItems<T>(this ICollection<T> collection, int count)
        {
            Assert.Equal(count, collection.Count);
        }
    }
}