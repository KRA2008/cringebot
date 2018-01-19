using NUnit.Framework;
using System.Collections.ObjectModel;
using Cringebot.Page.CustomElements;
using System.Collections.Generic;
using System;
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
                var maxValue = 10;
                
                for (int i = maxValue; i >= 0; i--)
                {
                    observableCollection.Add(i);
                }

                // Assert the collection is in reverse mode
                for (int i = maxValue; i >= 0; i--)
                {
                    Assert.AreEqual(i, observableCollection[maxValue - i]);
                }
                
                //act
                observableCollection.Sort((a, b) => { return a.CompareTo(b); });

                //assert
                for (int i = 0; i < maxValue; i++)
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

                Predicate<int> isOddPredicate = delegate (int a) { return a % 2 != 0; };

                //act
                allNumbers.FilterButPreserve(evenNumbers, isOddPredicate);

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
