using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using System.Xml;
namespace LiveSplit.Nestopia {
	public partial class SplitterSettings : UserControl {
		public List<SplitInfo> Splits { get; private set; }
		private bool isLoading;
		public SplitterSettings() {
			isLoading = true;
			InitializeComponent();

			Splits = new List<SplitInfo>();
			isLoading = false;
		}

		private void Settings_Load(object sender, EventArgs e) {
			LoadSettings();
		}
		public void LoadSettings() {
			isLoading = true;
			this.flowMain.SuspendLayout();

			for (int i = flowMain.Controls.Count - 1; i > 0; i--) {
				flowMain.Controls.RemoveAt(i);
			}

			foreach (SplitInfo split in Splits) {
				SplitterSplitSettings setting = new SplitterSplitSettings();
				setting.cboType.DataSource = GetAvailableDescriptions<SplitType>();
				setting.cboType.Text = SplitterSplitSettings.GetEnumDescription<SplitType>(split.Type);
				setting.cboSize.DataSource = GetAvailableDescriptions<ValueSize>();
				setting.cboSize.Text = SplitterSplitSettings.GetEnumDescription<ValueSize>(split.Size);
				setting.txtOffset.Text = split.Offset.ToString();
				setting.txtValue.Text = split.Value.ToString();
				AddHandlers(setting);

				flowMain.Controls.Add(setting);
			}

			isLoading = false;
			this.flowMain.ResumeLayout(true);
		}
		private void AddHandlers(SplitterSplitSettings setting) {
			setting.cboType.SelectedIndexChanged += new EventHandler(ControlChanged);
			setting.cboSize.SelectedIndexChanged += new EventHandler(ControlChanged);
			setting.txtOffset.TextChanged += new EventHandler(ControlChanged);
			setting.txtValue.TextChanged += new EventHandler(ControlChanged);
			setting.btnRemove.Click += new EventHandler(btnRemove_Click);
		}
		private void RemoveHandlers(SplitterSplitSettings setting) {
			setting.cboType.SelectedIndexChanged -= ControlChanged;
			setting.cboSize.SelectedIndexChanged -= ControlChanged;
			setting.txtOffset.TextChanged -= ControlChanged;
			setting.txtValue.TextChanged -= ControlChanged;
			setting.btnRemove.Click -= btnRemove_Click;
		}
		public void btnRemove_Click(object sender, EventArgs e) {
			for (int i = flowMain.Controls.Count - 1; i > 0; i--) {
				if (flowMain.Controls[i].Contains((Control)sender)) {
					RemoveHandlers((SplitterSplitSettings)((Button)sender).Parent);

					flowMain.Controls.RemoveAt(i);
					break;
				}
			}
			UpdateSplits();
		}
		public void ControlChanged(object sender, EventArgs e) {
			UpdateSplits();
		}
		public void UpdateSplits() {
			if (isLoading) return;

			Splits.Clear();
			foreach (Control c in flowMain.Controls) {
				if (c is SplitterSplitSettings) {
					SplitterSplitSettings setting = (SplitterSplitSettings)c;
					if (!string.IsNullOrEmpty(setting.cboType.Text)) {
						int offset;
						int.TryParse(setting.txtOffset.Text, out offset);
						long value;
						long.TryParse(setting.txtValue.Text, out value);
						Splits.Add(new SplitInfo() {
							Type = SplitterSplitSettings.GetEnumValue<SplitType>(setting.cboType.Text),
							Size = SplitterSplitSettings.GetEnumValue<ValueSize>(setting.cboSize.Text),
							Offset = offset,
							Value = value
						});
					}
				}
			}
		}
		public XmlNode UpdateSettings(XmlDocument document) {
			XmlElement xmlSettings = document.CreateElement("Settings");

			XmlElement xmlSplits = document.CreateElement("Splits");
			xmlSettings.AppendChild(xmlSplits);

			foreach (SplitInfo split in Splits) {
				XmlElement xmlSplit = document.CreateElement("Split");
				xmlSplit.InnerText = split.ToString();

				xmlSplits.AppendChild(xmlSplit);
			}

			return xmlSettings;
		}
		public void SetSettings(XmlNode settings) {
			Splits.Clear();
			XmlNodeList splitNodes = settings.SelectNodes(".//Splits/Split");
			foreach (XmlNode splitNode in splitNodes) {
				string splitDescription = splitNode.InnerText;
				Splits.Add(new SplitInfo(splitDescription));
			}
		}
		private void btnAddSplit_Click(object sender, EventArgs e) {
			SplitterSplitSettings setting = new SplitterSplitSettings();
			List<string> names = GetAvailableDescriptions<SplitType>();
			setting.cboType.DataSource = names;
			setting.cboType.Text = names[0];
			names = GetAvailableDescriptions<ValueSize>();
			setting.cboSize.DataSource = names;
			setting.cboSize.Text = names[0];
			AddHandlers(setting);

			flowMain.Controls.Add(setting);
			UpdateSplits();
		}
		private List<string> GetAvailableDescriptions<T>() where T : struct {
			List<string> values = new List<string>();
			foreach (T value in Enum.GetValues(typeof(T))) {
				string name = value.ToString();
				MemberInfo[] infos = typeof(T).GetMember(name);
				DescriptionAttribute[] descriptions = null;
				if (infos != null && infos.Length > 0) {
					descriptions = (DescriptionAttribute[])infos[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
				}
				if (descriptions != null && descriptions.Length > 0) {
					values.Add(descriptions[0].Description);
				} else {
					values.Add(name);
				}
			}
			return values;
		}
		private void flowMain_DragDrop(object sender, DragEventArgs e) {
			UpdateSplits();
		}
		private void flowMain_DragEnter(object sender, DragEventArgs e) {
			e.Effect = DragDropEffects.Move;
		}
		private void flowMain_DragOver(object sender, DragEventArgs e) {
			SplitterSplitSettings data = (SplitterSplitSettings)e.Data.GetData(typeof(SplitterSplitSettings));
			FlowLayoutPanel destination = (FlowLayoutPanel)sender;
			Point p = destination.PointToClient(new Point(e.X, e.Y));
			var item = destination.GetChildAtPoint(p);
			int index = destination.Controls.GetChildIndex(item, false);
			if (index == 0) {
				e.Effect = DragDropEffects.None;
			} else {
				e.Effect = DragDropEffects.Move;
				int oldIndex = destination.Controls.GetChildIndex(data);
				if (oldIndex != index) {
					destination.Controls.SetChildIndex(data, index);
					destination.Invalidate();
				}
			}
		}
	}
	public class SplitInfo {
		public SplitType Type { get; set; }
		public ValueSize Size { get; set; }
		public int Offset { get; set; }
		public long Value { get; set; }
		public long LastValue;
		public SplitInfo() { }
		public SplitInfo(string copy) {
			string[] info = copy.Split(',');
			if (info.Length > 0) {
				SplitType temp;
				if (Enum.TryParse(info[0], out temp)) {
					Type = temp;
				}
			}
			Size = ValueSize.UInt8;
			if (info.Length > 1) {
				ValueSize temp;
				if (Enum.TryParse(info[1], out temp)) {
					Size = temp;
				}
			}
			Offset = 0;
			if (info.Length > 2) {
				int temp;
				if (int.TryParse(info[2], out temp)) {
					Offset = temp;
				}
			}
			Value = 0;
			if (info.Length > 3) {
				long temp;
				if (long.TryParse(info[3], out temp)) {
					Value = temp;
				}
			}
		}
		public override string ToString() {
			return Type.ToString() + "," + Size.ToString() + "," + Offset.ToString() + "," + Value.ToString();
		}
	}
}