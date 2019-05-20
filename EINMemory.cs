using System;
using System.Diagnostics;
using System.Drawing;
namespace EndIsNigh {
	public partial class EINMemory {
		public static ProgramPointer PlayerData = new ProgramPointer(AutoDeref.None, new ProgramSignature(PointerVersion.Steam, "656e646e6967682e73776600????????????????????????656e646c6f616465", -0x120));
		public Process Program { get; set; }
		public bool IsHooked { get; set; } = false;
		private DateTime lastHooked = DateTime.MinValue;

		public bool InMap() {
			return PlayerData.Read<int>(Program, 0x10, -0x60, 0x18, 0x90) != 0;
		}
		public Point WorldMap() {
			int x = PlayerData.Read<int>(Program, 0x10, -0x60, 0x18, 0x90, 0x130);
			int y = PlayerData.Read<int>(Program, 0x10, -0x60, 0x18, 0x90, 0x134);
			return new Point(x, y);
		}
		public void SetWorldMap(int x, int y) {
			PlayerData.Write<int>(Program, x, 0x10, -0x60, 0x18, 0x90, 0x130);
			PlayerData.Write<int>(Program, y, 0x10, -0x60, 0x18, 0x90, 0x134);
			PlayerData.Write<int>(Program, 0, 0x10, -0x60, 0x18, 0x90, 0x144);
			PlayerData.Write<int>(Program, 9, 0x10, -0x60, 0x18, 0x90, 0x148);
			if (x == 60 && (y == 27 || y == 28)) {
				while (PlayerData.Read<int>(Program, 0x10, -0x60, 0x18, 0x90, 0x148) != 0) {
					System.Threading.Thread.Sleep(5);
				}
				SetPlayerPosition(21, 28);
			}
		}
		public PointD PlayerPosition() {
			double x = PlayerData.Read<double>(Program, 0x10, -0x60, 0x18, 0x90, 0x12c, 0x18, 0x28);
			double y = PlayerData.Read<double>(Program, 0x10, -0x60, 0x18, 0x90, 0x12c, 0x18, 0x30);
			return new PointD(x, y);
		}
		public void SetPlayerPosition(double x, double y) {
			PlayerData.Write<double>(Program, x, 0x10, -0x60, 0x18, 0x90, 0x12c, 0x18, 0x28);
			PlayerData.Write<double>(Program, y, 0x10, -0x60, 0x18, 0x90, 0x12c, 0x18, 0x30);
		}
		public PointD MousePosition() {
			double x = PlayerData.Read<double>(Program, 0x10, -0x60, 0x18, 0x90, 0x12c, 0x20, 0xb0);
			double y = PlayerData.Read<double>(Program, 0x10, -0x60, 0x18, 0x90, 0x12c, 0x20, 0xb8);
			return new PointD(x, y);
		}
		public bool IsDead() {
			return PlayerData.Read<bool>(Program, 0x10, -0x60, 0x18, 0x90, 0x12c, 0xb8);
		}
		public bool OnWallLeft() {
			return PlayerData.Read<bool>(Program, 0x10, -0x60, 0x18, 0x90, 0x12c, 0x6a8);
		}
		public bool OnWallRight() {
			return PlayerData.Read<bool>(Program, 0x10, -0x60, 0x18, 0x90, 0x12c, 0x6a9);
		}
		public bool OnCeiling() {
			return PlayerData.Read<bool>(Program, 0x10, -0x60, 0x18, 0x90, 0x12c, 0x6aa);
		}
		public bool OnGround() {
			return PlayerData.Read<bool>(Program, 0x10, -0x60, 0x18, 0x90, 0x12c, 0x6ab);
		}
		public bool HangingFromWall() {
			return PlayerData.Read<bool>(Program, 0x10, -0x60, 0x18, 0x90, 0x12c, 0x6ad);
		}
		public int CartLives() {
			return PlayerData.Read<int>(Program, 0x10, -0x60, 0x18, 0x90, 0x13c);
		}
		public void SetCartLives(int lives) {
			PlayerData.Write<int>(Program, lives, 0x10, -0x60, 0x18, 0x90, 0x13c);
		}

		public bool HookProcess() {
			IsHooked = Program != null && !Program.HasExited;
			if (!IsHooked && DateTime.Now > lastHooked.AddSeconds(1)) {
				lastHooked = DateTime.Now;
				Process[] processes = Process.GetProcessesByName("TheEndIsNigh");
				Program = processes != null && processes.Length > 0 ? processes[0] : null;

				if (Program != null && !Program.HasExited) {
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
	public struct PointD {
		public double X { get; set; }
		public double Y { get; set; }
		public PointD(double x, double y) {
			X = x;
			Y = y;
		}

		public bool Close(PointD other) {
			return Math.Abs(X - other.X) <= 0.001 && Math.Abs(Y - other.Y) <= 0.001;
		}
		public override string ToString() {
			return X.ToString("0.00") + ", " + Y.ToString("0.00");
		}
	}

	public enum PointerVersion {
		Steam
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
		public string Read(Process program, params int[] offsets) {
			GetPointer(program);
			return program.Read(Pointer, offsets);
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
		public void ClearPointer() {
			Pointer = IntPtr.Zero;
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
						if (MemoryReader.is64Bit) {
							Pointer = (IntPtr)program.Read<ulong>(Pointer);
						} else {
							Pointer = (IntPtr)program.Read<uint>(Pointer);
						}
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
			if (signatures != null) {
				MemorySearcher searcher = new MemorySearcher();
				searcher.MemoryFilter = delegate (MemInfo info) {
					return (info.State & 0x1000) != 0 && info.Type == 0x20000 && info.Protect == 4 && info.AllocationProtect == 4 && (long)info.RegionSize < 0x100000;
				};
				for (int i = 0; i < signatures.Length; i++) {
					ProgramSignature signature = signatures[i];

					IntPtr ptr = searcher.FindSignature(program, signature.Signature);
					if (ptr != IntPtr.Zero) {
						Version = signature.Version;
						return ptr + signature.Offset;
					}
				}
				return IntPtr.Zero;
			}

			if (MemoryReader.is64Bit) {
				return (IntPtr)program.Read<ulong>(program.MainModule.BaseAddress, offsets);
			} else {
				return (IntPtr)program.Read<uint>(program.MainModule.BaseAddress, offsets);
			}
		}
	}
}