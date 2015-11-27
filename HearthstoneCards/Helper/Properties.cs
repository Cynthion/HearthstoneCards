using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace HearthstoneCards.Helper
{
    public class Properties : DependencyObject
    {
        public static readonly DependencyProperty HtmlProperty
            = DependencyProperty.RegisterAttached("Html", typeof(string), typeof(Properties), new PropertyMetadata(null, Html_Changed));

        //public string Html
        //{
        //    get { return (string)GetValue(HtmlProperty); }
        //    set { SetValue(HtmlProperty, value); }
        //}

        public static void SetHtml(DependencyObject obj, string html)
        {
            obj.SetValue(HtmlProperty, html);
        }

        public static string GetHtml(DependencyObject obj)
        {
            return (string)obj.GetValue(HtmlProperty);
        }

        private static void Html_Changed(DependencyObject dObj, DependencyPropertyChangedEventArgs args)
        {
            var tb = dObj as TextBlock;
            if (tb == null)
            {
                return;
            }
            if (args.NewValue == null)
            {
                tb.Inlines.Clear();
                return;
            }

            tb.Inlines.Clear();
            var html = RemoveSymbols(args.NewValue as string);
            var blocks = GetBlocks(html);
            foreach (var block in blocks)
            {
                tb.Inlines.Add(block.CreateInline());
            }
        }

        private static string RemoveSymbols(string html)
        {
            return html.Replace("$", string.Empty).Replace("#", string.Empty);
        }

        private static IList<Block> GetBlocks(string html)
        {
            var blocks = new List<Block>();

            // bold
            // | |
            var splitText = html.Split(new[] { "<b>", "</b>" }, StringSplitOptions.None);
            for (var i = 0; i < splitText.Length; i++)
            {
                var block = new Block(splitText[i], i % 2 == 1 ? SpanType.Bold : SpanType.Normal);
                blocks.Add(block);
            }
            // | |b| |b(i)|n(i)|

            // italic
            for (var i = 0; i < blocks.Count; i++)
            {
                splitText = blocks[i].Text.Split(new[] {"<i>", "</i>"}, StringSplitOptions.None);

                if (splitText.Length > 1)
                {
                    // remove old block
                    var oldBlock = blocks[i];
                    blocks.RemoveAt(i);
                    
                    // add new blocks
                    for (var j = 0; j < splitText.Length; j++)
                    {
                        var newBlock = new Block(splitText[j], j % 2 == 1 ? SpanType.Italic : oldBlock.SpanType);
                        
                        // insert and skip block in outer loop
                        blocks.Insert(i++, newBlock);
                    }
                }
            }

            return blocks.Where(b => b.Text.Length > 0).ToList();
        }

    }
}
