using System;
using System.IO;
using System.Collections.Generic;

namespace FileSearch {
	public class C {
		public static int Iteration;
		public static void P(string s) {
			string result = "";
			for (int i = 0; i < Iteration; i++) {
				result += "\t";
			}
			Console.WriteLine(result + s);
		}
		public static void U() {
			Iteration++;
		}
		public static void D() {
			Iteration--;
		}
	}

	/* This class defines many types and methods that are used throughout the 
	 * FileSearch program */
	public class FileSearchType {

		/* Data Types used in FileSearch Program */
		public enum Type {
			NUMBER_T,
			BOOLEAN_T,
			STRING_T,
			DATETIME_T,
			FILEOBJECT_T
		}

		/* Possible truth values that BOOLEAN_T can be evaluated to */
		public enum TruthValue {
			TRUE_T = 1,
			FALSE_T = 0,
			UNKNOWN_T = 2
		}

		/* Attributes that can be grabbed from a file object */
		public enum FileObjectAttribute {
			NAME = 0,
			FULL_NAME = 1,
			EXTENSION = 2,
			PATH = 3,
			LENGTH = 4,
			IS_READ_ONLY = 5,
			IS_DIRECTORY = 6,
			IS_FILE = 7,
			CREATION_TIME = 8,
			LAST_ACCESS_TIME = 9,
			LAST_WRITE_TIME = 10,
			CONTENT = 11
		}
		/* Possible types of Select values that can be added to a Select Clause */
		public enum SelectFieldType {
			ATTRIBUTE_T = 0
		}

		/* Gets the Type from a Given FileObjectAttribute */
		public static Type GetType(FileObjectAttribute Attribute) {
			Type t = Type.NUMBER_T;
			switch (Attribute) {
				case FileObjectAttribute.NAME:
				case FileObjectAttribute.FULL_NAME:
				case FileObjectAttribute.EXTENSION:
				case FileObjectAttribute.PATH:
					t = Type.STRING_T;
					break;
				case FileObjectAttribute.LENGTH:
					t = Type.NUMBER_T;
					break;
				case FileObjectAttribute.IS_READ_ONLY:
				case FileObjectAttribute.IS_DIRECTORY:
				case FileObjectAttribute.IS_FILE:
					t = Type.BOOLEAN_T;
					break;
				case FileObjectAttribute.CREATION_TIME:
				case FileObjectAttribute.LAST_ACCESS_TIME:
				case FileObjectAttribute.LAST_WRITE_TIME:
					t = Type.DATETIME_T;
					break;
				case FileObjectAttribute.CONTENT:
					t = Type.FILEOBJECT_T;
					break;
			}
			return t;
		}

		/* Gets the FileObjectAttribute attr from the file object f */
		public static string GetAttribute(FileSystemInfo f, FileObjectAttribute attr) {
			string result = "";
			int x;
			switch (attr) {
				case FileObjectAttribute.NAME:
					x = f.Name.IndexOf(".");
					if (x == -1) {
						result = f.Name;
					} else {
						result = f.Name.Substring(0,x);
					}
					break;
				case FileObjectAttribute.FULL_NAME:
					result = f.Name;
					break;
				case FileObjectAttribute.EXTENSION:
					result = f.Extension;
					break;
				case FileObjectAttribute.PATH:
					result = f.FullName;
					break;
				case FileObjectAttribute.LENGTH:
					if (f is FileInfo) {
						result = ((FileInfo) f).Length.ToString();
					} else {
						result = "";
					}
					break;
				case FileObjectAttribute.IS_READ_ONLY:
					if (f is FileInfo) {
						if (((FileInfo) f).IsReadOnly) {
							result = "true";
						} else {
							result = "false";
						}
					} else {
						result = "false";
					}
					break;
				case FileObjectAttribute.IS_DIRECTORY:
					if (f is DirectoryInfo) {
						result = "true";
					} else {
						result = "false";
					}
					break;
				case FileObjectAttribute.IS_FILE:
					if (f is FileInfo) {
						result = "true";
					} else {
						result = "false";
					}
					break;
				case FileObjectAttribute.CREATION_TIME:
					result = f.CreationTime.ToString();
					break;
				case FileObjectAttribute.LAST_ACCESS_TIME:
					result = f.LastAccessTime.ToString();
					break;
				case FileObjectAttribute.LAST_WRITE_TIME:
					result = f.LastWriteTime.ToString();
					break;
			}
			return result;
		}

		/* Gets the Maximum bit width of a SelectFieldType */
		public static int GetBitWidthSelectFieldType() {
			int result = 0;
			int x = 0;
			foreach (SelectFieldType a in Enum.GetValues(typeof(SelectFieldType))) {
				if (x > (int)a) {
					x = (int) a;
				}
			}
			while (x > 0) {
				x >>= 1;
				++result;
			}
			return result;
		}

		/* Get all available attributes for a select clause */
		public static FileObjectAttribute[] GetSelectAttributes() {
			FileObjectAttribute[] Attributes;
			int index = 0;
			foreach (FileObjectAttribute Attribute in (FileObjectAttribute[]) Enum.GetValues(typeof(FileObjectAttribute))) {
				if (Attribute != FileObjectAttribute.CONTENT) {
					++index;
				}
			}
			Attributes = new FileObjectAttribute[index];
			index = 0;
			foreach (FileObjectAttribute Attribute in (FileObjectAttribute[]) Enum.GetValues(typeof(FileObjectAttribute))) {
				if (Attribute != FileObjectAttribute.CONTENT) {
					Attributes[index] = Attribute;
					++index;
				}
			}
			return Attributes;
		}

		/* Get all FileObjectAttributes that have a Type in t */
		public static FileObjectAttribute[] GetAttributesByType(List<Type> t) {
			List<FileObjectAttribute> l = new List<FileObjectAttribute>();
			foreach (FileObjectAttribute attr in Enum.GetValues(typeof(FileObjectAttribute))) {
				if (t.Contains(GetType(attr))) {
					l.Add(attr);
				}
			}
			FileObjectAttribute[] Attributes = new FileObjectAttribute[l.Count];
			for (int i = 0; i < Attributes.Length; i++) {
				Attributes[i] = l[i];
			}
			return Attributes;
		}

		/* Get the attribute corresponding to the string value str */
		public static FileObjectAttribute AttributeMap(string str) {
			FileObjectAttribute a = FileObjectAttribute.NAME;
			foreach (FileObjectAttribute attr in Enum.GetValues(typeof(FileObjectAttribute))) {
				if (str.Equals(ToString(attr))) {
					a = attr;
					break;
				}
			}
			return a;
		}

		/* Get a more formal string representation of a given FileObjectAttribute foa */
		public static string ToString(FileObjectAttribute foa) {
			string result = "";
			switch (foa) {
				case FileObjectAttribute.NAME:
					result = "Name";
					break;
				case FileObjectAttribute.FULL_NAME:
					result = "Full name";
					break;
				case FileObjectAttribute.EXTENSION:
					result = "Extension";
					break;
				case FileObjectAttribute.PATH:
					result = "Path";
					break;
				case FileObjectAttribute.LENGTH:
					result = "Length";
					break;
				case FileObjectAttribute.IS_READ_ONLY:
					result = "Is read only";
					break;
				case FileObjectAttribute.IS_DIRECTORY:
					result = "Is directory";
					break;
				case FileObjectAttribute.IS_FILE:
					result = "Is file";
					break;
				case FileObjectAttribute.CREATION_TIME:
					result = "Creation time";
					break;
				case FileObjectAttribute.LAST_ACCESS_TIME:
					result = "Last access time";
					break;
				case FileObjectAttribute.LAST_WRITE_TIME:
					result = "Last write time";
					break;
				case FileObjectAttribute.CONTENT:
					result = "Content";
					break;
			}
			return result;
		}

		/* Get a more formal string representation of a given TruthValue t */
		public static string ToString(TruthValue t) {
			string result = "";
			switch (t) {
				case TruthValue.TRUE_T:
					result = "true";
					break;
				case TruthValue.FALSE_T:
					result = "false";
					break;
				case TruthValue.UNKNOWN_T:
					result = "unknown";
					break;
			}
			return result;
		}
	}

	/* Abstract clause that is mean to be inherited by a clause of one of the types 
	 * Found in FileSearchType.Type */
	public abstract class ObjectClause {
		public string Tag;
		public List<ObjectClause> Args;

		// List of valid types for each argument
		public List<List<FileSearchType.Type>> ValidTypes;

		public abstract void Give(FileSystemInfo file);

		// Resets the values in the clause to the state before a file was given to it
		public abstract void Reset();

		// Sets the values in the clause based on the string provided
		public abstract void Set(string str);

		// Checks if the clause has the necessary attributes to get the values it needs
		public abstract bool HasNecessaryAttributes();
	}
}