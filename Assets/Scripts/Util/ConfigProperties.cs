using System;
using UnityEngine;

public class ConfigProperties {

	private static readonly String FILE_NAME = "config.ini";

	private static readonly String GENERAL_SECTION = "General";
	private static readonly String IS_SERVER = "IsServer";

	private static ConfigProperties instance;

	public static ConfigProperties Instance {
		get {
			if (instance == null) {
				instance = new ConfigProperties ();
			}
			return instance;
		}
	}

	public bool isServer { get; private set; }

	private ConfigProperties () {
		IniFileParser parser = new IniFileParser ();
		parser.Open (FILE_NAME);
		isServer = parser.ReadValue (GENERAL_SECTION, IS_SERVER, false);
		parser.Close ();
	}
}

