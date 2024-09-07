using System;
using System.IO;

namespace FileSearch {

	/* Represents some clause that will return a boolean value */
	public abstract class BooleanClause : ObjectClause {
		protected FileSearchType.TruthValue Value;

		public BooleanClause() {
			Value = FileSearchType.TruthValue.UNKNOWN_T;
		}
		public FileSearchType.TruthValue GetValue() {
			return Value;
		}
		public abstract bool Complete();
	}

	/* Represents a boolean literal (true or false) */
	public class BooleanLiteralClause : BooleanClause {
		public BooleanLiteralClause(FileSearchType.TruthValue t) {
			Value = t;
		}
		public override void Give(FileSystemInfo file) {}
		public override bool HasNecessaryAttributes() {
			return true;
		}
		public override void Reset() {}
		public override string ToString() {
			return FileSearchType.ToString(Value);
		}
		public override void Set(string str) {}
		public override bool Complete() {
			return true;
		}
	}
}
