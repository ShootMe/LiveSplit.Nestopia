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
			if (split == SplitType.Changed || split == SplitType.ChangedGreaterThan || split == SplitType.ChangedLessThan) {
				txtValue.Visible = false;
				btnRemove.Location = new System.Drawing.Point(352, 2);
			} else {
				btnRemove.Location = new System.Drawing.Point(438, 2);
				txtValue.Visible = true;
			}
			txtValue.Tag = txtValue.Visible;

			MemberInfo[] infos = typeof(SplitType).GetMember(Type);
			DescriptionAttribute[] descriptions = null;
			if (infos.Length > 0) {
				descriptions = (DescriptionAttribute[])infos[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
			}
			if (descriptions != null && descriptions.Length > 0) {
				ToolTips.SetToolTip(cboType, descriptions[0].Description);
			} else {
				ToolTips.SetToolTip(cboType, string.Empty);
			}
		}
		public static T GetEnumValue<T>(string text) {
			foreach (T item in Enum.GetValues(typeof(T))) {
				string name = item.ToString();

				MemberInfo[] infos = typeof(T).GetMember(name);
				DescriptionAttribute[] descriptions = null;
				if (infos.Length > 0) {
					descriptions = (DescriptionAttribute[])infos[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
				}
				DescriptionAttribute description = descriptions != null && descriptions.Length > 0 ? descriptions[0] : null;

				if (name.Equals(text, StringComparison.OrdinalIgnoreCase) || (description != null && description.Description.Equals(text, StringComparison.OrdinalIgnoreCase))) {
					return item;
				}
			}
			return default(T);
		}
		public static string GetEnumDescription<T>(T item) {
			string name = item.ToString();

			MemberInfo[] infos = typeof(T).GetMember(name);
			DescriptionAttribute[] descriptions = null;
			if (infos.Length > 0) {
				descriptions = (DescriptionAttribute[])infos[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
			}
			DescriptionAttribute description = descriptions != null && descriptions.Length > 0 ? descriptions[0] : null;

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
				string sizeDescription = cboSize.SelectedValue.ToString();
				ValueSize size = GetEnumValue<ValueSize>(sizeDescription);
				ValueSize = size.ToString();
				if (size == Nestopia.ValueSize.Manual) {
					txtValue.Tag = txtValue.Visible;
					txtValue.Visible = false;
					cboType.Visible = false;
					txtOffset.Visible = false;
					btnRemove.Location = new System.Drawing.Point(135, 2);
				} else {
					bool visible = txtValue.Tag == null ? true : (bool)txtValue.Tag;
					btnRemove.Location = new System.Drawing.Point(visible ? 438 : 352, 2);
					txtValue.Visible = visible;
					cboType.Visible = true;
					txtOffset.Visible = true;
				}
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