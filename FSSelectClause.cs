using System;
using System.IO;
using System.Collections.Generic;

namespace FileSearch {

	/* Represent one of the fields in the select clause */
	public class SelectField : IComparable<SelectField> {
		public FileSearchType.SelectFieldType Type;

		/* If the SelectField is of type ATTRIBUTE_T, this property stores the 
		 * FileObjectAttribute */
		public FileSearchType.FileObjectAttribute Attribute;

		public SelectField(FileSearchType.FileObjectAttribute attr) {
			Type = FileSearchType.SelectFieldType.ATTRIBUTE_T;
			Attribute = attr;
		}
		public int CompareTo(SelectField s) {
			return s.GetHashCode() - GetHashCode();
		}
		public bool EqualTo(FileSearchType.FileObjectAttribute attr) {
			return Type == FileSearchType.SelectFieldType.ATTRIBUTE_T && Attribute == attr;
		}
		public override bool Equals(object obj) {
			if (obj == null) {
				return false;
			}
			if (obj is SelectField) {
				bool result = false;
				switch (Type) {
					case FileSearchType.SelectFieldType.ATTRIBUTE_T:
						result = (obj as SelectField).Attribute == Attribute;
						break;
				}
				return result;
			} else {
				return false;
			}
		}
		public override int GetHashCode() {
			int result = (int)Type;
			int x = FileSearchType.GetBitWidthSelectFieldType();
			switch (Type) {
				case FileSearchType.SelectFieldType.ATTRIBUTE_T:
					result |= ((int) Attribute) << x;
					break;
			}
			return result;
		}
		public override string ToString() {
			string result = "SelectField(";
			result += FileSearchType.ToString(Attribute);
			return result + ")";
		}
	}

	/* Represents a list of attributes to grab from a file in a File Search Command */
	public class SelectClause {
		private List<SelectField> Fields;

		public SelectClause() {
			Fields = new List<SelectField>();
		}
		public List<SelectField> GetFields() {
			return Fields;
		}

		// Adds attr to the list of fields to grab
		public void Add(FileSearchType.FileObjectAttribute attr) {
			Fields.Add(new SelectField(attr));
			Fields.Sort();
		}
		// Removed attr from the list of fields to grab
		public void Remove(FileSearchType.FileObjectAttribute attr) {
			SelectField S = new SelectField(attr);
			Fields.Remove(S);
		}
		// Checks if attr is in the list of fields to grab
		public bool Contains(FileSearchType.FileObjectAttribute attr) {
			bool result = false;
			foreach (SelectField s in Fields) {
				if (s.EqualTo(attr)) {
					return true;
				}
			}
			return result;
		}
		
		/* Grabs the necessary attributes defined in Fields from f and 
		 * stores the string results in result */
		public void GetFieldsFrom(FileSystemInfo f, List<string> result) {
			foreach (SelectField sf in Fields) {
				if (sf.Type == FileSearchType.SelectFieldType.ATTRIBUTE_T) {
					result.Add(FileSearchType.ToString(sf.Attribute) + " : " + FileSearchType.GetAttribute(f,sf.Attribute));
				}
			}
		}
		public override string ToString() {
			string result = "SelectClause(";
			for (int i = 0; i < Fields.Count; i++) {
				if (i != 0) {
					result += ",";
				}
				result += Fields[i].ToString();
			}
			return result + ")";
		}
		public bool Complete() {
			return true;
		}
	}
}