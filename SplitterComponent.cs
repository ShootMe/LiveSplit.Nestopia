using LiveSplit.Model;
using LiveSplit.UI;
using LiveSplit.UI.Components;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Xml;
namespace LiveSplit.Nestopia {
	public class SplitterComponent : IComponent {
		public string ComponentName { get { return "Nestopia Autosplitter"; } }
		public TimerModel Model { get; set; }
		public IDictionary<string, Action> ContextMenuControls { get { return null; } }
		internal static string[] keys = { "CurrentSplit", "Pointer", "Type", "Size", "Offset", "Value", "LastValue" };
		private SplitterMemory mem;
		private int currentSplit = -1, lastLogCheck;
		private bool hasLog = false;
		private Dictionary<string, string> currentValues = new Dictionary<string, string>();
		private SplitterSettings settings;
		private static string LOGFILE = "_Nestopia.log";

		public SplitterComponent(LiveSplitState state) {
			mem = new SplitterMemory();
			settings = new SplitterSettings();
			foreach (string key in keys) {
				currentValues[key] = "";
			}

			if (state != null) {
				Model = new TimerModel() { CurrentState = state };
				state.OnReset += OnReset;
				state.OnPause += OnPause;
				state.OnResume += OnResume;
				state.OnStart += OnStart;
				state.OnSplit += OnSplit;
				state.OnUndoSplit += OnUndoSplit;
				state.OnSkipSplit += OnSkipSplit;
			}
		}

		public void GetValues() {
			if (!mem.HookProcess()) { return; }

			if (Model != null) {
				HandleSplits();
			}

			LogValues();
		}
		private void HandleSplits() {
			bool shouldSplit = false;

			if (currentSplit < Model.CurrentState.Run.Count && settings.Splits.Count > 0) {
				SplitInfo split = currentSplit + 1 < settings.Splits.Count ? settings.Splits[currentSplit + 1] : null;
				if (split != null && split.Size != ValueSize.Manual) {
					long value = ReadValue(split);
					switch (split.Type) {
						case SplitType.Equals: shouldSplit = value == split.Value && value != split.LastValue; break;
						case SplitType.GreaterThan: shouldSplit = value > split.Value && value != split.LastValue; break;
						case SplitType.LessThan: shouldSplit = value < split.Value && value != split.LastValue; break;
						case SplitType.Changed: shouldSplit = value != split.LastValue; break;
						case SplitType.ChangedGreaterThan: shouldSplit = value > split.LastValue; break;
						case SplitType.ChangedLessThan: shouldSplit = value < split.LastValue; break;
					}
					split.LastValue = value;

					if (!split.ShouldSplit && shouldSplit) {
						currentSplit++;
						shouldSplit = false;
					}
				}
			}

			HandleSplit(shouldSplit, false);
		}
		private long ReadValue(SplitInfo split) {
			long value = 0;
			if (split != null && split.Size != ValueSize.Manual) {
				switch (split.Size) {
					case ValueSize.UInt8: value = mem.Read<byte>(split.Offset); break;
					case ValueSize.Int8: value = mem.Read<sbyte>(split.Offset); break;
					case ValueSize.UInt16: value = mem.Read<ushort>(split.Offset); break;
					case ValueSize.Int16: value = mem.Read<short>(split.Offset); break;
					case ValueSize.UInt32: value = mem.Read<uint>(split.Offset); break;
					case ValueSize.Int32: value = mem.Read<int>(split.Offset); break;
				}
			}
			return value;
		}
		private void HandleSplit(bool shouldSplit, bool shouldReset = false) {
			if (shouldReset) {
				if (currentSplit >= 0) {
					Model.Reset();
				}
			} else if (shouldSplit) {
				if (currentSplit == -1) {
					Model.Start();
				} else {
					Model.Split();
				}
			}
		}
		private void LogValues() {
			if (lastLogCheck == 0) {
				hasLog = File.Exists(LOGFILE);
				lastLogCheck = 300;
			}
			lastLogCheck--;

			if (hasLog || !Console.IsOutputRedirected) {
				string prev = string.Empty, curr = string.Empty;
				SplitInfo split = currentSplit + 1 < settings.Splits.Count ? settings.Splits[currentSplit + 1] : null;
				foreach (string key in keys) {
					prev = currentValues[key];

					switch (key) {
						case "CurrentSplit": curr = currentSplit.ToString(); break;
						case "Pointer": curr = mem.Pointer(); break;
						case "Type": curr = split != null ? split.Type.ToString() : string.Empty; break;
						case "Size": curr = split != null ? split.Size.ToString() : string.Empty; break;
						case "Offset": curr = split != null ? split.Offset.ToString() : string.Empty; break;
						case "SplitValue": curr = split != null ? split.Value.ToString() : string.Empty; break;
						case "LastValue": curr = split != null ? split.LastValue.ToString() : string.Empty; break;
						case "Value": curr = ReadValue(split).ToString(); break;
						default: curr = string.Empty; break;
					}

					if (string.IsNullOrEmpty(prev)) { prev = string.Empty; }
					if (string.IsNullOrEmpty(curr)) { curr = string.Empty; }
					if (!prev.Equals(curr)) {
						WriteLog(DateTime.Now.ToString(@"HH\:mm\:ss.fff") + (Model != null ? " | " + Model.CurrentState.CurrentTime.RealTime.Value.ToString("G").Substring(3, 11) : "") + ": " + key + ": ".PadRight(16 - key.Length, ' ') + prev.PadLeft(25, ' ') + " -> " + curr);

						currentValues[key] = curr;
					}
				}
			}
		}

		public void Update(IInvalidator invalidator, LiveSplitState lvstate, float width, float height, LayoutMode mode) {
			GetValues();
		}
		private void UpdateSplitValue() {
			SplitInfo split = currentSplit + 1 < settings.Splits.Count ? settings.Splits[currentSplit + 1] : null;
			if (split != null) {
				split.LastValue = ReadValue(split);
			}
		}
		public void OnReset(object sender, TimerPhase e) {
			currentSplit = -1;
			Model.CurrentState.IsGameTimePaused = true;
			WriteLog("---------Reset----------------------------------");
		}
		public void OnResume(object sender, EventArgs e) {
			WriteLog("---------Resumed--------------------------------");
		}
		public void OnPause(object sender, EventArgs e) {
			WriteLog("---------Paused---------------------------------");
		}
		public void OnStart(object sender, EventArgs e) {
			currentSplit = 0;
			UpdateSplitValue();
			WriteLog("---------New Game-------------------------------");
		}
		public void OnUndoSplit(object sender, EventArgs e) {
			while (currentSplit > 0 && !settings.Splits[--currentSplit].ShouldSplit) { }
			while (currentSplit > 0 && !settings.Splits[currentSplit - 1].ShouldSplit) {
				currentSplit--;
			}
			UpdateSplitValue();
			WriteLog(DateTime.Now.ToString(@"HH\:mm\:ss.fff") + " | " + Model.CurrentState.CurrentTime.RealTime.Value.ToString("G").Substring(3, 11) + ": CurrentSplit: " + currentSplit.ToString().PadLeft(24, ' '));
		}
		public void OnSkipSplit(object sender, EventArgs e) {
			while (currentSplit < settings.Splits.Count && !settings.Splits[currentSplit].ShouldSplit) {
				currentSplit++;
			}
			currentSplit++;
			UpdateSplitValue();
			WriteLog(DateTime.Now.ToString(@"HH\:mm\:ss.fff") + " | " + Model.CurrentState.CurrentTime.RealTime.Value.ToString("G").Substring(3, 11) + ": CurrentSplit: " + currentSplit.ToString().PadLeft(24, ' '));
		}
		public void OnSplit(object sender, EventArgs e) {
			currentSplit++;
			UpdateSplitValue();
			WriteLog(DateTime.Now.ToString(@"HH\:mm\:ss.fff") + " | " + Model.CurrentState.CurrentTime.RealTime.Value.ToString("G").Substring(3, 11) + ": CurrentSplit: " + currentSplit.ToString().PadLeft(24, ' '));
		}
		private void WriteLog(string data) {
			if (hasLog || !Console.IsOutputRedirected) {
				if (Console.IsOutputRedirected) {
					using (StreamWriter wr = new StreamWriter(LOGFILE, true)) {
						wr.WriteLine(data);
					}
				} else {
					Console.WriteLine(data);
				}
			}
		}

		public Control GetSettingsControl(LayoutMode mode) { return settings; }
		public void SetSettings(XmlNode document) { settings.SetSettings(document); }
		public XmlNode GetSettings(XmlDocument document) { return settings.UpdateSettings(document); }
		public void DrawHorizontal(Graphics g, LiveSplitState state, float height, Region clipRegion) { }
		public void DrawVertical(Graphics g, LiveSplitState state, float width, Region clipRegion) { }
		public float HorizontalWidth { get { return 0; } }
		public float MinimumHeight { get { return 0; } }
		public float MinimumWidth { get { return 0; } }
		public float PaddingBottom { get { return 0; } }
		public float PaddingLeft { get { return 0; } }
		public float PaddingRight { get { return 0; } }
		public float PaddingTop { get { return 0; } }
		public float VerticalHeight { get { return 0; } }
		public void Dispose() { }
	}
}