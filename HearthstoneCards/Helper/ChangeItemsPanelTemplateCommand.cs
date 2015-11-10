using System;
using System.Windows.Input;
using Windows.UI.Xaml.Controls;

namespace HearthstoneCards.Helper
{
    /// <summary>
    /// A command changing the <see cref="ItemsPanelTemplate"/> of an <see cref="ItemsControlViewInfo"/> object.
    /// </summary>
    public class ChangeItemsPanelTemplateCommand : ICommand
    {
        private readonly ItemsControlViewInfo _viewInfo;

        public ChangeItemsPanelTemplateCommand(ItemsControlViewInfo viewInfo)
        {
            _viewInfo = viewInfo;
        }

        public bool CanExecute(object parameter)
        {
            var ipt = parameter as ItemsPanelTemplate;
            return _viewInfo != null && ipt != null;
        }

        public void Execute(object parameter)
        {
            var ipt = parameter as ItemsPanelTemplate;
            if (_viewInfo != null && ipt != null)
            {
                _viewInfo.ItemsPanelTemplate = ipt;
            }
        }

        // TODO implement usage
        public event EventHandler CanExecuteChanged;
    }
}
