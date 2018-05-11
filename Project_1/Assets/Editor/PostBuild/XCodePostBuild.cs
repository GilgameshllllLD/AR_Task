using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;

using UnityEditor.iOS.Xcode;
using System.IO;
    
public class XCodePostBuild : MonoBehaviour {
	// The name of the Settings.bundle
	const string SETTINGS_BUNDLE = "Settings.bundle";

	// The path to Settings.bundle in the repository
	static string PathToSettingsBundle {
		get { return Path.GetFullPath(Application.dataPath + "/../xcode/" + SETTINGS_BUNDLE); }
	}

	// Triggered after a build.
	[PostProcessBuild]
	public static void OnPostprocessBuild(BuildTarget buildTarget, string pathToBuiltProject) {
		if (buildTarget == BuildTarget.iOS) {
			UpdateXCodeProject(pathToBuiltProject);
		}
	}

	/*
	 * Our iOS xCode project needs an extra bundle called "Settings.bundle" so that 
	 * a few preferences appear in the iOS system preferences window.
	 * 
	 * This function ensures that the generated XCode project has that bundle.
	 *
	 * For details of the structure of the Settings.bundle, see
	 * https://developer.apple.com/library/ios/documentation/Cocoa/Conceptual/UserDefaults/Preferences/Preferences.html
	 */
	public static void UpdateXCodeProject(string pathToBuiltProject) {
		var projPath = PBXProject.GetPBXProjectPath(pathToBuiltProject);

		// Read in the xcode project
		var proj = new PBXProject();
		proj.ReadFromString(File.ReadAllText(projPath));

		// See if it already has the Settings.bundle file
		var target = proj.TargetGuidByName(PBXProject.GetUnityTargetName());
		if (!proj.ContainsFileByProjectPath(SETTINGS_BUNDLE)) {
			// If not, add a reference to the file, which is in the repo
			var fileGuid = proj.AddFile(PathToSettingsBundle, SETTINGS_BUNDLE, PBXSourceTree.Absolute);
			proj.AddFileToBuild(target, fileGuid);
			File.WriteAllText(projPath, proj.WriteToString());
			Debug.Log("Wrote " + SETTINGS_BUNDLE + " projPath");
		} else {
			Debug.Log(SETTINGS_BUNDLE + " already found in " + projPath);
		}
	}
}
