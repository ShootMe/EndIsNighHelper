namespace EndIsNigh {
	partial class EINHelper {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.lblNote = new System.Windows.Forms.Label();
			this.lblWorldPosition = new System.Windows.Forms.Label();
			this.lblPlayerPosition = new System.Windows.Forms.Label();
			this.btnMove = new System.Windows.Forms.Button();
			this.btnSavePosition = new System.Windows.Forms.Button();
			this.gameMap = new EndIsNigh.MiniMap();
			this.lblHoverText = new System.Windows.Forms.Label();
			this.lblCurrentMap = new System.Windows.Forms.Label();
			this.lblInfo = new System.Windows.Forms.Label();
			this.chkAutoSave = new System.Windows.Forms.CheckBox();
			this.chkInfiniteLives = new System.Windows.Forms.CheckBox();
			((System.ComponentModel.ISupportInitialize)(this.gameMap)).BeginInit();
			this.SuspendLayout();
			// 
			// lblNote
			// 
			this.lblNote.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lblNote.Font = new System.Drawing.Font("Courier New", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblNote.Location = new System.Drawing.Point(0, 0);
			this.lblNote.Name = "lblNote";
			this.lblNote.Size = new System.Drawing.Size(784, 561);
			this.lblNote.TabIndex = 3;
			this.lblNote.Text = "Not available";
			this.lblNote.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// lblWorldPosition
			// 
			this.lblWorldPosition.AutoSize = true;
			this.lblWorldPosition.Font = new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblWorldPosition.Location = new System.Drawing.Point(63, 5);
			this.lblWorldPosition.Name = "lblWorldPosition";
			this.lblWorldPosition.Size = new System.Drawing.Size(78, 18);
			this.lblWorldPosition.TabIndex = 0;
			this.lblWorldPosition.Text = "World: ";
			// 
			// lblPlayerPosition
			// 
			this.lblPlayerPosition.AutoSize = true;
			this.lblPlayerPosition.Font = new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblPlayerPosition.Location = new System.Drawing.Point(53, 41);
			this.lblPlayerPosition.Name = "lblPlayerPosition";
			this.lblPlayerPosition.Size = new System.Drawing.Size(78, 18);
			this.lblPlayerPosition.TabIndex = 4;
			this.lblPlayerPosition.Text = "Player:";
			// 
			// btnMove
			// 
			this.btnMove.Location = new System.Drawing.Point(210, 92);
			this.btnMove.Name = "btnMove";
			this.btnMove.Size = new System.Drawing.Size(92, 23);
			this.btnMove.TabIndex = 5;
			this.btnMove.Text = "Move To Saved";
			this.btnMove.UseVisualStyleBackColor = true;
			this.btnMove.Click += new System.EventHandler(this.btnMoveToSaved_Click);
			// 
			// btnSavePosition
			// 
			this.btnSavePosition.Location = new System.Drawing.Point(122, 92);
			this.btnSavePosition.Name = "btnSavePosition";
			this.btnSavePosition.Size = new System.Drawing.Size(82, 23);
			this.btnSavePosition.TabIndex = 11;
			this.btnSavePosition.Text = "Save Position";
			this.btnSavePosition.UseVisualStyleBackColor = true;
			this.btnSavePosition.Click += new System.EventHandler(this.btnSavePosition_Click);
			// 
			// gameMap
			// 
			this.gameMap.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gameMap.BackColor = System.Drawing.Color.Transparent;
			this.gameMap.Cells = null;
			this.gameMap.ErrorImage = null;
			this.gameMap.InitialImage = null;
			this.gameMap.Location = new System.Drawing.Point(-1, 121);
			this.gameMap.Name = "gameMap";
			this.gameMap.SelectedCell = null;
			this.gameMap.SelectedCellColor = System.Drawing.Color.Gold;
			this.gameMap.Size = new System.Drawing.Size(786, 441);
			this.gameMap.TabIndex = 12;
			this.gameMap.TabStop = false;
			this.gameMap.ClickedCell += new EndIsNigh.MiniMap.ClickedCellEventArgs(this.gameMap_ClickedCell);
			this.gameMap.HoverOverCell += new EndIsNigh.MiniMap.HoverOverCellEventArgs(this.gameMap_HoverOverCell);
			// 
			// lblHoverText
			// 
			this.lblHoverText.AutoSize = true;
			this.lblHoverText.Font = new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblHoverText.Location = new System.Drawing.Point(4, 125);
			this.lblHoverText.Name = "lblHoverText";
			this.lblHoverText.Size = new System.Drawing.Size(48, 18);
			this.lblHoverText.TabIndex = 13;
			this.lblHoverText.Text = "Map:";
			this.lblHoverText.Visible = false;
			// 
			// lblCurrentMap
			// 
			this.lblCurrentMap.AutoSize = true;
			this.lblCurrentMap.Font = new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblCurrentMap.Location = new System.Drawing.Point(3, 23);
			this.lblCurrentMap.Name = "lblCurrentMap";
			this.lblCurrentMap.Size = new System.Drawing.Size(128, 18);
			this.lblCurrentMap.TabIndex = 14;
			this.lblCurrentMap.Text = "Current Map:";
			// 
			// lblInfo
			// 
			this.lblInfo.AutoSize = true;
			this.lblInfo.Font = new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblInfo.Location = new System.Drawing.Point(3, 59);
			this.lblInfo.Name = "lblInfo";
			this.lblInfo.Size = new System.Drawing.Size(128, 18);
			this.lblInfo.TabIndex = 15;
			this.lblInfo.Text = "Player Info:";
			// 
			// chkAutoSave
			// 
			this.chkAutoSave.AutoSize = true;
			this.chkAutoSave.Location = new System.Drawing.Point(19, 96);
			this.chkAutoSave.Name = "chkAutoSave";
			this.chkAutoSave.Size = new System.Drawing.Size(97, 17);
			this.chkAutoSave.TabIndex = 16;
			this.chkAutoSave.Text = "Auto Save Pos";
			this.chkAutoSave.UseVisualStyleBackColor = true;
			// 
			// chkInfiniteLives
			// 
			this.chkInfiniteLives.AutoSize = true;
			this.chkInfiniteLives.Location = new System.Drawing.Point(308, 96);
			this.chkInfiniteLives.Name = "chkInfiniteLives";
			this.chkInfiniteLives.Size = new System.Drawing.Size(119, 17);
			this.chkInfiniteLives.TabIndex = 17;
			this.chkInfiniteLives.Text = "Infinite Lives In Cart";
			this.chkInfiniteLives.UseVisualStyleBackColor = true;
			// 
			// EINHelper
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(784, 561);
			this.Controls.Add(this.lblNote);
			this.Controls.Add(this.chkInfiniteLives);
			this.Controls.Add(this.chkAutoSave);
			this.Controls.Add(this.lblInfo);
			this.Controls.Add(this.lblCurrentMap);
			this.Controls.Add(this.lblHoverText);
			this.Controls.Add(this.gameMap);
			this.Controls.Add(this.btnSavePosition);
			this.Controls.Add(this.btnMove);
			this.Controls.Add(this.lblPlayerPosition);
			this.Controls.Add(this.lblWorldPosition);
			this.MinimumSize = new System.Drawing.Size(600, 400);
			this.Name = "EINHelper";
			this.ShowIcon = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.Text = "End Is Nigh";
			((System.ComponentModel.ISupportInitialize)(this.gameMap)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
		private System.Windows.Forms.Label lblNote;
		private System.Windows.Forms.Label lblWorldPosition;
		private System.Windows.Forms.Label lblPlayerPosition;
		private System.Windows.Forms.Button btnMove;
		private System.Windows.Forms.Button btnSavePosition;
		private MiniMap gameMap;
		private System.Windows.Forms.Label lblHoverText;
		private System.Windows.Forms.Label lblCurrentMap;
		private System.Windows.Forms.Label lblInfo;
		private System.Windows.Forms.CheckBox chkAutoSave;
		private System.Windows.Forms.CheckBox chkInfiniteLives;
	}
}