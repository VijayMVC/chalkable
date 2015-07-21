using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Chalkable.StandardImport.Models
{
    public class CsvContainer
    {
        public IList<string> Columns { get; set; }
        public IList<CsvRow> Rows { get; set; }
        public IList<ColumnMappingItem> Mapping { get; set; }

        public string ToString(char delimiter)
        {
            StringBuilder res = new StringBuilder(1024);
            for (int i = 0; i < Columns.Count; i++)
                res.Append('"').Append(Columns[i]).Append('"').Append(delimiter);
            res.Append('"').Append("State").Append('"').Append(delimiter);
            res.Append('"').Append("Message").Append('"').Append("\n");
            for (int i = 0; i < Rows.Count; i++)
            {
                for (int j = 0; j < Columns.Count; j++)
                {
                    res.Append('"').Append(Rows[i].Items[j]).Append('"').Append(delimiter);
                }
                res.Append('"').Append(CsvRowStateValueProvider.GetValue(Rows[i].State)).Append('"').Append(delimiter);
                res.Append('"').Append(Rows[i].ErrorMessage).Append('"').Append("\n");
            }
            return res.ToString();
        }

        public MemoryStream ToStream(char delimiter)
        {
            var s = new MemoryStream();
            var sw = new StreamWriter(s);
            sw.Write(ToString(delimiter));
            sw.Flush();
            s.Seek(0, SeekOrigin.Begin);
            return s;
        }

        private List<string> ParseLine(string s, char delimiter)
        {
            var res = new List<string>();
            var builder = new StringBuilder(1024);
            bool quot = false;
            for (int i = 0; i < s.Length; i++)
            {
                if (quot)
                {
                    if (s[i] == '"')
                        quot = false;
                    else
                        builder.Append(s[i]);
                }
                else
                {
                    if (s[i] == delimiter)
                    {
                        res.Add(builder.ToString().Trim());
                        builder.Clear();
                    }
                    else
                    {
                        if (s[i] == '"')
                            quot = true;
                        else
                            builder.Append(s[i]);
                    }
                }
            }
            res.Add(builder.ToString().Trim());
            return res;
        }

        public const char DELIMITOR = ',';
        protected CsvContainer(byte[] csv)
        {
            using (TextReader reader = new StreamReader(new MemoryStream(csv)))
            {
                Columns = new List<string>();
                string line = reader.ReadLine();
                Columns = ParseLine(line, DELIMITOR);
                Rows = new List<CsvRow>();
                while (line != null)
                {
                    line = reader.ReadLine();
                    if (line != null)
                    {
                        var row = new CsvRow();
                        row.State = CsvRowState.Ready;
                        row.StateNumber = (int)CsvRowState.Ready;
                        row.Items = ParseLine(line, DELIMITOR).Take(Columns.Count).ToList();
                        Rows.Add(row);
                    }
                }
            }
        }

        public bool ValidateMapping(out string message)
        {
            for (int i = 0; i < Mapping.Count; i++)
                if (Mapping[i].IsRequired && (Mapping[i].SourceColumnIndex < 0 || Mapping[i].SourceColumnIndex >= Columns.Count))
                {
                    message = string.Format("mapping for column {0} is required", Mapping[i].DestanationColumnName);
                    return false;
                }
            message = "ok";
            return true;
        }

        public List<string> ColumnValuesDistinct(int columnIndex)
        {
            var values = new List<string>();
            for (int i = 0; i < Rows.Count; i++)
            {
                var name = Rows[i].Items[columnIndex];
                if (!string.IsNullOrEmpty(name) && !values.Any(x => x == name))
                    values.Add(name);
            }
            return values;
        }

        public bool ValideteCellHasValue(int row, int column, string message)
        {
            if (string.IsNullOrEmpty(Rows[row].Items[column]))
            {
                Rows[row].State = CsvRowState.Failed;
                Rows[row].ErrorMessage = message;
                return false;
            }
            return true;
        }

        public bool ValideteCellHasValue(int row, string destinationName, string message)
        {
            return ValideteCellHasValue(row, GetSourceColumnIndex(destinationName), message);
        }
        
        public int GetSourceColumnIndex(string destinationName)
        {
            var c = Mapping.FirstOrDefault(x => x.DestanationColumnName.ToLower() == destinationName.ToLower());
            if (c != null)
                return c.SourceColumnIndex;
            throw new Exception(string.Format("Cant find destination column {0} in mapping", destinationName));
        }

        private CsvContainer() { }
        public CsvContainer MultipliByColumn(int columnIndex, char delimitor)
        {
            var res = new CsvContainer();
            res.Columns = Columns;
            res.Mapping = Mapping;
            res.Rows = new List<CsvRow>(Rows.Count);
            for (int i = 0; i < Rows.Count; i++)
            {
                var vals = Rows[i].Items[columnIndex].Split(delimitor);
                for (int j = 0; j < vals.Length; j++)
                {
                    var r = Rows[i].Clone();
                    r.Items[columnIndex] = vals[j];
                    res.Rows.Add(r);
                }
            }
            return res;
        }
        
        protected int GetColumnIndex(string columnName)
        {
            int sourceIndex = -1;
            var source = Columns.FirstOrDefault(x => x.ToLower() == columnName.ToLower());
            if (source != null)
                sourceIndex = Columns.IndexOf(source);
            return sourceIndex;
        }

        public string GetValue(int row, string columnName)
        {
            return Rows[row].Items[GetSourceColumnIndex(columnName)];
        } 

        public bool HasFaildRows()
        {
            return Rows.Any(row => row.State == CsvRowState.Failed);
        }
    }

    public class CsvRow
    {
        public IList<string> Items { get; set; }
        public CsvRowState State { get; set; }
        public int StateNumber
        {
            get { return (int)State; }
            set { State = (CsvRowState)value; }
        }
        public string ErrorMessage { get; set; }
        public string GetByIndex(int index)
        {
            if (index >= 0 && index < Items.Count)
                return Items[index];
            return null;
        }

        public CsvRow Clone()
        {
            var res = new CsvRow
            {
                Items = new List<string>(Items),
                ErrorMessage = ErrorMessage,
                State = State,
                StateNumber = StateNumber
            };
            return res;
        }
    }

    public enum CsvRowState
    {
        Ready = 0,
        Success = 1,
        Failed = 2
    }

    public static class CsvRowStateValueProvider
    {
        public static string GetValue(CsvRowState state)
        {
            switch (state)
            {
                case CsvRowState.Failed:
                    return "Failed";
                case CsvRowState.Ready:
                    return "Ready";
                case CsvRowState.Success:
                    return "Success";
                default:
                    throw new Exception("Invalid Csv row state");
            }
        }
    }

    public class ColumnMappingItem
    {
        public int SourceColumnIndex { get; set; }
        public string DestanationColumnName { get; set; }
        public bool IsRequired { get; set; }
        public string InCellSpliter { get; set; }
    }
}
