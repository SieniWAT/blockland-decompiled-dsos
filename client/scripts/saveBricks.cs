function testsb ()
{
	saveBricks ("base/saves/what/test.bls");
}

function testlb ()
{
	loadBricks ("base/saves/what/test.bls");
}

function saveBricks (%fileName, %description)
{
	%file = new FileObject ("");
	if (!isWriteableFileName (%fileName))
	{
		error ("ERROR: saveBricks() - Invalid Filename!");
		return;
	}
	if (fileExt (%fileName) !$= ".bls")
	{
		error ("ERROR: saveBricks() - File extension must be .bls");
		return;
	}
	%file.openForWrite (%fileName);
	%file.writeLine ("This is a Blockland save file.  You probably shouldn\'t modify it cause you\'ll screw it up.");
	%lineCount = getLineCount (%description);
	%file.writeLine (%lineCount);
	%i = 0;
	while (%i < %lineCount)
	{
		%line = getLine (%description, %i);
		%file.writeLine (%line);
		%i += 1;
	}
	%i = 0;
	while (%i < 64)
	{
		%color = getColorIDTable (%i);
		%file.writeLine (%color);
		%i += 1;
	}
	%group = ServerConnection.getId ();
	%count = %group.getCount ();
	%i = 0;
	while (%i < %count)
	{
		%obj = %group.getObject (%i);
		if (%obj.getClassName () $= "fxDTSBrick")
		{
			if (%obj.isPlanted () && !%obj.isDead ())
			{
				%objData = %obj.getDataBlock ();
				%uiName = %objData.uiName;
				%trans = %obj.getTransform ();
				%pos = getWords (%trans, 0, 2);
				%line = %uiName @ "\"" SPC %pos SPC %obj.getAngleID () SPC %obj.isBasePlate () SPC %obj.getColorID () SPC %obj.getPrintID () SPC %obj.isPearl () SPC %obj.isChrome ();
				%file.writeLine (%line);
			}
			else 
			{
				echo ("brick ", %obj, " not planted - skipping ... ");
			}
		}
		%i += 1;
	}
	%file.close ();
	%file.delete ();
}

function LoadBricks_GetColorDifference (%fileName)
{
	%file = new FileObject ("");
	if (!isFile (%fileName))
	{
		error ("ERROR: loadBricks() - File not found!");
	}
	%file.openForRead (%fileName);
	%file.readLine ();
	%lineCount = %file.readLine ();
	%i = 0;
	while (%i < %lineCount)
	{
		%file.readLine ();
		%i += 1;
	}
	%colorCount = 0;
	%i = 0;
	while (%i < 64)
	{
		if (getWord (getColorIDTable (%i), 3) > 0.001)
		{
			%colorCount += 1;
		}
		%i += 1;
	}
	%different = 0;
	%i = 0;
	while (%i < 64)
	{
		%color = %file.readLine ();
		%match = 0;
		%j = 0;
		while (%j < 64)
		{
			if (colorMatch (getColorIDTable (%j), %color))
			{
				%match = 1;
				break;
			}
			%j += 1;
		}
		if (%match == 0)
		{
			%different = 1;
			%colorCount += 1;
		}
		%i += 1;
	}
	%file.close ();
	%file.delete ();
	if (%different == 1)
	{
		if (%colorCount > 64)
		{
			return "REPLACE";
		}
		else 
		{
			return "APPEND";
		}
	}
	else 
	{
		return "SAME";
	}
}

function loadBricks (%fileName, %colorMethod)
{
	%file = new FileObject ("");
	if (!isFile (%fileName))
	{
		error ("ERROR: loadBricks() - File not found!");
	}
	if (!isObject (ServerGroup))
	{
		commandToServer ('StartBrickLoad');
	}
	else 
	{
		messageAll ('', "Loading bricks. Please wait.");
	}
	%file.openForRead (%fileName);
	%file.readLine ();
	%lineCount = %file.readLine ();
	%i = 0;
	while (%i < %lineCount)
	{
		%file.readLine ();
		%i += 1;
	}
	%colorCount = -1;
	%i = 0;
	while (%i < 64)
	{
		if (getWord (getColorIDTable (%i), 3) > 0.001)
		{
			%colorCount += 1;
		}
		%i += 1;
	}
	if (%colorMethod == 0)
	{
		
	}
	else if (%colorMethod == 1)
	{
		%divCount = 0;
		%i = 0;
		while (%i < 16)
		{
			if (getSprayCanDivisionSlot (%i) != 0)
			{
				%divCount += 1;
			}
			else 
			{
				break;
			}
			%i += 1;
		}
	}
	else if (%colorMethod == 2)
	{
		%colorCount = -1;
		%divCount = -1;
	}
	else if (%colorMethod == 3)
	{
		
	}
	%i = 0;
	while (%i < 64)
	{
		%color = %file.readLine ();
		%red = getWord (%color, 0);
		%green = getWord (%color, 1);
		%blue = getWord (%color, 2);
		%alpha = getWord (%color, 3);
		if (%colorMethod == 0)
		{
			if (%alpha >= 0.0001)
			{
				%match = 0;
				%j = 0;
				while (%j < 64)
				{
					if (colorMatch (getColorIDTable (%j), %color))
					{
						%colorTranslation[%i] = %j;
						%match = 1;
						break;
					}
					%j += 1;
				}
				if (%match == 0)
				{
					error ("ERROR: loadBricks() - color method 0 specified but match not found for color " @ %color);
				}
			}
		}
		else if (%colorMethod == 1)
		{
			if (%alpha >= 0.0001)
			{
				%match = 0;
				%j = 0;
				while (%j < 64)
				{
					if (colorMatch (getColorIDTable (%j), %color))
					{
						%colorTranslation[%i] = %j;
						%match = 1;
						break;
					}
					%j += 1;
				}
				if (%match == 0)
				{
					setSprayCanColor (%colorCount += 1, %color);
					%colorTranslation[%i] = %colorCount;
				}
			}
		}
		else if (%colorMethod == 2)
		{
			setSprayCanColor (%colorCount += 1, %color);
			%colorTranslation[%i] = %i;
		}
		else if (%colorMethod == 3)
		{
			if (%alpha >= 0.0001)
			{
				%minDiff = 99999;
				%matchIdx = -1;
				%j = 0;
				while (%j < 64)
				{
					%checkColor = getColorIDTable (%j);
					%checkRed = getWord (%checkColor, 0);
					%checkGreen = getWord (%checkColor, 1);
					%checkBlue = getWord (%checkColor, 2);
					%checkAlpha = getWord (%checkColor, 3);
					%diff = 0;
					%diff += mAbs (mAbs (%checkRed) - mAbs (%red));
					%diff += mAbs (mAbs (%checkGreen) - mAbs (%green));
					%diff += mAbs (mAbs (%checkBlue) - mAbs (%blue));
					%diff += mAbs (mAbs (%checkAlpha) - mAbs (%alpha));
					if (%diff < %minDiff)
					{
						%minDiff = %diff;
						%matchIdx = %j;
					}
					%j += 1;
				}
				if (%matchIdx == -1)
				{
					error ("ERROR - LoadBricks() - Nearest match failed - wtf.");
				}
				else 
				{
					%colorTranslation[%i] = %matchIdx;
				}
			}
		}
		%i += 1;
	}
	if (%colorMethod == 1)
	{
		setSprayCanDivision (%divCount, %colorCount, "File");
	}
	if (%colorMethod != 0 && %colorMethod != 3)
	{
		$maxSprayColors = colorCount;
		%clientIndex = 0;
		while (%clientIndex < ClientGroup.getCount ())
		{
			%cl = ClientGroup.getObject (%clientIndex);
			%cl.transmitStaticBrickData ();
			%cl.transmitDataBlocks (1);
			commandToClient (%cl, 'PlayGui_LoadPaint');
			%clientIndex += 1;
		}
	}
	if ($UINameTableCreated == 0)
	{
		createUiNameTable ();
	}
	%brickCount = 0;
	%failureCount = 0;
	while (!%file.isEOF ())
	{
		%line = %file.readLine ();
		%quotePos = strstr (%line, "\"");
		%uiName = getSubStr (%line, 0, %quotePos);
		%line = getSubStr (%line, %quotePos + 2, 9999);
		%pos = getWords (%line, 0, 2);
		%angId = getWord (%line, 3);
		%isBaseplate = getWord (%line, 4);
		%colorId = %colorTranslation[mFloor (getWord (%line, 5))];
		%printId = getWord (%line, 6);
		%isPearl = getWord (%line, 7);
		%isChrome = getWord (%line, 8);
		%db = $uiNameTable[%uiName];
		if (isObject (ServerGroup))
		{
			if (%db)
			{
				%trans = %pos;
				if (%angId == 0)
				{
					%trans = %trans SPC " 1 0 0 0";
				}
				else if (%angId == 1)
				{
					%trans = %trans SPC " 0 0 1" SPC $piOver2;
				}
				else if (%angId == 2)
				{
					%trans = %trans SPC " 0 0 1" SPC $pi;
				}
				else if (%angId == 3)
				{
					%trans = %trans SPC " 0 0 -1" SPC $piOver2;
				}
				%b = new fxDTSBrick ("")
				{
					dataBlock = %db;
					angleID = %angId;
					isBasePlate = %isBaseplate;
					colorID = %colorId;
					printID = %printId;
					isPearl = %isPearl;
					isChrome = %isChrome;
					isPlanted = 1;
				};
				MissionCleanup.add (%b);
				%b.setTransform (%trans);
				%err = %b.plant ();
				if (%err == 1 || %err == 3 || %err == 5)
				{
					%failureCount += 1;
					%b.delete ();
				}
				else 
				{
					%brickCount += 1;
				}
			}
			else 
			{
				error ("ERROR: loadBricks() - DataBlock not found for brick named ", %uiName);
				%failureCount += 1;
			}
		}
		else 
		{
			if (!$IamAdmin)
			{
				echo ("$IamAdmin == 0, aborting load");
				return;
			}
			%line = %db SPC %pos SPC %angId SPC %isBaseplate SPC %colorId SPC %printId SPC %isPearl SPC %isChrome;
			commandToServer ('LoadBrick', %line);
		}
	}
	if (!isObject (ServerGroup))
	{
		commandToServer ('EndBrickLoad');
	}
	else 
	{
		%time = getSimTime () - $LoadingBricks_StartTime;
		messageAll ('', %brickCount @ " bricks loaded in " @ getTimeString (%time / 1000));
		if (%failureCount > 1)
		{
			messageAll ('', %failureCount @ " bricks could not be placed");
		}
		else if (%failureCount == 1)
		{
			messageAll ('', "1 brick could not be placed");
		}
	}
	%file.close ();
	%file.delete ();
}

function createUiNameTable ()
{
	$UINameTableCreated = 1;
	%dbCount = getDataBlockGroupSize ();
	%i = 0;
	while (%i < %dbCount)
	{
		%db = getDataBlock (%i);
		if (%db.getClassName () $= "fxDTSBrickData")
		{
			$uiNameTable[%db.uiName] = %db;
		}
		%i += 1;
	}
}

function LoadBricksGui::onWake (%null)
{
	LoadBricks_MapMenu.clear ();
	%fileName = findFirstFile ("base/saves/*");
	while (%fileName !$= "")
	{
		%filePath = filePath (%fileName);
		%subDir = getSubStr (%filePath, 11, 9999);
		if (%subDir !$= "")
		{
			LoadBricks_AddMapFolder (%subDir);
		}
		%fileName = findNextFile ("base/saves/*");
	}
	LoadBricks_MapMenu.sort ();
	%id = 0;
	%text = LoadBricks_MapMenu.getTextById (%id);
	while (%text !$= "")
	{
		if (%text $= $MapSaveName)
		{
			break;
		}
		%text = LoadBricks_MapMenu.getTextById (%id += 1);
	}
	if (%text $= "")
	{
		$LoadBricks_LastMapMenuID = mFloor ($LoadBricks_LastMapMenuID);
		LoadBricks_MapMenu.setSelected ($LoadBricks_LastMapMenuID);
	}
	else 
	{
		LoadBricks_MapMenu.setSelected (%id);
	}
	LoadBricks_MapClick ();
}

function LoadBricks_AddMapFolder (%name)
{
	%rowCount = 0;
	%rowText = LoadBricks_MapMenu.getTextById (%rowCount);
	while (%rowText !$= "")
	{
		if (%rowText $= %name)
		{
			return;
		}
		%rowText = LoadBricks_MapMenu.getTextById (%rowCount += 1);
	}
	LoadBricks_MapMenu.add (%name, %rowCount);
}

function LoadBricks_MapClick ()
{
	%mapName = LoadBricks_MapMenu.getText ();
	$LoadBricks_LastMapMenuID = LoadBricks_MapMenu.getSelected ();
	if (%mapName $= "")
	{
		return;
	}
	LoadBricks_FileList.clear ();
	%dir = "base/saves/" @ %mapName @ "/*";
	%fileName = findFirstFile (%dir);
	%count = 0;
	while (%fileName !$= "")
	{
		%baseName = fileBase (%fileName);
		LoadBricks_FileList.addRow (%count, %baseName);
		%count += 1;
		%fileName = findNextFile (%dir);
	}
}

function LoadBricks_FileClick ()
{
	%id = LoadBricks_FileList.getSelectedId ();
	%fileName = LoadBricks_FileList.getRowTextById (%id);
	%fullPath = "base/saves/" @ $MapSaveName @ "/" @ %fileName @ ".bls";
	%description = SaveBricks_GetFileDescription (%fullPath);
	LoadBricks_Description.setText (%description);
}

function LoadBricks_Load ()
{
	$LoadingBricks_StartTime = getSimTime ();
	%mapName = LoadBricks_MapMenu.getText ();
	if (%mapName $= "")
	{
		return;
	}
	%id = LoadBricks_FileList.getSelectedId ();
	%fileName = LoadBricks_FileList.getRowTextById (%id);
	if (%fileName $= "")
	{
		return;
	}
	%fullPath = "base/saves/" @ %mapName @ "/" @ %fileName @ ".bls";
	%colorDiff = LoadBricks_GetColorDifference (%fullPath);
	if (%colorDiff $= "SAME")
	{
		loadBricks (%fullPath);
		Canvas.popDialog (LoadBricksGui);
		Canvas.popDialog (EscapeMenu);
	}
	else if (%colorDiff $= "APPEND")
	{
		BTN_LoadBricksColor_Append.setVisible (1);
		$LoadBricksColor_File = %fullPath;
		Canvas.pushDialog (LoadBricksColorGui);
	}
	else if (%colorDiff $= "REPLACE")
	{
		BTN_LoadBricksColor_Append.setVisible (0);
		$LoadBricksColor_File = %fullPath;
		Canvas.pushDialog (LoadBricksColorGui);
	}
}

function SaveBricksGui::onWake (%null)
{
	SaveBricks_FileName.setText ("");
	SaveBricks_Description.setText ("");
	SaveBricks_FileList.clear ();
	SaveBricks_Window.setText ("Save Bricks - " @ $MapSaveName);
	%savePath = "base/saves/" @ $MapSaveName @ "/*";
	%fileName = findFirstFile (%savePath);
	%count = 0;
	while (%fileName !$= "")
	{
		%baseName = fileBase (%fileName);
		SaveBricks_FileList.addRow (%count, %baseName);
		%count += 1;
		%fileName = findNextFile (%savePath);
	}
}

function SaveBricks_ClickFileList ()
{
	%id = SaveBricks_FileList.getSelectedId ();
	%fileName = SaveBricks_FileList.getRowTextById (%id);
	SaveBricks_FileName.setText (%fileName);
	%fullPath = "base/saves/" @ $MapSaveName @ "/" @ %fileName @ ".bls";
	SaveBricks_Description.setText (SaveBricks_GetFileDescription (%fullPath));
}

function SaveBricks_Save ()
{
	%fileName = SaveBricks_FileName.getValue ();
	if (%fileName $= "")
	{
		return;
	}
	%fullPath = "base/saves/" @ $MapSaveName @ "/" @ %fileName @ ".bls";
	%description = SaveBricks_Description.getText ();
	if (isFile (%fullPath))
	{
		
	}
	saveBricks (%fullPath, %description);
	Canvas.popDialog (SaveBricksGui);
	Canvas.popDialog (EscapeMenu);
}

function SaveBricks_GetFileDescription (%fileName)
{
	%file = new FileObject ("");
	if (fileExt (%fileName) !$= ".bls")
	{
		error ("ERROR : SaveBricks_GetFileDescription() - Filename does not end in .bls");
		return;
	}
	if (!isFile (%fileName))
	{
		error ("ERROR : SaveBricks_GetFileDescription() - File does not exist");
		return;
	}
	%file.openForRead (%fileName);
	%file.readLine ();
	%lineCount = %file.readLine ();
	%description = "";
	%i = 0;
	while (%i < %lineCount)
	{
		%line = %file.readLine ();
		if (%i == 0)
		{
			%description = %line;
		}
		else 
		{
			%description = %description @ "\n" @ %line;
		}
		%i += 1;
	}
	%file.close ();
	%file.delete ();
	return %description;
}

function colorMatch (%colorA, %colorB)
{
	%i = 0;
	while (%i < 4)
	{
		if (mAbs (getWord (%colorA, %i) - getWord (%colorB, %i)) > 0.005)
		{
			return 0;
		}
		%i += 1;
	}
	return 1;
}

