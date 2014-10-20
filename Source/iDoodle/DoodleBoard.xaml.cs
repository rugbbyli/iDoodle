using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Shapes;

namespace iDoodle
{
    public partial class DoodleBoard : UserControl
    {
        PathFigure fig;
        PathGeometry geo;
        Path path;

        bool m_isTouch = false;

        Stack<UIElement> StepStack = new Stack<UIElement>();

        public DoodleBoard()
        {
            InitializeComponent();
            //path.FillRule = FillRule.Nonzero;
            this.PenWidth = 5;
            this.PenFill = new SolidColorBrush(Colors.Black);
            this.PenStroke = new SolidColorBrush(Colors.Black);
            this.PenOpacity = 1.0;
        }

        private void LayoutRoot_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (m_isTouch)
            {
                LineSegment seg = new LineSegment();
                seg.Point = e.GetPosition(LayoutRoot);

                fig.Segments.Add(seg);
                path.Data = geo;

                if (!LayoutRoot.Children.Contains(path))
                {
                    LayoutRoot.Children.Add(path);
                }
            }
            //System.Diagnostics.Debug.WriteLine("MouseMove");
        }

        private void LayoutRoot_MouseDown(object sender, System.Windows.Input.MouseEventArgs e)
        {
            fig = new PathFigure();
            fig.IsClosed = false;
            fig.IsFilled = false;
            fig.StartPoint = e.GetPosition(LayoutRoot);
            geo = new PathGeometry();
            geo.FillRule = FillRule.Nonzero;
            geo.Figures.Add(fig);
            path = new Path();
            path.Data = geo;
            path.Fill = this.PenFill;
            path.Stroke = this.PenStroke;
            path.Opacity = this.PenOpacity;
            path.StrokeThickness = this.PenWidth;
            path.StrokeEndLineCap = PenLineCap.Round;
            path.StrokeStartLineCap = PenLineCap.Round;
            this.m_isTouch = true;
        }

        private void LayoutRoot_MouseUp(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (path != null && !LayoutRoot.Children.Contains(path))
            {
                EllipseGeometry ell = new EllipseGeometry();
                
                ell.Center = e.GetPosition(LayoutRoot);
                ell.RadiusX = 0.1;
                ell.RadiusY = 0.1;

                var point = new Path();
                point.Data = ell;
                point.Fill = this.PenFill;
                point.Stroke = this.PenStroke;
                point.StrokeThickness = this.PenWidth;
                LayoutRoot.Children.Add(point);
            }
            this.m_isTouch = false;
        }

        private void LayoutRoot_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if(m_isTouch) LayoutRoot_MouseDown(this, e);
        }

        private void LayoutRoot_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            
        }

        #region public interface

        /// <summary>
        /// 生成画图板的一副实时图，可用于预览或保存
        /// </summary>
        /// <returns></returns>
        public WriteableBitmap GetImage(bool big = false)
        {
            if (!big)
            {
                var scale = new ScaleTransform();
                scale.ScaleX = 0.5;
                scale.ScaleY = 0.5;
                return new WriteableBitmap(LayoutRoot, scale);
            }
            return new WriteableBitmap(LayoutRoot, null);
        }

        /// <summary>
        /// 撤销画图板的最后一次操作
        /// </summary>
        public void StepBack()
        {
            if (this.CanStepBack)
            {
                StepStack.Push(LayoutRoot.Children.Last());
                LayoutRoot.Children.RemoveAt(LayoutRoot.Children.Count - 1);
                //PathRoot.Data = path;
            }
        }

        /// <summary>
        /// 确定是否可以撤销操作
        /// </summary>
        public bool CanStepBack
        {
            get
            {
                return LayoutRoot.Children.Count > 0;
            }
        }

        /// <summary>
        /// 重做画图板的最近一次撤销
        /// </summary>
        public void StepForward()
        {
            if (CanStepForward)
            {
                LayoutRoot.Children.Add(StepStack.Pop());
                //PathRoot.Data = path;
            }
        }

        /// <summary>
        /// 确定是否可以重做撤销操作
        /// </summary>
        public bool CanStepForward
        {
            get
            {
                return StepStack.Count > 0;
            }
        }

        /// <summary>
        /// 清除画图板上所有的内容
        /// </summary>
        public void Clear()
        {
            LayoutRoot.Children.Clear();
            //PathRoot.Data = path;
        }

        /// <summary>
        /// 获取或设置画笔的宽度
        /// </summary>
        public double PenWidth { get; set; }

        public Brush PenFill { get; set; }

        public Brush PenStroke { get; set; }

        public Brush BoardBrush
        {
            get { return LayoutRoot.Background; }
            set
            {
                LayoutRoot.Background = value;
            }
        }

        public double PenOpacity { get; set; }

        #endregion
    }
}
