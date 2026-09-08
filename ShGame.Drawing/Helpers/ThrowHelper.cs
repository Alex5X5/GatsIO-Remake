namespace ShGame.Drawing.Helpers;

using System;

internal static class ThrowHelper {

	internal static void ThrowIfNull(object? param, string message) {
		if (param == null)
			throw new ArgumentNullException(message);
	}
}
