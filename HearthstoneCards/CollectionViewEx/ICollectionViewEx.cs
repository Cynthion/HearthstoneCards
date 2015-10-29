using System;
using System.Collections;
using System.Collections.Generic;
using Windows.UI.Xaml.Data;

namespace HearthstoneCards.CollectionViewEx
{
    /// <summary>
    /// Extends the WinRT ICollectionView to provide sorting and filtering.
    /// </summary>
    public interface ICollectionViewEx : ICollectionView
    {
        bool CanFilter { get; }
        Predicate<object> Filter { get; set; }

        bool CanSort { get; }
        IList<SortDescription> SortDescriptions { get; }

        //bool CanGroup { get; }
        //IList<GroupDescription> GroupDescriptions { get; }
            
        IEnumerable SourceCollection { get; }

        IDisposable DeferRefresh();

        void Refresh();
    }
}
