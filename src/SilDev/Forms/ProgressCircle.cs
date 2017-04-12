﻿#region auto-generated FILE INFORMATION

// ==============================================
// This file is distributed under the MIT License
// ==============================================
// 
// Filename: ProgressCircle.cs
// Version:  2017-04-10 12:26
// 
// Copyright (c) 2017, Si13n7 Developments (r)
// All rights reserved.
// ______________________________________________

#endregion

namespace SilDev.Forms
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Windows.Forms;

    /// <summary>
    ///     Represents a <see cref="ProgressCircle"/> control.
    /// </summary>
    public class ProgressCircle : Control
    {
        private readonly IContainer components = null;
        private readonly Timer _timer = new Timer();
        private bool _isActive;
        private int _progressValue;
        private int _spokes = 9;
        private int _thickness = 4;
        private int _innerRadius = 6;
        private int _outerRadius = 7;
        private PointF _centerPoint;
        private Color _foreColor;
        private Color[] _colors;
        private double[] _angles;

        /// <summary>
        ///     Gets or sets a value indicating whether this <see cref="ProgressCircle"/>
        ///     is active.
        /// </summary>
        public bool Active
        {
            get { return _isActive; }
            set
            {
                _isActive = value;
                ActiveTimer();
            }
        }

        /// <summary>
        ///     Gets or sets the number of spokes.
        /// </summary>
        public int Spokes
        {
            get
            {
                if (_spokes <= 0)
                    _spokes = 9;
                return _spokes;
            }
            set
            {
                if (_spokes == value || _spokes <= 0)
                    return;
                _spokes = value;
                GenerateColorsPallet();
                GetSpokesAngles();
                Invalidate();
            }
        }

        /// <summary>
        ///     Gets or sets the spoke thickness.
        /// </summary>
        public int Thickness
        {
            get
            {
                if (_thickness <= 0)
                    _thickness = 4;
                return _thickness;
            }
            set
            {
                _thickness = value;
                Invalidate();
            }
        }

        /// <summary>
        ///     Gets or sets the inner circle radius.
        /// </summary>
        public int InnerRadius
        {
            get
            {
                if (_innerRadius <= 0)
                    _innerRadius = 6;
                return _innerRadius;
            }
            set
            {
                _innerRadius = value;
                Invalidate();
            }
        }

        /// <summary>
        ///     Gets or sets the outer circle radius.
        /// </summary>
        public int OuterRadius
        {
            get
            {
                if (_outerRadius <= 0)
                    _outerRadius = 7;
                return _outerRadius;
            }
            set
            {
                _outerRadius = value;
                Invalidate();
            }
        }

        /// <summary>
        ///     Gets or sets the rotation speed.
        /// </summary>
        public int RotationSpeed
        {
            get { return _timer.Interval; }
            set
            {
                if (value > 0)
                    _timer.Interval = value;
            }
        }

        /// <summary>
        ///     Sets the circle appearance.
        /// </summary>
        /// <param name="spokes">
        ///     The number of spokes.
        /// </param>
        /// <param name="thickness">
        ///     The spoke thickness.
        /// </param>
        /// <param name="innerRadius">
        ///     The inner circle radius.
        /// </param>
        /// <param name="outerRadius">
        ///     The outer circle radius.
        /// </param>
        public void SetAppearance(int spokes, int thickness, int innerRadius, int outerRadius)
        {
            Spokes = spokes;
            Thickness = thickness;
            InnerRadius = innerRadius;
            OuterRadius = outerRadius;
            Invalidate();
        }

        /// <summary>
        ///     Gets or sets the foreground color of the control.
        /// </summary>
        public override Color ForeColor
        {
            get { return _foreColor; }
            set
            {
                _foreColor = value;
                GenerateColorsPallet();
            }
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ProgressCircle"/> class.
        /// </summary>
        public ProgressCircle()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.SupportsTransparentBackColor, true);

            GenerateColorsPallet();
            GetSpokesAngles();
            GetControlCenterPoint();

            _timer.Tick += Timer_Tick;
            ActiveTimer();

            Resize += ProgressCircle_Resize;
        }

        private void ProgressCircle_Resize(object sender, EventArgs e) =>
            GetControlCenterPoint();

        /// <summary>
        ///     Retrieves the size of a rectangular area into which a control can be
        ///     fitted.
        /// </summary>
        /// <param name="size">
        ///     The custom-sized area for a control.
        /// </param>
        public override Size GetPreferredSize(Size size) =>
            new Size(size.Width = (_outerRadius + _thickness) * 2, size.Height);

        private void Timer_Tick(object sender, EventArgs e)
        {
            _progressValue = ++_progressValue % _spokes;
            Invalidate();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                components?.Dispose();
            base.Dispose(disposing);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (_spokes > 0)
            {
                e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
                var pos = _progressValue;
                for (var i = 0; i < _spokes; i++)
                {
                    pos = pos % _spokes;
                    using (var pen = new Pen(new SolidBrush(_colors[i]), _thickness))
                    {
                        pen.StartCap = LineCap.Round;
                        pen.EndCap = LineCap.Round;
                        e.Graphics.DrawLine(pen, GetCoordinate(_centerPoint, _innerRadius, _angles[pos]), GetCoordinate(_centerPoint, _outerRadius, _angles[pos]));
                    }
                    pos++;
                }
            }
            base.OnPaint(e);
        }

        private void GenerateColorsPallet()
        {
            var colors = new Color[_spokes];
            var increment = (byte)(byte.MaxValue / _spokes);
            byte percentageOfDarken = 0;
            for (var i = 0; i < _spokes; i++)
                if (Active)
                    if (i == 0 || i < _spokes - _spokes)
                        colors[i] = ForeColor;
                    else
                    {
                        percentageOfDarken += increment;
                        colors[i] = Color.FromArgb(percentageOfDarken, ForeColor.R, ForeColor.G, ForeColor.B);
                    }
                else
                    colors[i] = ForeColor;
            _colors = colors;
        }

        private void GetControlCenterPoint() =>
            _centerPoint = new PointF(Width / 2f, Height / 2f - 1);

        private static PointF GetCoordinate(PointF circleCenter, int radius, double angle)
        {
            var d = Math.PI * angle / 180d;
            return new PointF(circleCenter.X + radius * (float)Math.Cos(d), circleCenter.Y + radius * (float)Math.Sin(d));
        }

        private void GetSpokesAngles()
        {
            var angles = new double[_spokes];
            var angle = 360d / _spokes;
            for (var i = 0; i < _spokes; i++)
                angles[i] = i == 0 ? angle : angles[i - 1] + angle;
            _angles = angles;
        }

        private void ActiveTimer()
        {
            if (_isActive)
                _timer?.Start();
            else
            {
                _timer?.Stop();
                _progressValue = 0;
            }
            GenerateColorsPallet();
            Invalidate();
        }
    }
}