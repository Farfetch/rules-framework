namespace Rules.Framework.Tests.Core
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using FluentAssertions;
    using Rules.Framework.Core;
    using Xunit;

    public class DictionarySlimTests
    {
        [Fact]
        public void AddByKeyAndValue_GivenExistentKey_AssignsNewValueToExistentKey()
        {
            // Arrange
            var key = "some-key";
            var value = "some-value";
            var newValue = "some-new-value";
            var size = 10;
            var dictionarySlim = new DictionarySlim<string, string>(size)
            {
                { key, value },
            };

            // Act
            var actual = () => dictionarySlim.Add(key, newValue);

            // Assert
            actual.Should().ThrowExactly<ArgumentException>()
                .Which.ParamName.Should().Be("key");
        }

        [Fact]
        public void AddByKeyAndValue_GivenKeyNull_ThrowsArgumentNullException()
        {
            // Arrange
            string key = null;
            var value = "some-value";
            var size = 10;
            var dictionarySlim = new DictionarySlim<string, string>(size);

            // Act
            var actual = () => dictionarySlim.Add(key, value);

            // Assert
            actual.Should().ThrowExactly<ArgumentNullException>()
                .Which.ParamName.Should().Be("key");
        }

        [Fact]
        public void AddByKeyAndValue_GivenNonExistentKey_AssignsNewKeyAndNewValue()
        {
            // Arrange
            var key = "some-key";
            var value = "some-value";
            var size = 10;
            var dictionarySlim = new DictionarySlim<string, string>(size);

            // Act
            dictionarySlim.Add(key, value);

            // Assert
            dictionarySlim.Keys.Should().Contain(key);
            dictionarySlim.Values.Should().Contain(value);
            dictionarySlim.Count.Should().Be(1);
        }

        [Fact]
        public void AddByKeyAndValue_GivenNonExistentKeyWhenOnSizeLimit_GrowsEntriesAndAssignsNewKeyAndNewValue()
        {
            // Arrange
            var key = "some-key";
            var value = "some-value";
            var size = 2;
            var dictionarySlim = new DictionarySlim<string, string>(size)
            {
                { "key1", "value1" },
                { "key2", "value2" },
            };

            // Act
            dictionarySlim.Add(key, value);

            // Assert
            dictionarySlim.Keys.Should().Contain(key);
            dictionarySlim.Values.Should().Contain(value);
            dictionarySlim.Count.Should().Be(3);
        }

        [Fact]
        public void AddByKeyValuePair_GivenExistentKey_AssignsNewValueToExistentKey()
        {
            // Arrange
            var keyValuePair = new KeyValuePair<string, string>("some-key", "some-value");
            var newKeyValuePair = new KeyValuePair<string, string>("some-key", "some-new-value");
            var size = 10;
            var dictionarySlim = new DictionarySlim<string, string>(size)
            {
                keyValuePair,
            };

            // Act
            var actual = () => dictionarySlim.Add(newKeyValuePair);

            // Assert
            actual.Should().ThrowExactly<ArgumentException>()
                .Which.ParamName.Should().Be("item");
        }

        [Fact]
        public void AddByKeyValuePair_GivenKeyNull_ThrowsArgumentNullException()
        {
            // Arrange
            var keyValuePair = new KeyValuePair<string, string>(null, "some-value");
            var size = 10;
            var dictionarySlim = new DictionarySlim<string, string>(size);

            // Act
            var actual = () => dictionarySlim.Add(keyValuePair);

            // Assert
            actual.Should().ThrowExactly<ArgumentException>()
                .Which.ParamName.Should().Be("item");
        }

        [Fact]
        public void AddByKeyValuePair_GivenNonExistentKey_AssignsNewKeyAndNewValue()
        {
            // Arrange
            var keyValuePair = new KeyValuePair<string, string>("some-key", "some-value");
            var size = 10;
            var dictionarySlim = new DictionarySlim<string, string>(size);

            // Act
            dictionarySlim.Add(keyValuePair);

            // Assert
            dictionarySlim.Keys.Should().Contain(keyValuePair.Key);
            dictionarySlim.Values.Should().Contain(keyValuePair.Value);
            dictionarySlim.Count.Should().Be(1);
        }

        [Fact]
        public void AddByKeyValuePair_GivenNonExistentKeyWhenOnSizeLimit_GrowsEntriesAndAssignsNewKeyAndNewValue()
        {
            // Arrange
            var keyValuePair = new KeyValuePair<string, string>("some-key", "some-value");
            var size = 2;
            var dictionarySlim = new DictionarySlim<string, string>(size)
            {
                { "key1", "value1" },
                { "key2", "value2" },
            };

            // Act
            dictionarySlim.Add(keyValuePair);

            // Assert
            dictionarySlim.Keys.Should().Contain(keyValuePair.Key);
            dictionarySlim.Values.Should().Contain(keyValuePair.Value);
            dictionarySlim.Count.Should().Be(3);
        }

        [Fact]
        public void Clear_WhenCalledOnDictionarySlimWith2Elements_RemovesElementsAndSetsCountToZero()
        {
            // Arrange
            var size = 2;
            var dictionarySlim = new DictionarySlim<string, string>(size)
            {
                { "key1", "value1" },
                { "key2", "value2" },
            };

            // Act
            dictionarySlim.Clear();

            // Assert
            dictionarySlim.Count.Should().Be(0);
            dictionarySlim.Keys.Should().BeEmpty();
            dictionarySlim.Values.Should().BeEmpty();
        }

        [Theory]
        [InlineData("key2", "value2", true)]
        [InlineData("key3", "value3", false)]
        [InlineData("key2", "value1", false)]
        public void Contains_GivenKeyValuePair_ReturnsBool(string key, string value, bool result)
        {
            // Arrange
            var keyValuePair = new KeyValuePair<string, string>(key, value);
            var size = 2;
            var dictionarySlim = new DictionarySlim<string, string>(size)
            {
                { "key1", "value1" },
                { "key2", "value2" },
            };

            // Act
            var actual = dictionarySlim.Contains(keyValuePair);

            // Assert
            actual.Should().Be(result);
        }

        [Theory]
        [InlineData("key2", true)]
        [InlineData("key3", false)]
        public void ContainsKey_GivenKey_ReturnsBool(string key, bool result)
        {
            // Arrange
            var size = 2;
            var dictionarySlim = new DictionarySlim<string, string>(size)
            {
                { "key1", "value1" },
                { "key2", "value2" },
            };

            // Act
            var actual = dictionarySlim.ContainsKey(key);

            // Assert
            actual.Should().Be(result);
        }

        [Fact]
        public void CopyTo_GivenArrayAndInitialIndexAndInsufficientLengthToHoldDictionarySlimElements_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            var size = 2;
            var dictionarySlim = new DictionarySlim<string, string>(size)
            {
                { "key1", "value1" },
                { "key2", "value2" },
            };
            var array = new KeyValuePair<string, string>[4];
            array[0] = new KeyValuePair<string, string>("another-key1", "another-value1");
            array[1] = new KeyValuePair<string, string>("another-key2", "another-value2");
            var arrayIndex = 3;

            // Act
            var actual = () => dictionarySlim.CopyTo(array, arrayIndex);

            // Assert
            actual.Should().ThrowExactly<ArgumentOutOfRangeException>()
                .WithParameterName("array");
        }

        [Fact]
        public void CopyTo_GivenArrayAndInitialIndexAndSufficientLengthToHoldDictionarySlimElements_CopiesAllElementsToArray()
        {
            // Arrange
            var size = 2;
            var dictionarySlim = new DictionarySlim<string, string>(size)
            {
                { "key1", "value1" },
                { "key2", "value2" },
            };
            var array = new KeyValuePair<string, string>[4];
            array[0] = new KeyValuePair<string, string>("another-key1", "another-value1");
            array[1] = new KeyValuePair<string, string>("another-key2", "another-value2");
            var arrayIndex = 2;

            // Act
            dictionarySlim.CopyTo(array, arrayIndex);

            // Assert
            var equivalentArray = new[]
            {
                new KeyValuePair<string, string>("another-key1", "another-value1"),
                new KeyValuePair<string, string>("another-key2", "another-value2"),
                new KeyValuePair<string, string>("key1", "value1"),
                new KeyValuePair<string, string>("key2", "value2"),
            };

            array.Should().BeEquivalentTo(equivalentArray);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(3)]
        [InlineData(4)]
        public void CopyTo_GivenArrayAndInitialIndexSmallerThanZero_ThrowsArgumentOutOfRangeException(int arrayIndex)
        {
            // Arrange
            var size = 2;
            var dictionarySlim = new DictionarySlim<string, string>(size)
            {
                { "key1", "value1" },
                { "key2", "value2" },
            };
            var array = new KeyValuePair<string, string>[3];
            array[0] = new KeyValuePair<string, string>("another-key1", "another-value1");
            array[1] = new KeyValuePair<string, string>("another-key2", "another-value2");

            // Act
            var actual = () => dictionarySlim.CopyTo(array, arrayIndex);

            // Assert
            actual.Should().ThrowExactly<ArgumentOutOfRangeException>()
                .WithParameterName("arrayIndex")
                .Which.ActualValue.Should().Be(arrayIndex);
        }

        [Fact]
        public void CopyTo_GivenArrayNullAndInitialIndex_ThrowsArgumentNullException()
        {
            // Arrange
            var size = 2;
            var dictionarySlim = new DictionarySlim<string, string>(size)
            {
                { "key1", "value1" },
                { "key2", "value2" },
            };
            KeyValuePair<string, string>[] array = null;
            var arrayIndex = 2;

            // Act
            var actual = () => dictionarySlim.CopyTo(array, arrayIndex);

            // Assert
            actual.Should().ThrowExactly<ArgumentNullException>()
                .WithParameterName("array");
        }

        [Fact]
        public void Ctor_GivenSizeGreaterThanZero_CreatesInstance()
        {
            // Arrange
            var size = 10;

            // Act
            var actual = new DictionarySlim<int, string>(size);

            // Assert
            actual.Should().NotBeNull();
            actual.Count.Should().Be(0);
            actual.IsReadOnly.Should().BeFalse();
            actual.Keys.Should().NotBeNull().And.BeEmpty();
            actual.Values.Should().NotBeNull().And.BeEmpty();
        }

        [Fact]
        public void Ctor_GivenSizeZero_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            var size = 0;

            // Act
            var actual = () => new DictionarySlim<int, string>(size);

            // Assert
            actual.Should().ThrowExactly<ArgumentOutOfRangeException>()
                .Which.ParamName.Should().Be("size");
        }

        [Fact]
        public void Ctor_GivenSourceDictionaryNull_ThrowsArgumentNullException()
        {
            IDictionary<int, string> sourceDictionary = null;

            // Act
            var actual = () => new DictionarySlim<int, string>(sourceDictionary);

            // Assert
            actual.Should().ThrowExactly<ArgumentNullException>()
                .Which.ParamName.Should().Be("source");
        }

        [Fact]
        public void Ctor_GivenSourceDictionarySlimWith2Keys_CreatesInstance()
        {
            // Arrange
            var source = new DictionarySlim<int, string>(2)
            {
                { 1, "test1" },
                { 2, "test2" },
            };

            // Act
            var actual = new DictionarySlim<int, string>(source);

            // Assert
            actual.Should().NotBeNull();
            actual.Count.Should().Be(2);
            actual.IsReadOnly.Should().BeFalse();
            actual.Keys.Should().NotBeNull().And.HaveCount(2);
            actual.Keys.Should().Contain(1);
            actual.Keys.Should().Contain(2);
            actual.Values.Should().NotBeNull().And.HaveCount(2);
            actual.Values.Should().Contain("test1");
            actual.Values.Should().Contain("test2");
        }

        [Fact]
        public void Ctor_GivenSourceDictionaryWith2Keys_CreatesInstance()
        {
            // Arrange
            var source = new Dictionary<int, string>
            {
                { 1, "test1" },
                { 2, "test2" },
            };

            // Act
            var actual = new DictionarySlim<int, string>(source);

            // Assert
            actual.Should().NotBeNull();
            actual.Count.Should().Be(2);
            actual.IsReadOnly.Should().BeFalse();
            actual.Keys.Should().NotBeNull().And.HaveCount(2);
            actual.Keys.Should().Contain(1);
            actual.Keys.Should().Contain(2);
            actual.Values.Should().NotBeNull().And.HaveCount(2);
            actual.Values.Should().Contain("test1");
            actual.Values.Should().Contain("test2");
        }

        [Fact]
        public void Enumerator_WhenCalledOnDictionarySlimWith2Elements_ReturnsEnumeratorAndEnumeratesEachOfThe2Elements()
        {
            // Arrange
            var size = 2;
            var dictionarySlim = new DictionarySlim<string, string>(size)
            {
                { "key1", "value1" },
                { "key2", "value2" },
            };
            var enumerable = (IEnumerable)dictionarySlim;

            // Act
            var enumerator = enumerable.GetEnumerator();
            var moveNext1 = enumerator.MoveNext();
            var element1 = enumerator.Current;
            var moveNext2 = enumerator.MoveNext();
            var element2 = enumerator.Current;
            var moveNext3 = enumerator.MoveNext();
            enumerator.Reset();
            var moveNext4 = enumerator.MoveNext();

            // Assert
            enumerator.Should().NotBeNull();
            element1.Should().NotBeNull()
                .And.BeOfType<KeyValuePair<string, string>>();
            var keyValuePair1 = (KeyValuePair<string, string>)element1;
            keyValuePair1.Key.Should().Be("key1");
            keyValuePair1.Value.Should().Be("value1");
            element2.Should().NotBeNull()
                .And.BeOfType<KeyValuePair<string, string>>();
            var keyValuePair2 = (KeyValuePair<string, string>)element2;
            keyValuePair2.Key.Should().Be("key2");
            keyValuePair2.Value.Should().Be("value2");
            moveNext1.Should().BeTrue();
            moveNext2.Should().BeTrue();
            moveNext3.Should().BeFalse();
            moveNext4.Should().BeTrue();
        }

        [Fact]
        public void EnumeratorGeneric_WhenCalledOnDictionarySlimWith2Elements_ReturnsEnumeratorAndEnumeratesEachOfThe2Elements()
        {
            // Arrange
            var size = 2;
            var dictionarySlim = new DictionarySlim<string, string>(size)
            {
                { "key1", "value1" },
                { "key2", "value2" },
            };

            // Act
            var enumerator = dictionarySlim.GetEnumerator();
            var moveNext1 = enumerator.MoveNext();
            var keyValuePair1 = enumerator.Current;
            var moveNext2 = enumerator.MoveNext();
            var keyValuePair2 = enumerator.Current;
            var moveNext3 = enumerator.MoveNext();
            enumerator.Reset();
            var moveNext4 = enumerator.MoveNext();

            // Assert
            enumerator.Should().NotBeNull();
            keyValuePair1.Key.Should().Be("key1");
            keyValuePair1.Value.Should().Be("value1");
            keyValuePair2.Key.Should().Be("key2");
            keyValuePair2.Value.Should().Be("value2");
            moveNext1.Should().BeTrue();
            moveNext2.Should().BeTrue();
            moveNext3.Should().BeFalse();
            moveNext4.Should().BeTrue();
        }

        [Fact]
        public void IndexerGet_GivenExistentKey_ReturnsValue()
        {
            // Arrange
            var key = "some-key";
            var value = "some-value";
            var size = 10;
            var dictionarySlim = new DictionarySlim<string, string>(size)
            {
                { key, value },
            };

            // Act
            var actual = dictionarySlim[key];

            // Assert
            actual.Should().Be(value);
        }

        [Fact]
        public void IndexerGet_GivenKeyNull_ThrowsArgumentNullException()
        {
            // Arrange
            string key = null;
            var size = 10;
            var dictionarySlim = new DictionarySlim<string, string>(size);

            // Act
            var actual = () => _ = dictionarySlim[key];

            // Assert
            actual.Should().ThrowExactly<ArgumentNullException>()
                .Which.ParamName.Should().Be("key");
        }

        [Fact]
        public void IndexerGet_GivenNonExistentKey_ThrowsKeyNotFoundException()
        {
            // Arrange
            var key = "some-key";
            var size = 10;
            var dictionarySlim = new DictionarySlim<string, string>(size);

            // Act
            var actual = () => _ = dictionarySlim[key];

            // Assert
            actual.Should().ThrowExactly<KeyNotFoundException>()
                .Which.Message.Should().Contain(key);
        }

        [Fact]
        public void IndexerSet_GivenExistentKey_AssignsNewValueToExistentKey()
        {
            // Arrange
            var key = "some-key";
            var value = "some-value";
            var newValue = "some-new-value";
            var size = 10;
            var dictionarySlim = new DictionarySlim<string, string>(size)
            {
                { key, value },
            };

            // Act
            dictionarySlim[key] = newValue;

            // Assert
            dictionarySlim.Keys.Should().Contain(key);
            dictionarySlim.Values.Should().Contain(newValue);
            dictionarySlim.Values.Should().NotContain(value);
            dictionarySlim.Count.Should().Be(1);
        }

        [Fact]
        public void IndexerSet_GivenKeyNull_ThrowsArgumentNullException()
        {
            // Arrange
            string key = null;
            var value = "some-value";
            var size = 10;
            var dictionarySlim = new DictionarySlim<string, string>(size);

            // Act
            var actual = () => dictionarySlim[key] = value;

            // Assert
            actual.Should().ThrowExactly<ArgumentNullException>()
                .Which.ParamName.Should().Be("key");
        }

        [Fact]
        public void IndexerSet_GivenNonExistentKey_AssignsNewKeyAndNewValue()
        {
            // Arrange
            var key = "some-key";
            var value = "some-value";
            var size = 10;
            var dictionarySlim = new DictionarySlim<string, string>(size);

            // Act
            dictionarySlim[key] = value;

            // Assert
            dictionarySlim.Keys.Should().Contain(key);
            dictionarySlim.Values.Should().Contain(value);
            dictionarySlim.Count.Should().Be(1);
        }

        [Fact]
        public void IndexerSet_GivenNonExistentKeyWhenOnSizeLimit_GrowsEntriesAndAssignsNewKeyAndNewValue()
        {
            // Arrange
            var key = "some-key";
            var value = "some-value";
            var size = 2;
            var dictionarySlim = new DictionarySlim<string, string>(size)
            {
                { "key1", "value1" },
                { "key2", "value2" },
            };

            // Act
            dictionarySlim[key] = value;

            // Assert
            dictionarySlim.Keys.Should().Contain(key);
            dictionarySlim.Values.Should().Contain(value);
            dictionarySlim.Count.Should().Be(3);
        }

        [Fact]
        public void RemoveByKey_GivenExistentKey_RemovesAndReturnsTrue()
        {
            // Arrange
            var key = "some-key";
            var value = "some-value";
            var size = 10;
            var dictionarySlim = new DictionarySlim<string, string>(size)
            {
                { "key1", "value1" },
                { key, value },
                { "key2", "value2" },
            };

            // Act
            var actual = dictionarySlim.Remove(key);

            // Assert
            actual.Should().BeTrue();
            dictionarySlim.Keys.Should().NotContain(key);
            dictionarySlim.Values.Should().NotContain(value);
            dictionarySlim.Count.Should().Be(2);
        }

        [Fact]
        public void RemoveByKey_GivenKeyNull_ThrowsArgumentNullException()
        {
            // Arrange
            string key = null;
            var size = 10;
            var dictionarySlim = new DictionarySlim<string, string>(size);

            // Act
            var actual = () => dictionarySlim.Remove(key);

            // Assert
            actual.Should().ThrowExactly<ArgumentNullException>()
                .Which.ParamName.Should().Be("key");
        }

        [Fact]
        public void RemoveByKey_GivenNonExistentKey_ReturnsFalse()
        {
            // Arrange
            var key = "some-key";
            var value = "some-value";
            var size = 10;
            var dictionarySlim = new DictionarySlim<string, string>(size)
            {
                { "key1", "value1" },
                { "key2", "value2" },
            };

            // Act
            var actual = dictionarySlim.Remove(key);

            // Assert
            actual.Should().BeFalse();
            dictionarySlim.Keys.Should().NotContain(key);
            dictionarySlim.Values.Should().NotContain(value);
            dictionarySlim.Count.Should().Be(2);
        }

        [Fact]
        public void RemoveByKeyValuePair_GivenExistentKey_RemovesAndReturnsTrue()
        {
            // Arrange
            var keyValuePair = new KeyValuePair<string, string>("some-key", "some-value");
            var size = 10;
            var dictionarySlim = new DictionarySlim<string, string>(size)
            {
                { "key1", "value1" },
                { keyValuePair.Key, keyValuePair.Value },
                { "key2", "value2" },
            };

            // Act
            var actual = dictionarySlim.Remove(keyValuePair);

            // Assert
            actual.Should().BeTrue();
            dictionarySlim.Keys.Should().NotContain(keyValuePair.Key);
            dictionarySlim.Values.Should().NotContain(keyValuePair.Value);
            dictionarySlim.Count.Should().Be(2);
        }

        [Fact]
        public void RemoveByKeyValuePair_GivenKeyNull_ThrowsArgumentNullException()
        {
            // Arrange
            var keyValuePair = new KeyValuePair<string, string>(null, "some-value");
            var size = 10;
            var dictionarySlim = new DictionarySlim<string, string>(size);

            // Act
            var actual = () => dictionarySlim.Remove(keyValuePair);

            // Assert
            actual.Should().ThrowExactly<ArgumentException>()
                .Which.ParamName.Should().Be("item");
        }

        [Fact]
        public void RemoveByKeyValuePair_GivenNonExistentKey_ReturnsFalse()
        {
            // Arrange
            var keyValuePair = new KeyValuePair<string, string>("some-key", "some-value");
            var size = 10;
            var dictionarySlim = new DictionarySlim<string, string>(size)
            {
                { "key1", "value1" },
                { "key2", "value2" },
            };

            // Act
            var actual = dictionarySlim.Remove(keyValuePair);

            // Assert
            actual.Should().BeFalse();
            dictionarySlim.Keys.Should().NotContain(keyValuePair.Key);
            dictionarySlim.Values.Should().NotContain(keyValuePair.Value);
            dictionarySlim.Count.Should().Be(2);
        }

        [Fact]
        public void TryGetValue_GivenExistentKey_ReturnsValue()
        {
            // Arrange
            var key = "some-key";
            var value = "some-value";
            var size = 10;
            var dictionarySlim = new DictionarySlim<string, string>(size)
            {
                { key, value },
            };

            // Act
            var actual = dictionarySlim.TryGetValue(key, out var result);

            // Assert
            actual.Should().BeTrue();
            result.Should().Be(value);
        }

        [Fact]
        public void TryGetValue_GivenKeyNull_ThrowsArgumentNullException()
        {
            // Arrange
            string key = null;
            var size = 10;
            var dictionarySlim = new DictionarySlim<string, string>(size);

            // Act
            var actual = () => _ = dictionarySlim.TryGetValue(key, out var _);

            // Assert
            actual.Should().ThrowExactly<ArgumentNullException>()
                .Which.ParamName.Should().Be("key");
        }

        [Fact]
        public void TryGetValue_GivenNonExistentKey_ThrowsKeyNotFoundException()
        {
            // Arrange
            var key = "some-key";
            var size = 10;
            var dictionarySlim = new DictionarySlim<string, string>(size);

            // Act
            var actual = dictionarySlim.TryGetValue(key, out var result);

            // Assert
            actual.Should().BeFalse();
            result.Should().Be(default);
        }
    }
}