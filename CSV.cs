using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.IO;
public static class CSV {
	private static Encoding encoding = Encoding.GetEncoding(1252);

	public static string FromDataView(DataTable results, bool writeHeader, char delimiter, params string[] columns) {
		if (columns == null || columns.Length == 0) {
			columns = new string[results.Columns.Count];
			int i = 0;
			foreach (DataColumn col in results.Columns) {
				columns[i++] = col.ColumnName;
			}
		}
		StringBuilder csv = WriteHeader(results, writeHeader, delimiter, columns);
		char[] search = new char[] { delimiter, (char)13, (char)10 };
		foreach (DataRowView row in results.DefaultView) {
			bool start = true;
			foreach (string name in columns) {
				if (results.Columns.Contains(name)) {
					WriteValue(csv, delimiter, start, row[name]);
					start = false;
				}
			}
			csv.AppendLine();
		}
		return csv.ToString();
	}
	public static string FromDataTable(DataTable results, bool writeHeader, char delimiter, params string[] columns) {
		if (columns == null || columns.Length == 0) {
			columns = new string[results.Columns.Count];
			int i = 0;
			foreach (DataColumn col in results.Columns) {
				columns[i++] = col.ColumnName;
			}
		}
		StringBuilder csv = WriteHeader(results, writeHeader, delimiter, columns);

		foreach (DataRow row in results.Rows) {
			bool start = true;
			foreach (string name in columns) {
				if (results.Columns.Contains(name)) {
					WriteValue(csv, delimiter, start, row[name]);
					start = false;
				}
			}
			csv.AppendLine();
		}
		return csv.ToString();
	}
	private static StringBuilder WriteHeader(DataTable results, bool writeHeader, char delimiter, params string[] columns) {
		StringBuilder csv = new StringBuilder();
		if (writeHeader) {
			bool start = true;
			foreach (string col in columns) {
				if (results.Columns.Contains(col)) {
					string name = col.Replace("\"", "\"\"");
					if ((start && name.Equals("ID", StringComparison.OrdinalIgnoreCase)) || name.IndexOf(delimiter) >= 0)
						name = '\"' + name + '\"';
					if (csv.Length == 0) {
						csv.Append(name);
					} else {
						csv.Append(delimiter).Append(name);
					}
					start = false;
				}
			}
			csv.AppendLine();
		}
		return csv;
	}
	public static void FromDataTable(Stream stream, DataTable results, bool writeHeader, char delimiter, params string[] columns) {
		if (columns == null || columns.Length == 0) {
			columns = new string[results.Columns.Count];
			int i = 0;
			foreach (DataColumn col in results.Columns) {
				columns[i++] = col.ColumnName;
			}
		}
		StringBuilder csv = WriteHeader(results, writeHeader, delimiter, columns);

		foreach (DataRow row in results.Rows) {
			bool start = true;
			foreach (string name in columns) {
				if (results.Columns.Contains(name)) {
					WriteValue(csv, delimiter, start, row[name]);
					start = false;
				}
			}
			csv.AppendLine();
			byte[] data = encoding.GetBytes(csv.ToString());
			stream.Write(data, 0, data.Length);
			csv.Length = 0;
		}

		if (csv.Length > 0) {
			byte[] data = encoding.GetBytes(csv.ToString());
			stream.Write(data, 0, data.Length);
		}
	}
	private static void WriteValue(StringBuilder csv, char delimiter, bool start, object value) {
		string svalue;
		if (value is DateTime) {
			DateTime date = (DateTime)value;
			svalue = date.TimeOfDay.Ticks == 0 ? date.ToShortDateString() : date.ToString();
		} else if (value is bool) {
			svalue = (bool)value ? "1" : "0";
		} else {
			svalue = value.ToString();
		}
		svalue = svalue.Replace("\"", "\"\"");
		if (svalue.IndexOfAny(new char[] { delimiter, (char)13, (char)10 }) >= 0)
			svalue = '\"' + svalue + '\"';
		if (start) {
			csv.Append(svalue);
		} else {
			csv.Append(delimiter).Append(svalue);
		}
	}
	public static DataTable ToDataTable(string data, char delimiter = ',', bool hasHeaders = true, params Type[] columnTypes) {
		DataTable dt = new DataTable();
		int i = 0, f = 1;
		StringBuilder field = new StringBuilder();
		bool firstLine = true;
		bool includeQuote = false;
		bool finished = false;
		bool addField = false;
		DataRow row = null;
		List<string> fields = new List<string>();
		while (i < data.Length) {
			char d = data[i++];
			bool end = i >= data.Length;
			switch (d) {
				case '"':
					if (includeQuote && i < data.Length && data[i] == '"') {
						field.Append(d);
						i++;
					} else {
						includeQuote = !includeQuote;
					}
					if (end) { addField = true; }
					break;
				case '\r':
				case '\n':
					if (includeQuote) {
						field.Append(d);
					} else {
						finished = true;
						while (i < data.Length) {
							if (data[i] != '\r' && data[i] != '\n') {
								break;
							}
							i++;
						}
						addField = true;
					}
					break;
				default:
					if (d == delimiter) {
						if (includeQuote) {
							field.Append(d);
						} else {
							addField = true;
						}
					} else {
						if (end) { addField = true; }
						field.Append(d);
					}
					break;
			}
			if (addField) {
				if (!firstLine) {
					string currentField = field.ToString();
					if (!string.IsNullOrEmpty(currentField)) {
						if (columnTypes != null && columnTypes.Length > 0 && columnTypes[f] == typeof(bool)) {
							row[f++] = currentField == "1" || currentField.ToLower() == "true";
						} else {
							row[f++] = currentField;
						}
					} else {
						f++;
					}
					field.Length = 0;
					if (d == delimiter) {
						if (f == dt.Columns.Count) {
							throw new Exception("Found too many columns on data row " + dt.Rows.Count + 1 + ".");
						} else if (end) {
							row[f++] = string.Empty;
						}
					}
					if (finished || end) {
						dt.Rows.Add(row);
						row = dt.NewRow();
						f = 0;
						finished = false;
					}
					addField = false;
				} else {
					if (hasHeaders) {
						string column = field.ToString();
						if (string.IsNullOrEmpty(column)) { throw new Exception("Column name cannot be blank."); }
						if (dt.Columns.Contains(column)) { throw new Exception("Column " + column + " is specified multiple times."); }
						dt.Columns.Add(column, columnTypes == null || columnTypes.Length <= f - 1 ? typeof(string) : columnTypes[f - 1]);
						f++;
						field.Length = 0;
					} else {
						Type type = columnTypes == null || columnTypes.Length <= f - 1 ? typeof(string) : columnTypes[f - 1];
						dt.Columns.Add("Column" + f++, type);
						fields.Add(field.ToString());
						field.Length = 0;
						if (d == delimiter && end) {
							type = columnTypes == null || columnTypes.Length <= f - 1 ? typeof(string) : columnTypes[f - 1];
							dt.Columns.Add("Column" + f++, type);
							fields.Add(string.Empty);
						}
					}
					if (finished) {
						firstLine = false;
						includeQuote = false;
						addField = false;
						finished = false;
						f = 0;
						if (dt.Columns.Count == 0) { throw new Exception("No columns found."); }
						if (fields.Count > 0) {
							dt.Rows.Add(fields.ToArray());
							fields.Clear();
						}
						row = dt.NewRow();
					}
				}
				addField = false;
			}
		}

		return dt;
	}
}