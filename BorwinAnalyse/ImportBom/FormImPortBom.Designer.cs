namespace BorwinAnalyse.Forms
{
    partial class FormImPortBom
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.kryptonNavigator1 = new ComponentFactory.Krypton.Navigator.KryptonNavigator();
            this.kryptonPage1 = new ComponentFactory.Krypton.Navigator.KryptonPage();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.kryptonPanel1 = new ComponentFactory.Krypton.Toolkit.KryptonPanel();
            this.btnChange = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.btnDeleteBom = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.btnBomTitleSetting = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.dgv_BOM = new ComponentFactory.Krypton.Toolkit.KryptonDataGridView();
            this.kryptonPage2 = new ComponentFactory.Krypton.Navigator.KryptonPage();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.kryptonPanel2 = new ComponentFactory.Krypton.Toolkit.KryptonPanel();
            this.btnDeleteXY = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.btnXYTitleSetting = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.dgvXYData = new ComponentFactory.Krypton.Toolkit.KryptonDataGridView();
            this.kryptonPage3 = new ComponentFactory.Krypton.Navigator.KryptonPage();
            this.kryptonRichTextBox1 = new ComponentFactory.Krypton.Toolkit.KryptonRichTextBox();
            this.kryptonPanel3 = new ComponentFactory.Krypton.Toolkit.KryptonPanel();
            this.btnImport = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.btnAnalyFile = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.btnOpenFileXY = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.btnOpenFileBom = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.kryptonLabel2 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.txtXYPath = new ComponentFactory.Krypton.Toolkit.KryptonTextBox();
            this.kryptonLabel1 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.txtBomPath = new ComponentFactory.Krypton.Toolkit.KryptonTextBox();
            this.btnAnayRow = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonNavigator1)).BeginInit();
            this.kryptonNavigator1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPage1)).BeginInit();
            this.kryptonPage1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel1)).BeginInit();
            this.kryptonPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_BOM)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPage2)).BeginInit();
            this.kryptonPage2.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel2)).BeginInit();
            this.kryptonPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvXYData)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPage3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel3)).BeginInit();
            this.kryptonPanel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.kryptonNavigator1, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.kryptonRichTextBox1, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.kryptonPanel3, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 150F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1011, 797);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // kryptonNavigator1
            // 
            this.kryptonNavigator1.Bar.BarMapExtraText = ComponentFactory.Krypton.Navigator.MapKryptonPageText.ToolTipBody;
            this.kryptonNavigator1.Bar.BarMapImage = ComponentFactory.Krypton.Navigator.MapKryptonPageImage.Large;
            this.kryptonNavigator1.Bar.BarMapText = ComponentFactory.Krypton.Navigator.MapKryptonPageText.TextTitle;
            this.kryptonNavigator1.Bar.BarMultiline = ComponentFactory.Krypton.Navigator.BarMultiline.Expandline;
            this.kryptonNavigator1.Bar.CheckButtonStyle = ComponentFactory.Krypton.Toolkit.ButtonStyle.NavigatorStack;
            this.kryptonNavigator1.Bar.ItemMinimumSize = new System.Drawing.Size(40, 40);
            this.kryptonNavigator1.Bar.ItemSizing = ComponentFactory.Krypton.Navigator.BarItemSizing.Individual;
            this.kryptonNavigator1.Bar.TabBorderStyle = ComponentFactory.Krypton.Toolkit.TabBorderStyle.SlantOutsizeBoth;
            this.kryptonNavigator1.Bar.TabStyle = ComponentFactory.Krypton.Toolkit.TabStyle.HighProfile;
            this.kryptonNavigator1.Button.ButtonDisplayLogic = ComponentFactory.Krypton.Navigator.ButtonDisplayLogic.Context;
            this.kryptonNavigator1.Button.CloseButtonAction = ComponentFactory.Krypton.Navigator.CloseButtonAction.RemovePageAndDispose;
            this.kryptonNavigator1.Button.CloseButtonDisplay = ComponentFactory.Krypton.Navigator.ButtonDisplay.Hide;
            this.kryptonNavigator1.Button.ContextButtonAction = ComponentFactory.Krypton.Navigator.ContextButtonAction.SelectPage;
            this.kryptonNavigator1.Button.ContextButtonDisplay = ComponentFactory.Krypton.Navigator.ButtonDisplay.Logic;
            this.kryptonNavigator1.Button.ContextMenuMapImage = ComponentFactory.Krypton.Navigator.MapKryptonPageImage.Small;
            this.kryptonNavigator1.Button.ContextMenuMapText = ComponentFactory.Krypton.Navigator.MapKryptonPageText.TextTitle;
            this.kryptonNavigator1.Button.NextButtonAction = ComponentFactory.Krypton.Navigator.DirectionButtonAction.ModeAppropriateAction;
            this.kryptonNavigator1.Button.NextButtonDisplay = ComponentFactory.Krypton.Navigator.ButtonDisplay.Logic;
            this.kryptonNavigator1.Button.PreviousButtonAction = ComponentFactory.Krypton.Navigator.DirectionButtonAction.ModeAppropriateAction;
            this.kryptonNavigator1.Button.PreviousButtonDisplay = ComponentFactory.Krypton.Navigator.ButtonDisplay.Logic;
            this.kryptonNavigator1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.kryptonNavigator1.Group.GroupBackStyle = ComponentFactory.Krypton.Toolkit.PaletteBackStyle.ControlClient;
            this.kryptonNavigator1.Group.GroupBorderStyle = ComponentFactory.Krypton.Toolkit.PaletteBorderStyle.ControlClient;
            this.kryptonNavigator1.Header.HeaderStyleBar = ComponentFactory.Krypton.Toolkit.HeaderStyle.Secondary;
            this.kryptonNavigator1.Header.HeaderStylePrimary = ComponentFactory.Krypton.Toolkit.HeaderStyle.Primary;
            this.kryptonNavigator1.Header.HeaderStyleSecondary = ComponentFactory.Krypton.Toolkit.HeaderStyle.Secondary;
            this.kryptonNavigator1.Location = new System.Drawing.Point(3, 153);
            this.kryptonNavigator1.Name = "kryptonNavigator1";
            this.kryptonNavigator1.NavigatorMode = ComponentFactory.Krypton.Navigator.NavigatorMode.BarTabGroup;
            this.kryptonNavigator1.PageBackStyle = ComponentFactory.Krypton.Toolkit.PaletteBackStyle.ControlClient;
            this.kryptonNavigator1.Pages.AddRange(new ComponentFactory.Krypton.Navigator.KryptonPage[] {
            this.kryptonPage1,
            this.kryptonPage2,
            this.kryptonPage3});
            this.kryptonNavigator1.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Office2007Blue;
            this.kryptonNavigator1.Panel.PanelBackStyle = ComponentFactory.Krypton.Toolkit.PaletteBackStyle.PanelClient;
            this.kryptonNavigator1.SelectedIndex = 0;
            this.kryptonNavigator1.Size = new System.Drawing.Size(1005, 541);
            this.kryptonNavigator1.TabIndex = 0;
            this.kryptonNavigator1.Text = "kryptonNavigator1";
            // 
            // kryptonPage1
            // 
            this.kryptonPage1.AutoHiddenSlideSize = new System.Drawing.Size(200, 200);
            this.kryptonPage1.Controls.Add(this.tableLayoutPanel2);
            this.kryptonPage1.Flags = 65534;
            this.kryptonPage1.LastVisibleSet = true;
            this.kryptonPage1.MinimumSize = new System.Drawing.Size(50, 50);
            this.kryptonPage1.Name = "kryptonPage1";
            this.kryptonPage1.Size = new System.Drawing.Size(1003, 499);
            this.kryptonPage1.Text = "BOM信息";
            this.kryptonPage1.ToolTipStyle = ComponentFactory.Krypton.Toolkit.LabelStyle.ToolTip;
            this.kryptonPage1.ToolTipTitle = "Page ToolTip";
            this.kryptonPage1.UniqueName = "9012D9F442854BDEDE96BA959665F228";
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 1;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Controls.Add(this.kryptonPanel1, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.dgv_BOM, 0, 1);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 2;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(1003, 499);
            this.tableLayoutPanel2.TabIndex = 0;
            // 
            // kryptonPanel1
            // 
            this.kryptonPanel1.Controls.Add(this.btnAnayRow);
            this.kryptonPanel1.Controls.Add(this.btnChange);
            this.kryptonPanel1.Controls.Add(this.btnDeleteBom);
            this.kryptonPanel1.Controls.Add(this.btnBomTitleSetting);
            this.kryptonPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.kryptonPanel1.Location = new System.Drawing.Point(3, 3);
            this.kryptonPanel1.Name = "kryptonPanel1";
            this.kryptonPanel1.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Office2007Blue;
            this.kryptonPanel1.Size = new System.Drawing.Size(997, 74);
            this.kryptonPanel1.TabIndex = 0;
            // 
            // btnChange
            // 
            this.btnChange.Location = new System.Drawing.Point(349, 7);
            this.btnChange.Name = "btnChange";
            this.btnChange.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Office2007Blue;
            this.btnChange.Size = new System.Drawing.Size(172, 58);
            this.btnChange.TabIndex = 2;
            this.btnChange.Values.Image = global::BorwinAnalyse.Properties.Resources._23;
            this.btnChange.Values.Text = "修改";
            this.btnChange.Click += new System.EventHandler(this.btnChange_Click);
            // 
            // btnDeleteBom
            // 
            this.btnDeleteBom.Location = new System.Drawing.Point(195, 7);
            this.btnDeleteBom.Name = "btnDeleteBom";
            this.btnDeleteBom.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Office2007Blue;
            this.btnDeleteBom.Size = new System.Drawing.Size(148, 58);
            this.btnDeleteBom.TabIndex = 1;
            this.btnDeleteBom.Values.Image = global::BorwinAnalyse.Properties.Resources._21;
            this.btnDeleteBom.Values.Text = "删除行";
            this.btnDeleteBom.Click += new System.EventHandler(this.btnDeleteBom_Click);
            // 
            // btnBomTitleSetting
            // 
            this.btnBomTitleSetting.Location = new System.Drawing.Point(5, 7);
            this.btnBomTitleSetting.Name = "btnBomTitleSetting";
            this.btnBomTitleSetting.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Office2007Blue;
            this.btnBomTitleSetting.Size = new System.Drawing.Size(184, 58);
            this.btnBomTitleSetting.TabIndex = 0;
            this.btnBomTitleSetting.Values.Image = global::BorwinAnalyse.Properties.Resources._002_01;
            this.btnBomTitleSetting.Values.Text = "列标题设置";
            this.btnBomTitleSetting.Click += new System.EventHandler(this.btnBomTitleSetting_Click);
            // 
            // dgv_BOM
            // 
            this.dgv_BOM.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv_BOM.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgv_BOM.Location = new System.Drawing.Point(3, 83);
            this.dgv_BOM.Name = "dgv_BOM";
            this.dgv_BOM.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Office2007Blue;
            this.dgv_BOM.RowHeadersWidth = 53;
            this.dgv_BOM.RowTemplate.Height = 28;
            this.dgv_BOM.Size = new System.Drawing.Size(997, 413);
            this.dgv_BOM.TabIndex = 1;
            this.dgv_BOM.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgv_BOM_CellClick);
            // 
            // kryptonPage2
            // 
            this.kryptonPage2.AutoHiddenSlideSize = new System.Drawing.Size(200, 200);
            this.kryptonPage2.Controls.Add(this.tableLayoutPanel3);
            this.kryptonPage2.Flags = 65534;
            this.kryptonPage2.LastVisibleSet = true;
            this.kryptonPage2.MinimumSize = new System.Drawing.Size(50, 50);
            this.kryptonPage2.Name = "kryptonPage2";
            this.kryptonPage2.Size = new System.Drawing.Size(1003, 499);
            this.kryptonPage2.Text = "XY数据";
            this.kryptonPage2.ToolTipStyle = ComponentFactory.Krypton.Toolkit.LabelStyle.ToolTip;
            this.kryptonPage2.ToolTipTitle = "Page ToolTip";
            this.kryptonPage2.UniqueName = "10428354626942D309862DE3E57579CA";
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 1;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.Controls.Add(this.kryptonPanel2, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.dgvXYData, 0, 1);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 2;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(1003, 499);
            this.tableLayoutPanel3.TabIndex = 1;
            // 
            // kryptonPanel2
            // 
            this.kryptonPanel2.Controls.Add(this.btnDeleteXY);
            this.kryptonPanel2.Controls.Add(this.btnXYTitleSetting);
            this.kryptonPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.kryptonPanel2.Location = new System.Drawing.Point(3, 3);
            this.kryptonPanel2.Name = "kryptonPanel2";
            this.kryptonPanel2.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Office2007Blue;
            this.kryptonPanel2.Size = new System.Drawing.Size(997, 74);
            this.kryptonPanel2.TabIndex = 0;
            // 
            // btnDeleteXY
            // 
            this.btnDeleteXY.Location = new System.Drawing.Point(195, 7);
            this.btnDeleteXY.Name = "btnDeleteXY";
            this.btnDeleteXY.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Office2007Blue;
            this.btnDeleteXY.Size = new System.Drawing.Size(175, 58);
            this.btnDeleteXY.TabIndex = 2;
            this.btnDeleteXY.Values.Image = global::BorwinAnalyse.Properties.Resources._21;
            this.btnDeleteXY.Values.Text = "删除行";
            this.btnDeleteXY.Click += new System.EventHandler(this.btnDeleteXY_Click);
            // 
            // btnXYTitleSetting
            // 
            this.btnXYTitleSetting.Location = new System.Drawing.Point(5, 7);
            this.btnXYTitleSetting.Name = "btnXYTitleSetting";
            this.btnXYTitleSetting.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Office2007Blue;
            this.btnXYTitleSetting.Size = new System.Drawing.Size(184, 58);
            this.btnXYTitleSetting.TabIndex = 1;
            this.btnXYTitleSetting.Values.Image = global::BorwinAnalyse.Properties.Resources._002_01;
            this.btnXYTitleSetting.Values.Text = "列标题设置";
            this.btnXYTitleSetting.Click += new System.EventHandler(this.btnXYTitleSetting_Click);
            // 
            // dgvXYData
            // 
            this.dgvXYData.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvXYData.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvXYData.Location = new System.Drawing.Point(3, 83);
            this.dgvXYData.Name = "dgvXYData";
            this.dgvXYData.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Office2007Blue;
            this.dgvXYData.RowHeadersWidth = 53;
            this.dgvXYData.RowTemplate.Height = 28;
            this.dgvXYData.Size = new System.Drawing.Size(997, 413);
            this.dgvXYData.TabIndex = 1;
            this.dgvXYData.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvXYData_CellClick);
            // 
            // kryptonPage3
            // 
            this.kryptonPage3.AutoHiddenSlideSize = new System.Drawing.Size(200, 200);
            this.kryptonPage3.Flags = 65534;
            this.kryptonPage3.LastVisibleSet = true;
            this.kryptonPage3.MinimumSize = new System.Drawing.Size(50, 50);
            this.kryptonPage3.Name = "kryptonPage3";
            this.kryptonPage3.Size = new System.Drawing.Size(1003, 499);
            this.kryptonPage3.Text = "位号图";
            this.kryptonPage3.ToolTipStyle = ComponentFactory.Krypton.Toolkit.LabelStyle.ToolTip;
            this.kryptonPage3.ToolTipTitle = "Page ToolTip";
            this.kryptonPage3.UniqueName = "FE7C361621A245AE338F47E5099E02B6";
            // 
            // kryptonRichTextBox1
            // 
            this.kryptonRichTextBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.kryptonRichTextBox1.Location = new System.Drawing.Point(3, 700);
            this.kryptonRichTextBox1.Name = "kryptonRichTextBox1";
            this.kryptonRichTextBox1.Size = new System.Drawing.Size(1005, 94);
            this.kryptonRichTextBox1.TabIndex = 1;
            this.kryptonRichTextBox1.Text = "";
            // 
            // kryptonPanel3
            // 
            this.kryptonPanel3.Controls.Add(this.btnImport);
            this.kryptonPanel3.Controls.Add(this.btnAnalyFile);
            this.kryptonPanel3.Controls.Add(this.btnOpenFileXY);
            this.kryptonPanel3.Controls.Add(this.btnOpenFileBom);
            this.kryptonPanel3.Controls.Add(this.kryptonLabel2);
            this.kryptonPanel3.Controls.Add(this.txtXYPath);
            this.kryptonPanel3.Controls.Add(this.kryptonLabel1);
            this.kryptonPanel3.Controls.Add(this.txtBomPath);
            this.kryptonPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.kryptonPanel3.Location = new System.Drawing.Point(3, 3);
            this.kryptonPanel3.Name = "kryptonPanel3";
            this.kryptonPanel3.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Office2007Blue;
            this.kryptonPanel3.Size = new System.Drawing.Size(1005, 144);
            this.kryptonPanel3.TabIndex = 2;
            // 
            // btnImport
            // 
            this.btnImport.Location = new System.Drawing.Point(724, 27);
            this.btnImport.Name = "btnImport";
            this.btnImport.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Office2007Blue;
            this.btnImport.Size = new System.Drawing.Size(224, 75);
            this.btnImport.TabIndex = 7;
            this.btnImport.Values.Image = global::BorwinAnalyse.Properties.Resources.Download_24px;
            this.btnImport.Values.Text = "确定导入";
            this.btnImport.Click += new System.EventHandler(this.btnImport_Click);
            // 
            // btnAnalyFile
            // 
            this.btnAnalyFile.Location = new System.Drawing.Point(595, 26);
            this.btnAnalyFile.Name = "btnAnalyFile";
            this.btnAnalyFile.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Office2007Blue;
            this.btnAnalyFile.Size = new System.Drawing.Size(123, 76);
            this.btnAnalyFile.TabIndex = 6;
            this.btnAnalyFile.Values.Image = global::BorwinAnalyse.Properties.Resources._186;
            this.btnAnalyFile.Values.Text = "刷新";
            this.btnAnalyFile.Click += new System.EventHandler(this.btnAnalyFile_Click);
            // 
            // btnOpenFileXY
            // 
            this.btnOpenFileXY.Location = new System.Drawing.Point(510, 63);
            this.btnOpenFileXY.Name = "btnOpenFileXY";
            this.btnOpenFileXY.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Office2007Blue;
            this.btnOpenFileXY.Size = new System.Drawing.Size(79, 51);
            this.btnOpenFileXY.TabIndex = 5;
            this.btnOpenFileXY.Values.Image = global::BorwinAnalyse.Properties.Resources.Open_24px;
            this.btnOpenFileXY.Values.Text = "";
            this.btnOpenFileXY.Click += new System.EventHandler(this.btnOpenFileXY_Click);
            // 
            // btnOpenFileBom
            // 
            this.btnOpenFileBom.Location = new System.Drawing.Point(510, 9);
            this.btnOpenFileBom.Name = "btnOpenFileBom";
            this.btnOpenFileBom.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Office2007Blue;
            this.btnOpenFileBom.Size = new System.Drawing.Size(79, 51);
            this.btnOpenFileBom.TabIndex = 4;
            this.btnOpenFileBom.Values.Image = global::BorwinAnalyse.Properties.Resources.Open_24px;
            this.btnOpenFileBom.Values.Text = "";
            this.btnOpenFileBom.Click += new System.EventHandler(this.btnOpenFileBom_Click);
            // 
            // kryptonLabel2
            // 
            this.kryptonLabel2.Location = new System.Drawing.Point(28, 69);
            this.kryptonLabel2.Name = "kryptonLabel2";
            this.kryptonLabel2.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Office2007Blue;
            this.kryptonLabel2.Size = new System.Drawing.Size(103, 38);
            this.kryptonLabel2.StateCommon.ShortText.Font = new System.Drawing.Font("宋体", 16.128F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.kryptonLabel2.StateCommon.ShortText.ImageStyle = ComponentFactory.Krypton.Toolkit.PaletteImageStyle.Inherit;
            this.kryptonLabel2.StateCommon.ShortText.Trim = ComponentFactory.Krypton.Toolkit.PaletteTextTrim.Inherit;
            this.kryptonLabel2.TabIndex = 3;
            this.kryptonLabel2.Values.Text = "XY文件";
            // 
            // txtXYPath
            // 
            this.txtXYPath.Location = new System.Drawing.Point(160, 68);
            this.txtXYPath.Name = "txtXYPath";
            this.txtXYPath.Size = new System.Drawing.Size(344, 29);
            this.txtXYPath.TabIndex = 2;
            // 
            // kryptonLabel1
            // 
            this.kryptonLabel1.Location = new System.Drawing.Point(10, 26);
            this.kryptonLabel1.Name = "kryptonLabel1";
            this.kryptonLabel1.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Office2007Blue;
            this.kryptonLabel1.Size = new System.Drawing.Size(117, 38);
            this.kryptonLabel1.StateCommon.ShortText.Font = new System.Drawing.Font("宋体", 16.128F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.kryptonLabel1.StateCommon.ShortText.ImageStyle = ComponentFactory.Krypton.Toolkit.PaletteImageStyle.Inherit;
            this.kryptonLabel1.StateCommon.ShortText.Trim = ComponentFactory.Krypton.Toolkit.PaletteTextTrim.Inherit;
            this.kryptonLabel1.TabIndex = 1;
            this.kryptonLabel1.Values.Text = "BOM文件";
            // 
            // txtBomPath
            // 
            this.txtBomPath.Location = new System.Drawing.Point(160, 23);
            this.txtBomPath.Name = "txtBomPath";
            this.txtBomPath.Size = new System.Drawing.Size(344, 29);
            this.txtBomPath.TabIndex = 0;
            // 
            // btnAnayRow
            // 
            this.btnAnayRow.Location = new System.Drawing.Point(527, 7);
            this.btnAnayRow.Name = "btnAnayRow";
            this.btnAnayRow.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Office2007Blue;
            this.btnAnayRow.Size = new System.Drawing.Size(287, 58);
            this.btnAnayRow.TabIndex = 3;
            this.btnAnayRow.Values.Image = global::BorwinAnalyse.Properties.Resources._147;
            this.btnAnayRow.Values.Text = "分析选中行";
            this.btnAnayRow.Click += new System.EventHandler(this.btnAnayRow_Click);
            // 
            // FormImPortBom
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1011, 797);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "FormImPortBom";
            this.Text = "导入BOM";
            this.tableLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.kryptonNavigator1)).EndInit();
            this.kryptonNavigator1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPage1)).EndInit();
            this.kryptonPage1.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel1)).EndInit();
            this.kryptonPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgv_BOM)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPage2)).EndInit();
            this.kryptonPage2.ResumeLayout(false);
            this.tableLayoutPanel3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel2)).EndInit();
            this.kryptonPanel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvXYData)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPage3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel3)).EndInit();
            this.kryptonPanel3.ResumeLayout(false);
            this.kryptonPanel3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private ComponentFactory.Krypton.Navigator.KryptonNavigator kryptonNavigator1;
        private ComponentFactory.Krypton.Navigator.KryptonPage kryptonPage1;
        private ComponentFactory.Krypton.Navigator.KryptonPage kryptonPage2;
        private ComponentFactory.Krypton.Navigator.KryptonPage kryptonPage3;
        private ComponentFactory.Krypton.Toolkit.KryptonRichTextBox kryptonRichTextBox1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private ComponentFactory.Krypton.Toolkit.KryptonPanel kryptonPanel1;
        private ComponentFactory.Krypton.Toolkit.KryptonDataGridView dgv_BOM;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private ComponentFactory.Krypton.Toolkit.KryptonPanel kryptonPanel2;
        private ComponentFactory.Krypton.Toolkit.KryptonDataGridView dgvXYData;
        private ComponentFactory.Krypton.Toolkit.KryptonButton btnBomTitleSetting;
        private ComponentFactory.Krypton.Toolkit.KryptonButton btnXYTitleSetting;
        private ComponentFactory.Krypton.Toolkit.KryptonPanel kryptonPanel3;
        private ComponentFactory.Krypton.Toolkit.KryptonButton btnOpenFileBom;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel2;
        private ComponentFactory.Krypton.Toolkit.KryptonTextBox txtXYPath;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel1;
        private ComponentFactory.Krypton.Toolkit.KryptonTextBox txtBomPath;
        private ComponentFactory.Krypton.Toolkit.KryptonButton btnAnalyFile;
        private ComponentFactory.Krypton.Toolkit.KryptonButton btnOpenFileXY;
        private ComponentFactory.Krypton.Toolkit.KryptonButton btnImport;
        private ComponentFactory.Krypton.Toolkit.KryptonButton btnDeleteBom;
        private ComponentFactory.Krypton.Toolkit.KryptonButton btnDeleteXY;
        private ComponentFactory.Krypton.Toolkit.KryptonButton btnChange;
        private ComponentFactory.Krypton.Toolkit.KryptonButton btnAnayRow;
    }
}