using System;
using System.IO;

namespace FileSearch {

	/* Represents a clause that will return a file object */
	public class FileObjectClause : ObjectClause {
		private string Path;
		private FileSearchType.FileObjectAttribute Relation;
		private bool HasAttributes;

		public FileObjectClause() {
			Path = "";
		}
		public FileObjectClause(FileSearchType.FileObjectAttribute R) {
			Relation = R;
		}
		public FileSearchType.FileObjectAttribute GetRelation() {
			return Relation;
		}

		/* Verifies if the File referenced by this object contains the
		 * string value given by the StringClause Clause in the content
		 * of this file */
		public bool Contains(StringClause Clause) {
			bool result = false;
			switch (Relation) {
				case FileSearchType.FileObjectAttribute.CONTENT:
					if (File.Exists(Path)) {

						// Scan the file
						using (StreamReader reader = new StreamReader(Path)) {
							string line;
							string s = Clause.GetValue();
							while ((line = reader.ReadLine()) != null && !result) {
								result = line.Contains(s);
							}
						}
					} else {
						result = false;
					}
					break;
			}
			return result;
		}

		/* Returns the FileSystemInfo object referenced by the Path
		 * that this object holds */
		public FileSystemInfo GetValue() {
			FileSystemInfo f = null;
			switch (Relation) {
				case FileSearchType.FileObjectAttribute.CONTENT:
					f = new FileInfo(Path);
					break;
			}
			return f;
		}

		/* Grabs the Path of the file object given to it */
		public override void Give(FileSystemInfo file) {
			Path = file.FullName;
			HasAttributes = true;
		}
		public override bool HasNecessaryAttributes() {
			return HasAttributes;
		}
		public override void Reset() {
			Path = "";
			HasAttributes = false;
		}
		public override string ToString() {
			return FileSearchType.ToString(Relation);
		}
		public override void Set(string str) {}
	}
}