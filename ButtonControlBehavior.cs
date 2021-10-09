using Microsoft.Xaml.Behaviors;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

// 参考
// https://qiita.com/soi/items/94b652850e5ad5394330
namespace TextConvert
{
    /// <summary>
    /// Button に対するクリック動作のビヘイビア
    /// </summary>
    internal class ButtonControlBehavior
    {


        /// <summary>
        /// 指定されたオブジェクトを含む行を親のItemsControlから削除する
        /// </summary>
        public static void RemoveItemFromParent(DependencyObject elementInItem)
        {
            DependencyObject parent = elementInItem;
            var parentTree = new List<DependencyObject> { parent };

            //指定されたオブジェクトのVisualTree上の親を順番に探索し、ItemsControlを探す。
            //ただし、DataGridは中間にいるDataGridCellsPresenterは無視する
            while (parent != null && !(parent is ItemsControl) || parent is DataGridCellsPresenter)
            {
                parent = VisualTreeHelper.GetParent(parent);
                parentTree.Add(parent);
            }
            if (!(parent is ItemsControl itemsControl))
                return;

            //ItemsControlの行にあたるオブジェクトを探索履歴の後ろから検索
            var item = parentTree
                .LastOrDefault(x => itemsControl.IsItemItsOwnContainer(x));

            int? removeIndex = itemsControl.ItemContainerGenerator?.IndexFromContainer(item);

            if (removeIndex == null || removeIndex < 0)
                return;

            //Bindingしていた場合はItemsSource、違うならItemsから削除する
            IEnumerable targetList = (itemsControl.ItemsSource ?? itemsControl.Items);

            switch (targetList)
            {
                case IList il:
                    il.RemoveAt((int)removeIndex);
                    return;
                case IEditableCollectionView iECV:
                    iECV.RemoveAt((int)removeIndex);
                    return;
            }
        }





        #region RemoveItem添付プロパティ
        public static bool GetRemoveItem(DependencyObject obj) => (bool)obj.GetValue(RemoveItemProperty);
        public static void SetRemoveItem(DependencyObject obj, bool value) => obj.SetValue(RemoveItemProperty, value);
        public static readonly DependencyProperty RemoveItemProperty = DependencyProperty.RegisterAttached(
            "RemoveItem",
            typeof(bool),
            typeof(ButtonControlBehavior),
            new PropertyMetadata(default(bool), OnRemoveItemChanged)
            );

        private static void OnRemoveItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is ButtonBase button))
                return;

            if (!(e.NewValue is bool b))
                return;

            if (b)
                button.Click += RemoveItem;
            else
                button.Click -= RemoveItem;
        }
        private static void RemoveItem(object sender, RoutedEventArgs e) => RemoveItemFromParent(sender as DependencyObject);
        #endregion
    }
}
