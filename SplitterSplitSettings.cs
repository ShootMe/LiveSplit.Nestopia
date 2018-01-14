using System;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Windows.Forms;
namespace LiveSplit.Nestopia {
	public partial class SplitterSplitSettings : UserControl {
		public string Type { get; set; } = "";
		public string ValueSize { get; set; } = "Unsigned 8 Bits";
		public int Offset { get; set; } = 0;
		public long Value { get; set; } = 0;
		private int mX = 0;
		private int mY = 0;
		private bool isDragging = false;
		public SplitterSplitSettings() {
			InitializeComponent();
			cboSize.SelectedItem = ValueSize;
			txtOffset.Text = Offset.ToString();
			txtValue.Text = Value.ToString();
		}
		private void cboType_Validating(object sender, CancelEventArgs e) {
			string item = GetItemInList(cboSize);
			if (string.IsNullOrEmpty(item)) {
				cboType.SelectedItem = SplitType.Equals;
			} else {
				cboType.SelectedItem = GetEnumValue<SplitType>(item);
			}
		}
		private void cboType_SelectedIndexChanged(object sender, EventArgs e) {
			string splitDescription = cboType.SelectedValue.ToString();
			SplitType split = GetEnumValue<SplitType>(splitDescription);
			Type = split.ToString();
			MemberInfo info = typeof(SplitType).GetMember(Type)[0];
			if (split == SplitType.Changed || split == SplitType.ChangedGreaterThan || split == SplitType.ChangedLessThan) {
				txtValue.Visible = false;
				btnRemove.Location = new System.Drawing.Point(363, 2);
			} else {
				btnRemove.Location = new System.Drawing.Point(438, 2);
				txtValue.Visible = true;
			}
			DescriptionAttribute description = (DescriptionAttribute)info.GetCustomAttributes(typeof(DescriptionAttribute), false)[0];
			ToolTips.SetToolTip(cboType, description.Description);
		}
		public static T GetEnumValue<T>(string text) {
			foreach (T item in Enum.GetValues(typeof(T))) {
				string name = item.ToString();
				MemberInfo info = typeof(T).GetMember(name)[0];
				object[] attributes = info.GetCustomAttributes(typeof(DescriptionAttribute), false);
				DescriptionAttribute description = attributes != null && attributes.Length > 0 ? (DescriptionAttribute)attributes[0] : null;

				if (name.Equals(text, StringComparison.OrdinalIgnoreCase) || (description != null && description.Description.Equals(text, StringComparison.OrdinalIgnoreCase))) {
					return item;
				}
			}
			return default(T);
		}
		public static string GetEnumDescription<T>(T item) {
			string name = item.ToString();
			MemberInfo info = typeof(T).GetMember(name)[0];
			object[] attributes = info.GetCustomAttributes(typeof(DescriptionAttribute), false);
			DescriptionAttribute description = attributes != null && attributes.Length > 0 ? (DescriptionAttribute)attributes[0] : null;

			return description == null ? name : description.Description;
		}
		private void cboSize_Validating(object sender, CancelEventArgs e) {
			string item = GetItemInList(cboSize);
			if (string.IsNullOrEmpty(item)) {
				item = "Unsigned 8 Bits";
			}
			cboSize.SelectedItem = item;
		}
		private void cboSize_SelectedIndexChanged(object sender, EventArgs e) {
			if (cboSize.SelectedItem != null) {
				ValueSize = cboSize.SelectedItem.ToString();
			}
		}
		private void txtOffset_Validating(object sender, CancelEventArgs e) {
			int temp;
			if (!int.TryParse(txtOffset.Text, NumberStyles.Any, null, out temp) || temp < 0 || temp > 2048) {
				txtOffset.Text = "0";
			} else {
				txtOffset.Text = temp.ToString();
			}
		}
		private void txtOffset_TextChanged(object sender, EventArgs e) {
			int temp;
			if (!int.TryParse(txtOffset.Text, NumberStyles.Any, null, out temp) || temp < 0 || temp > 2048) {
				Offset = 0;
			} else {
				Offset = temp;
			}
		}
		private void txtValue_Validating(object sender, CancelEventArgs e) {
			long temp;
			if (!long.TryParse(txtValue.Text, NumberStyles.Any, null, out temp) || temp > uint.MaxValue || temp < int.MinValue) {
				txtValue.Text = "0";
			} else {
				txtValue.Text = temp.ToString();
			}
		}
		private void txtValue_TextChanged(object sender, EventArgs e) {
			long temp;
			if (!long.TryParse(txtValue.Text, NumberStyles.Any, null, out temp) || temp > uint.MaxValue || temp < int.MinValue) {
				Value = 0;
			} else {
				Value = temp;
			}
		}
		private string GetItemInList(ComboBox cbo) {
			string item = cbo.Text;
			for (int i = cbo.Items.Count - 1; i >= 0; i += -1) {
				object ob = cbo.Items[i];
				if (ob.ToString().Equals(item, StringComparison.OrdinalIgnoreCase)) {
					return ob.ToString();
				}
			}
			return null;
		}
		private void picHandle_MouseMove(object sender, MouseEventArgs e) {
			if (!isDragging) {
				if (e.Button == MouseButtons.Left) {
					int num1 = mX - e.X;
					int num2 = mY - e.Y;
					if (((num1 * num1) + (num2 * num2)) > 20) {
						DoDragDrop(this, DragDropEffects.All);
						isDragging = true;
						return;
					}
				}
			}
		}
		private void picHandle_MouseDown(object sender, MouseEventArgs e) {
			mX = e.X;
			mY = e.Y;
			isDragging = false;
		}
	}
	public class ToolTipAttribute : Attribute {
		public string ToolTip { get; set; }
		public ToolTipAttribute(string text) {
			ToolTip = text;
		}
	}
}