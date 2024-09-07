using System;
using System.IO;

namespace FileSearch {
	public class FromClause {

		// Stores the directory/file path that the query will be grabbing from
		private string Path;
		public FromClause(string f = "") {
			Path = f;
		}
		public string GetPath() {
			return Path;
		}
		public void SetPath(string s) {
			Path = s;
		}
		public FileSystemInfo GetFileObject() {
			FileSystemInfo f;
			if (Directory.Exists(Path)) {
				f = new DirectoryInfo(Path);
			} else {
				f = new FileInfo(Path);
			}
			return f;
		}
		public string[] GetFiles() {
			string[] files;
			if (File.Exists(Path)) {
				files = new string[1];
				files[0] = Path;
			} else if (Directory.Exists(Path)) {
				files = Directory.GetFiles(Path);
			} else {
				files = new string[0];
			}
			return files;
		}
		public override string ToString() {
			return "FromClause(\"" + Path + "\")";
		}
		public bool Complete() {
			return true;
		}
	}
}