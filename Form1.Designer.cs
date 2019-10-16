namespace RapChessGui
{
	partial class FormChess
	{
		/// <summary>
		/// Wymagana zmienna projektanta.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Wyczyść wszystkie używane zasoby.
		/// </summary>
		/// <param name="disposing">prawda, jeżeli zarządzane zasoby powinny zostać zlikwidowane; Fałsz w przeciwnym wypadku.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Kod generowany przez Projektanta formularzy systemu Windows

		/// <summary>
		/// Metoda wymagana do obsługi projektanta — nie należy modyfikować
		/// jej zawartości w edytorze kodu.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormChess));
			this.imageList1 = new System.Windows.Forms.ImageList(this.components);
			this.timer1 = new System.Windows.Forms.Timer(this.components);
			this.menuStrip1 = new System.Windows.Forms.MenuStrip();
			this.newGameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.backToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.playersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.panel2 = new System.Windows.Forms.Panel();
			this.labPonderT = new System.Windows.Forms.Label();
			this.labNpsT = new System.Windows.Forms.Label();
			this.labDepthT = new System.Windows.Forms.Label();
			this.labScoreT = new System.Windows.Forms.Label();
			this.labTimeT = new System.Windows.Forms.Label();
			this.labNameT = new System.Windows.Forms.Label();
			this.panTop = new System.Windows.Forms.Panel();
			this.panel4 = new System.Windows.Forms.Panel();
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this.tabPageNormal = new System.Windows.Forms.TabPage();
			this.richTextBox1 = new System.Windows.Forms.RichTextBox();
			this.butStop = new System.Windows.Forms.Button();
			this.bStart = new System.Windows.Forms.Button();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.comboBoxB = new System.Windows.Forms.ComboBox();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.comboBoxW = new System.Windows.Forms.ComboBox();
			this.tabPageTraining = new System.Windows.Forms.TabPage();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.label15 = new System.Windows.Forms.Label();
			this.label14 = new System.Windows.Forms.Label();
			this.label13 = new System.Windows.Forms.Label();
			this.label12 = new System.Windows.Forms.Label();
			this.label11 = new System.Windows.Forms.Label();
			this.label10 = new System.Windows.Forms.Label();
			this.label9 = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			this.label7 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.label16 = new System.Windows.Forms.Label();
			this.labGames = new System.Windows.Forms.Label();
			this.butTraining = new System.Windows.Forms.Button();
			this.groupBox4 = new System.Windows.Forms.GroupBox();
			this.labTrainTime = new System.Windows.Forms.Label();
			this.comboBoxTeacher = new System.Windows.Forms.ComboBox();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.comboBoxTrained = new System.Windows.Forms.ComboBox();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.panel3 = new System.Windows.Forms.Panel();
			this.labPonderB = new System.Windows.Forms.Label();
			this.labNpsB = new System.Windows.Forms.Label();
			this.labDepthB = new System.Windows.Forms.Label();
			this.labScoreB = new System.Windows.Forms.Label();
			this.labTimeB = new System.Windows.Forms.Label();
			this.labNameB = new System.Windows.Forms.Label();
			this.panBottom = new System.Windows.Forms.Panel();
			this.panel1 = new System.Windows.Forms.Panel();
			this.labLast = new System.Windows.Forms.Label();
			this.labMove = new System.Windows.Forms.Label();
			this.timerStart = new System.Windows.Forms.Timer(this.components);
			this.menuStrip1.SuspendLayout();
			this.panel2.SuspendLayout();
			this.panel4.SuspendLayout();
			this.tabControl1.SuspendLayout();
			this.tabPageNormal.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.tabPageTraining.SuspendLayout();
			this.tableLayoutPanel1.SuspendLayout();
			this.groupBox4.SuspendLayout();
			this.groupBox3.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			this.panel3.SuspendLayout();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// imageList1
			// 
			this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
			this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
			this.imageList1.Images.SetKeyName(0, "Pawn_White.gif");
			this.imageList1.Images.SetKeyName(1, "Knight_White.gif");
			this.imageList1.Images.SetKeyName(2, "Bishop_White.gif");
			this.imageList1.Images.SetKeyName(3, "Rook_White.gif");
			this.imageList1.Images.SetKeyName(4, "Queen_White.gif");
			this.imageList1.Images.SetKeyName(5, "King_White.gif");
			this.imageList1.Images.SetKeyName(6, "Pawn_Black.gif");
			this.imageList1.Images.SetKeyName(7, "Knight_Black.gif");
			this.imageList1.Images.SetKeyName(8, "Bishop_Black.gif");
			this.imageList1.Images.SetKeyName(9, "Rook_Black.gif");
			this.imageList1.Images.SetKeyName(10, "Queen_Black.gif");
			this.imageList1.Images.SetKeyName(11, "King_Black.gif");
			// 
			// timer1
			// 
			this.timer1.Interval = 10;
			this.timer1.Tick += new System.EventHandler(this.Timer1_Tick_1);
			// 
			// menuStrip1
			// 
			this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newGameToolStripMenuItem,
            this.backToolStripMenuItem,
            this.playersToolStripMenuItem,
            this.optionsToolStripMenuItem});
			this.menuStrip1.Location = new System.Drawing.Point(0, 0);
			this.menuStrip1.Name = "menuStrip1";
			this.menuStrip1.Size = new System.Drawing.Size(848, 24);
			this.menuStrip1.TabIndex = 1;
			this.menuStrip1.Text = "menuStrip1";
			// 
			// newGameToolStripMenuItem
			// 
			this.newGameToolStripMenuItem.Name = "newGameToolStripMenuItem";
			this.newGameToolStripMenuItem.Size = new System.Drawing.Size(77, 20);
			this.newGameToolStripMenuItem.Text = "New Game";
			this.newGameToolStripMenuItem.Click += new System.EventHandler(this.NewGameToolStripMenuItem_Click);
			// 
			// backToolStripMenuItem
			// 
			this.backToolStripMenuItem.Name = "backToolStripMenuItem";
			this.backToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
			this.backToolStripMenuItem.Text = "Back";
			this.backToolStripMenuItem.Click += new System.EventHandler(this.BackToolStripMenuItem_Click);
			// 
			// playersToolStripMenuItem
			// 
			this.playersToolStripMenuItem.Name = "playersToolStripMenuItem";
			this.playersToolStripMenuItem.Size = new System.Drawing.Size(56, 20);
			this.playersToolStripMenuItem.Text = "Players";
			this.playersToolStripMenuItem.Click += new System.EventHandler(this.PlayersToolStripMenuItem_Click);
			// 
			// optionsToolStripMenuItem
			// 
			this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
			this.optionsToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
			this.optionsToolStripMenuItem.Text = "Options";
			this.optionsToolStripMenuItem.Click += new System.EventHandler(this.OptionsToolStripMenuItem_Click);
			// 
			// panel2
			// 
			this.panel2.BackColor = System.Drawing.Color.Silver;
			this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.panel2.Controls.Add(this.labPonderT);
			this.panel2.Controls.Add(this.labNpsT);
			this.panel2.Controls.Add(this.labDepthT);
			this.panel2.Controls.Add(this.labScoreT);
			this.panel2.Controls.Add(this.labTimeT);
			this.panel2.Controls.Add(this.labNameT);
			this.panel2.Controls.Add(this.panTop);
			this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
			this.panel2.Location = new System.Drawing.Point(0, 24);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(848, 30);
			this.panel2.TabIndex = 7;
			// 
			// labPonderT
			// 
			this.labPonderT.BackColor = System.Drawing.Color.LightGray;
			this.labPonderT.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.labPonderT.Dock = System.Windows.Forms.DockStyle.Left;
			this.labPonderT.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
			this.labPonderT.ForeColor = System.Drawing.Color.Black;
			this.labPonderT.Location = new System.Drawing.Point(711, 0);
			this.labPonderT.Name = "labPonderT";
			this.labPonderT.Size = new System.Drawing.Size(137, 26);
			this.labPonderT.TabIndex = 8;
			this.labPonderT.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// labNpsT
			// 
			this.labNpsT.BackColor = System.Drawing.Color.LightGray;
			this.labNpsT.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.labNpsT.Dock = System.Windows.Forms.DockStyle.Left;
			this.labNpsT.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
			this.labNpsT.ForeColor = System.Drawing.Color.Black;
			this.labNpsT.Location = new System.Drawing.Point(574, 0);
			this.labNpsT.Name = "labNpsT";
			this.labNpsT.Size = new System.Drawing.Size(137, 26);
			this.labNpsT.TabIndex = 7;
			this.labNpsT.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// labDepthT
			// 
			this.labDepthT.BackColor = System.Drawing.Color.LightGray;
			this.labDepthT.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.labDepthT.Dock = System.Windows.Forms.DockStyle.Left;
			this.labDepthT.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
			this.labDepthT.ForeColor = System.Drawing.Color.Black;
			this.labDepthT.Location = new System.Drawing.Point(437, 0);
			this.labDepthT.Name = "labDepthT";
			this.labDepthT.Size = new System.Drawing.Size(137, 26);
			this.labDepthT.TabIndex = 6;
			this.labDepthT.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// labScoreT
			// 
			this.labScoreT.BackColor = System.Drawing.Color.LightGray;
			this.labScoreT.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.labScoreT.Dock = System.Windows.Forms.DockStyle.Left;
			this.labScoreT.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
			this.labScoreT.ForeColor = System.Drawing.Color.Black;
			this.labScoreT.Location = new System.Drawing.Point(300, 0);
			this.labScoreT.Name = "labScoreT";
			this.labScoreT.Size = new System.Drawing.Size(137, 26);
			this.labScoreT.TabIndex = 5;
			this.labScoreT.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// labTimeT
			// 
			this.labTimeT.BackColor = System.Drawing.Color.LightGray;
			this.labTimeT.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.labTimeT.Dock = System.Windows.Forms.DockStyle.Left;
			this.labTimeT.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
			this.labTimeT.ForeColor = System.Drawing.Color.Black;
			this.labTimeT.Location = new System.Drawing.Point(163, 0);
			this.labTimeT.Name = "labTimeT";
			this.labTimeT.Size = new System.Drawing.Size(137, 26);
			this.labTimeT.TabIndex = 3;
			this.labTimeT.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// labNameT
			// 
			this.labNameT.BackColor = System.Drawing.Color.LightGray;
			this.labNameT.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.labNameT.Dock = System.Windows.Forms.DockStyle.Left;
			this.labNameT.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
			this.labNameT.ForeColor = System.Drawing.Color.Black;
			this.labNameT.Location = new System.Drawing.Point(26, 0);
			this.labNameT.Name = "labNameT";
			this.labNameT.Size = new System.Drawing.Size(137, 26);
			this.labNameT.TabIndex = 2;
			this.labNameT.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// panTop
			// 
			this.panTop.BackColor = System.Drawing.Color.Silver;
			this.panTop.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.panTop.Dock = System.Windows.Forms.DockStyle.Left;
			this.panTop.Location = new System.Drawing.Point(0, 0);
			this.panTop.Name = "panTop";
			this.panTop.Size = new System.Drawing.Size(26, 26);
			this.panTop.TabIndex = 1;
			// 
			// panel4
			// 
			this.panel4.Controls.Add(this.tabControl1);
			this.panel4.Controls.Add(this.pictureBox1);
			this.panel4.Dock = System.Windows.Forms.DockStyle.Top;
			this.panel4.Location = new System.Drawing.Point(0, 54);
			this.panel4.Name = "panel4";
			this.panel4.Size = new System.Drawing.Size(848, 576);
			this.panel4.TabIndex = 9;
			// 
			// tabControl1
			// 
			this.tabControl1.Appearance = System.Windows.Forms.TabAppearance.FlatButtons;
			this.tabControl1.Controls.Add(this.tabPageNormal);
			this.tabControl1.Controls.Add(this.tabPageTraining);
			this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabControl1.Location = new System.Drawing.Point(576, 0);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(272, 576);
			this.tabControl1.TabIndex = 9;
			// 
			// tabPageNormal
			// 
			this.tabPageNormal.Controls.Add(this.richTextBox1);
			this.tabPageNormal.Controls.Add(this.butStop);
			this.tabPageNormal.Controls.Add(this.bStart);
			this.tabPageNormal.Controls.Add(this.groupBox2);
			this.tabPageNormal.Controls.Add(this.groupBox1);
			this.tabPageNormal.Location = new System.Drawing.Point(4, 25);
			this.tabPageNormal.Name = "tabPageNormal";
			this.tabPageNormal.Padding = new System.Windows.Forms.Padding(3);
			this.tabPageNormal.Size = new System.Drawing.Size(264, 547);
			this.tabPageNormal.TabIndex = 0;
			this.tabPageNormal.Text = "Game";
			this.tabPageNormal.UseVisualStyleBackColor = true;
			// 
			// richTextBox1
			// 
			this.richTextBox1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.richTextBox1.Location = new System.Drawing.Point(3, 142);
			this.richTextBox1.Name = "richTextBox1";
			this.richTextBox1.Size = new System.Drawing.Size(258, 402);
			this.richTextBox1.TabIndex = 22;
			this.richTextBox1.Text = "";
			// 
			// butStop
			// 
			this.butStop.Dock = System.Windows.Forms.DockStyle.Top;
			this.butStop.Location = new System.Drawing.Point(3, 119);
			this.butStop.Name = "butStop";
			this.butStop.Size = new System.Drawing.Size(258, 23);
			this.butStop.TabIndex = 21;
			this.butStop.Text = "Stop calculating";
			this.butStop.UseVisualStyleBackColor = true;
			this.butStop.Click += new System.EventHandler(this.ButStop_Click);
			// 
			// bStart
			// 
			this.bStart.Dock = System.Windows.Forms.DockStyle.Top;
			this.bStart.Location = new System.Drawing.Point(3, 96);
			this.bStart.Name = "bStart";
			this.bStart.Size = new System.Drawing.Size(258, 23);
			this.bStart.TabIndex = 20;
			this.bStart.Text = "New game";
			this.bStart.UseVisualStyleBackColor = true;
			this.bStart.Click += new System.EventHandler(this.ButStart_Click);
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.comboBoxB);
			this.groupBox2.Dock = System.Windows.Forms.DockStyle.Top;
			this.groupBox2.Location = new System.Drawing.Point(3, 50);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(258, 46);
			this.groupBox2.TabIndex = 19;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Black";
			// 
			// comboBoxB
			// 
			this.comboBoxB.Dock = System.Windows.Forms.DockStyle.Top;
			this.comboBoxB.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxB.FormattingEnabled = true;
			this.comboBoxB.Location = new System.Drawing.Point(3, 16);
			this.comboBoxB.Name = "comboBoxB";
			this.comboBoxB.Size = new System.Drawing.Size(252, 21);
			this.comboBoxB.Sorted = true;
			this.comboBoxB.TabIndex = 1;
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.comboBoxW);
			this.groupBox1.Dock = System.Windows.Forms.DockStyle.Top;
			this.groupBox1.Location = new System.Drawing.Point(3, 3);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(258, 47);
			this.groupBox1.TabIndex = 18;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "White";
			// 
			// comboBoxW
			// 
			this.comboBoxW.Dock = System.Windows.Forms.DockStyle.Top;
			this.comboBoxW.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxW.Location = new System.Drawing.Point(3, 16);
			this.comboBoxW.Name = "comboBoxW";
			this.comboBoxW.Size = new System.Drawing.Size(252, 21);
			this.comboBoxW.Sorted = true;
			this.comboBoxW.TabIndex = 2;
			// 
			// tabPageTraining
			// 
			this.tabPageTraining.Controls.Add(this.tableLayoutPanel1);
			this.tabPageTraining.Controls.Add(this.labGames);
			this.tabPageTraining.Controls.Add(this.butTraining);
			this.tabPageTraining.Controls.Add(this.groupBox4);
			this.tabPageTraining.Controls.Add(this.groupBox3);
			this.tabPageTraining.Location = new System.Drawing.Point(4, 25);
			this.tabPageTraining.Name = "tabPageTraining";
			this.tabPageTraining.Padding = new System.Windows.Forms.Padding(3);
			this.tabPageTraining.Size = new System.Drawing.Size(264, 547);
			this.tabPageTraining.TabIndex = 1;
			this.tabPageTraining.Text = "Training";
			this.tabPageTraining.UseVisualStyleBackColor = true;
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
			this.tableLayoutPanel1.ColumnCount = 5;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
			this.tableLayoutPanel1.Controls.Add(this.label15, 4, 2);
			this.tableLayoutPanel1.Controls.Add(this.label14, 3, 2);
			this.tableLayoutPanel1.Controls.Add(this.label13, 2, 2);
			this.tableLayoutPanel1.Controls.Add(this.label12, 1, 2);
			this.tableLayoutPanel1.Controls.Add(this.label11, 0, 2);
			this.tableLayoutPanel1.Controls.Add(this.label10, 4, 1);
			this.tableLayoutPanel1.Controls.Add(this.label9, 3, 1);
			this.tableLayoutPanel1.Controls.Add(this.label8, 2, 1);
			this.tableLayoutPanel1.Controls.Add(this.label7, 1, 1);
			this.tableLayoutPanel1.Controls.Add(this.label5, 4, 0);
			this.tableLayoutPanel1.Controls.Add(this.label4, 3, 0);
			this.tableLayoutPanel1.Controls.Add(this.label3, 2, 0);
			this.tableLayoutPanel1.Controls.Add(this.label2, 1, 0);
			this.tableLayoutPanel1.Controls.Add(this.label6, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this.label16, 0, 1);
			this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Top;
			this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 158);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 3;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
			this.tableLayoutPanel1.Size = new System.Drawing.Size(258, 100);
			this.tableLayoutPanel1.TabIndex = 25;
			// 
			// label15
			// 
			this.label15.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.label15.AutoSize = true;
			this.label15.Location = new System.Drawing.Point(208, 67);
			this.label15.Name = "label15";
			this.label15.Size = new System.Drawing.Size(46, 32);
			this.label15.TabIndex = 14;
			this.label15.Text = "label15";
			this.label15.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label14
			// 
			this.label14.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.label14.AutoSize = true;
			this.label14.Location = new System.Drawing.Point(158, 67);
			this.label14.Name = "label14";
			this.label14.Size = new System.Drawing.Size(43, 32);
			this.label14.TabIndex = 13;
			this.label14.Text = "label14";
			this.label14.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label13
			// 
			this.label13.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.label13.AutoSize = true;
			this.label13.Location = new System.Drawing.Point(108, 67);
			this.label13.Name = "label13";
			this.label13.Size = new System.Drawing.Size(43, 32);
			this.label13.TabIndex = 12;
			this.label13.Text = "label13";
			this.label13.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label12
			// 
			this.label12.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.label12.AutoSize = true;
			this.label12.Location = new System.Drawing.Point(58, 67);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(43, 32);
			this.label12.TabIndex = 11;
			this.label12.Text = "label12";
			this.label12.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label11
			// 
			this.label11.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.label11.AutoSize = true;
			this.label11.Location = new System.Drawing.Point(4, 67);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(47, 32);
			this.label11.TabIndex = 10;
			this.label11.Text = "Teacher";
			this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label10
			// 
			this.label10.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.label10.AutoSize = true;
			this.label10.Location = new System.Drawing.Point(208, 34);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(46, 32);
			this.label10.TabIndex = 9;
			this.label10.Text = "label10";
			this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label9
			// 
			this.label9.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.label9.AutoSize = true;
			this.label9.Location = new System.Drawing.Point(158, 34);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(43, 32);
			this.label9.TabIndex = 8;
			this.label9.Text = "label9";
			this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label8
			// 
			this.label8.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.label8.AutoSize = true;
			this.label8.Location = new System.Drawing.Point(108, 34);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(43, 32);
			this.label8.TabIndex = 7;
			this.label8.Text = "label8";
			this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label7
			// 
			this.label7.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(58, 34);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(43, 32);
			this.label7.TabIndex = 6;
			this.label7.Text = "label7";
			this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label5
			// 
			this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(208, 1);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(46, 32);
			this.label5.TabIndex = 4;
			this.label5.Text = "Result";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label4
			// 
			this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(158, 1);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(43, 32);
			this.label4.TabIndex = 3;
			this.label4.Text = "Loose";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label3
			// 
			this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(108, 1);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(43, 32);
			this.label3.TabIndex = 2;
			this.label3.Text = "Draw";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label2
			// 
			this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(58, 1);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(43, 32);
			this.label2.TabIndex = 1;
			this.label2.Text = "Win";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label6
			// 
			this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(4, 1);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(47, 32);
			this.label6.TabIndex = 0;
			this.label6.Text = "Player";
			this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label16
			// 
			this.label16.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.label16.AutoSize = true;
			this.label16.Location = new System.Drawing.Point(4, 34);
			this.label16.Name = "label16";
			this.label16.Size = new System.Drawing.Size(47, 32);
			this.label16.TabIndex = 5;
			this.label16.Text = "Trained";
			this.label16.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// labGames
			// 
			this.labGames.Dock = System.Windows.Forms.DockStyle.Top;
			this.labGames.Location = new System.Drawing.Point(3, 134);
			this.labGames.Name = "labGames";
			this.labGames.Size = new System.Drawing.Size(258, 24);
			this.labGames.TabIndex = 22;
			this.labGames.Text = "Games 0";
			this.labGames.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// butTraining
			// 
			this.butTraining.Dock = System.Windows.Forms.DockStyle.Top;
			this.butTraining.Location = new System.Drawing.Point(3, 111);
			this.butTraining.Name = "butTraining";
			this.butTraining.Size = new System.Drawing.Size(258, 23);
			this.butTraining.TabIndex = 21;
			this.butTraining.Text = "Start";
			this.butTraining.UseVisualStyleBackColor = true;
			this.butTraining.Click += new System.EventHandler(this.ButTraining_Click);
			// 
			// groupBox4
			// 
			this.groupBox4.Controls.Add(this.labTrainTime);
			this.groupBox4.Controls.Add(this.comboBoxTeacher);
			this.groupBox4.Dock = System.Windows.Forms.DockStyle.Top;
			this.groupBox4.Location = new System.Drawing.Point(3, 50);
			this.groupBox4.Name = "groupBox4";
			this.groupBox4.Size = new System.Drawing.Size(258, 61);
			this.groupBox4.TabIndex = 20;
			this.groupBox4.TabStop = false;
			this.groupBox4.Text = "Teacher";
			// 
			// labTrainTime
			// 
			this.labTrainTime.Dock = System.Windows.Forms.DockStyle.Fill;
			this.labTrainTime.Location = new System.Drawing.Point(3, 37);
			this.labTrainTime.Name = "labTrainTime";
			this.labTrainTime.Size = new System.Drawing.Size(252, 21);
			this.labTrainTime.TabIndex = 24;
			this.labTrainTime.Text = "Time 1 s";
			this.labTrainTime.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// comboBoxTeacher
			// 
			this.comboBoxTeacher.Dock = System.Windows.Forms.DockStyle.Top;
			this.comboBoxTeacher.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxTeacher.Location = new System.Drawing.Point(3, 16);
			this.comboBoxTeacher.Name = "comboBoxTeacher";
			this.comboBoxTeacher.Size = new System.Drawing.Size(252, 21);
			this.comboBoxTeacher.Sorted = true;
			this.comboBoxTeacher.TabIndex = 2;
			// 
			// groupBox3
			// 
			this.groupBox3.Controls.Add(this.comboBoxTrained);
			this.groupBox3.Dock = System.Windows.Forms.DockStyle.Top;
			this.groupBox3.Location = new System.Drawing.Point(3, 3);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(258, 47);
			this.groupBox3.TabIndex = 19;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "Trained";
			// 
			// comboBoxTrained
			// 
			this.comboBoxTrained.Dock = System.Windows.Forms.DockStyle.Top;
			this.comboBoxTrained.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxTrained.Location = new System.Drawing.Point(3, 16);
			this.comboBoxTrained.Name = "comboBoxTrained";
			this.comboBoxTrained.Size = new System.Drawing.Size(252, 21);
			this.comboBoxTrained.Sorted = true;
			this.comboBoxTrained.TabIndex = 2;
			// 
			// pictureBox1
			// 
			this.pictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Left;
			this.pictureBox1.Location = new System.Drawing.Point(0, 0);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(576, 576);
			this.pictureBox1.TabIndex = 8;
			this.pictureBox1.TabStop = false;
			this.pictureBox1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.PictureBox1_MouseClick);
			// 
			// panel3
			// 
			this.panel3.BackColor = System.Drawing.Color.Silver;
			this.panel3.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.panel3.Controls.Add(this.labPonderB);
			this.panel3.Controls.Add(this.labNpsB);
			this.panel3.Controls.Add(this.labDepthB);
			this.panel3.Controls.Add(this.labScoreB);
			this.panel3.Controls.Add(this.labTimeB);
			this.panel3.Controls.Add(this.labNameB);
			this.panel3.Controls.Add(this.panBottom);
			this.panel3.Dock = System.Windows.Forms.DockStyle.Top;
			this.panel3.Location = new System.Drawing.Point(0, 630);
			this.panel3.Name = "panel3";
			this.panel3.Size = new System.Drawing.Size(848, 30);
			this.panel3.TabIndex = 10;
			// 
			// labPonderB
			// 
			this.labPonderB.BackColor = System.Drawing.Color.LightGray;
			this.labPonderB.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.labPonderB.Dock = System.Windows.Forms.DockStyle.Left;
			this.labPonderB.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
			this.labPonderB.ForeColor = System.Drawing.Color.Black;
			this.labPonderB.Location = new System.Drawing.Point(711, 0);
			this.labPonderB.Name = "labPonderB";
			this.labPonderB.Size = new System.Drawing.Size(137, 26);
			this.labPonderB.TabIndex = 8;
			this.labPonderB.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// labNpsB
			// 
			this.labNpsB.BackColor = System.Drawing.Color.LightGray;
			this.labNpsB.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.labNpsB.Dock = System.Windows.Forms.DockStyle.Left;
			this.labNpsB.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
			this.labNpsB.ForeColor = System.Drawing.Color.Black;
			this.labNpsB.Location = new System.Drawing.Point(574, 0);
			this.labNpsB.Name = "labNpsB";
			this.labNpsB.Size = new System.Drawing.Size(137, 26);
			this.labNpsB.TabIndex = 7;
			this.labNpsB.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// labDepthB
			// 
			this.labDepthB.BackColor = System.Drawing.Color.LightGray;
			this.labDepthB.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.labDepthB.Dock = System.Windows.Forms.DockStyle.Left;
			this.labDepthB.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
			this.labDepthB.ForeColor = System.Drawing.Color.Black;
			this.labDepthB.Location = new System.Drawing.Point(437, 0);
			this.labDepthB.Name = "labDepthB";
			this.labDepthB.Size = new System.Drawing.Size(137, 26);
			this.labDepthB.TabIndex = 6;
			this.labDepthB.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// labScoreB
			// 
			this.labScoreB.BackColor = System.Drawing.Color.LightGray;
			this.labScoreB.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.labScoreB.Dock = System.Windows.Forms.DockStyle.Left;
			this.labScoreB.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
			this.labScoreB.ForeColor = System.Drawing.Color.Black;
			this.labScoreB.Location = new System.Drawing.Point(300, 0);
			this.labScoreB.Name = "labScoreB";
			this.labScoreB.Size = new System.Drawing.Size(137, 26);
			this.labScoreB.TabIndex = 5;
			this.labScoreB.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// labTimeB
			// 
			this.labTimeB.BackColor = System.Drawing.Color.LightGray;
			this.labTimeB.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.labTimeB.Dock = System.Windows.Forms.DockStyle.Left;
			this.labTimeB.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
			this.labTimeB.ForeColor = System.Drawing.Color.Black;
			this.labTimeB.Location = new System.Drawing.Point(163, 0);
			this.labTimeB.Name = "labTimeB";
			this.labTimeB.Size = new System.Drawing.Size(137, 26);
			this.labTimeB.TabIndex = 4;
			this.labTimeB.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// labNameB
			// 
			this.labNameB.BackColor = System.Drawing.Color.LightGray;
			this.labNameB.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.labNameB.Dock = System.Windows.Forms.DockStyle.Left;
			this.labNameB.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
			this.labNameB.ForeColor = System.Drawing.Color.Black;
			this.labNameB.Location = new System.Drawing.Point(26, 0);
			this.labNameB.Name = "labNameB";
			this.labNameB.Size = new System.Drawing.Size(137, 26);
			this.labNameB.TabIndex = 3;
			this.labNameB.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// panBottom
			// 
			this.panBottom.BackColor = System.Drawing.Color.Silver;
			this.panBottom.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.panBottom.Dock = System.Windows.Forms.DockStyle.Left;
			this.panBottom.Location = new System.Drawing.Point(0, 0);
			this.panBottom.Name = "panBottom";
			this.panBottom.Size = new System.Drawing.Size(26, 26);
			this.panBottom.TabIndex = 0;
			// 
			// panel1
			// 
			this.panel1.BackColor = System.Drawing.Color.Black;
			this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.panel1.Controls.Add(this.labLast);
			this.panel1.Controls.Add(this.labMove);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel1.ForeColor = System.Drawing.Color.Gainsboro;
			this.panel1.Location = new System.Drawing.Point(0, 660);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(848, 38);
			this.panel1.TabIndex = 11;
			// 
			// labLast
			// 
			this.labLast.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.labLast.Dock = System.Windows.Forms.DockStyle.Fill;
			this.labLast.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
			this.labLast.ForeColor = System.Drawing.Color.Gainsboro;
			this.labLast.Location = new System.Drawing.Point(137, 0);
			this.labLast.Name = "labLast";
			this.labLast.Size = new System.Drawing.Size(707, 34);
			this.labLast.TabIndex = 2;
			this.labLast.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// labMove
			// 
			this.labMove.BackColor = System.Drawing.Color.Black;
			this.labMove.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.labMove.Dock = System.Windows.Forms.DockStyle.Left;
			this.labMove.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
			this.labMove.ForeColor = System.Drawing.Color.Gainsboro;
			this.labMove.Location = new System.Drawing.Point(0, 0);
			this.labMove.Name = "labMove";
			this.labMove.Size = new System.Drawing.Size(137, 34);
			this.labMove.TabIndex = 1;
			this.labMove.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// timerStart
			// 
			this.timerStart.Interval = 6000;
			this.timerStart.Tick += new System.EventHandler(this.TimerStart_Tick);
			// 
			// FormChess
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.Control;
			this.ClientSize = new System.Drawing.Size(848, 698);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.panel3);
			this.Controls.Add(this.panel4);
			this.Controls.Add(this.panel2);
			this.Controls.Add(this.menuStrip1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MainMenuStrip = this.menuStrip1;
			this.MaximizeBox = false;
			this.Name = "FormChess";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "RapChessGui";
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormChess_FormClosed);
			this.Load += new System.EventHandler(this.FormChess_Load);
			this.menuStrip1.ResumeLayout(false);
			this.menuStrip1.PerformLayout();
			this.panel2.ResumeLayout(false);
			this.panel4.ResumeLayout(false);
			this.tabControl1.ResumeLayout(false);
			this.tabPageNormal.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			this.groupBox1.ResumeLayout(false);
			this.tabPageTraining.ResumeLayout(false);
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel1.PerformLayout();
			this.groupBox4.ResumeLayout(false);
			this.groupBox3.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			this.panel3.ResumeLayout(false);
			this.panel1.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
		private System.Windows.Forms.ImageList imageList1;
		private System.Windows.Forms.Timer timer1;
		private System.Windows.Forms.MenuStrip menuStrip1;
		private System.Windows.Forms.ToolStripMenuItem newGameToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem playersToolStripMenuItem;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.Panel panel4;
		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.Panel panel3;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Label labLast;
		private System.Windows.Forms.Label labMove;
		private System.Windows.Forms.Panel panTop;
		private System.Windows.Forms.Panel panBottom;
		private System.Windows.Forms.Label labNameT;
		private System.Windows.Forms.Label labNameB;
		private System.Windows.Forms.Label labTimeT;
		private System.Windows.Forms.Label labTimeB;
		private System.Windows.Forms.Label labScoreT;
		private System.Windows.Forms.Label labScoreB;
		private System.Windows.Forms.Label labPonderT;
		private System.Windows.Forms.Label labNpsT;
		private System.Windows.Forms.Label labDepthT;
		private System.Windows.Forms.Label labPonderB;
		private System.Windows.Forms.Label labNpsB;
		private System.Windows.Forms.Label labDepthB;
		private System.Windows.Forms.ToolStripMenuItem backToolStripMenuItem;
		private System.Windows.Forms.TabControl tabControl1;
		private System.Windows.Forms.TabPage tabPageNormal;
		private System.Windows.Forms.TabPage tabPageTraining;
		private System.Windows.Forms.RichTextBox richTextBox1;
		private System.Windows.Forms.Button butStop;
		private System.Windows.Forms.Button bStart;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.ComboBox comboBoxB;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.ComboBox comboBoxW;
		private System.Windows.Forms.Button butTraining;
		private System.Windows.Forms.GroupBox groupBox4;
		private System.Windows.Forms.ComboBox comboBoxTeacher;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.ComboBox comboBoxTrained;
		private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
		private System.Windows.Forms.Timer timerStart;
		private System.Windows.Forms.Label labTrainTime;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.Label label15;
		private System.Windows.Forms.Label label14;
		private System.Windows.Forms.Label label13;
		private System.Windows.Forms.Label label12;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label16;
		private System.Windows.Forms.Label labGames;
	}
}

