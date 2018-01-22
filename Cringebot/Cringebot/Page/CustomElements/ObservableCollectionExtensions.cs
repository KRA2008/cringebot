using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Cringebot.Page.CustomElements
{
    public static class ObservableCollectionExtensions
    {
        public static void Sort<T>(this ObservableCollection<T> collection, Comparison<T> comparison)
        {
            var sortableList = new List<T>(collection);
            sortableList.Sort(comparison);

            for (var i = 0; i < sortableList.Count; i++)
            {
                collection.Move(collection.IndexOf(sortableList[i]), i);
            }
        }

        public static void FilterButPreserve<T>(this ObservableCollection<T> collection, List<T> filtride, Predicate<T> predicate)
        {
            var inElementsGoingOut = new List<T>();
            foreach (var formerlyInElement in collection)
            {
                if(!predicate(formerlyInElement))
                {
                    inElementsGoingOut.Add(formerlyInElement);
                }
            }

            foreach (var outgoingElement in inElementsGoingOut)
            {
                collection.Remove(outgoingElement);
                filtride.Add(outgoingElement);
            }

            var outElementsComingIn = new List<T>();
            foreach(var formerlyOutElement in filtride)
            {
                if(predicate(formerlyOutElement))
                {
                    outElementsComingIn.Add(formerlyOutElement);
                }
            }

            foreach (var incomingElement in outElementsComingIn)
            {
                collection.Add(incomingElement);
                filtride.Remove(incomingElement);
            }
        }
    }
}
