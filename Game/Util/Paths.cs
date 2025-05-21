using System.IO;
using System.Reflection;
using System.Text;

namespace ShGame.Game.Util;


public static class Paths {

	public static void ExtractFiles() {
		string resourceNamespacePrefix = "ShGame.Assets";

		// Get executing assembly
		var assembly = Assembly.GetExecutingAssembly();
		var resourceNames = assembly.GetManifestResourceNames();

		// Determine output path next to the running .exe
		string exeDirectory = AppDomain.CurrentDomain.BaseDirectory;

		for(int i=0; i<resourceNames.Length; i++) {
			// Filter resources inside the specified namespace
			if (!resourceNames[i].StartsWith(resourceNamespacePrefix))
				continue;
			string resourceName = resourceNames[i];
			while (resourceName.Split('.').Length>2) {
				resourceName = new StringBuilder(resourceName)
					.Insert(resourceName.IndexOf('.')+1, '\\')
						.Remove(resourceName.IndexOf('.'), 1)
							.ToString();
			}
			Console.WriteLine(resourceName);
			resourceName=exeDirectory+resourceName;
			if (File.Exists(resourceName))
				continue;
            Directory.CreateDirectory(Path.GetDirectoryName(resourceName)!);
            using Stream? resourceStream = assembly.GetManifestResourceStream(resourceNames[i]);
            if (resourceStream == null) {
                Console.WriteLine($"Failed to load resource: {resourceName}");
                continue;
            }

            using FileStream fileStream = new FileStream(resourceName, FileMode.Create, FileAccess.Write);
            resourceStream.CopyTo(fileStream);
            Console.WriteLine($"Extracted: {resourceName}");

        }
        //resourceName+=resourceNameresourceName.Split('.')[resourceName.IndexOf('.')]='\\';

  //      // Create a relative file path (e.g., from MyProject.Assets.images.logo.png => images\logo.png)
  //      string relativePath = resourceName.Substring(resourceNamespacePrefix.Length + 1).Replace('.', Path.DirectorySeparatorChar);
		//	// Fix file extension: ensure last "." becomes a real extension (e.g. .png)
		//	int lastSeparator = relativePath.LastIndexOf(Path.DirectorySeparatorChar);
		//	int lastDot = relativePath.LastIndexOf('.');
		//	if (lastDot > lastSeparator) {
		//		relativePath = relativePath[..lastDot] + "." + relativePath[(lastDot + 1)..];
		//	}

		//	string fullOutputPath = Path.Combine(outputPath, relativePath);

		//	// Ensure any subfolders are created
		//	Directory.CreateDirectory(Path.GetDirectoryName(fullOutputPath)!);

		//	if (resourceStream == null) {
		//		Console.WriteLine($"Failed to load resource: {resourceName}");
		//		continue;
		//	}

		//	using FileStream fileStream = new FileStream(fullOutputPath, FileMode.Create, FileAccess.Write);
		//	resourceStream.CopyTo(fileStream);

		//	Console.WriteLine($"Extracted: {relativePath}");
		//}

		//Console.WriteLine($"\nAll resources extracted to: {outputPath}");
	}
	private static bool Exists(string path) =>
		Path.Exists(path);

	

	private static void ExtractAssembly() {
		
	}

	public static string AssetsPath(string filePath) {
		var assembly = Assembly.GetExecutingAssembly().GetManifestResourceStream(filePath);
		return "";
	}
}
