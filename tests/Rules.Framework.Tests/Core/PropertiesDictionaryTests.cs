namespace Rules.Framework.Tests.Core
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using FluentAssertions;
    using Rules.Framework.Core;
    using Xunit;

    public class PropertiesDictionaryTests
    {
        [Fact]
        public void AddByKeyAndValue_GivenKeyAndDictionarySlimLimitNotReached_AssignsValueToKey()
        {
            // Arrange
            var key = "some-key";
            var value = "some-value";
            var size = 10;
            var dictionarySlim = new DictionarySlim<string, object>(size);
            var propertiesDictionary = new PropertiesDictionary(dictionarySlim);

            // Act
            propertiesDictionary.Add(key, value);

            // Assert
            propertiesDictionary.Keys.Should().Contain(key);
            propertiesDictionary.Values.Should().Contain(value);
            propertiesDictionary.Count.Should().Be(1);
        }

        [Fact]
        public void AddByKeyAndValue_GivenKeyAndDictionarySlimLimitReached_ChangesToDictionaryAndAssignsValueToKey()
        {
            // Arrange Arrange
            var key = "some-key";
            var value = "some-value";
            var size = 5;
            var dictionarySlim = new DictionarySlim<string, object>(size);
            for (int i = 0; i < size; i++)
            {
                dictionarySlim[$"key{i}"] = $"value{i}";
            }
            var propertiesDictionary = new PropertiesDictionary(dictionarySlim);

            // Act
            propertiesDictionary.Add(key, value);

            // Assert
            propertiesDictionary.Keys.Should().Contain(key);
            propertiesDictionary.Values.Should().Contain(value);
            propertiesDictionary.Count.Should().Be(6);
        }

        [Fact]
        public void AddByKeyValuePair_GivenKeyAndDictionarySlimLimitNotReached_AssignsValueToKey()
        {
            // Arrange
            var keyValuePair = new KeyValuePair<string, object>("some-key", "some-value");
            var size = 10;
            var dictionarySlim = new DictionarySlim<string, object>(size);
            var propertiesDictionary = new PropertiesDictionary(dictionarySlim);

            // Act
            propertiesDictionary.Add(keyValuePair);

            // Assert
            propertiesDictionary.Keys.Should().Contain(keyValuePair.Key);
            propertiesDictionary.Values.Should().Contain(keyValuePair.Value);
            propertiesDictionary.Count.Should().Be(1);
        }

        [Fact]
        public void AddByKeyValuePair_GivenKeyAndDictionarySlimLimitReached_ChangesToDictionaryAndAssignsValueToKey()
        {
            // Arrange Arrange
            var keyValuePair = new KeyValuePair<string, object>("some-key", "some-value");
            var size = 5;
            var dictionarySlim = new DictionarySlim<string, object>(size);
            for (int i = 0; i < size; i++)
            {
                dictionarySlim[$"key{i}"] = $"value{i}";
            }
            var propertiesDictionary = new PropertiesDictionary(dictionarySlim);

            // Act
            propertiesDictionary.Add(keyValuePair);

            // Assert
            propertiesDictionary.Keys.Should().Contain(keyValuePair.Key);
            propertiesDictionary.Values.Should().Contain(keyValuePair.Value);
            propertiesDictionary.Count.Should().Be(6);
        }

        [Fact]
        public void Clear_WhenCalledOnPropertiesDictionaryWith2Elements_RemovesElementsAndSetsCountToZero()
        {
            // Arrange
            var size = 2;
            var propertiesDictionary = new PropertiesDictionary(size)
            {
                { "key1", "value1" },
                { "key2", "value2" },
            };

            // Act
            propertiesDictionary.Clear();

            // Assert
            propertiesDictionary.Count.Should().Be(0);
            propertiesDictionary.Keys.Should().BeEmpty();
            propertiesDictionary.Values.Should().BeEmpty();
        }

        [Fact]
        public void Contains_GivenKeyValuePair_ReturnsBool()
        {
            // Arrange
            var keyValuePair = new KeyValuePair<string, object>("key2", "value2");
            var size = 2;
            var propertiesDictionary = new PropertiesDictionary(size)
            {
                { "key1", "value1" },
                { "key2", "value2" },
            };

            // Act
            var actual = propertiesDictionary.Contains(keyValuePair);

            // Assert
            actual.Should().BeTrue();
        }

        [Fact]
        public void ContainsKey_GivenKey_ReturnsBool()
        {
            // Arrange
            string key = "key1";
            var size = 2;
            var propertiesDictionary = new PropertiesDictionary(size)
            {
                { "key1", "value1" },
                { "key2", "value2" },
            };

            // Act
            var actual = propertiesDictionary.ContainsKey(key);

            // Assert
            actual.Should().BeTrue();
        }

        [Fact]
        public void CopyTo_GivenArrayAndInitialIndexAndSufficientLengthToHoldDictionarySlimElements_CopiesAllElementsToArray()
        {
            // Arrange
            var size = 2;
            var dictionarySlim = new DictionarySlim<string, object>(size)
            {
                { "key1", "value1" },
                { "key2", "value2" },
            };
            var propertiesDictionary = new PropertiesDictionary(dictionarySlim);
            var array = new KeyValuePair<string, object>[4];
            array[0] = new KeyValuePair<string, object>("another-key1", "another-value1");
            array[1] = new KeyValuePair<string, object>("another-key2", "another-value2");
            var arrayIndex = 2;

            // Act
            propertiesDictionary.CopyTo(array, arrayIndex);

            // Assert
            var equivalentArray = new[]
            {
                new KeyValuePair<string, object>("another-key1", "another-value1"),
                new KeyValuePair<string, object>("another-key2", "another-value2"),
                new KeyValuePair<string, object>("key1", "value1"),
                new KeyValuePair<string, object>("key2", "value2"),
            };

            array.Should().BeEquivalentTo(equivalentArray);
        }

        [Fact]
        public void Ctor_GivenSizeGreaterThanZero_CreatesInstance()
        {
            // Arrange
            var size = 10;

            // Act
            var actual = new PropertiesDictionary(size);

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
            var actual = () => new PropertiesDictionary(size);

            // Assert
            actual.Should().ThrowExactly<ArgumentOutOfRangeException>()
                .Which.ParamName.Should().Be("size");
        }

        [Fact]
        public void Ctor_GivenSourceDictionaryEmpty_CreatesInstance()
        {
            // Arrange
            var source = new DictionarySlim<string, object>(2);

            // Act
            var actual = new PropertiesDictionary(source);

            // Assert
            actual.Should().NotBeNull();
            actual.Count.Should().Be(0);
            actual.IsReadOnly.Should().BeFalse();
            actual.Keys.Should().NotBeNull().And.HaveCount(0);
            actual.Values.Should().NotBeNull().And.HaveCount(0);
        }

        [Fact]
        public void Ctor_GivenSourceDictionaryNull_ThrowsArgumentNullException()
        {
            IDictionary<string, object> sourceDictionary = null;

            // Act
            var actual = () => new PropertiesDictionary(sourceDictionary);

            // Assert
            actual.Should().ThrowExactly<ArgumentNullException>()
                .Which.ParamName.Should().Be("source");
        }

        [Fact]
        public void Ctor_GivenSourceDictionaryWith2Keys_CreatesInstance()
        {
            // Arrange
            var source = new Dictionary<string, object>
            {
                { "key1", "test1" },
                { "key2", "test2" },
            };

            // Act
            var actual = new PropertiesDictionary(source);

            // Assert
            actual.Should().NotBeNull();
            actual.Count.Should().Be(2);
            actual.IsReadOnly.Should().BeFalse();
            actual.Keys.Should().NotBeNull().And.HaveCount(2);
            actual.Keys.Should().Contain("key1");
            actual.Keys.Should().Contain("key2");
            actual.Values.Should().NotBeNull().And.HaveCount(2);
            actual.Values.Should().Contain("test1");
            actual.Values.Should().Contain("test2");
        }

        [Fact]
        public void Ctor_GivenSourceDictionaryWithMoreKeysThanDictionarySlimLimit_CreatesInstance()
        {
            // Arrange
            var iterations = 6;
            var source = new Dictionary<string, object>();

            for (int i = 0; i < iterations; i++)
            {
                source[$"key{i}"] = $"value{i}";
            }

            // Act
            var actual = new PropertiesDictionary(source);

            // Assert
            actual.Should().NotBeNull();
            actual.Count.Should().Be(iterations);
            actual.IsReadOnly.Should().BeFalse();
            actual.Keys.Should().NotBeNull().And.HaveCount(iterations);
            actual.Values.Should().NotBeNull().And.HaveCount(iterations);
            for (int i = 0; i < iterations; i++)
            {
                actual.Keys.Should().Contain($"key{i}");
                actual.Values.Should().Contain($"value{i}");
            }
        }

        [Fact]
        public void Enumerator_NoConditions_ReturnsEnumerator()
        {
            // Arrange
            var size = 2;
            var dictionarySlim = new DictionarySlim<string, object>(size)
            {
                { "key1", "value1" },
                { "key2", "value2" },
            };
            var propertiesDictionary = new PropertiesDictionary(dictionarySlim);
            var enumerable = propertiesDictionary as IEnumerable;

            // Act
            var enumerator = enumerable!.GetEnumerator();

            // Assert
            enumerator.Should().NotBeNull();
        }

        [Fact]
        public void EnumeratorGeneric_NoConditions_ReturnsEnumerator()
        {
            // Arrange
            var size = 2;
            var dictionarySlim = new DictionarySlim<string, object>(size)
            {
                { "key1", "value1" },
                { "key2", "value2" },
            };
            var propertiesDictionary = new PropertiesDictionary(dictionarySlim);

            // Act
            var enumerator = propertiesDictionary.GetEnumerator();

            // Assert
            enumerator.Should().NotBeNull();
        }

        [Fact]
        public void IndexerGet_GivenKey_ReturnsValue()
        {
            // Arrange
            var key = "some-key";
            var value = "some-value";
            var size = 10;
            var dictionarySlim = new DictionarySlim<string, object>(size)
            {
                { key, value },
            };
            var propertiesDictionary = new PropertiesDictionary(dictionarySlim);

            // Act
            var actual = propertiesDictionary[key];

            // Assert
            actual.Should().Be(value);
        }

        [Fact]
        public void IndexerSet_GivenKeyAndDictionarySlimLimitNotReached_AssignsValueToKey()
        {
            // Arrange
            var key = "some-key";
            var value = "some-value";
            var newValue = "some-new-value";
            var size = 10;
            var dictionarySlim = new DictionarySlim<string, object>(size)
            {
                { key, value },
            };
            var propertiesDictionary = new PropertiesDictionary(dictionarySlim);

            // Act
            propertiesDictionary[key] = newValue;

            // Assert
            propertiesDictionary.Keys.Should().Contain(key);
            propertiesDictionary.Values.Should().Contain(newValue);
            propertiesDictionary.Values.Should().NotContain(value);
            propertiesDictionary.Count.Should().Be(1);
        }

        [Fact]
        public void IndexerSet_GivenKeyAndDictionarySlimLimitReached_ChangesToDictionaryAndAssignsValueToKey()
        {
            // Arrange Arrange
            var key = "some-key";
            var value = "some-value";
            var size = 5;
            var dictionarySlim = new DictionarySlim<string, object>(size);
            for (int i = 0; i < size; i++)
            {
                dictionarySlim[$"key{i}"] = $"value{i}";
            }
            var propertiesDictionary = new PropertiesDictionary(dictionarySlim);

            // Act
            propertiesDictionary[key] = value;

            // Assert
            propertiesDictionary.Keys.Should().Contain(key);
            propertiesDictionary.Values.Should().Contain(value);
            propertiesDictionary.Count.Should().Be(6);
        }

        [Fact]
        public void RemoveByKey_GivenKey_RemovesAndReturnsTrue()
        {
            // Arrange
            var key = "some-key";
            var value = "some-value";
            var size = 10;
            var dictionarySlim = new DictionarySlim<string, object>(size)
            {
                { "key1", "value1" },
                { key, value },
                { "key2", "value2" },
            };
            var propertiesDictionary = new PropertiesDictionary(dictionarySlim);

            // Act
            var actual = propertiesDictionary.Remove(key);

            // Assert
            actual.Should().BeTrue();
            propertiesDictionary.Keys.Should().NotContain(key);
            propertiesDictionary.Values.Should().NotContain(value);
            propertiesDictionary.Count.Should().Be(2);
        }

        [Fact]
        public void RemoveByKeyValuePair_GivenKey_RemovesAndReturnsTrue()
        {
            // Arrange
            var keyValuePair = new KeyValuePair<string, object>("some-key", "some-value");
            var size = 10;
            var dictionarySlim = new DictionarySlim<string, object>(size)
            {
                { "key1", "value1" },
                { keyValuePair.Key, keyValuePair.Value },
                { "key2", "value2" },
            };
            var propertiesDictionary = new PropertiesDictionary(dictionarySlim);

            // Act
            var actual = propertiesDictionary.Remove(keyValuePair);

            // Assert
            actual.Should().BeTrue();
            propertiesDictionary.Keys.Should().NotContain(keyValuePair.Key);
            propertiesDictionary.Values.Should().NotContain(keyValuePair.Value);
            propertiesDictionary.Count.Should().Be(2);
        }

        [Fact]
        public void TryGetValue_GivenKey_ReturnsValue()
        {
            // Arrange
            var key = "some-key";
            var value = "some-value";
            var size = 10;
            var dictionarySlim = new DictionarySlim<string, object>(size)
            {
                { key, value },
            };
            var propertiesDictionary = new PropertiesDictionary(dictionarySlim);

            // Act
            var actual = propertiesDictionary.TryGetValue(key, out var result);

            // Assert
            actual.Should().BeTrue();
            result.Should().Be(value);
        }
    }
}