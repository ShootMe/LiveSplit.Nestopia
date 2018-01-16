using LiveSplit.Memory;
using System;
using System.Diagnostics;
namespace LiveSplit.Nestopia {
	public partial class SplitterMemory {
		private static ProgramPointer RAM = new ProgramPointer(AutoDeref.None, 0x3b1388);
		public Process Program { get; set; }
		public bool IsHooked { get; set; } = false;
		private DateTime lastHooked;

		public SplitterMemory() {
			lastHooked = DateTime.MinValue;
		}
		public string Pointer() {
			return RAM.GetPointer(Program).ToString("X");
		}
		public T Read<T>(int address) where T : struct {
			return RAM.Read<T>(Program, 0x0, address);
		}
		public bool HookProcess() {
			IsHooked = Program != null && !Program.HasExited;
			if (!IsHooked && DateTime.Now > lastHooked.AddSeconds(1)) {
				lastHooked = DateTime.Now;
				Process[] processes = Process.GetProcesses();
				Program = null;
				for (int i = 0; i < processes.Length; i++) {
					Process process = processes[i];
					if (process.ProcessName.Equals("nestopia", StringComparison.OrdinalIgnoreCase)) {
						Program = process;
						break;
					} else if (process.ProcessName.Equals("fceux", StringComparison.OrdinalIgnoreCase)) {
						Program = process;
						break;
					}
				}

				if (Program != null) {
					MemoryReader.Update64Bit(Program);
					IsHooked = true;
				}
			}

			return IsHooked;
		}
		public void Dispose() {
			if (Program != null) {
				Program.Dispose();
			}
		}
	}
	public enum PointerVersion {
		V1
	}
	public enum AutoDeref {
		None,
		Single,
		Double
	}
	public class ProgramSignature {
		public PointerVersion Version { get; set; }
		public string Signature { get; set; }
		public int Offset { get; set; }
		public ProgramSignature(PointerVersion version, string signature, int offset) {
			Version = version;
			Signature = signature;
			Offset = offset;
		}
		public override string ToString() {
			return Version.ToString() + " - " + Signature;
		}
	}
	public class ProgramPointer {
		private int lastID;
		private DateTime lastTry;
		private ProgramSignature[] signatures;
		private int[] offsets;
		public IntPtr Pointer { get; private set; }
		public PointerVersion Version { get; private set; }
		public AutoDeref AutoDeref { get; private set; }

		public ProgramPointer(AutoDeref autoDeref, params ProgramSignature[] signatures) {
			AutoDeref = autoDeref;
			this.signatures = signatures;
			lastID = -1;
			lastTry = DateTime.MinValue;
		}
		public ProgramPointer(AutoDeref autoDeref, params int[] offsets) {
			AutoDeref = autoDeref;
			this.offsets = offsets;
			lastID = -1;
			lastTry = DateTime.MinValue;
		}

		public T Read<T>(Process program, params int[] offsets) where T : struct {
			GetPointer(program);
			return program.Read<T>(Pointer, offsets);
		}
		public byte[] ReadBytes(Process program, int length, params int[] offsets) {
			GetPointer(program);
			return program.Read(Pointer, length, offsets);
		}
		public void Write<T>(Process program, T value, params int[] offsets) where T : struct {
			GetPointer(program);
			program.Write<T>(Pointer, value, offsets);
		}
		public void Write(Process program, byte[] value, params int[] offsets) {
			GetPointer(program);
			program.Write(Pointer, value, offsets);
		}
		public IntPtr GetPointer(Process program) {
			if (program == null) {
				Pointer = IntPtr.Zero;
				lastID = -1;
				return Pointer;
			} else if (program.Id != lastID) {
				Pointer = IntPtr.Zero;
				lastID = program.Id;
			}

			if (Pointer == IntPtr.Zero && DateTime.Now > lastTry.AddSeconds(1)) {
				lastTry = DateTime.Now;

				Pointer = GetVersionedFunctionPointer(program);
				if (Pointer != IntPtr.Zero) {
					if (AutoDeref != AutoDeref.None) {
						Pointer = (IntPtr)program.Read<uint>(Pointer);
						if (AutoDeref == AutoDeref.Double) {
							if (MemoryReader.is64Bit) {
								Pointer = (IntPtr)program.Read<ulong>(Pointer);
							} else {
								Pointer = (IntPtr)program.Read<uint>(Pointer);
							}
						}
					}
				}
			}
			return Pointer;
		}
		private IntPtr GetVersionedFunctionPointer(Process program) {
			if (program.ProcessName.Equals("nestopia", StringComparison.OrdinalIgnoreCase)) {
				MemorySearcher searcher = new MemorySearcher();
				searcher.MemoryFilter = delegate (MemInfo info) {
					return (info.Protect & 0x4) != 0 && (info.State & 0x1000) != 0 && (info.Type & 0x20000) != 0;
				};

				searcher.GetMemoryInfo(program.Handle);
				for (int i = 0; i < searcher.memoryInfo.Count; i++) {
					byte[] data = searcher.ReadMemory(program, i);
					int pointer1 = BitConverter.ToInt32(data, 0);
					int pointer2 = BitConverter.ToInt32(data, 4);
					long padding = BitConverter.ToInt64(data, 8);
					if (pointer1 == pointer2 && pointer1 != 0 && padding == 0) {
						return searcher.memoryInfo[i].BaseAddress + 0xab8;
					}
				}

				return IntPtr.Zero;
			}

			return program.MainModule.BaseAddress + offsets[0];
		}
	}
}