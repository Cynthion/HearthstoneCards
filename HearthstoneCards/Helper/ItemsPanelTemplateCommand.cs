using System;
using System.Windows.Input;
using Windows.UI.Xaml.Controls;

namespace HearthstoneCards.Helper
{
    public class ItemsPanelTemplateCommand : ICommand
    {
        private readonly ItemsControl _itemsControl;

        public ItemsPanelTemplateCommand(ItemsControl itemsControl)
        {
            _itemsControl = itemsControl;
        }

        public bool CanExecute(object parameter)
        {
            var ipt = parameter as ItemsPanelTemplate;
            return _itemsControl != null && ipt != null;
        }

        public void Execute(object parameter)
        {
            var ipt = parameter as ItemsPanelTemplate;
            if (_itemsControl != null && ipt != null)
            {
                _itemsControl.ItemsPanel = ipt;
            }
        }

        // TODO implement usage
        public event EventHandler CanExecuteChanged;
    }
}
