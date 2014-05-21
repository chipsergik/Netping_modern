using System;
using System.Collections.Generic;
using NetPing.Models;
using WebGrease.Css.Extensions;

namespace NetPing_modern.DAL
{
    internal class SPTermComparerByCustomSortOrder : IComparer<SPTerm>
    {
        private readonly List<Guid> _sortOrder = null;

        public SPTermComparerByCustomSortOrder(string customSortOrder)
        {
            if (string.IsNullOrEmpty(customSortOrder)) 
                return;

            _sortOrder = new List<Guid>();
            customSortOrder.Split(':').ForEach(id => _sortOrder.Add(new Guid(id)));
        }

        private int IndexOf(List<Guid> list, Guid id)
        {
            return list.IndexOf(id) + 1;
        }

        public int Compare(SPTerm x, SPTerm y)
        {
            if (_sortOrder == null)
                return 0;

            return IndexOf(_sortOrder, x.Id).CompareTo(IndexOf(_sortOrder, y.Id));
        }
    }
}