using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class BuildCompressedPlayer  {

	[MenuItem("Build/BuildPlayer")]
	static void Build()
	{
		BuildPipeline.BuildPlayer (
			EditorBuildSettings.scenes, "export.apk",
			BuildTarget.Android,
			BuildOptions.CompressWithLz4);
	}
}
