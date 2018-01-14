namespace LiveSplit.Nestopia {
	partial class SplitterSplitSettings {
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SplitterSplitSettings));
			this.btnRemove = new System.Windows.Forms.Button();
			this.cboType = new System.Windows.Forms.ComboBox();
			this.ToolTips = new System.Windows.Forms.ToolTip(this.components);
			this.picHandle = new System.Windows.Forms.PictureBox();
			this.cboSize = new System.Windows.Forms.ComboBox();
			this.txtOffset = new System.Windows.Forms.TextBox();
			this.txtValue = new System.Windows.Forms.TextBox();
			((System.ComponentModel.ISupportInitialize)(this.picHandle)).BeginInit();
			this.SuspendLayout();
			// 
			// btnRemove
			// 
			this.btnRemove.Image = ((System.Drawing.Image)(resources.GetObject("btnRemove.Image")));
			this.btnRemove.Location = new System.Drawing.Point(438, 2);
			this.btnRemove.Name = "btnRemove";
			this.btnRemove.Size = new System.Drawing.Size(26, 23);
			this.btnRemove.TabIndex = 4;
			this.ToolTips.SetToolTip(this.btnRemove, "Remove this setting");
			this.btnRemove.UseVisualStyleBackColor = true;
			// 
			// cboType
			// 
			this.cboType.FormattingEnabled = true;
			this.cboType.Location = new System.Drawing.Point(190, 3);
			this.cboType.Name = "cboType";
			this.cboType.Size = new System.Drawing.Size(167, 21);
			this.cboType.TabIndex = 2;
			this.cboType.SelectedIndexChanged += new System.EventHandler(this.cboType_SelectedIndexChanged);
			this.cboType.Validating += new System.ComponentModel.CancelEventHandler(this.cboType_Validating);
			// 
			// picHandle
			// 
			this.picHandle.Cursor = System.Windows.Forms.Cursors.SizeAll;
			this.picHandle.Image = ((System.Drawing.Image)(resources.GetObject("picHandle.Image")));
			this.picHandle.Location = new System.Drawing.Point(3, 4);
			this.picHandle.Name = "picHandle";
			this.picHandle.Size = new System.Drawing.Size(20, 20);
			this.picHandle.TabIndex = 5;
			this.picHandle.TabStop = false;
			this.picHandle.MouseDown += new System.Windows.Forms.MouseEventHandler(this.picHandle_MouseDown);
			this.picHandle.MouseMove += new System.Windows.Forms.MouseEventHandler(this.picHandle_MouseMove);
			// 
			// cboSize
			// 
			this.cboSize.FormattingEnabled = true;
			this.cboSize.Location = new System.Drawing.Point(22, 3);
			this.cboSize.Name = "cboSize";
			this.cboSize.Size = new System.Drawing.Size(107, 21);
			this.cboSize.TabIndex = 0;
			this.cboSize.SelectedIndexChanged += new System.EventHandler(this.cboSize_SelectedIndexChanged);
			this.cboSize.Validating += new System.ComponentModel.CancelEventHandler(this.cboSize_Validating);
			// 
			// txtOffset
			// 
			this.txtOffset.Location = new System.Drawing.Point(135, 3);
			this.txtOffset.Name = "txtOffset";
			this.txtOffset.Size = new System.Drawing.Size(49, 20);
			this.txtOffset.TabIndex = 1;
			this.txtOffset.TextChanged += new System.EventHandler(this.txtOffset_TextChanged);
			this.txtOffset.Validating += new System.ComponentModel.CancelEventHandler(this.txtOffset_Validating);
			// 
			// txtValue
			// 
			this.txtValue.Location = new System.Drawing.Point(363, 3);
			this.txtValue.Name = "txtValue";
			this.txtValue.Size = new System.Drawing.Size(69, 20);
			this.txtValue.TabIndex = 3;
			this.txtValue.TextChanged += new System.EventHandler(this.txtValue_TextChanged);
			this.txtValue.Validating += new System.ComponentModel.CancelEventHandler(this.txtValue_Validating);
			// 
			// SplitterSplitSettings
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.Controls.Add(this.txtValue);
			this.Controls.Add(this.txtOffset);
			this.Controls.Add(this.cboSize);
			this.Controls.Add(this.btnRemove);
			this.Controls.Add(this.cboType);
			this.Controls.Add(this.picHandle);
			this.Margin = new System.Windows.Forms.Padding(0);
			this.Name = "SplitterSplitSettings";
			this.Size = new System.Drawing.Size(470, 28);
			((System.ComponentModel.ISupportInitialize)(this.picHandle)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
		public System.Windows.Forms.Button btnRemove;
		public System.Windows.Forms.ComboBox cboType;
		private System.Windows.Forms.ToolTip ToolTips;
		private System.Windows.Forms.PictureBox picHandle;
		public System.Windows.Forms.ComboBox cboSize;
		public System.Windows.Forms.TextBox txtOffset;
		public System.Windows.Forms.TextBox txtValue;
	}
}
