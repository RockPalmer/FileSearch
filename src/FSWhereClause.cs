using System;
using System.IO;
using System.Collections.Generic;

namespace FileSearch {

	/* Represents all the conditions that a file must pass before the necessary
	 * fields defined in the Select clause of a command can be grabbed from it */
	public class WhereClause : BooleanClause {
		public BooleanClause SubClause;

		public WhereClause() {}
		public WhereClause(BooleanClause Clause) {
			SubClause = Clause;
		}
		public void Set(BooleanClause Clause) {
			SubClause = Clause;
		}
		public override void Give(FileSystemInfo file) {
			SubClause.Give(file);
			Value = SubClause.GetValue();
		}
		public override bool HasNecessaryAttributes() {
			return SubClause.HasNecessaryAttributes();
		}
		public override void Reset() {
			SubClause.Reset();
			Value = FileSearchType.TruthValue.UNKNOWN_T;
		}
		public override string ToString() {
			string result = "WhereClause(";
			if (SubClause != null) {
				result += SubClause.ToString();
			}
			return result + ")";
		}

		/* Defines all the allowed clauses that can be used for SubClause */
		public static string[] AllowedOperators() {
			string[] s = new string[1];
			s[0] = "CONTAINS";
			return s;
		}
		public override void Set(string str) {}
		public override bool Complete() {
			return SubClause != null && SubClause.Complete();
		}
	}

	/* Represents a clause that takes 2 arguments and performs 1 of the 2 following tasks:
	 * 
	 * (1) If the first argument (a1) is a file and the second argument (a2) is a string,
	 * 		returns true iff the content of a1 contains the string a2 
	 * (2) If the both arguments are strings,
	 * 		returns true iff the string a1 contains the string a2 */
	public class ContainsClause: BooleanClause {
		public ContainsClause() {

			// Add slots for the 2 arguments
			Args = new List<ObjectClause>();
			Args.Add(null);
			Args.Add(null);

			// Define the allowed types for the 2 arguments
			ValidTypes = new List<List<FileSearchType.Type>>();
			ValidTypes.Add(new List<FileSearchType.Type>());
			ValidTypes.Add(new List<FileSearchType.Type>());

			ValidTypes[0].Add(FileSearchType.Type.STRING_T);
			ValidTypes[0].Add(FileSearchType.Type.FILEOBJECT_T);
			ValidTypes[1].Add(FileSearchType.Type.STRING_T);
		}
		public void SetArg(int index, ObjectClause Clause) {
			Args[index] = Clause;
		}
		public override void Give(FileSystemInfo file) {
			if (Args[0] is StringClause) {
				if (Args[1] is StringClause) {
					/* If both args are strings */

					// If either argument does not have the information it needs,
					// Give the file object to them
					if (!Args[0].HasNecessaryAttributes()) {
						Args[0].Give(file);
					}
					if (!Args[1].HasNecessaryAttributes()) {
						Args[1].Give(file);
					}

					// If either arg still doesn't have all the info it needs, the function,
					// returns unknown, otherwise
					if (!Args[0].HasNecessaryAttributes() || !Args[1].HasNecessaryAttributes()) {
						Value = FileSearchType.TruthValue.UNKNOWN_T;
					} else {

						// If both args DO have all the info they need, the clause is 
						// evaluated
						if ((Args[0] as StringClause).Contains(Args[1] as StringClause)) {
							Value = FileSearchType.TruthValue.TRUE_T;
						} else {
							Value = FileSearchType.TruthValue.FALSE_T;
						}
					}
				}
			} else if (Args[0] is FileObjectClause) {
				if (Args[1] is StringClause) {
					/* If the first arg is a file and the second arg is a string */

					// If either argument does not have the information it needs,
					// Give the file object to them
					if (!Args[0].HasNecessaryAttributes()) {
						Args[0].Give(file);
					}
					if (!Args[1].HasNecessaryAttributes()) {
						Args[1].Give(file);
					}
					if (!Args[0].HasNecessaryAttributes() || !Args[1].HasNecessaryAttributes()) {
						Value = FileSearchType.TruthValue.UNKNOWN_T;
					} else {
						if ((Args[0] as FileObjectClause).Contains(Args[1] as StringClause)) {
							Value = FileSearchType.TruthValue.TRUE_T;
						} else {
							Value = FileSearchType.TruthValue.FALSE_T;
						}
					}
				}
			}
		}
		public override bool HasNecessaryAttributes() {
			return Args[0].HasNecessaryAttributes() && Args[1].HasNecessaryAttributes();
		}

		/* Resets both arguments for the next file */
		public override void Reset() {
			Args[0].Reset();
			Args[1].Reset();
			if (!Args[0].HasNecessaryAttributes() || !Args[1].HasNecessaryAttributes()) {
				Value = FileSearchType.TruthValue.UNKNOWN_T;
			} else {
				if (Args[0] is StringClause) {
					if (Args[1] is FileObjectClause) {
						if ((Args[0] as StringClause).Contains(Args[1] as StringClause)) {
							Value = FileSearchType.TruthValue.TRUE_T;
						} else {
							Value = FileSearchType.TruthValue.FALSE_T;
						}
					}
				}
			}
		}
		public override string ToString () {
			string result = "";
			if (Args[0] != null) {
				result += Args[0].ToString() + " ";
			}
			result += "CONTAINS";
			if (Args[1] != null) {
				result += " " + Args[1].ToString();
			}
			return result;
		}
		public override void Set(string str) {}

		/* Returns true if there are at least 2 arguments */
		public override bool Complete() {
			return Args.Count > 0 && Args[0] != null && Args[1] != null;
		}
	}
}
