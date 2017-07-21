using System;
using System.Diagnostics;
using System.Drawing;
namespace EndIsNigh {
	public partial class EINMemory {
		public Process Program { get; set; }
		public bool IsHooked { get; set; } = false;
		private DateTime lastHooked = DateTime.MinValue;

		public bool InMap() {
			return ProgramPointer.PLAYERDATA.Read<int>(Program, 0x10, -0x60, 0x18, 0x90) != 0;
		}
		public Point WorldMap() {
			int x = ProgramPointer.PLAYERDATA.Read<int>(Program, 0x10, -0x60, 0x18, 0x90, 0x130);
			int y = ProgramPointer.PLAYERDATA.Read<int>(Program, 0x10, -0x60, 0x18, 0x90, 0x134);
			return new Point(x, y);
		}
		public void SetWorldMap(int x, int y) {
			ProgramPointer.PLAYERDATA.Write<int>(Program, x, 0x10, -0x60, 0x18, 0x90, 0x130);
			ProgramPointer.PLAYERDATA.Write<int>(Program, y, 0x10, -0x60, 0x18, 0x90, 0x134);
			ProgramPointer.PLAYERDATA.Write<int>(Program, 0, 0x10, -0x60, 0x18, 0x90, 0x144);
			ProgramPointer.PLAYERDATA.Write<int>(Program, 9, 0x10, -0x60, 0x18, 0x90, 0x148);
			if (x == 60 && (y == 27 || y == 28)) {
				while (ProgramPointer.PLAYERDATA.Read<int>(Program, 0x10, -0x60, 0x18, 0x90, 0x148) != 0) {
					System.Threading.Thread.Sleep(5);
				}
				SetPlayerPosition(21, 28);
			}
		}
		public PointD PlayerPosition() {
			double x = ProgramPointer.PLAYERDATA.Read<double>(Program, 0x10, -0x60, 0x18, 0x90, 0x12c, 0x18, 0x28);
			double y = ProgramPointer.PLAYERDATA.Read<double>(Program, 0x10, -0x60, 0x18, 0x90, 0x12c, 0x18, 0x30);
			return new PointD(x, y);
		}
		public void SetPlayerPosition(double x, double y) {
			ProgramPointer.PLAYERDATA.Write<double>(Program, x, 0x10, -0x60, 0x18, 0x90, 0x12c, 0x18, 0x28);
			ProgramPointer.PLAYERDATA.Write<double>(Program, y, 0x10, -0x60, 0x18, 0x90, 0x12c, 0x18, 0x30);
		}
		public PointD MousePosition() {
			double x = ProgramPointer.PLAYERDATA.Read<double>(Program, 0x10, -0x60, 0x18, 0x90, 0x12c, 0x20, 0xb0);
			double y = ProgramPointer.PLAYERDATA.Read<double>(Program, 0x10, -0x60, 0x18, 0x90, 0x12c, 0x20, 0xb8);
			return new PointD(x, y);
		}
		public bool IsDead() {
			return ProgramPointer.PLAYERDATA.Read<bool>(Program, 0x10, -0x60, 0x18, 0x90, 0x12c, 0xb8);
		}
		public bool OnWallLeft() {
			return ProgramPointer.PLAYERDATA.Read<bool>(Program, 0x10, -0x60, 0x18, 0x90, 0x12c, 0x6a8);
		}
		public bool OnWallRight() {
			return ProgramPointer.PLAYERDATA.Read<bool>(Program, 0x10, -0x60, 0x18, 0x90, 0x12c, 0x6a9);
		}
		public bool OnCeiling() {
			return ProgramPointer.PLAYERDATA.Read<bool>(Program, 0x10, -0x60, 0x18, 0x90, 0x12c, 0x6aa);
		}
		public bool OnGround() {
			return ProgramPointer.PLAYERDATA.Read<bool>(Program, 0x10, -0x60, 0x18, 0x90, 0x12c, 0x6ab);
		}
		public bool HangingFromWall() {
			return ProgramPointer.PLAYERDATA.Read<bool>(Program, 0x10, -0x60, 0x18, 0x90, 0x12c, 0x6ad);
		}
		public int CartLives() {
			return ProgramPointer.PLAYERDATA.Read<int>(Program, 0x10, -0x60, 0x18, 0x90, 0x13c);
		}
		public void SetCartLives(int lives) {
			ProgramPointer.PLAYERDATA.Write<int>(Program, lives, 0x10, -0x60, 0x18, 0x90, 0x13c);
		}

		public bool HookProcess() {
			if ((Program == null || Program.HasExited) && DateTime.Now > lastHooked.AddSeconds(1)) {
				lastHooked = DateTime.Now;
				Process[] processes = Process.GetProcessesByName("TheEndIsNigh");
				Program = processes.Length == 0 ? null : processes[0];
			}

			IsHooked = Program != null && !Program.HasExited;

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
		V1
	}
	public enum PointerType {
		PlayerData
	}
	public class ProgramSignature {
		public PointerVersion Version { get; set; }
		public string Signature { get; set; }
		public ProgramSignature(PointerVersion version, string signature) {
			Version = version;
			Signature = signature;
		}
		public override string ToString() {
			return Version.ToString() + " - " + Signature;
		}
	}
	public class ProgramPointer {
		public static ProgramPointer PLAYERDATA = new ProgramPointer(PointerType.PlayerData, false, 0x2718BC);

		private int lastID;
		private DateTime lastTry;
		private ProgramSignature[] signatures;
		private int[] offsets;
		public PointerType Name { get; private set; }
		public IntPtr Pointer { get; private set; }
		public PointerVersion Version { get; private set; }
		public bool AutoDeref { get; private set; }

		private ProgramPointer(PointerType name, bool autoDeref, params ProgramSignature[] signatures) {
			Name = name;
			AutoDeref = autoDeref;
			this.signatures = signatures;
			lastID = -1;
			lastTry = DateTime.MinValue;
		}
		private ProgramPointer(PointerType name, bool autoDeref, params int[] offsets) {
			Name = name;
			AutoDeref = autoDeref;
			this.offsets = offsets;
			lastID = -1;
			lastTry = DateTime.MinValue;
		}

		public T Read<T>(Process program, params int[] offsets) where T : struct {
			GetPointer(program);
			return program.Read<T>(Pointer, offsets);
		}
		public void Write<T>(Process program, T value, params int[] offsets) where T : struct {
			GetPointer(program);
			program.Write<T>(Pointer, value, offsets);
		}
		private void GetPointer(Process program) {
			if ((program?.HasExited).GetValueOrDefault(true)) {
				Pointer = IntPtr.Zero;
				lastID = -1;
				return;
			} else if (program.Id != lastID) {
				Pointer = IntPtr.Zero;
				lastID = program.Id;
			}

			if (Pointer == IntPtr.Zero && DateTime.Now > lastTry.AddSeconds(1)) {
				lastTry = DateTime.Now;

				Pointer = GetVersionedFunctionPointer(program);
				if (Pointer != IntPtr.Zero) {
					if (AutoDeref) {
						Pointer = (IntPtr)program.Read<uint>(Pointer);
					}
				}
			}
		}
		private IntPtr GetVersionedFunctionPointer(Process program) {
			if (signatures != null) {
				for (int i = 0; i < signatures.Length; i++) {
					ProgramSignature signature = signatures[i];

					IntPtr ptr = program.FindSignatures(signature.Signature)[0];
					if (ptr != IntPtr.Zero) {
						Version = signature.Version;
						return ptr;
					}
				}
			} else {
				IntPtr ptr = (IntPtr)program.Read<uint>(program.MainModule.BaseAddress, offsets);
				if (ptr != IntPtr.Zero) {
					return ptr;
				}
			}

			return IntPtr.Zero;
		}
	}
}