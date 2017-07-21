using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;
namespace EndIsNigh {
	public partial class EINHelper : Form {
		public EINMemory Memory { get; set; }
		private DateTime lastCheck = DateTime.MinValue;
		private Dictionary<string, Point> mapCoords = new Dictionary<string, Point>();
		private Dictionary<Point, string> mapRevCoords = new Dictionary<Point, string>();
		private int lastX = -1, lastY = -1;
		private PointD lastPlayer = new PointD(-1, -1);
		private PointD? savedPos;
		private string lastMap = "";
		private bool inMap = false, shouldRespawn = false;
		private int frameCount = 0, savedLives = 0;
		private MiniMapCell[,] cells = null;
		private Dictionary<string, LevelEntry> idToEntry = new Dictionary<string, LevelEntry>();

		[STAThread]
		public static void Main(string[] args) {
			try {
				Application.EnableVisualStyles();
				Application.SetCompatibleTextRenderingDefault(false);
				Application.Run(new EINHelper());
			} catch (Exception ex) {
				Console.WriteLine(ex.ToString());
			}
		}
		public EINHelper() {
			try {
				this.DoubleBuffered = true;
				InitializeComponent();
				Text = "End Is Nigh Helper " + Assembly.GetExecutingAssembly().GetName().Version.ToString();
				Memory = new EINMemory();

				Thread t = new Thread(UpdateLoop);
				t.IsBackground = true;
				t.Start();
			} catch (Exception ex) {
				MessageBox.Show(ex.ToString());
			}
		}

		private void UpdateLoop() {
			bool lastHooked = false;
			while (true) {
				try {
					bool hooked = Memory.HookProcess();
					if (hooked) {
						UpdateValues();
					}
					if (lastHooked != hooked) {
						lastHooked = hooked;
						this.Invoke((Action)delegate () { lblNote.Visible = !hooked; });
						if (hooked) {
							InitializeMiniMap();
						}
					}
					Thread.Sleep(12);
				} catch { }
			}
		}
		private void InitializeLevelEntries() {
			if (idToEntry.Count != 0) { return; }

			for (int i = 0; i <= 20; i++) {
				idToEntry.Add("1-" + i, "The End " + i + ",bdbdbd");
				if (i >= 1 && i <= 7) {
					idToEntry.Add("1xu-" + i, "The End " + i + " U ???,bdbdbd");
				}
				if (i >= 1 && i <= 6) {
					idToEntry.Add("1xd-" + i, "The End " + i + " D ???,bdbdbd");
				}

				idToEntry.Add("1d-" + i, "Anguish " + i + ",c5000a");

				if (i >= 1 && i <= 8) {
					idToEntry.Add("6x-" + i, "Anguish " + i + " ???,c5000a");
				}
			}

			for (int i = 0; i <= 19; i++) {
				idToEntry.Add("2-" + i, "Arid Flats " + (i + 1) + ",7a5550");
				if (i >= 1 && i <= 9) {
					idToEntry.Add("2xu-" + i, "Arid Flats " + (i + 1) + " U ???,7a5550");
				}
				if (i >= 1 && i <= 5) {
					idToEntry.Add("2xd-" + i, "Arid Flats " + (i + 1) + " D ???,7a5550");
				}

				idToEntry.Add("2d-" + i, "Gloom " + (i + 1) + ",723593");

				if (i >= 1 && i <= 6) {
					idToEntry.Add("7x-" + i, "Gloom " + i + " ???,723593");
				}
			}

			for (int i = 1; i <= 20; i++) {
				idToEntry.Add("3-" + i, "Overflow " + i + ",7eb2b4");
				if (i <= 6) {
					idToEntry.Add("3xu-" + i, "Overflow " + i + " U ???,7eb2b4");
				}
				if (i <= 9) {
					idToEntry.Add("3xd-" + i, "Overflow " + i + " D ???,7eb2b4");
				}
			}

			idToEntry.Add("4-1hub", "The Split,b89c69");

			for (int i = 1; i <= 16; i++) {
				idToEntry.Add("4a-" + i, "Wall of Sorrow " + i + ",b8872a");
				if (i <= 9) {
					idToEntry.Add("4ax-" + i, "Wall of Sorrow " + i + " ???,b8872a");
				}
			}

			for (int i = 1; i <= 22; i++) {
				idToEntry.Add("5a-" + i, "SS Exodus " + i + ",ebd0c4");
				if (i == 5) { idToEntry.Add("5a-5x", "SS Exodus 5S,ebd0c4"); }
				if (i <= 14) {
					idToEntry.Add("5ax-" + i, "SS Exodus " + i + " ???,ebd0c4");
				}
			}

			for (int i = 1; i <= 15; i++) {
				idToEntry.Add("4b-" + i, "Retrograde " + i + ",b897b0");
				if (i <= 9) {
					idToEntry.Add("4bx-" + i, "Retrograde " + i + " ???,b897b0");
				}
			}

			for (int i = 1; i <= 23; i++) {
				idToEntry.Add("5b-" + i, "The Machine " + i + ",707b6a");
				if (i <= 13) {
					idToEntry.Add("5bxu-" + i, "The Machine " + i + " U ???,707b6a");
				}
				if (i <= 8) {
					idToEntry.Add("5bxd-" + i, "The Machine " + i + " D ???,707b6a");
				}

				idToEntry.Add("3d-" + i, "Blight " + i + ",789c76");
				if (i <= 8) {
					idToEntry.Add("8x-" + i, "Blight " + i + " ???,789c76");
				}
			}

			for (int i = 1; i <= 15; i++) {
				idToEntry.Add("4c-" + i, "The Hollows " + i + ",acd8e7");
				if (i <= 9) {
					idToEntry.Add("4cx-" + i, "The Hollows " + i + " ???,acd8e7");
				}
			}

			for (int i = 1; i <= 21; i++) {
				idToEntry.Add("5c-" + i, "Golgotha " + i + ",a6000d");
				if (i <= 12) {
					idToEntry.Add("5cx-" + i, "Golgotha " + i + " ???,a6000d");
				}

				idToEntry.Add("6-" + i, "Ruin " + i + ",939393");
				if (i == 1) { idToEntry.Add("6-1end", "Ruin 1 End,939393"); }

				if (i <= 10 && i != 9) {
					idToEntry.Add("9x-" + i, "Ruin " + i + " ???,939393");
				}
			}

			for (int i = 0; i <= 15; i++) {
				idToEntry.Add("7-" + i, "Nevermore " + i + ",60789e");
			}

			for (int i = 1; i <= 8; i++) {
				idToEntry.Add("10x-" + i, "The Future " + i + ",7a98c8");
			}

			for (int i = 0; i <= 9; i++) {
				idToEntry.Add("c9-" + i, "The End Is Nigh " + i + ",92bd27");
				idToEntry.Add("g9-" + i, "The End Is Nigh Glitch " + i + ",134f13");
			}

			for (int i = 1; i <= 10; i++) {
				idToEntry.Add("c1-" + i, "Martaman " + i + ",3bab9f");
				idToEntry.Add("c2-" + i, "Blaster Massacre " + i + ",d29d56");
				idToEntry.Add("c3-" + i, "River City Rancid " + i + ",99ccbd");
				idToEntry.Add("c4a-" + i, "Ash Climber " + i + ",cbc525");
				idToEntry.Add("c4b-" + i, "Rubble Bobble " + i + ",e9a9ac");
				idToEntry.Add("c4c-" + i, "Catastrovania " + i + ",774397");
				idToEntry.Add("c5a-" + i, "Fallen Fantasy " + i + ",ad5600");
				idToEntry.Add("c5b-" + i, "Morbid Gear " + i + ",d78cbf");
				idToEntry.Add("c5c-" + i, "Dig Dead " + i + ",df212f");

				string extra = (i > 1 && i < 10 ? "" : "x");
				idToEntry.Add("$c1-" + i + (i < 10 ? "" : "x"), "Super Mega Cart 1-" + i + ",3bab9f");
				idToEntry.Add("$c2-" + i + extra, "Super Mega Cart 2-" + i + ",d29d56");
				idToEntry.Add("$c3-" + i + extra, "Super Mega Cart 3-" + i + ",99ccbd");
				idToEntry.Add("$c4c-" + i + extra, "Super Mega Cart 4-" + i + ",774397");
				idToEntry.Add("$c5c-" + i + extra, "Super Mega Cart 5-" + i + ",df212f");
				idToEntry.Add("$c4a-" + i + extra, "Super Mega Cart 6-" + i + ",cbc525");
				idToEntry.Add("$c5a-" + i + extra, "Super Mega Cart 7-" + i + ",ad5600");
				idToEntry.Add("$c4b-" + i + extra, "Super Mega Cart 8-" + i + ",e9a9ac");
				idToEntry.Add("$c5b-" + i + extra, "Super Mega Cart 9-" + i + ",d78cbf");
				idToEntry.Add("c6-" + i, "Super Mega Cart 10-" + i + ",c5000a");

				idToEntry.Add("g0-" + i, "Ghosts N Grieving " + i + ",88bc5b");
				idToEntry.Add("g1-" + i, "Spike Tales " + i + ",dddddd");
				idToEntry.Add("g2-" + i, "Scab or Die " + i + ",b3000c");
				idToEntry.Add("g3-" + i, "Tombs and Torture " + i + ",954d0b");
				idToEntry.Add("g4-" + i, "Pus Man " + i + ",3bab9f");
				idToEntry.Add("g5-" + i, "Dead Racer " + i + ",f00092");
			}

			for (int i = 1; i <= 8; i++) {
				idToEntry.Add("i1-" + i, "Denial 1-" + i + ",6c88ce");
				if (i <= 7) {
					idToEntry.Add("i2-" + i, "Anger 2-" + i + ",aa63ce");
				}
				if (i <= 6) {
					idToEntry.Add("i3-" + i, "Bargaining 3-" + i + ",805053");
					idToEntry.Add("i4-" + i, "Depression 4-" + i + ",271d1b");
				}

				if (i <= 7) {
					idToEntry.Add("$i1-" + i, "Acceptance 1-" + i + ",4a5d8d");
				}
				if (i > 1) {
					if (i <= 6) {
						idToEntry.Add("$i2-" + i, "Anger 2-" + i + ",aa63ce");
					}
					if (i <= 5) {
						idToEntry.Add("$i3-" + i, "Bargaining 3-" + i + ",805053");
						idToEntry.Add("$i4-" + i, "Depression 4-" + i + ",271d1b");
					}
				}
				if (i <= 5) {
					idToEntry.Add("i5-" + i, "Acceptance 5-" + i + ",828377");
				}
			}
			idToEntry.Add("$i1-8x", "Acceptance 1-8,4a5d8d");
			idToEntry.Add("$i2-1x", "Anger 2-1,aa63ce");
			idToEntry.Add("$i2-7x", "Anger 2-7,aa63ce");
			idToEntry.Add("$i3-1x", "Bargaining 3-1,805053");
			idToEntry.Add("$i3-6x", "Bargaining 3-6,805053");
			idToEntry.Add("$i4-1x", "Depression 4-1,271d1b");
			idToEntry.Add("$i4-6x", "Depression 4-6,271d1b");
		}
		private void InitializeMiniMap() {
			InitializeLevelEntries();

			string mapCSV = ReadCSVFromGPAK();
			DataTable map = CSV.ToDataTable(mapCSV, ',', false);
			cells = new MiniMapCell[map.Rows.Count, map.Columns.Count];

			for (int i = 0; i < map.Rows.Count; i++) {
				DataRow row = map.Rows[i];
				for (int j = 0; j < map.Columns.Count; j++) {
					string mapID = row[j] as string;
					if (!string.IsNullOrEmpty(mapID)) {
						mapID = mapID.Replace(".lvl", "");
						if (mapID != "..") {
							Point pos = new Point(j, i);
							mapCoords.Add(mapID, pos);
							mapRevCoords.Add(pos, mapID);
							LevelEntry entry = null;
							idToEntry.TryGetValue(mapID, out entry);
							cells[i, j] = new MiniMapCell(mapID, string.IsNullOrEmpty(entry?.Name) ? "???" : entry?.Name, j, i, CellType.Normal, (entry?.Color).GetValueOrDefault(Color.White));
						} else {
							cells[i, j] = new MiniMapCell(mapID, mapID, j, i, CellType.PathHorizontal, Color.FromArgb(60, 60, 60));
						}
					}
				}
			}

			gameMap.Cells = cells;
		}
		private string ReadCSVFromGPAK() {
			string gpak = Path.Combine(Path.GetDirectoryName(Memory.Program.MainModule.FileName), "game.gpak");

			byte[] csvData = null;
			using (FileStream file = new FileStream(gpak, FileMode.Open)) {
				int count = ReadInt(file);
				int position = 0;
				int csvLength = 0;

				bool foundCSV = false;
				for (int i = 0; i < count; i++) {
					int textLen = ReadShort(file);

					string path = ReadText(file, textLen);
					if (!foundCSV && path.IndexOf("map.csv", StringComparison.OrdinalIgnoreCase) > 0) {
						foundCSV = true;
					}

					if (!foundCSV) {
						position += ReadInt(file);
					} else if (csvLength == 0) {
						csvLength = ReadInt(file);
						csvData = new byte[csvLength];
					} else {
						ReadInt(file);
					}
				}

				file.Position += position;

				file.Read(csvData, 0, csvLength);
				file.Close();
			}

			return Encoding.UTF8.GetString(csvData);
		}
		private int ReadInt(FileStream file) {
			byte[] data = new byte[4];
			file.Read(data, 0, 4);
			return BitConverter.ToInt32(data, 0);
		}
		private int ReadShort(FileStream file) {
			byte[] data = new byte[2];
			file.Read(data, 0, 2);
			return BitConverter.ToInt16(data, 0);
		}
		private string ReadText(FileStream file, int length) {
			byte[] data = new byte[length];
			file.Read(data, 0, length);
			return Encoding.UTF8.GetString(data);
		}
		public void UpdateValues() {
			if (this.InvokeRequired) {
				this.Invoke((Action)UpdateValues);
			} else {
				inMap = Memory.InMap();
				if (inMap) {
					Point world = Memory.WorldMap();
					PointD player = Memory.PlayerPosition();
					bool wallLeft = Memory.OnWallLeft();
					bool wallRight = Memory.OnWallRight();
					bool onCeiling = Memory.OnCeiling();
					bool onGround = Memory.OnGround();
					bool isDead = Memory.IsDead();
					bool hanging = Memory.HangingFromWall();
					int cartLives = Memory.CartLives();
					int cartContinues = cartLives / 10;

					if ((world.X != lastX || world.Y != lastY) && mapRevCoords.Count > 0) {
						lastX = world.X;
						lastY = world.Y;
						mapRevCoords.TryGetValue(new Point(lastX, lastY), out lastMap);
						gameMap.SelectedCell = cells[lastY, lastX];
						savedPos = null;
					}

					if (chkAutoSave.Checked) {
						if (player.Close(lastPlayer) && onGround) {
							frameCount++;
							if (frameCount >= 60) {
								savedPos = player;
								frameCount = 0;
							}
						} else {
							frameCount = 0;
						}

						if (Memory.IsDead()) {
							if (savedPos.HasValue) {
								shouldRespawn = true;
							}
						} else if (shouldRespawn) {
							shouldRespawn = false;
							Memory.SetPlayerPosition(savedPos.Value.X, savedPos.Value.Y);
						}
					}

					if (chkInfiniteLives.Checked) {
						if (cartContinues < 10) {
							if (savedLives == 0 || cartLives > savedLives) {
								savedLives = cartLives;
							} else if (savedLives == cartLives && (cartLives % 10) == 0) {
								savedLives++;
							}
							if (cartLives < savedLives) {
								Memory.SetCartLives(savedLives);
							}
						} else {
							savedLives = 0;
						}
					} else if (savedLives > 0) {
						savedLives = 0;
					}

					lastPlayer = player;

					LevelEntry entry = null;
					idToEntry.TryGetValue(lastMap, out entry);

					lblWorldPosition.Text = "World: (" + world.X + ", " + world.Y + ")";
					lblCurrentMap.Text = "Current Map: " + entry?.Name + " (" + lastMap + ")";
					lblPlayerPosition.Text = "Player: (" + player.ToString() + ")" + (savedPos.HasValue ? " Saved: (" + savedPos.Value.ToString() + ")" : "");
					lblInfo.Text = "Player Info: " + (cartContinues < 10 ? "Continues(" + cartContinues + ") " : "") + (onGround ? "OnGround " : "InAir ") + (onCeiling ? "OnCeiling " : "") + (wallLeft || (hanging && !wallRight) ? "OnWallL " : "") + (wallRight ? "OnWallR " : "") + (hanging ? "Hanging " : "") + (isDead ? "Dead " : "");
				} else {
					lblWorldPosition.Text = "World: (?, ?)";
					lblCurrentMap.Text = "Current Map: ?";
					lblPlayerPosition.Text = "Player: (?, ?)";
					lblInfo.Text = "Player Info: ?";
				}
			}
		}

		private void btnMoveToSaved_Click(object sender, EventArgs e) {
			if (savedPos.HasValue) {
				Memory.SetPlayerPosition(savedPos.Value.X, savedPos.Value.Y);
			}
		}
		private void btnSavePosition_Click(object sender, EventArgs e) {
			savedPos = Memory.PlayerPosition();
		}
		private void gameMap_HoverOverCell(object sender, MiniMapCell cell) {
			string cellText = cell == null || cell.Type >= CellType.PathHorizontal ? "N/A" : cell.Name + " (" + cell.ID + ")";
			lblHoverText.Text = "Map: " + cellText;
			lblHoverText.Visible = cell != null && cell.Type < CellType.PathHorizontal;
		}
		private void gameMap_ClickedCell(object sender, MiniMapCell cell) {
			try {
				if (Memory.InMap()) {
					Memory.SetWorldMap(cell.X, cell.Y);
				}
			} catch (Exception ex) {
				MessageBox.Show(ex.ToString());
			}
		}
	}
	public class LevelEntry {
		public string Name { get; set; }
		public Color Color { get; set; }

		public LevelEntry(string name, Color color) {
			Name = name;
			Color = color;
		}

		public static implicit operator LevelEntry(string s) {
			int index = s.IndexOf(',');
			string name = s;
			if (index > 0) {
				name = s.Substring(0, index);
			} else {
				index = s.Length - 1;
			}
			string colorHex = s.Length - index - 1 < 6 ? "FFFFFF" : s.Substring(index + 1);
			int rgb = 0;
			int.TryParse(colorHex, NumberStyles.HexNumber, null, out rgb);
			rgb |= 0xff << 24;
			return new LevelEntry(name, Color.FromArgb(rgb));
		}
	}
}