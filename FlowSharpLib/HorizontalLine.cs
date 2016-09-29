﻿using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace FlowSharpLib
{
	public enum AvailableLineCap
	{
		None,
		Arrow,
	};

	public class HorizontalLine : Line// , ILine
	{
		// Fixes background erase issues with dynamic connector.
		public override Rectangle UpdateRectangle { get { return DisplayRectangle.Grow(anchorSize + 1 + BorderPen.Width); } }

		public HorizontalLine(Canvas canvas) : base(canvas)
		{
			HasCornerAnchors = false;
			HasCenterAnchors = false;
			HasLeftRightAnchors = true;
			HasCornerConnections = false;
			HasCenterConnections = false;
			HasLeftRightConnections = true;
		}

		public override GraphicElement Clone(Canvas canvas)
		{
			HorizontalLine line = (HorizontalLine)base.Clone(canvas);
			line.StartCap = StartCap;
			line.EndCap = EndCap;

			return line;
		}

		public override Rectangle DefaultRectangle()
		{
			return new Rectangle(20, 20, 40, 20);
		}

		public override void MoveAnchor(ConnectionPoint cpShape, ConnectionPoint cp)
		{
			if (cp.Type == GripType.Start)
			{
				DisplayRectangle = new Rectangle(cpShape.Point.X, cpShape.Point.Y -BaseController.MIN_HEIGHT/2, DisplayRectangle.Size.Width, DisplayRectangle.Size.Height);
			}
			else
			{
				DisplayRectangle = new Rectangle(cpShape.Point.X-DisplayRectangle.Size.Width, cpShape.Point.Y - BaseController.MIN_HEIGHT/2, DisplayRectangle.Size.Width, DisplayRectangle.Size.Height);
			}

			// TODO: Redraw is updating too much in this case -- causes jerky motion of attached shape.
			canvas.Controller.Redraw(this, (cpShape.Point.X - cp.Point.X).Abs() + BaseController.MIN_WIDTH, (cpShape.Point.Y - cp.Point.Y).Abs() + BaseController.MIN_HEIGHT);
		}

		public override void Draw(Graphics gr)
		{
			// See CustomLineCap for creating other possible endcaps besides arrows.
			// Note that AdjustableArrowCap derives from CustomLineCap!
			// https://msdn.microsoft.com/en-us/library/system.drawing.drawing2d.customlinecap(v=vs.110).aspx

			AdjustableArrowCap adjCap = new AdjustableArrowCap(5, 5, true);
			Pen pen = (Pen)BorderPen.Clone();

			if (ShowLineAsSelected)
			{
				pen.Color = pen.Color.ToArgb() == Color.Red.ToArgb() ? Color.Blue : Color.Red;
			}

			if (StartCap == AvailableLineCap.Arrow)
			{
				pen.CustomStartCap = adjCap;
			}

			if (EndCap == AvailableLineCap.Arrow)
			{
				pen.CustomEndCap = adjCap;
			}

			gr.DrawLine(pen, DisplayRectangle.LeftMiddle(), DisplayRectangle.RightMiddle());
			pen.Dispose();
			adjCap.Dispose();

			base.Draw(gr);
		}
	}
}
