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

        private static IEnumerable<Block> GetBlocks(string html)
        {
            var blocks = new List<Block> {new Block(html, SpanType.Normal)};

            SplitBlocks(blocks, new []{ "<b>", "</b>"}, SpanType.Bold);
            SplitBlocks(blocks, new []{ "<i>", "</i>"}, SpanType.Italic);

            return blocks.Where(b => b.Text.Length > 0).ToList();
        }

        /// <summary>
        /// Processes all provided blocks based on the separators and span type.
        /// <see cref="blocks"/> must at least contain one element.
        /// </summary>
        private static void SplitBlocks(IList<Block> blocks, string[] separators, SpanType spanType)
        {
            for (var i = 0; i < blocks.Count; i++)
            {
                var splitText = blocks[i].Text.Split(separators.ToArray(), StringSplitOptions.None);
                if (splitText.Length > 1)
                {
                    // remove old block
                    var oldBlock = blocks[i];
                    blocks.RemoveAt(i);

                    // add new blocks
                    for (var j = 0; j < splitText.Length; j++)
                    {
                        var block = new Block(splitText[j], j % 2 == 1 ? spanType : oldBlock.SpanType);

                        // insert and skip block in outer loop
                        blocks.Insert(i++, block);
                    }
                }
            }
        }
    }
}
