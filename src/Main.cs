using FileSearch;
using System;
using System.IO;
using System.Threading;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

namespace FSUI {
	public class FileSearchUI : Form {
		private Panel WhereOuterPanel;
		private GroupBox WhereGroupBox;
		private FlowLayoutPanel QueryPanel;
		private FlowLayoutPanel RightPanel;
		private FlowLayoutPanel WhereInnerPanel;
		private List<CheckBox> SelectCheckBoxes;
		private FileSearchCommand Command;
		private TextBox FromTextBox;
		private OpenFileDialog FileSelect;
		private FolderBrowserDialog DirectorySelect;
		private int ClauseWidth;

		public static void Main(String[] args) {
			Application.Run(new FileSearchUI());
		}
		public FileSearchUI() {
			Command = new FileSearchCommand(
				new SelectClause(),
				new FromClause()
			);
			
			CreateGUI();
		}

		// Builds the entre GUI
		private void CreateGUI() {
			ClauseWidth = 678;

			this.Name = "File Search";
			this.Text = "File Search";
			this.WindowState = FormWindowState.Maximized;

			SplitContainer MainPanel = new SplitContainer();
			MainPanel.Dock = DockStyle.Fill;
			MainPanel.BorderStyle = BorderStyle.FixedSingle;
			MainPanel.Panel1.Padding = new Padding(10);
			this.Controls.Add(MainPanel);

			RightPanel = new FlowLayoutPanel();
			RightPanel.Dock = DockStyle.Fill;
			RightPanel.AutoSize = true;
			RightPanel.AutoScroll = true;
			RightPanel.FlowDirection = FlowDirection.TopDown;
			MainPanel.Panel2.Controls.Add(RightPanel);

			Panel LeftPanel = new Panel();
			LeftPanel.BorderStyle = BorderStyle.FixedSingle;
			LeftPanel.Dock = DockStyle.Fill;
			MainPanel.Panel1.Controls.Add(LeftPanel);

			Panel QueryOuterPanel = new Panel();
			QueryOuterPanel.Dock = DockStyle.Fill;
			QueryOuterPanel.Padding = new Padding(10);
			LeftPanel.Controls.Add(QueryOuterPanel);

			QueryPanel = new FlowLayoutPanel();
			QueryPanel.FlowDirection = FlowDirection.TopDown;
			QueryPanel.Dock = DockStyle.Fill;
			QueryPanel.WrapContents = false;
			QueryPanel.AutoScroll = true;
			QueryPanel.BorderStyle = BorderStyle.FixedSingle;
			QueryOuterPanel.Controls.Add(QueryPanel);

			Panel RunQueryButtonPanel = new Panel();
			RunQueryButtonPanel.Dock = DockStyle.Bottom;
			LeftPanel.Controls.Add(RunQueryButtonPanel);

			Button RunQueryButton = new Button();
			RunQueryButton.Text = "Run Query";
			RunQueryButton.AutoSize = true;
			RunQueryButton.Dock = DockStyle.None;
			RunQueryButton.Anchor = AnchorStyles.None;
			RunQueryButton.Click += RunQuery;
			RunQueryButtonPanel.Controls.Add(RunQueryButton);

			RunQueryButtonPanel.Padding = new Padding(
				0,
				(RunQueryButtonPanel.Height - RunQueryButton.Height) / 2,
				0,
				0
			);

			SetupSelect();

			SetupFrom();
			
			MainPanel.SplitterDistance = MainPanel.Width / 2;

			SetupWhere();
		}
		// Builds the SELECT box
		private void SetupSelect() {

			GroupBox SelectGroupBox = new GroupBox();
			SelectGroupBox.Text = "SELECT";
			SelectGroupBox.Width = ClauseWidth;
			SelectGroupBox.Padding = new Padding(10);
			QueryPanel.Controls.Add(SelectGroupBox);

			FlowLayoutPanel SelectInnerPanel = new FlowLayoutPanel();
			SelectInnerPanel.FlowDirection = FlowDirection.TopDown;
			SelectGroupBox.Controls.Add(SelectInnerPanel);

			ResizeGroupBoxBefore(SelectGroupBox);

			List<FlowLayoutPanel> SelectRows = new List<FlowLayoutPanel>();
			SelectRows.Add(new FlowLayoutPanel());
			SelectRows[0].FlowDirection = FlowDirection.LeftToRight;
			SelectRows[0].AutoSize = true;
			SelectInnerPanel.Controls.Add(SelectRows[0]);

			SelectCheckBoxes = new List<CheckBox>();

			FileSearchType.FileObjectAttribute[] Attributes = FileSearchType.GetSelectAttributes();
			int row_index = 0;
			int column_index = 0;
			int SelectPadd = SelectInnerPanel.Padding.Left + SelectInnerPanel.Padding.Right;
			foreach (FileSearchType.FileObjectAttribute attr in Attributes) {
				SelectCheckBoxes.Add(new CheckBox());
				SelectCheckBoxes[column_index].CheckedChanged += AlterSelection;
				SelectCheckBoxes[column_index].Text = FileSearchType.ToString(attr);
				SelectCheckBoxes[column_index].AutoSize = true;
				SelectRows[row_index].Controls.Add(SelectCheckBoxes[column_index]);
				if (SelectRows[row_index].Width + SelectPadd >= SelectGroupBox.Width) {
					++row_index;
					SelectRows.Add(new FlowLayoutPanel());
					SelectRows[row_index].FlowDirection = FlowDirection.LeftToRight;
					SelectRows[row_index].AutoSize = true;
					SelectInnerPanel.Controls.Add(SelectRows[row_index]);
				}
				++column_index;
			}
			ResizeGroupBoxAfter(SelectGroupBox);
		}
		// Builds the FROM box
		private void SetupFrom() {
			GroupBox FromGroupBox = new GroupBox();
			FromGroupBox.Text = "FROM";
			FromGroupBox.Padding = new Padding(10);
			FromGroupBox.Width = ClauseWidth;
			QueryPanel.Controls.Add(FromGroupBox);

			FlowLayoutPanel FromInnerPanel = new FlowLayoutPanel();
			FromInnerPanel.WrapContents = false;
			FromInnerPanel.FlowDirection = FlowDirection.TopDown;
			FromGroupBox.Controls.Add(FromInnerPanel);

			ResizeGroupBoxBefore(FromGroupBox);

			List<Panel> FromRows = new List<Panel>();
			FromRows.Add(new Panel());
			FromInnerPanel.Controls.Add(FromRows[0]);

			FromTextBox = new TextBox();
			FromTextBox.TextChanged += ChangeFromDirectory;
			FromTextBox.Dock = DockStyle.Fill;
			FromRows[0].Padding = new Padding(5);
			FromRows[0].Width = FromInnerPanel.Width - FromInnerPanel.Padding.Left - FromInnerPanel.Padding.Right;
			FromRows[0].Controls.Add(FromTextBox);

			Button FromSelectFileButton = new Button();
			FromSelectFileButton.Text = "Select File";
			FromSelectFileButton.Dock = DockStyle.Right;
			FromSelectFileButton.Click += SelectFile;
			FromSelectFileButton.AutoSize = true;
			FromRows[0].Controls.Add(FromSelectFileButton);

			Button FromSelectDirectoryButton = new Button();
			FromSelectDirectoryButton.Text = "Select Folder";
			FromSelectDirectoryButton.Dock = DockStyle.Right;
			FromSelectDirectoryButton.Click += SelectFolder;
			FromSelectDirectoryButton.AutoSize = true;
			FromRows[0].Controls.Add(FromSelectDirectoryButton);
			FromRows[0].Height = FromTextBox.Height + FromRows[0].Padding.Top + FromRows[0].Padding.Bottom;

			ResizeGroupBoxAfter(FromGroupBox);
		}
		// Builds the WHERE box
		private void SetupWhere() {
			WhereOuterPanel = new Panel();
			WhereOuterPanel.AutoSize = true;
			QueryPanel.Controls.Add(WhereOuterPanel);

			Button AddWhereClauseButton = new Button();
			AddWhereClauseButton.Text = "Add Where Clause";
			AddWhereClauseButton.AutoSize = true;
			AddWhereClauseButton.Click += ToggleWhereClause;
			WhereOuterPanel.Controls.Add(AddWhereClauseButton);
		}
		// Resizes the given group box based on the contents of it added at the start
		private void ResizeGroupBoxBefore(GroupBox gb) {
			Control c = gb.Controls[0];
			c.Location = new Point(
				gb.Padding.Left,
				gb.Padding.Top + 10
			);
			c.Width = gb.Width - gb.Padding.Right - gb.Padding.Left;
		}
		// Resizes the given group box based on the contents of it once all the necessary
		// elements are added
		private void ResizeGroupBoxAfter(GroupBox gb) {;
			Control c = gb.Controls[0];
			int h = c.Padding.Top + c.Padding.Bottom;
			foreach (Control ctrl in c.Controls) {
				if (ctrl.Visible == false) {
					h += ctrl.Height;
				}
			}
			c.Height = h;
			gb.Height = c.Height + gb.Padding.Top + gb.Padding.Bottom + 10;
		}
		// Triggered whenever the user changes the attributes selected in the SELECT
		// box
		private void AlterSelection(object sender, EventArgs e) {
			CheckBox c = sender as CheckBox;
			int index = SelectCheckBoxes.IndexOf(c);
			if (c.Checked) {
				Command.Select.Add(FileSearchType.AttributeMap(c.Text));
			} else {
				Command.Select.Remove(FileSearchType.AttributeMap(c.Text));
			}
		}
		// Triggered whenever the user clicks the "Select Folder" button
		private void SelectFolder(object sender, EventArgs e) {
			string text = "";
			DialogResult result = DialogResult.No;
			Thread worker = new Thread(() => {
				using (DirectorySelect = new FolderBrowserDialog()) {
					DirectorySelect.SelectedPath = System.IO.Path.GetDirectoryName(Application.ExecutablePath);
					result = DirectorySelect.ShowDialog();
					if (result == DialogResult.OK) {
						text = DirectorySelect.SelectedPath;
					}
				}
			});
			worker.SetApartmentState(ApartmentState.STA);
			worker.Start();
			worker.Join();
			if (result == DialogResult.OK) {
				FromTextBox.Text = text;
			}
		}
		// Triggered whenever the user clicks the "Select File" button
		private void SelectFile(object sender, EventArgs e) {
			using (FileSelect = new OpenFileDialog()) {

				FileSelect.InitialDirectory = System.IO.Path.GetDirectoryName(Application.ExecutablePath);
				FileSelect.RestoreDirectory = true;
				FileSelect.ShowHelp = true;
				
				if (FileSelect.ShowDialog() == DialogResult.OK) {
					FromTextBox.Text = FileSelect.FileName;
				}
			}
		}
		// Triggered whenever the user clicks the "Run Query" button
		private void RunQuery(object sender, EventArgs e) {

			// Clear the results panel
			RightPanel.Controls.Clear();

			// Only run the query if the command is complete
			if (Command.Complete()) {

				// Add the temporary "Running..." label until the results come back
				RightPanel.Controls.Add(new Label {
					Text = "Running...",
					AutoSize = true
				});
				List<string> lines = new List<string>();
				Thread worker = new Thread(() => {
					lines = Command.Run();
				});
				worker.SetApartmentState(ApartmentState.STA);
				worker.Start();
				worker.Join();

				// Remove temporyary label
				RightPanel.Controls.Remove(RightPanel.Controls[0]);

				// Add results to results panel
				foreach (string line in lines) {
					RightPanel.Controls.Add(new Label {
						Text = line,
						AutoSize = true
					});
				}
			} else {

				// Let the user know that the command is incomplete
				RightPanel.Controls.Add(new Label {
					Text = "Error: Missing Arguments",
					AutoSize = true
				});
			}
		}
		// Triggered whenever the text in the textbox in the FROM box changes
		private void ChangeFromDirectory(object sender, EventArgs e) {
			Command.From.SetPath((sender as TextBox).Text);
		}
		// Triggered whenever the WHERE box is added or removed
		private void ToggleWhereClause(object sender, EventArgs e) {
			if (Command.Where == null) {
				Command.Where = new WhereClause();
				foreach (Control c in WhereOuterPanel.Controls) {
					WhereOuterPanel.Controls.Remove(c);
				}

				Button RemoveWhereClauseButton = new Button();
				RemoveWhereClauseButton.Text = "\u00D7";
				RemoveWhereClauseButton.Click += ToggleWhereClause;
				RemoveWhereClauseButton.Width = RemoveWhereClauseButton.Height;
				WhereOuterPanel.Controls.Add(RemoveWhereClauseButton);

				WhereGroupBox = new GroupBox();
				WhereGroupBox.Width = ClauseWidth;
				WhereGroupBox.Text = "WHERE";
				WhereOuterPanel.Controls.Add(WhereGroupBox);
				RemoveWhereClauseButton.Location = new Point(
					WhereGroupBox.Width - RemoveWhereClauseButton.Width - 4,7
				);

				WhereInnerPanel = new FlowLayoutPanel();
				WhereInnerPanel.FlowDirection = FlowDirection.TopDown;
				WhereInnerPanel.WrapContents = false;
				WhereInnerPanel.AutoSize = true;
				WhereGroupBox.Controls.Add(WhereInnerPanel);

				ResizeGroupBoxBefore(WhereGroupBox);

				Label WhereOperatorLabel = new Label();
				WhereOperatorLabel.AutoSize = true;
				WhereOperatorLabel.Text = "Operation:";
				WhereInnerPanel.Controls.Add(WhereOperatorLabel);

				ComboBox WhereOperatorSelect = new ComboBox();
				WhereOperatorSelect.AutoSize = true;
				WhereOperatorSelect.DropDownStyle = ComboBoxStyle.DropDownList;
				WhereOperatorSelect.Items.AddRange(WhereClause.AllowedOperators());
				WhereOperatorSelect.SelectedIndex = 0;
				WhereOperatorSelect.TextChanged += SelectClauseOperator;
				WhereInnerPanel.Controls.Add(WhereOperatorSelect);

				ResizeGroupBoxAfter(WhereGroupBox);

				WhereOuterPanel.ResumeLayout();

				SelectClauseOperator(WhereOperatorSelect, EventArgs.Empty);
			} else {
				Command.Where = null;
				foreach (Control c in WhereOuterPanel.Controls) {
					WhereOuterPanel.Controls.Remove(c);
				}
				WhereOuterPanel.Controls.Remove(WhereGroupBox);

				Button AddWhereClauseButton = new Button();
				AddWhereClauseButton.Text = "Add Where Clause";
				AddWhereClauseButton.AutoSize = true;
				AddWhereClauseButton.Click += ToggleWhereClause;
				WhereOuterPanel.Controls.Add(AddWhereClauseButton);
			}
		}
		// Triggered whenever the ComboBox for the operator in the WHERE box is changed
		private void SelectClauseOperator(object sender, EventArgs e) {
			string s = (string) ((ComboBox)sender).SelectedItem;
			switch (s) {
				case "CONTAINS":
					Command.Where.SubClause = new ContainsClause();
					Command.Where.SubClause.Args[0] = new StringLiteralClause();
					Command.Where.SubClause.Args[1] = new StringLiteralClause();
					WhereInnerPanel.SuspendLayout();

					// Remove all options from where clause
					for (int i = 2; i < WhereInnerPanel.Controls.Count; i++) {
						WhereInnerPanel.Controls.Remove(WhereInnerPanel.Controls[i]);
					}

					{
						Button[] CollapseBtns = new Button[2];
						Panel[] HeaderPanels = new Panel[2];
						Label[] HeaderLabels = new Label[2];
						FlowLayoutPanel[] BtnCtrlPanels = new FlowLayoutPanel[2];
						RadioButton[,] ToggleButtons = new RadioButton[2,2];
						Panel[] InputBoxes = new Panel[2];

						HeaderPanels[0] = new Panel {
							Width = WhereInnerPanel.Width
								- WhereInnerPanel.Padding.Left
								- WhereInnerPanel.Padding.Right
								- 5,
							Margin = new Padding(0),
							BorderStyle = BorderStyle.FixedSingle
						};

						CollapseBtns[0] = new Button {
							Text = "+",
							AutoSize = true,
							Tag = "Collapse-Button-1"
						};
						CollapseBtns[0].Width = CollapseBtns[0].Height;
						CollapseBtns[0].Location = new Point(
							HeaderPanels[0].Width - CollapseBtns[0].Width - 5,3
						);

						HeaderLabels[0] = new Label {
							Text = "Argument 1",
							AutoSize = true
						};

						BtnCtrlPanels[0] = new FlowLayoutPanel {
							FlowDirection = FlowDirection.LeftToRight,
							Visible = false,
							Tag = "Collapse-Button-1"
						};

						ToggleButtons[0,0] = new RadioButton {
							Text = "Manual Input",
							Tag = "Arg-1",
							Checked = true,
							AutoSize = true
						};

						ToggleButtons[0,1] = new RadioButton {
							Text = "File Attribute",
							Tag = "Arg-1",
							AutoSize = true
						};

						InputBoxes[0] = new Panel {
							Tag = "Arg-1 Collapse-Button-1",
							Width = WhereInnerPanel.Width
								- WhereInnerPanel.Padding.Left
								- WhereInnerPanel.Padding.Right
								- 5,
							Visible = false
						};

						HeaderPanels[1] = new Panel {
							Width = WhereInnerPanel.Width
								- WhereInnerPanel.Padding.Left
								- WhereInnerPanel.Padding.Right
								- 5,
							Margin = new Padding(0),
							BorderStyle = BorderStyle.FixedSingle
						};

						CollapseBtns[1] = new Button {
							Text = "+",
							AutoSize = true,
							Tag = "Collapse-Button-2"
						};
						CollapseBtns[1].Width = CollapseBtns[1].Height;
						CollapseBtns[1].Location = new Point(
							HeaderPanels[1].Width - CollapseBtns[1].Width - 5,3
						);

						HeaderLabels[1] = new Label {
							Text = "Argument 2",
							AutoSize = true
						};

						BtnCtrlPanels[1] = new FlowLayoutPanel {
							FlowDirection = FlowDirection.LeftToRight,
							Visible = false,
							Tag = "Collapse-Button-2"
						};

						ToggleButtons[1,0] = new RadioButton {
							Text = "Manual Input",
							Tag = "Arg-2",
							Checked = true,
							AutoSize = true
						};

						ToggleButtons[1,1] = new RadioButton {
							Text = "File Attribute",
							Tag = "Arg-2",
							AutoSize = true
						};

						InputBoxes[1] = new Panel {
							Tag = "Arg-2 Collapse-Button-2",
							Width = WhereInnerPanel.Width
								- WhereInnerPanel.Padding.Left
								- WhereInnerPanel.Padding.Right
								- 5,
							Visible = false
						};

						WhereInnerPanel.Controls.Add(HeaderPanels[0]);
						HeaderPanels[0].Controls.Add(CollapseBtns[0]);
						HeaderPanels[0].Controls.Add(HeaderLabels[0]);
						WhereInnerPanel.Controls.Add(BtnCtrlPanels[0]);
						BtnCtrlPanels[0].Controls.Add(ToggleButtons[0,0]);
						BtnCtrlPanels[0].Controls.Add(ToggleButtons[0,1]);
						WhereInnerPanel.Controls.Add(InputBoxes[0]);

						WhereInnerPanel.Controls.Add(HeaderPanels[1]);
						HeaderPanels[1].Controls.Add(CollapseBtns[1]);
						HeaderPanels[1].Controls.Add(HeaderLabels[1]);
						WhereInnerPanel.Controls.Add(BtnCtrlPanels[1]);
						BtnCtrlPanels[1].Controls.Add(ToggleButtons[1,0]);
						BtnCtrlPanels[1].Controls.Add(ToggleButtons[1,1]);
						WhereInnerPanel.Controls.Add(InputBoxes[1]);

						HeaderPanels[0].Height = CollapseBtns[0].Height
							 + HeaderPanels[0].Padding.Bottom
							 + HeaderPanels[0].Padding.Top
							 + 10;

						if (ToggleButtons[0,0].Height > ToggleButtons[0,1].Height) {
							BtnCtrlPanels[0].Height = ToggleButtons[0,0].Height
								+ BtnCtrlPanels[0].Padding.Top
								+ BtnCtrlPanels[0].Padding.Bottom
								+ 10;
						} else {
							BtnCtrlPanels[0].Height = ToggleButtons[0,1].Height
								+ BtnCtrlPanels[0].Padding.Top
								+ BtnCtrlPanels[0].Padding.Bottom
								+ 10;
						}

						HeaderPanels[1].Height = CollapseBtns[1].Height
							 + HeaderPanels[1].Padding.Bottom
							 + HeaderPanels[1].Padding.Top
							 + 10;

						if (ToggleButtons[1,0].Height > ToggleButtons[1,1].Height) {
							BtnCtrlPanels[1].Height = ToggleButtons[1,0].Height
								+ BtnCtrlPanels[1].Padding.Top
								+ BtnCtrlPanels[1].Padding.Bottom
								+ 10;
						} else {
							BtnCtrlPanels[1].Height = ToggleButtons[1,1].Height
								+ BtnCtrlPanels[1].Padding.Top
								+ BtnCtrlPanels[1].Padding.Bottom
								+ 10;
						}

						foreach (Button B in CollapseBtns) {
							B.Click += ShowBox;
						}
						foreach (RadioButton B in ToggleButtons) {
							B.CheckedChanged += ToggleInputBox;
						}

						WhereInnerPanel.ResumeLayout();
						ResizeGroupBoxAfter(WhereGroupBox);

						ToggleInputBox(ToggleButtons[0,0],EventArgs.Empty);
						ToggleInputBox(ToggleButtons[1,0],EventArgs.Empty);
						
						ResizeGroupBoxAfter(WhereGroupBox);
					}
					break;
			}
		}
		// Triggered whenever a dropdown panel is collapsed or expanded
		private void ShowBox(object sender, EventArgs e) {
			WhereInnerPanel.SuspendLayout();
			Button B = sender as Button;

			// Find all the panels that have the same tag as this button
			foreach (Control c in WhereInnerPanel.Controls) {
				if (c.Tag != null) {
					foreach (string tag in c.Tag.ToString().Split(' ')) {
						if (tag.Equals(B.Tag.ToString())) {

							// Either show or hide the panel
							c.Visible = B.Text.Equals("+");
							break;
						}
					}
				}
			}
			if (B.Text.Equals("+")) {
				B.Text = "-";
			} else {
				B.Text = "+";
			}
			WhereInnerPanel.ResumeLayout();
			ResizeGroupBoxAfter(WhereGroupBox);
		}
		// Triggered whenever any of the arguments in the WHERE box
		// are changed from "Manual Input" to "File Attribute" or vice versa
		private void ToggleInputBox(object sender, EventArgs e) {
			RadioButton B = sender as RadioButton;
			if (B.Checked) {
				int index = -1;

				// Find the panel that is referenced by this RadioButton
				for (int i = 0; i < WhereInnerPanel.Controls.Count; i++) {
					if (WhereInnerPanel.Controls[i].Tag != null) {
						foreach (string tag in WhereInnerPanel.Controls[i].Tag.ToString().Split(' ')) {
							if (tag.Equals(B.Tag.ToString())) {
								index = i;
								break;
							}
						}
					}
					if (index != -1) {
						break;
					}
				}
				Panel P = WhereInnerPanel.Controls[index] as Panel;

				// Clear the panel
				P.Controls.Clear();

				// Get the argument index for this Where clause argument
				int arg = GetArgIndex(P);
				if (B.Text.Equals("Manual Input")) {

					// Add a text box so the user can manually input the arg
					P.Controls.Add(new TextBox {
						Width = WhereInnerPanel.Width
							- WhereInnerPanel.Padding.Left
							- WhereInnerPanel.Padding.Right
							- 10
					});
					Command.Where.SubClause.Args[arg] = new StringLiteralClause();
					P.Controls[0].TextChanged += SetClauseArgs;
				} else {

					// Add a ComboBox so the user can select the file attribute they
					// want
					ComboBox Box = new ComboBox {
						Width = WhereInnerPanel.Width
							- WhereInnerPanel.Padding.Left
							- WhereInnerPanel.Padding.Right
							- 10,
						DropDownStyle = ComboBoxStyle.DropDownList
					};
					P.Controls.Add(Box);

					FileSearchType.FileObjectAttribute[] Attributes = FileSearchType.GetAttributesByType(
						Command.Where.SubClause.ValidTypes[arg]
					);
					foreach (FileSearchType.FileObjectAttribute attr in Attributes) {
						Box.Items.Add(FileSearchType.ToString(attr));
					}
					Box.SelectedIndex = 0;
					Command.Where.SubClause.Args[arg] = new StringAttributeClause();
					Box.SelectedIndexChanged += SetClauseArgs;
				}
				P.Height = P.Controls[0].Height
					+ P.Padding.Top
					+ P.Padding.Bottom;
				
				// Trigger the new control to update the where clause
				SetClauseArgs(P.Controls[0],EventArgs.Empty);
			}
		}
		// Triggered whenever the text in one of the WHERE box arguments is changed
		private void SetClauseArgs(object sender, EventArgs e) {
			Control P = sender as Control;
			int arg = GetArgIndex(P.Parent);
			if (FileSearchType.AttributeMap(P.Text) == FileSearchType.FileObjectAttribute.CONTENT) {
				Command.Where.SubClause.Args[arg] = new FileObjectClause(FileSearchType.FileObjectAttribute.CONTENT);
			} else {
				if (Command.Where.SubClause.Args[arg] is FileObjectClause) {
					Command.Where.SubClause.Args[arg] = new StringAttributeClause();
				}
				Command.Where.SubClause.Args[arg].Set(P.Text);
			}
		}
		// Grabs the Where clause argument that corresponds to the given Control c
		private int GetArgIndex(Control c) {
			int arg = -1;
			string tag = c.Tag.ToString();
			string sarg = "";
			int index = tag.IndexOf("Arg-");
			for (int i = index + 4; i < tag.Length; i++) {
				if (!Char.IsDigit(tag[i])) {
					break;
				}
				sarg += tag[i];
			}
			arg = int.Parse(sarg) - 1;
			return arg;
		}
	}
}
