function onMissionDownloadPhase1 (%null, %null)
{
	MessageHud.close ();
	LoadingProgress.setValue (0);
	LoadingProgressTxt.setValue ("LOADING DATABLOCKS");
}

function onPhase1Progress (%progress)
{
	LoadingProgress.setValue (%progress);
	Canvas.repaint ();
}

function onPhase1Complete ()
{
	
}

function onMissionDownloadPhase2 ()
{
	LoadingProgress.setValue (0);
	LoadingProgressTxt.setValue ("LOADING OBJECTS");
	Canvas.repaint ();
}

function onPhase2Progress (%progress)
{
	LoadingProgress.setValue (%progress);
	Canvas.repaint ();
}

function onPhase2Complete ()
{
	
}

function onFileChunkReceived (%fileName, %ofs, %size)
{
	LoadingProgress.setValue (%ofs / %size);
	LoadingProgressTxt.setValue ("Downloading " @ %fileName @ "...");
}

function onMissionDownloadPhase3 ()
{
	LoadingProgress.setValue (0);
	LoadingProgressTxt.setValue ("LIGHTING MISSION (This only happens once)");
	Canvas.repaint ();
}

function onPhase3Progress (%progress)
{
	LoadingProgress.setValue (%progress);
}

function onPhase3Complete ()
{
	LoadingProgress.setValue (1);
	$lightingMission = 0;
}

function onMissionDownloadComplete ()
{
	
}

addMessageCallback ('MsgLoadInfo', handleLoadInfoMessage);
addMessageCallback ('MsgLoadDescripition', handleLoadDescriptionMessage);
addMessageCallback ('MsgLoadMapPicture', handleLoadMapPictureMessage);
addMessageCallback ('MsgLoadInfoDone', handleLoadInfoDoneMessage);
function handleLoadInfoMessage (%null, %null, %mapName, %mapSaveName)
{
	Canvas.setContent ("LoadingGui");
	%line = 0;
	while (%line < LoadingGui.qLineCount)
	{
		LoadingGui.qLine[%line] = "";
		%line += 1;
	}
	LoadingGui.qLineCount = 0;
	LOAD_MapName.setText (%mapName);
	$MapSaveName = %mapSaveName;
}

function handleLoadDescriptionMessage (%null, %null, %line)
{
	%text = LOAD_MapDescription.getText ();
	if (%text !$= "")
	{
		LOAD_MapDescription.setText (%text @ "\n" @ %line);
	}
	else 
	{
		LOAD_MapDescription.setText (%line);
	}
	return;
	LoadingGui.qLine[LoadingGui.qLineCount] = %line;
	LoadingGui.qLineCount += 1;
	%text = "<spush><font:Arial:16>";
	%line = 0;
	while (%line < LoadingGui.qLineCount - 1)
	{
		%text = %text @ LoadingGui.qLine[%line] @ " ";
		%line += 1;
	}
	%text = %text @ LoadingGui.qLine[%line] @ "<spop>";
	LOAD_MapDescription.setText (%text);
}

function handleLoadMapPictureMessage (%null, %null, %imageName)
{
	LOAD_MapPicture.setBitmap (%imageName);
}

function handleLoadInfoDoneMessage (%null, %null)
{
	
}

