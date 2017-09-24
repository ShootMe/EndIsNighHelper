using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
namespace EndIsNigh {
	public enum CellType {
		Normal,
		Start,
		End,
		PathHorizontal,
		PathVertical,
		PathBoth
	}
	public class MiniMapCell {
		public string ID { get; set; }
		public string Name { get; set; }
		public CellType Type { get; set; }
		public Color Color { get; set; }
		public int X { get; set; }
		public int Y { get; set; }
		public MiniMapCell(string id, string name, int x, int y, CellType type, Color color) {
			ID = id;
			Name = name;
			X = x;
			Y = y;
			Type = type;
			Color = color;
		}
		public override string ToString() {
			return ID + " (" + Name + ")[" + X + ", " + Y + "] " + Type.ToString();
		}
	}
	public class MiniMap : PictureBox {
		[Browsable(false)]
		public MiniMapCell[,] Cells {
			get { return cells; }
			set {
				cells = value;
				Invalidate();
			}
		}
		[DefaultValue(0)]
		public int Opacity { get { return opacity; } set { opacity = value; Invalidate(); } }
		[DefaultValue(typeof(Color), "Transparent")]
		public Color BackgroundColor { get { return backColor; } set { backColor = value; Invalidate(); } }
		[Browsable(false)]
		public new Image InitialImage { get; set; }
		[Browsable(false)]
		public new Image ErrorImage { get; set; }
		[Browsable(false)]
		public new Color BackColor { get { return base.BackColor; } set { base.BackColor = value; } }
		[Browsable(false)]
		public MiniMapCell SelectedCell { get { return selectedCell; } set { selectedCell = value; Invalidate(); } }
		[DefaultValue(typeof(Color), "Black")]
		public Color SelectedCellColor { get { return selectedCellColor; } set { selectedCellColor = value; Invalidate(); } }
		[DefaultValue(typeof(Color), "Black")]
		public Color HoverCellColor { get { return hoverCellColor; } set { hoverCellColor = value; Invalidate(); } }

		public delegate void ClickedCellEventArgs(object sender, MiniMapCell cell);
		public event ClickedCellEventArgs ClickedCell;

		public delegate void HoverOverCellEventArgs(object sender, MiniMapCell cell);
		public event HoverOverCellEventArgs HoverOverCell;

		private MiniMapCell[,] cells;
		private MiniMapCell selectedCell;
		private int opacity;
		private Color backColor, selectedCellColor, hoverCellColor;
		private Point lastMousePosition = new Point(-1, -1);
		private Point lastCellPosition = new Point(-1, -1);

		public MiniMap() {
			opacity = 0;
			backColor = Color.Transparent;
			hoverCellColor = Color.Black;
			selectedCellColor = Color.Black;
		}

		protected override void OnMouseClick(MouseEventArgs e) {
			if (lastCellPosition.X >= 0 && lastCellPosition.Y >= 0) {
				MiniMapCell hoverCell = cells[lastCellPosition.Y, lastCellPosition.X];
				if (hoverCell != null && hoverCell.Type < CellType.PathHorizontal) {
					ClickedCell?.Invoke(this, hoverCell);
				}
			}
		}
		protected override void OnResize(EventArgs e) {
			base.OnResize(e);
			Invalidate();
		}
		protected override void OnMouseLeave(EventArgs e) {
			base.OnMouseLeave(e);
			lastMousePosition = new Point(-1, -1);
			lastCellPosition = new Point(-1, -1);
			Invalidate();
		}
		protected override void OnMouseMove(MouseEventArgs e) {
			base.OnMouseMove(e);

			if (cells == null) { return; }

			lastMousePosition = e.Location;

			int w = Width; int h = Height;
			int rowCount = cells.GetLength(0);
			int colCount = cells.GetLength(1);
			double colInc = colCount <= 1 ? (double)w - 1 : (double)(w - 1 - colCount) / colCount;
			double rowInc = rowCount <= 1 ? (double)h - 1 : (double)(h - 1 - rowCount) / rowCount;

			int cellX = -1, cellY = -1;
			double currentRowVal = 0;
			for (int i = 0; i < rowCount; i++) {
				double lastRowVal = currentRowVal;
				currentRowVal += rowInc + 1;

				if (lastMousePosition.Y >= lastRowVal && lastMousePosition.Y < currentRowVal) {
					cellY = i;
					break;
				}
			}

			double currentColVal = 0;
			for (int i = 0; i < colCount; i++) {
				double lastColVal = currentColVal;
				currentColVal += colInc + 1;

				if (lastMousePosition.X >= lastColVal && lastMousePosition.X < currentColVal) {
					cellX = i;
					break;
				}
			}

			if ((cellX != lastCellPosition.X || cellY != lastCellPosition.Y) && cellX >= 0 && cellY >= 0) {
				lastCellPosition = new Point(cellX, cellY);
				MiniMapCell hoverCell = cells[cellY, cellX];
				HoverOverCell?.Invoke(this, hoverCell);
				Invalidate();
			}
		}
		protected override void OnPaintBackground(PaintEventArgs e) {
			base.OnPaintBackground(e);
			Graphics g = e.Graphics;

			if (Parent != null) {
				base.BackColor = Color.Transparent;
				int index = Parent.Controls.GetChildIndex(this);

				for (int i = Parent.Controls.Count - 1; i > index; i--) {
					Control c = Parent.Controls[i];
					if (c.Bounds.IntersectsWith(Bounds) && c.Visible) {
						Bitmap bmp = new Bitmap(c.Width, c.Height, g);
						c.DrawToBitmap(bmp, c.ClientRectangle);

						g.TranslateTransform(c.Left - Left, c.Top - Top);
						g.DrawImageUnscaled(bmp, Point.Empty);
						g.TranslateTransform(Left - c.Left, Top - c.Top);
						bmp.Dispose();
					}
				}
				g.FillRectangle(new SolidBrush(Color.FromArgb(Opacity * 255 / 100, BackgroundColor)), this.ClientRectangle);
			} else {
				g.Clear(Color.Transparent);
				g.FillRectangle(new SolidBrush(Color.FromArgb(Opacity * 255 / 100, BackgroundColor)), this.ClientRectangle);
			}
		}
		protected override void OnPaint(PaintEventArgs e) {
			base.OnPaint(e);
			if (cells == null) { return; }

			int w = Width; int h = Height;

			Graphics g = e.Graphics;
			Pen line = new Pen(Color.Black, 1);
			line.DashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
			g.DrawLine(line, 0, 0, w - 1, 0);
			g.DrawLine(line, 0, 0, 0, h - 1);
			g.DrawLine(line, w - 1, 0, w - 1, h - 1);
			g.DrawLine(line, 0, h - 1, w - 1, h - 1);

			int rowCount = cells.GetLength(0);
			int colCount = cells.GetLength(1);
			double colInc = colCount <= 1 ? (double)w - 1 : (double)(w - 1 - colCount) / colCount;
			double rowInc = rowCount <= 1 ? (double)h - 1 : (double)(h - 1 - rowCount) / rowCount;

			double currentRowVal = 0;
			for (int i = 0; i < rowCount; i++) {
				double lastRowVal = currentRowVal;
				currentRowVal += rowInc + 1;

				double currentColVal = 0;
				for (int j = 0; j < colCount; j++) {
					double lastColVal = currentColVal;
					currentColVal += colInc + 1;

					MiniMapCell cell = cells[i, j];
					if (cell != null) {
						DrawCell(cell, g, line, colCount, rowCount, j, i, (int)lastColVal, (int)currentColVal, (int)lastRowVal, (int)currentRowVal);
					}
				}
			}
		}
		private void DrawCell(MiniMapCell cell, Graphics g, Pen line, int colCount, int rowCount, int col, int row, int lastColVal, int currentColVal, int lastRowVal, int currentRowVal) {
			g.DrawLine(line, lastColVal, lastRowVal, lastColVal, currentRowVal);
			g.DrawLine(line, lastColVal, lastRowVal, currentColVal, lastRowVal);
			if (row + 1 < rowCount) {
				g.DrawLine(line, lastColVal, currentRowVal, currentColVal, currentRowVal);
			}
			if (col + 1 < colCount) {
				g.DrawLine(line, currentColVal, lastRowVal, currentColVal, currentRowVal);
			}

			int rowVal = row + 1 == rowCount ? Height - 2 : currentRowVal - 1;
			int colVal = col + 1 == colCount ? Width - 2 : currentColVal - 1;

			g.FillRectangle(new SolidBrush(cell.Color), lastColVal + 1, lastRowVal + 1, colVal - lastColVal, rowVal - lastRowVal);
			if (selectedCell != null && selectedCell == cell) {
				g.DrawRectangle(line, lastColVal + 1, lastRowVal + 1, colVal - lastColVal - 1, rowVal - lastRowVal - 1);
				g.DrawRectangle(line, lastColVal + 2, lastRowVal + 2, colVal - lastColVal - 3, rowVal - lastRowVal - 3);
			}

			if (lastCellPosition.X >= 0 && lastCellPosition.Y >= 0) {
				MiniMapCell hoverCell = cells[lastCellPosition.Y, lastCellPosition.X];
				if (hoverCell != null && hoverCell == cell && hoverCell.Type < CellType.PathHorizontal) {
					g.DrawLine(line, lastColVal, rowVal, currentColVal, rowVal);
					g.DrawLine(line, colVal, lastRowVal, colVal, currentRowVal);
					g.FillRectangle(new SolidBrush(Color.FromArgb(90, hoverCellColor.R, hoverCellColor.G, hoverCellColor.B)), lastColVal + 1, lastRowVal + 1, colVal - lastColVal, rowVal - lastRowVal);
				}
			}

			if (cell.Type == CellType.Start) {
				float xsize = TextRenderer.MeasureText("S", DefaultFont).Width / 2.0f - 1;
				g.DrawString("S", DefaultFont, new SolidBrush(ForeColor), lastColVal + (colVal - lastColVal) / 2 - xsize, lastRowVal + (rowVal - lastRowVal) / 2 - DefaultFont.SizeInPoints / 2.0f - 1);
			} else if (cell.Type == CellType.End) {
				float xsize = TextRenderer.MeasureText("E", DefaultFont).Width / 2.0f - 1;
				g.DrawString("E", DefaultFont, new SolidBrush(ForeColor), lastColVal + (colVal - lastColVal) / 2 - xsize, lastRowVal + (rowVal - lastRowVal) / 2 - DefaultFont.SizeInPoints / 2.0f - 1);
			}
		}
	}
}