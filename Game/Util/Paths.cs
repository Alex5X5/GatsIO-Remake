﻿using System.IO;
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
	}

	public static string AssetsPath(string filePath) => 
		Path.Combine(Assembly.GetExecutingAssembly().Location, filePath);
}
