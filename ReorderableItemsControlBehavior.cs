using Microsoft.Xaml.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;


// 参考
// http://yujiro15.net/blog/index.php?id=151
namespace TextConvert
{
    

    /// <summary>
    /// ItemsControl に対するドラッグ＆ドロップによる並べ替え動作をおこなうビヘイビアを表します。
    /// </summary>
    internal class ReorderableItemsControlBehavior
    {

        /// <summary>
        /// ドラッグ中の一時データ
        /// </summary>
        private static DragDropObject temporaryData;

        #region Callback 添付プロパティ

        /// <summary>
        /// Callback 添付プロパティの定義
        /// </summary>
        public static readonly DependencyProperty CallbackProperty = DependencyProperty.RegisterAttached(
            "Callback",
            typeof(Action<int>),
            typeof(ReorderableItemsControlBehavior),
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
                itemsControl.PreviewMouseLeftButtonDown += OnPreviewMouseLeftButtonDown;
                itemsControl.PreviewMouseMove += OnPreviewMouseMove;
                itemsControl.PreviewMouseLeftButtonUp += OnPreviewMouseLeftButtonUp;
                itemsControl.PreviewDragEnter += OnPreviewDragEnter;
                itemsControl.PreviewDragLeave += OnPreviewDragLeave;
                itemsControl.PreviewDrop += OnPreviewDrop;
            }
            else
            {
                itemsControl.PreviewMouseLeftButtonDown -= OnPreviewMouseLeftButtonDown;
                itemsControl.PreviewMouseMove -= OnPreviewMouseMove;
                itemsControl.PreviewMouseLeftButtonUp -= OnPreviewMouseLeftButtonUp;
                itemsControl.PreviewDragEnter -= OnPreviewDragEnter;
                itemsControl.PreviewDragLeave -= OnPreviewDragLeave;
                itemsControl.PreviewDrop -= OnPreviewDrop;
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
            if(parent != null)
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
        /// PreviewMouseLeftButtonDown イベントハンドラ
        /// 掴むアイテムを捕捉する
        /// </summary>
        /// <param name="sender">イベント発行元</param>
        /// <param name="e">イベント引数</param>
        private static void OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var control = sender as FrameworkElement;
            temporaryData = new DragDropObject();
            temporaryData.Start = e.GetPosition(Window.GetWindow(control));
            temporaryData.DraggedItem = GetTemplatedRootElement(e.OriginalSource as FrameworkElement);
            e.Handled = false;
        }

        /// <summary>
        /// PreviewMouseLeftButtonUp イベントハンドラ
        /// 単純にクリック操作されたときの処理
        /// </summary>
        /// <param name="sender">イベント発行元</param>
        /// <param name="e">イベント引数</param>
        private static void OnPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            temporaryData = null;
            e.Handled = false;
        }


        /// <summary>
        /// PreviewMouseMove イベントハンドラ
        /// アイテムを掴んだままマウスが動いたことを確認してドラッグ操作へ移行する
        /// </summary>
        /// <param name="sender">イベント発行元</param>
        /// <param name="e">イベント引数</param>
        private static void OnPreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (temporaryData != null)
            {
                var control = sender as FrameworkElement;
                var current = e.GetPosition(Window.GetWindow(control));
                if (temporaryData.CheckStartDragging(current))
                {
                    DragDrop.DoDragDrop(control, temporaryData.DraggedItem, DragDropEffects.Move);
                    // この先は Drop イベント処理後におこなわれる
                    temporaryData = null;
                }
            }
        }


        /// <summary>
        /// PreviewDragEnter イベントハンドラ
        /// コントロール外からコントロール内に入ったときの処理
        /// </summary>
        /// <param name="sender">イベント発行元</param>
        /// <param name="e">イベント引数</param>
        private static void OnPreviewDragEnter(object sender, DragEventArgs e)
        {
            if (temporaryData == null) return;
            temporaryData.IsDroppable = true;
        }

        /// <summary>
        /// PreviewDragLeave イベントハンドラ
        /// コントロール外にはみ出たときの処理
        /// </summary>
        /// <param name="sender">イベント発行元</param>
        /// <param name="e">イベント引数</param>
        private static void OnPreviewDragLeave(object sender, DragEventArgs e)
        {
            if (temporaryData == null) return;
            temporaryData.IsDroppable = false;
        }

        /// <summary>
        /// PreviewDrop イベントハンドラ
        /// ドロップされたときの処理
        /// </summary>
        /// <param name="sender">イベント発行元</param>
        /// <param name="e">イベント引数</param>
        private static void OnPreviewDrop(object sender, DragEventArgs e)
        {
            if (temporaryData == null) return;
            if (temporaryData.IsDroppable)
            {
                var itemsControl = sender as ItemsControl;
                // 異なる ItemsControl 間でドロップ処理されないようにするために
                // 同一 ItemsControl 内にドラッグされたコンテナが存在することを確認する
                if (itemsControl.ItemContainerGenerator.IndexFromContainer(temporaryData.DraggedItem) >= 0)
                {
                    var targetContainer = GetTemplatedRootElement(e.OriginalSource as FrameworkElement);
                    var index = itemsControl.ItemContainerGenerator.IndexFromContainer(targetContainer);
                    if (index >= 0)
                    {
                        var callback = GetCallback(itemsControl);
                        callback(index);
                    }
                }
            }

            // 終了後は DragDrop.DoDragDrop() メソッド呼び出し元へ戻る
        }

        #endregion イベントハンドラ


    }
}
