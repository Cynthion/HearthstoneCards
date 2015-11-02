using System;
using System.Windows.Input;

namespace HearthstoneCards.CollectionViewEx
{
    public class SortCommand : ICommand
    {
        private readonly ICollectionViewEx _cv;

        public SortCommand(ICollectionViewEx cv)
        {
            _cv = cv;
        }

        /// <summary>
        /// Expects a parameter of type <see cref="SortDescription"/>.
        /// </summary>
        /// <param name="parameter">The <see cref="SortDescription"/> option to be checked for execution.</param>
        public bool CanExecute(object parameter)
        {
            var sd = parameter as SortDescription;
            if (_cv == null || sd == null)
            {
                return false;
            }

            // check if already sorted by the provided description
            if (_cv.SortDescriptions.Count > 0)
            {
                return _cv.SortDescriptions[0].Equals(sd);
            }

            return true;
        }

        /// <summary>
        /// Expects a parameter of type <see cref="SortDescription"/>.
        /// </summary>
        /// <param name="parameter">The <see cref="SortDescription"/> option to be executed.</param>
        public void Execute(object parameter)
        {
            var sd = parameter as SortDescription;
            if (_cv != null && sd != null)
            {
                using (_cv.DeferRefresh())
                {
                    _cv.SortDescriptions.Clear();
                    _cv.SortDescriptions.Add(sd);
                }
            }
        }

        public event EventHandler CanExecuteChanged;
    }
}
