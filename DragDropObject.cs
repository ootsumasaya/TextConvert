using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TextConvert
{
    

    /// <summary>
    /// ドラッグ＆ドロップに関するデータを表します。
    /// </summary>
    public class DragDropObject
    {
        /// <summary>
        /// ドラッグ開始座標を取得または設定します。
        /// </summary>
        public Point Start { get; set; }

        /// <summary>
        /// ドラッグ対象であるオブジェクトを取得または設定します。
        /// </summary>
        public FrameworkElement DraggedItem { get; set; }

        /// <summary>
        /// ドロップ可能かどうかを取得または設定します。
        /// </summary>
        public bool IsDroppable { get; set; }

        /// <summary>
        /// ドラッグを開始していいかどうかを確認します。
        /// </summary>
        /// <param name="current">現在のマウス座標を指定します。</param>
        /// <returns>十分マウスが移動している場合に true を返します。</returns>
        public bool CheckStartDragging(Point current)
        {
            return (current - this.Start).Length - MinimumDragPoint.Length > 0;
        }

        /// <summary>
        /// ドラッグ開始に必要な最短距離を示すベクトル
        /// </summary>
        private static readonly Vector MinimumDragPoint = new Vector(SystemParameters.MinimumHorizontalDragDistance, SystemParameters.MinimumVerticalDragDistance);
    }
}
