using System;
using System.IO;

namespace FileSearch {

	/* Represents a clause that returns an string value */
	public abstract class StringClause : ObjectClause {
		protected string Value;

		public string GetValue() {
			return Value;
		}
		public bool Contains(StringClause s) {
			return Value.Contains(s.GetValue());
		}
	}

	/* Represents a string literal */
	public class StringLiteralClause : StringClause {
		public StringLiteralClause(string s = "") {
			Value = s;
		}
		public override void Give(FileSystemInfo file) {}
		public override bool HasNecessaryAttributes() {
			return true;
		}
		public override void Reset() {}
		public override string ToString() {
			return "\"" + Value + "\"";
		}
		public override void Set(string str) {
			Value = str;
		}
	}

	/* Represents a FileObjectAttribute of type STRING_T */
	public class StringAttributeClause : StringClause {

		// Keeps track of whether the necessary attribute has been provided yet through
		// the Give() method
		private bool AttributeProvided;
		private FileSearchType.FileObjectAttribute Attribute;

		public StringAttributeClause() {}
		public StringAttributeClause(FileSearchType.FileObjectAttribute Attr) {
			if (FileSearchType.GetType(Attr) == FileSearchType.Type.STRING_T) {
				Attribute = Attr;
			}
		}
		public override void Give(FileSystemInfo file) {
			Value = FileSearchType.GetAttribute(file,Attribute);
			AttributeProvided = true;
		}
		public override bool HasNecessaryAttributes() {
			return AttributeProvided;
		}
		public override void Reset() {
			AttributeProvided = false;
		}
		public override string ToString() {
			return FileSearchType.ToString(Attribute);
		}
		public override void Set(string str) {
			Attribute = FileSearchType.AttributeMap(str);
		}
	}
}
