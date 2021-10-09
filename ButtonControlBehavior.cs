using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace TextConvert
{
    /// <summary>
    /// Button に対するクリック動作のビヘイビア
    /// </summary>
    internal class ButtonControlBehavior
    {


        #region Callback 添付プロパティ

        /// <summary>
        /// Callback 添付プロパティの定義
        /// </summary>
        public static readonly DependencyProperty CallbackProperty = DependencyProperty.RegisterAttached(
            "Callback",
            typeof(Action<int>),
            typeof(ButtonControlBehavior),
            new PropertyMetadata(null, OnCallbackPropertyChanged)
            );

        /// <summary>
        /// Callback 添付プロパティを取得します。
        /// </summary>
        /// <param name="target">対象とする DependencyObject を指定します。</param>
        /// <returns>取得した値を返します。</returns>
        public static Action<int> GetCallback(DependencyObject target)
        {
            return (Action<int>)target.GetValue(CallbackProperty);
        }

        /// <summary>
        /// Callback 添付プロパティを設定します。
        /// </summary>
        /// <param name="target">対象とする DependencyObject を指定します。</param>
        /// <param name="value">設定する値を指定します。</param>
        public static void SetCallback(DependencyObject target, Action<int> value)
        {
            target.SetValue(CallbackProperty, value);
        }

        /// <summary>
        /// Callback 添付プロパティ変更イベントハンドラ
        /// </summary>
        /// <param name="d">イベント発行元</param>
        /// <param name="e">イベント引数</param>
        private static void OnCallbackPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var itemsControl = d as ItemsControl;
            if (itemsControl == null) return;

            if (GetCallback(itemsControl) != null)
            {
                itemsControl.PreviewMouseLeftButtonUp += OnPreviewMouseLeftButtonUp;
            }
            else
            {
                itemsControl.PreviewMouseLeftButtonUp -= OnPreviewMouseLeftButtonUp;
            }
        }

        #endregion Callback 添付プロパティ

        #region イベントハンドラ

        /// <summary>
        /// 指定された FrameworkElement に対するテンプレートのルート要素を取得します。
        /// </summary>
        /// <param name="element">FrameworkElement を指定します。</param>
        /// <returns>TemplatedParent を辿った先のルート要素を返します。</returns>
        private static FrameworkElement GetTemplatedRootElement(FrameworkElement element)
        {
            var parent = element.TemplatedParent as FrameworkElement;
            if (parent != null)
            {
                while (parent.TemplatedParent != null)
                {
                    parent = parent.TemplatedParent as FrameworkElement;
                }
                return parent;
            }
            else
            {
                return element;
            }

        }


        /// <summary>
        /// PreviewMouseLeftButtonUp イベントハンドラ
        /// 単純にクリック操作されたときの処理
        /// </summary>
        /// <param name="sender">イベント発行元</param>
        /// <param name="e">イベント引数</param>
        private static void OnPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var itemsControl = sender as ItemsControl;
            var targetContainer = GetTemplatedRootElement(e.OriginalSource as FrameworkElement);
            var index = itemsControl.ItemContainerGenerator.IndexFromContainer(targetContainer);
            if (index >= 0)
            {
                var callback = GetCallback(itemsControl);
                callback(index);
            }
        }


        #endregion イベントハンドラ
    }
}
