using System.Collections.Generic;
using System.Collections.ObjectModel;
using Cringebot.CustomElements;
using NUnit.Framework;
using SharpTestsEx;

namespace Cringebot.Tests.Page.CustomElements
{
    public class ObservableCollectionExtensionsTests
    {
        public class SortMethod : ObservableCollectionExtensionsTests
        {
            [Test]
            public void ShouldSort()
            {
                //arrange
                var observableCollection = new ObservableCollection<int>();
                const int MAX_VALUE = 10;
                
                for (var i = MAX_VALUE; i >= 0; i--)
                {
                    observableCollection.Add(i);
                }

                // Assert the collection is in reverse mode
                for (var i = MAX_VALUE; i >= 0; i--)
                {
                    Assert.AreEqual(i, observableCollection[MAX_VALUE - i]);
                }
                
                //act
                observableCollection.Sort((a, b) => a.CompareTo(b));

                //assert
                for (var i = 0; i < MAX_VALUE; i++)
                {
                    Assert.AreEqual(i, observableCollection[i]);
                }
            }
        }

        public class FilterButPreserveMethod : ObservableCollectionExtensionsTests
        {
            [Test]
            public void ShouldFilterAndDepositFiltrideInList()
            {
                //arrange
                var allNumbers = new ObservableCollection<int>
                {
                    1,
                    2,
                    3
                };
                var evenNumbers = new List<int>
                {
                    4,
                    5
                };

                bool IsOddPredicate(int a) => a % 2 != 0;

                //act
                allNumbers.FilterButPreserve(evenNumbers, IsOddPredicate);

                //assert
                allNumbers.Should().Contain(1);
                allNumbers.Should().Contain(3);
                allNumbers.Should().Contain(5);
                allNumbers.Should().Not.Contain(2);
                allNumbers.Should().Not.Contain(4);
                evenNumbers.Should().Contain(2);
                evenNumbers.Should().Contain(4);
            }
        }
    }
}
