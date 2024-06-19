function recordingsDlg::onWake ()
{
	RecordingsDlgList.clear ();
	%i = 0;
	%filespec = $currentMod @ "/recordings/*.rec";
	echo (%filespec);
	%file = findFirstFile (%filespec);
	while (%file !$= "")
	{
		%fileName = fileBase (%file);
		if (strstr (%file, "/CVS/") == -1)
		{
			RecordingsDlgList.addRow (%i += 1, %fileName);
		}
		%file = findNextFile (%filespec);
	}
	RecordingsDlgList.sort (0);
	RecordingsDlgList.setSelectedRow (0);
	RecordingsDlgList.scrollVisible (0);
}

function StartSelectedDemo ()
{
	%sel = RecordingsDlgList.getSelectedId ();
	%rowText = RecordingsDlgList.getRowTextById (%sel);
	%file = $currentMod @ "/recordings/" @ getField (%rowText, 0) @ ".rec";
	new GameConnection (ServerConnection);
	RootGroup.add (ServerConnection);
	if (ServerConnection.playDemo (%file))
	{
		Canvas.popDialog (recordingsDlg);
		ServerConnection.prepDemoPlayback ();
	}
	else 
	{
		MessageBoxOK ("Playback Failed", "Demo playback failed for file \'" @ %file @ "\'.");
		if (isObject (ServerConnection))
		{
			ServerConnection.delete ();
		}
	}
}

function startDemoRecord ()
{
	ServerConnection.stopRecording ();
	if (ServerConnection.isDemoPlaying ())
	{
		return;
	}
	%i = 0;
	while (%i < 1000)
	{
		%num = %i;
		if (%num < 10)
		{
			%num = 0 @ %num;
		}
		if (%num < 100)
		{
			%num = 0 @ %num;
		}
		%file = $currentMod @ "/recordings/demo" @ %num @ ".rec";
		if (!isFile (%file))
		{
			break;
		}
		%i += 1;
	}
	if (%i == 1000)
	{
		return;
	}
	$DemoFileName = %file;
	ChatHud.addLine ("\c4Recording to file [\c2" @ $DemoFileName @ "\cr].");
	ServerConnection.prepDemoRecord ();
	ServerConnection.startRecording ($DemoFileName);
	if (!ServerConnection.isDemoRecording ())
	{
		deleteFile ($DemoFileName);
		ChatHud.addLine ("\c3 *** Failed to record to file [\c2" @ $DemoFileName @ "\cr].");
		$DemoFileName = "";
	}
}

function stopDemoRecord ()
{
	if (ServerConnection.isDemoRecording ())
	{
		ChatHud.addLine ("\c4Recording file [\c2" @ $DemoFileName @ "\cr] finished.");
		ServerConnection.stopRecording ();
	}
}

function demoPlaybackComplete ()
{
	disconnect ();
	Canvas.setContent ("MainMenuGui");
	Canvas.pushDialog (recordingsDlg);
}

