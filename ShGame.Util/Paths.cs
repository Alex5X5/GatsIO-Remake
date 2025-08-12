namespace ShGame.Util;

using System;
using System.IO;
using System.Reflection;
using System.Text;

public static class Paths {

	public static Assembly AssetAssembly; 

	public static void ExtractFiles() {
		string resourceNamespacePrefix = "ShGame.Util.Assets";

		// Get executing assembly
		AssetAssembly = Assembly.GetExecutingAssembly();
		
		var resourceNames = AssetAssembly.GetManifestResourceNames();

		// Determine output path next to the running .exe
		string exeDirectory = AppDomain.CurrentDomain.BaseDirectory;

		int dots = 0;
		string name = AssetAssembly.GetName().FullName;
		foreach (char c in name.ToCharArray()) { 
			if(c==' ')
				break;
			if (c=='.')
				dots++;
		}

		for(int i=0; i<resourceNames.Length; i++) {
			// Filter resources inside the specified namespace
			if (!resourceNames[i].StartsWith(resourceNamespacePrefix))
				continue;
			string resourceName = resourceNames[i];
			resourceName = resourceName.Replace(resourceNamespacePrefix+".", "");
			while (resourceName.Split('.').Length-dots>1) {
				resourceName = new StringBuilder(resourceName)
					.Insert(resourceName.IndexOf('.')+1, '\\')
						.Remove(resourceName.IndexOf('.'), 1)
							.ToString();
			}
			resourceName=exeDirectory+"ShGame\\Assets\\"+resourceName;
			if (File.Exists(resourceName))
				continue;
            Directory.CreateDirectory(Path.GetDirectoryName(resourceName)!);
            using Stream? resourceStream = AssetAssembly.GetManifestResourceStream(resourceNames[i]);
            if (resourceStream == null) {
                Console.WriteLine($"Failed to load resource: {resourceName}");
                continue;
            }

            using FileStream fileStream = new FileStream(resourceName, FileMode.Create, FileAccess.Write);
            resourceStream.CopyTo(fileStream);
            Console.WriteLine($"Extracted: {resourceName}");

        }
	}

	public static string AssetsPath(string fileName) {
		string assemplyName = AssetAssembly.GetName().Name+".dll";
		string assemblyLocation = AssetAssembly.Location;
		string trimedName = assemblyLocation.Trim(assemplyName.ToCharArray());
		trimedName = Path.Combine(trimedName+@"ShGame\Assets\", fileName);
		return trimedName;
	} 
}
