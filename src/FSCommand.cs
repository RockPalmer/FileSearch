using System;
using System.IO;
using System.Collections.Generic;

namespace FileSearch {

	/* This class represents any FileSearch command that the user could run */
	public class FileSearchCommand {
		public SelectClause Select;
		public FromClause From;
		public WhereClause Where;

		public FileSearchCommand(SelectClause S, FromClause F = null, WhereClause W = null) {
			Select = S;
			From = F;
			Where = W;
		}

		/* Runs the command and returns the results of the query as a List of strings */
		public List<string> Run() {
			List<string> results = new List<string>();

			if (From != null) {
				/* If From is not null, only grab files defined in the From clause */

				FileSystemInfo f = null;

				// Grab All Files defined in the From clause
				string[] files = From.GetFiles();
				if (Where != null) {
					/* If Where is not null, pass all files through the where clause.
					 * If they pass the Where clause, then use the Select clause
					 * to grab the necessary values from the file */

					foreach (string Path in files) {
						if (Directory.Exists(Path)) {
							f = new DirectoryInfo(Path);
						} else if (File.Exists(Path)) {
							f = new FileInfo(Path);
						}
						if (Where.GetValue() == FileSearchType.TruthValue.TRUE_T) {
							/* If the Where clause already evaluates as true, do not
							 * bother passing the file through it */

							results.Add("--NEW FILE--");

							// Grab the necessary fields from the file
							Select.GetFieldsFrom(f,results);
						} else {
							/* If the Where clause does not already evaluate as true, 
							 * do not pass the file through it */
							Where.Give(f);
							if (Where.GetValue() == FileSearchType.TruthValue.TRUE_T) {
								/* If the Where clause evaluates as true, grab the 
								 * necessary fields from it */

								results.Add("--NEW FILE--");
								Select.GetFieldsFrom(f,results);
							}

							// Reset the where clause for the next file
							Where.Reset();
						}
					}
				} else {
					/* If Where is null, assume all files from the From clause
					 * are files we need to get the necessary fields from */

					foreach (string Path in files) {
						if (Directory.Exists(Path)) {
							f = new DirectoryInfo(Path);
						} else if (File.Exists(Path)) {
							f = new FileInfo(Path);
						}
						results.Add("--NEW FILE--");
						Select.GetFieldsFrom(f,results);
					}
				}
			}

			results.Add("--FINISHED--");

			return results;
		}
		public override string ToString() {
			string result = Select.ToString();
			if (From != null) {
				result += "\n" + From.ToString();
			}
			if (Where != null) {
				result += "\n" + Where.ToString();
			}
			return result;
		}

		/* Checks if the Command is complete and, therefore, if the command can be run 
		 * yet */
		public bool Complete() {
			return (
				Select != null && Select.Complete() && (
					From == null ||
					From.Complete()
				) && (
					Where == null ||
					Where.Complete()
				)
			);
		}
	}
}
