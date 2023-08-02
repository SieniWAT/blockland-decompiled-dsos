$Game::EndGameScore = 0;
$Game::EndGamePause = 10;
function makePadString (%char, %num)
{
	%i = 0;
	while (%i < %num)
	{
		%ret = %ret @ %char;
		%i += 1;
	}
}

function onNeedRelight ()
{
	
}

function onServerCreated ()
{
	$Game::StartTime = 0;
	exec ("./DamageTypes.cs");
	initDefaultDamageTypes ();
	if (1)
	{
		exec ("./allGameScripts.cs");
		if ($Server::Dedicated)
		{
			if (!isFunction ("serverCmdRequestEventTables") || !isFunction ("ServerCmdRequestExtendedBrickInfo") || !isFunction ("SimObject", "serializeEventToString") || !isFunction ("GameConnection", "TransmitExtendedBrickInfo") || !isFunction ("auth_Init_Server") || !isFunction ("authTCPobj_Server", "onConnected") || !isFunction ("authTCPobj_Server", "onLine") || !isFunction ("WebCom_PostServer") || !isFunction ("postServerTCPObj", "onConnected") || !isFunction ("postServerTCPObj", "onLine") || !isFunction ("GameConnection", "onConnect") || !isFunction ("GameConnection", "authCheck"))
			{
				error ("ERROR: Required functions missing from allGameScripts (server)");
				return;
			}
		}
		else if (!isFunction ("serverCmdRequestEventTables") || !isFunction ("ServerCmdRequestExtendedBrickInfo") || !isFunction ("SimObject", "serializeEventToString") || !isFunction ("GameConnection", "TransmitExtendedBrickInfo") || !isFunction ("auth_Init_Client") || !isFunction ("authTCPobj_Client", "onConnected") || !isFunction ("authTCPobj_Client", "onLine") || !isFunction ("auth_Init_Server") || !isFunction ("authTCPobj_Server", "onConnected") || !isFunction ("authTCPobj_Server", "onLine") || !isFunction ("mainMenuGui", "onWake") || !isFunction ("WebCom_PostServer") || !isFunction ("postServerTCPObj", "onConnected") || !isFunction ("postServerTCPObj", "onLine") || !isFunction ("GameConnection", "onConnect") || !isFunction ("GameConnection", "authCheck"))
		{
			error ("ERROR: Required functions missing from allGameScripts");
			schedule (10, 0, disconnect);
			schedule (100, 0, MessageBoxOK, "File Error", "Required script files have been corrupted.\n\nPlease re-install the game.");
			return;
		}
	}
	else 
	{
		exec ("./simGroup.cs");
		exec ("./queue.cs");
		exec ("./serverLoadBricks.cs");
		exec ("./constants.cs");
		exec ("./kickBan.cs");
		exec ("./serverCmd.cs");
		exec ("./serverCmdPlay.cs");
		exec ("./icons.cs");
		exec ("./audioProfiles.cs");
		exec ("./camera.cs");
		exec ("./markers.cs");
		exec ("./triggers.cs");
		exec ("./inventory.cs");
		exec ("./shapeBase.cs");
		exec ("./item.cs");
		exec ("./staticShape.cs");
		exec ("./explosion.cs");
		exec ("./weapon.cs");
		exec ("./radiusDamage.cs");
		exec ("./projectile.cs");
		exec ("./player.cs");
		exec ("./aiPlayer.cs");
		exec ("./playerDeath.cs");
		exec ("./playerSpawn.cs");
		exec ("./hammer.cs");
		exec ("./wrench.cs");
		exec ("./wand.cs");
		exec ("./sprayCan.cs");
		exec ("./AdminWand.cs");
		exec ("./FXCans/flatCan.cs");
		exec ("./FXCans/pearlCan.cs");
		exec ("./FXCans/chromeCan.cs");
		exec ("./FXCans/glowCan.cs");
		exec ("./FXCans/blinkCan.cs");
		exec ("./FXCans/swirlCan.cs");
		exec ("./FXCans/rainbowCan.cs");
		exec ("./FXCans/stableCan.cs");
		exec ("./FXCans/jelloCan.cs");
		exec ("./brickWeapon.cs");
		exec ("./bricks.cs");
		exec ("./fxDTSBrick.cs");
		exec ("./vehicle.cs");
		exec ("./vehicleSpawnMarker.cs");
		exec ("./precipitation.cs");
		exec ("./particles.cs");
		exec ("./clock.cs");
		exec ("./consoleCmd.cs");
		exec ("./lights.cs");
		exec ("./effects.cs");
		exec ("./emote.cs");
		exec ("./music.cs");
		exec ("./brickSpawnPoint.cs");
		exec ("./BrickMan.cs");
		exec ("./TrustList.cs");
		exec ("./TrustListCheck.cs");
		exec ("./MiniGame.cs");
		exec ("./miniGameCheck.cs");
		exec ("./interactionCheck.cs");
		exec ("./AddOns.cs");
		exec ("./prints.cs");
		exec ("base/server/authComServer.cs");
	}
	%dataBlockCount_Paint = DataBlockGroup.getCount ();
	setSprayCanColors ();
	%dataBlockCount_Paint = DataBlockGroup.getCount () - %dataBlockCount_Paint;
	if (!$Server::Dedicated)
	{
		Canvas.setContent ("LoadingGui");
		LOAD_MapPicture.setBitmap ("base/data/missions/default");
		LoadingProgress.setValue (0);
		LoadingSecondaryProgress.setValue (0);
		LoadingProgressTxt.setValue ("LOADING ADD-ONS");
		Canvas.repaint ();
	}
	%dataBlockCount_AddOns = DataBlockGroup.getCount ();
	loadAddOns ();
	%dataBlockCount_AddOns = DataBlockGroup.getCount () - %dataBlockCount_AddOns;
	if (!$Server::Dedicated)
	{
		LoadingProgress.setValue (0);
		LoadingSecondaryProgress.setValue (0);
		LoadingProgressTxt.setValue ("LOADING MUSIC");
		Canvas.repaint ();
	}
	%dataBlockCount_Music = DataBlockGroup.getCount ();
	createMusicDatablocks ();
	%dataBlockCount_Music = DataBlockGroup.getCount () - %dataBlockCount_Music;
	verifyBrickUINames ();
	echo ("");
	echo ("");
	echo ("Datablock Report: ");
	echo ("  Base:    " @ DataBlockGroup.getCount () - (%dataBlockCount_Paint + %dataBlockCount_AddOns + %dataBlockCount_Music));
	echo ("  Paint:   " @ %dataBlockCount_Paint);
	echo ("  Add-Ons: " @ %dataBlockCount_AddOns);
	echo ("  Music:   " @ %dataBlockCount_Music);
	echo ("  Total:   " @ DataBlockGroup.getCount ());
	echo ("");
	$Game::StartTime = $Sim::Time;
	loadPrintedBrickTextures ();
	serverLoadAvatarNames ();
	if ($Server::Dedicated && !$Server::LAN)
	{
		auth_Init_Server ();
	}
	CreateBanManager ();
	InitMinigameColors ();
}

function onServerDestroyed ()
{
	
}

function verifyBrickUINames ()
{
	%size = getDataBlockGroupSize ();
	%i = 0;
	while (%i < %size)
	{
		%db = getDataBlock (%i);
		if (%db.getClassName () !$= "fxDTSBrickData")
		{
			
		}
		else if (%db.uiName $= "")
		{
			error ("ERROR: Brick datablock \"" @ %db.getName () @ "\" has no uiname");
		}
		else if (%uiNamePresent[%db.uiName])
		{
			if (%db.category !$= "" && %db.subCategory !$= "")
			{
				error ("ERROR: Brick datablock \"" @ %db.getName () @ "\" has the same uiname as \"" @ %uiNamePresent[%db.uiName].getName () @ "\" (" @ %db.uiName @ ") - removing.");
				%db.uiName = "";
			}
		}
		else if (%db.category !$= "" && %db.subCategory !$= "")
		{
			%uiNamePresent[%db.uiName] = %db;
		}
		%i += 1;
	}
}

function onMissionLoaded ()
{
	startGame ();
	$Server::MissionName = MissionInfo.name;
	$Server::BrickCount = 0;
	if ($Pref::Server::BrickLimit > 256000)
	{
		$Pref::Server::BrickLimit = 256000;
	}
	if ($Pref::Server::BrickLimit <= 0)
	{
		$Pref::Server::BrickLimit = 256000;
	}
	$IamAdmin = 2;
	snapshotGameAssets ();
	WebCom_PostServer ();
	pingMatchMakerLoop ();
	if ($Server::Dedicated)
	{
		schedule (2000, 0, echo, "Dedicated server is now running.");
	}
}

function onMissionEnded ()
{
	cancel ($Game::Schedule);
	$Game::Running = 0;
	$Game::Cycling = 0;
}

function startGame ()
{
	if ($Game::Running)
	{
		error ("startGame: End the game first!");
		return;
	}
	%clientIndex = 0;
	while (%clientIndex < ClientGroup.getCount ())
	{
		%cl = ClientGroup.getObject (%clientIndex);
		commandToClient (%cl, 'GameStart');
		%cl.score = 0;
		%clientIndex += 1;
	}
	if ($Pref::Server::TimeLimit)
	{
		$Game::Schedule = schedule ($Pref::Server::TimeLimit * 1000, 0, "onGameDurationEnd");
	}
	$Game::Running = 1;
}

function endGame ()
{
	if (!$Game::Running)
	{
		error ("endGame: No game running!");
		return;
	}
	endAllMinigames ();
	setTimeScale (19038, 1);
	%clientIndex = 0;
	while (%clientIndex < ClientGroup.getCount ())
	{
		%cl = ClientGroup.getObject (%clientIndex);
		commandToClient (%cl, 'TimeScale', getTimeScale ());
		%clientIndex += 1;
	}
	cancel ($Game::Schedule);
	%clientIndex = 0;
	while (%clientIndex < ClientGroup.getCount ())
	{
		%cl = ClientGroup.getObject (%clientIndex);
		commandToClient (%cl, 'GameEnd');
		%clientIndex += 1;
	}
	resetMission ();
	$Game::Running = 0;
	$Game::MissionCleaningUp = 0;
}

function onGameDurationEnd ()
{
	if ($Game::Duration && !isObject (EditorGui))
	{
		cycleGame ();
	}
}

function cycleGame ()
{
	if (!$Game::Cycling)
	{
		$Game::Cycling = 1;
		$Game::Schedule = schedule (0, 0, "onCycleExec");
	}
}

function onCycleExec ()
{
	endGame ();
	$Game::Schedule = schedule ($Game::EndGamePause * 1000, 0, "onCyclePauseEnd");
}

function onCyclePauseEnd ()
{
	$Game::Cycling = 0;
	%search = $Server::MissionFileSpec;
	%file = findFirstFile (%search);
	while (%file !$= "")
	{
		if (%file $= $Server::MissionFile)
		{
			%file = findNextFile (%search);
			if (%file $= "")
			{
				%file = findFirstFile (%search);
			}
			break;
		}
		%file = findNextFile (%search);
	}
	loadMission (%file);
}

function GameConnection::onClientEnterGame (%client)
{
	if (%client.getBLID () $= "")
	{
		%client.schedule (10, delete);
		return;
	}
	if (!$Server::LAN)
	{
		%doReset = 1;
		%count = ClientGroup.getCount ();
		%i = 0;
		while (%i < %count)
		{
			%cl = ClientGroup.getObject (%i);
			if (%cl == %client)
			{
				
			}
			else if (%cl.getBLID () == %client.getBLID ())
			{
				%doReset = 0;
				break;
			}
			%i += 1;
		}
		if (%client.isLocal ())
		{
			%doReset = 0;
		}
		if (%doReset)
		{
			%client.resetVehicles ();
		}
	}
	%client.undoStack = New_QueueSO (512);
	commandToClient (%client, 'SetBuildingDisabled', 0);
	commandToClient (%client, 'SetPaintingDisabled', 0);
	commandToClient (%client, 'SetPlayingMiniGame', 0);
	commandToClient (%client, 'SetRunningMiniGame', 0);
	commandToClient (%client, 'SetRemoteServerData', $Server::LAN);
	commandToClient (%client, 'TimeScale', getTimeScale ());
	%client.transmitMaxPlayers ();
	if (!$Server::LAN)
	{
		%client.transmitServerName ();
	}
	%client.Camera = new Camera ("")
	{
		dataBlock = Observer;
	};
	MissionCleanup.add (%client.Camera);
	%client.Camera.scopeToClient (%client);
	%client.score = 0;
	commandToClient (%client, 'clearMapList');
	%client.bpsCount = 0;
	%client.bpsTime = %currTime;
	sendLetterPrintInfo (%client);
	commandToClient (%client, 'PSD_KillPrints');
	commandToClient (%client, 'PlayGui_LoadPaint');
	%client.spawnPlayer ();
}

function GameConnection::setCanRespawn (%client, %val)
{
	%client.canRespawn = %val;
}

function GameConnection::onClientLeaveGame (%client)
{
	%client = %client;
	serverCmdStopTalking (%client);
	%mg = %client.miniGame;
	if (isObject (%mg))
	{
		%mg.removeMember (%client);
	}
	if (isObject (%client.light))
	{
		%client.light.delete ();
	}
	if (isObject (%client.undoStack))
	{
		%client.undoStack.delete ();
	}
	%player = %client.Player;
	if (isObject (%player))
	{
		if (isObject (%player.tempBrick))
		{
			%player.tempBrick.delete ();
		}
	}
	if (isObject (%client.tcpObj))
	{
		%client.tcpObj.delete ();
	}
	if (isObject (%client.Camera))
	{
		%client.Camera.delete ();
	}
	if (isObject (%client.Player))
	{
		%client.Player.delete ();
	}
	if (!$Server::LAN)
	{
		%doReset = 1;
		%count = ClientGroup.getCount ();
		%i = 0;
		while (%i < %count)
		{
			%cl = ClientGroup.getObject (%i);
			if (%cl == %client)
			{
				
			}
			else if (%cl.getBLID () == %client.getBLID ())
			{
				%doReset = 0;
				break;
			}
			%i += 1;
		}
		if (!isObject ($ServerGroup))
		{
			%doReset = 0;
		}
		if (%doReset)
		{
			%client.resetVehicles ();
			if ($Pref::Server::ClearEventsOnClientExit)
			{
				if (isObject (%client.brickGroup))
				{
					%quotaObject = %client.brickGroup.QuotaObject;
					if (isObject (%quotaObject))
					{
						%quotaObject.cancelEventsEvent = schedule (31000, %quotaObject, "cancelQuotaSchedules", %quotaObject);
						%quotaObject.cancelProjectilesEvent = %quotaObject.schedule (31000, $TypeMasks::ProjectileObjectType);
					}
				}
			}
		}
	}
	if (isObject (%client.brickGroup))
	{
		if ($Server::LAN)
		{
			if (%client.getBLID () != getLAN_BLID ())
			{
				error ("ERROR: GameConnection::onClientLeaveGame() - Client \"" @ %client.getPlayerName () @ "\" has invalid LAN bl_id (" @ %client.getBLID () @ ").");
				%client.brickGroup.delete ();
			}
		}
		else 
		{
			if (%client.bl_id $= "" || %client.getBLID () == -1)
			{
				%client.brickGroup.delete ();
			}
			%client.brickGroup.quitTime = getSimTime ();
			cleanUpBrickEmptyGroups ();
		}
	}
	else if ($missionRunning)
	{
		if (%client.hasAuthedOnce)
		{
			error ("ERROR: GameConnection::onClientLeaveGame() - Client \"" @ %client.getPlayerName () @ "\" has no brick group.");
		}
	}
}

function cleanUpBrickEmptyGroups ()
{
	%count = ClientGroup.getCount ();
	%i = 0;
	while (%i < %count)
	{
		%cl = ClientGroup.getObject (%i);
		%cl.brickGroup.DoNotDelete = 1;
		%i += 1;
	}
	%currTime = getSimTime ();
	%i = 0;
	while (%i < mainBrickGroup.getCount ())
	{
		%brickGroup = mainBrickGroup.getObject (%i);
		if (%brickGroup.DoNotDelete == 1)
		{
			
		}
		else if (%brickGroup.getCount () > 0)
		{
			
		}
		else if (%currTime - %brickGroup.quitTime < 30 * 60 * 1000)
		{
			
		}
		else 
		{
			%brickGroup.delete ();
			%i -= 1;
		}
		%i += 1;
	}
	%count = ClientGroup.getCount ();
	%i = 0;
	while (%i < %count)
	{
		%cl = ClientGroup.getObject (%i);
		%cl.brickGroup.DoNotDelete = "";
		%i += 1;
	}
}

function GameConnection::onLeaveMissionArea (%client)
{
	
}

function GameConnection::onEnterMissionArea (%client)
{
	
}

function GameConnection::onDeath (%client, %sourceObject, %sourceClient, %damageType, %damLoc)
{
	%player = %client.Player;
	if (isObject (%player))
	{
		%player.setShapeName ("", 8564862);
		if (isObject (%player.tempBrick))
		{
			%player.tempBrick.delete ();
			%player.tempBrick = "";
		}
		%player.client = 0;
	}
	else 
	{
		warn ("WARNING: No player object in GameConnection::onDeath() for client \'" @ %client @ "\'");
	}
	if (isObject (%client.Camera) && isObject (%client.Player))
	{
		%client.Camera.setMode ("Corpse", %client.Player);
		%client.setControlObject (%client.Camera);
	}
	%client.Player = 0;
	if ($Damage::Direct[%damageType] != 1)
	{
		if (getSimTime () - %player.lastDirectDamageTime < 100)
		{
			if (%player.lastDirectDamageType !$= "")
			{
				%damageType = %player.lastDirectDamageType;
			}
		}
	}
	if (%damageType == $DamageType::Impact)
	{
		if (isObject (%player.lastPusher))
		{
			if (getSimTime () - %player.lastPushTime <= 1000)
			{
				%sourceClient = %player.lastPusher;
			}
		}
	}
	%message = "%2 killed %1";
	if (%sourceClient == %client || %sourceClient == 0)
	{
		%message = $DeathMessage_Suicide[%damageType];
	}
	else 
	{
		%message = $DeathMessage_Murder[%damageType];
	}
	if ($Damage::Direct[%damageType] == 1 && %player.getWaterCoverage () < 0.05)
	{
		if (%sourceClient && isObject (%sourceClient.Player))
		{
			%playerVelocity = ((VectorLen (VectorSub (%player.preHitVelocity, %sourceClient.Player.getVelocity ())) / 2.64) * 6 * 3600) / 5280;
		}
		else 
		{
			%playerVelocity = ((VectorLen (%player.preHitVelocity) / 2.64) * 6 * 3600) / 5280;
		}
		%playerPos = %player.getPosition ();
		%mask = $TypeMasks::InteriorObjectType | $TypeMasks::TerrainObjectType | $TypeMasks::FxBrickObjectType;
		%res0 = containerRayCast (VectorAdd (%playerPos, "0 0 2"), VectorAdd (%playerPos, "0 0  -6.8"), %mask);
		%res1 = containerRayCast (VectorAdd (%playerPos, "0 0 2"), VectorAdd (%playerPos, "0 -1 -6.8"), %mask);
		%res2 = containerRayCast (VectorAdd (%playerPos, "0 0 2"), VectorAdd (%playerPos, "1 1  -6.8"), %mask);
		%res3 = containerRayCast (VectorAdd (%playerPos, "0 0 2"), VectorAdd (%playerPos, "-1 1 -6.8"), %mask);
		if (!isObject (getWord (%res0, 0)) && !isObject (getWord (%res1, 0)) && !isObject (getWord (%res2, 0)) && !isObject (getWord (%res3, 0)))
		{
			%range = round ((VectorLen (VectorSub (%playerPos, %sourceObject.originPoint)) / 2.65) * 6);
			if (isObject (%sourceClient.Player))
			{
				%sourceClient.Player.emote (winStarProjectile, 1);
			}
			%sourceClient.play2D (rewardSound);
			commandToClient (%sourceClient, 'BottomPrint', "<bitmap:base/client/ui/ci/star>\c3 MID AIR KILL - " @ %client.getPlayerName () @ " " @ round (%playerVelocity) @ "MPH, " @ %range @ "ft!", 3);
			commandToClient (%client, 'BottomPrint', "\c5 MID AIR\'d by " @ %sourceClient.getPlayerName () @ " - " @ round (%playerVelocity) @ "MPH, " @ %range @ "ft!", 3);
		}
	}
	if (isObject (%client.miniGame))
	{
		if (%sourceClient == %client)
		{
			%client.incScore (%client.miniGame.Points_KillSelf);
		}
		else if (%sourceClient == 0)
		{
			%client.incScore (%client.miniGame.Points_Die);
		}
		else 
		{
			%sourceClient.incScore (%client.miniGame.Points_KillPlayer);
			%client.incScore (%client.miniGame.Points_Die);
		}
	}
	%clientName = %client.getPlayerName ();
	if (isObject (%sourceClient))
	{
		%sourceClientName = %sourceClient.getPlayerName ();
	}
	else 
	{
		%sourceClientName = "";
	}
	if (isObject (%client.miniGame))
	{
		%client.miniGame.messageAllExcept (%client, 'MsgClientKilled', %message, %client.getPlayerName (), %sourceClientName);
		messageClient (%client, 'MsgYourDeath', %message, %client.getPlayerName (), %sourceClientName, %client.miniGame.respawnTime);
	}
	else 
	{
		messageAllExcept (%client, -1, 'MsgClientKilled', %message, %client.getPlayerName (), %sourceClientName);
		messageClient (%client, 'MsgYourDeath', %message, %client.getPlayerName (), %sourceClientName, $Game::MinRespawnTime);
	}
	return;
	if (%sourceClient == %client || %sourceClient == 0)
	{
		if (isObject (%client.miniGame))
		{
			%client.miniGame.messageAllExcept (%client, 'MsgClientKilled', %message, %client.getPlayerName (), %sourceClient.getPlayerName ());
			messageClient (%client, 'MsgYourDeath', %message, %client.getPlayerName (), %sourceClient.getPlayerName (), %client.miniGame.respawnTime);
		}
		else 
		{
			messageAllExcept (%client, -1, 'MsgClientKilled', %message, %client.getPlayerName (), %sourceClient.getPlayerName ());
			messageClient (%client, 'MsgYourDeath', %message, %client.getPlayerName (), %sourceClient.getPlayerName (), $Game::MinRespawnTime);
		}
		if (isObject (%client.miniGame))
		{
			%client.incScore (%client.miniGame.Points_KillSelf);
		}
	}
	else if (isObject (%sourceClient))
	{
		if (isObject (%client.miniGame))
		{
			%client.miniGame.messageAllExcept (%client, 'MsgClientKilled', '%1 was killed by %2!', %client.getPlayerName (), %sourceClient.getPlayerName ());
			messageClient (%client, 'MsgYourDeath', '%1 was killed by %2!', %client.getPlayerName (), %sourceClient.getPlayerName (), %client.miniGame.respawnTime);
		}
		else 
		{
			messageAllExcept (%client, -1, 'MsgClientKilled', '%1 was killed by %2!', %client.getPlayerName (), %sourceClient.getPlayerName ());
			messageClient (%client, 'MsgYourDeath', '%1 was killed by %2!', %client.getPlayerName (), %sourceClient.getPlayerName (), $Game::MinRespawnTime);
		}
		if (isObject (%client.miniGame))
		{
			%client.incScore (%client.miniGame.Points_Die);
		}
		if (isObject (%sourceClient.miniGame))
		{
			%sourceClient.incScore (%sourceClient.miniGame.Points_KillPlayer);
		}
	}
	else 
	{
		if (isObject (%client.miniGame))
		{
			%client.incScore (%client.miniGame.Points_Die);
		}
		if (isObject (%client.miniGame))
		{
			%client.miniGame.messageAllExcept (%client, 'MsgClientKilled', '%1 dies.', %client.getPlayerName ());
			messageClient (%client, 'MsgYourDeath', '%1 dies.', %client.getPlayerName (), "", %client.miniGame.respawnTime);
		}
		else 
		{
			messageAllExcept (%client, -1, 'MsgClientKilled', '%1 dies.', %client.getPlayerName ());
			messageClient (%client, 'MsgYourDeath', '%1 dies.', %client.getPlayerName (), "", $Game::MinRespawnTime);
		}
	}
}

function GameConnection::InstantRespawn (%client, %clientagain)
{
	%player = %client.Player;
	if (isObject (%player))
	{
		if (isObject (%player.tempBrick))
		{
			%player.tempBrick.delete ();
		}
		%player.delete ();
	}
	if (isObject (%client.light))
	{
		%client.light.delete ();
	}
	%client.spawnPlayer ();
}

function GameConnection::spawnPlayer (%client)
{
	if (isObject (%client.Player))
	{
		if (%client.Player.getDamagePercent () < 1)
		{
			%client.Player.delete ();
		}
	}
	%spawnPoint = %client.getSpawnPoint ();
	%client.createPlayer (%spawnPoint);
	if (isObject (%client.Camera))
	{
		%client.Camera.unmountImage (0);
	}
	messageClient (%client, 'MsgYourSpawn');
	if (!%client.hasSpawnedOnce)
	{
		%client.hasSpawnedOnce = 1;
		messageAllExcept (%client, -1, '', '\c1%1 spawned.', %client.getPlayerName ());
		echo (%client.getPlayerName () @ " spawned.");
	}
}

function GameConnection::getSpawnPoint (%client)
{
	if (isObject (%client.miniGame))
	{
		%spawnPoint = %client.miniGame.pickSpawnPoint (%client);
	}
	else 
	{
		%spawnPoint = %client.brickGroup.getBrickSpawnPoint ();
	}
	return %spawnPoint;
}

function GameConnection::createPlayer (%client, %spawnPoint)
{
	if (%client.Player > 0)
	{
		error ("Attempting to create an angus ghost!");
	}
	if (isObject (%client.miniGame))
	{
		if (!%client.miniGame.ending)
		{
			%data = %client.miniGame.playerDataBlock;
		}
		else 
		{
			%data = PlayerStandardArmor;
		}
	}
	else 
	{
		%data = PlayerStandardArmor;
	}
	%oldQuotaObject = getCurrentQuotaObject ();
	if (isObject (%oldQuotaObject))
	{
		setCurrentQuotaObject (GlobalQuota);
	}
	%player = new Player ("")
	{
		dataBlock = %data;
		client = %client;
	};
	MissionCleanup.add (%player);
	%client.Player = %player;
	%player.weaponCount = 0;
	%player.spawnTime = getSimTime ();
	if (isObject (%oldQuotaObject))
	{
		setCurrentQuotaObject (%oldQuotaObject);
	}
	commandToClient (%client, 'ShowEnergyBar', %data.showEnergyBar);
	applyCharacterPrefs (%client);
	commandToClient (%client, 'PlayGui_CreateToolHud', %player.getDataBlock ().maxTools);
	%client = %client;
	%mg = %client.miniGame;
	if (isObject (%mg))
	{
		if (!%mg.ending)
		{
			%player.setShapeNameColor ($MiniGameColorF[%mg.colorIdx]);
			%i = 0;
			while (%i < 5)
			{
				if (isObject (%mg.startEquip[%i]))
				{
					%player.tool[%i] = %mg.startEquip[%i];
				}
				else 
				{
					%player.tool[%i] = 0;
				}
				messageClient (%client, 'MsgItemPickup', "", %i, %player.tool[%i], 1);
				%i += 1;
			}
		}
		else 
		{
			%player.setShapeNameColor ("1 1 1");
			%player.GiveDefaultEquipment (1);
		}
	}
	else 
	{
		%player.setShapeNameColor ("1 1 1");
		%player.GiveDefaultEquipment (1);
	}
	%player.currWeaponSlot = -1;
	%player.setTransform (%spawnPoint);
	%player.setEnergyLevel (%player.getDataBlock ().maxEnergy);
	%player.setShapeName (%client.getPlayerName (), 8564862);
	%player.canDismount = 1;
	if (isObject (%client.Camera))
	{
		%client.Camera.setTransform (%player.getEyeTransform ());
	}
	%client.Player = %player;
	%client.setControlObject (%player);
	%p = new Projectile ("")
	{
		dataBlock = spawnProjectile;
		initialVelocity = "0 0 0";
		initialPosition = %player.getHackPosition ();
		sourceObject = %player;
		sourceSlot = 0;
		client = %client;
	};
	if (isObject (%p))
	{
		%p.setScale (%player.getScale ());
		MissionCleanup.add (%p);
	}
}

function pickSpawnPoint ()
{
	%groupName = "MissionGroup/PlayerDropPoints";
	%group = nameToID (%groupName);
	if (%group != -1)
	{
		%count = %group.getCount ();
		if (%count != 0)
		{
			%index = getRandom (%count - 1);
			%spawn = %group.getObject (%index);
			%rayHeight = %spawn.RayHeight;
			if (%rayHeight $= "")
			{
				%rayHeight = 10;
			}
			%trans = %spawn.getTransform ();
			%transX = getWord (%trans, 0);
			%transY = getWord (%trans, 1);
			%transZ = getWord (%trans, 2);
			%i = 0;
			while (%i < 1000)
			{
				%r = getRandom (%spawn.radius * 10) / 10;
				%ang = getRandom ($pi * 2 * 100) / 100;
				%transX = getWord (%trans, 0);
				%transY = getWord (%trans, 1);
				%transZ = getWord (%trans, 2);
				%offsetX = getRandom () * %spawn.radius * 2 - %spawn.radius;
				%offsetY = getRandom () * %spawn.radius * 2 - %spawn.radius;
				if (VectorLen (%offsetX SPC %offsetY SPC 0) > %spawn.radius)
				{
					
				}
				else 
				{
					%transX += %offsetX;
					%transY += %offsetY;
					%start = %transX SPC %transY SPC %transZ + %rayHeight;
					%end = %transX SPC %transY SPC %transZ - 2;
					%mask = $TypeMasks::FxBrickObjectType | $TypeMasks::InteriorObjectType | $TypeMasks::StaticShapeObjectType | $TypeMasks::PlayerObjectType;
					%scanTarg = containerRayCast (%start, %end, %mask, 0);
					if (%scanTarg)
					{
						%scanPos = posFromRaycast (%scanTarg);
						%transZ = getWord (%scanPos, 2);
						%boxCenter = VectorAdd (%scanPos, "0 0 1.6");
						%mask = $TypeMasks::FxBrickObjectType | $TypeMasks::StaticShapeObjectType | $TypeMasks::PlayerObjectType;
						if (containerBoxClear (%mask, %boxCenter, 0.6, 0.6, 1.3))
						{
							break;
						}
					}
				}
				%i += 1;
			}
			if (%spawn.directional)
			{
				%spawnAngle = " " @ getWords (%spawn.getTransform (), 3, 6);
			}
			else 
			{
				%spawnAngle = " 0 0 1 " @ getRandom ($pi * 2 * 100) / 100;
			}
			%returnTrans = %transX @ " " @ %transY @ " " @ %transZ @ %spawnAngle;
			return %returnTrans;
		}
		else 
		{
			error ("No spawn points found in " @ %groupName);
		}
	}
	else 
	{
		error ("Missing spawn points group " @ %groupName);
	}
	error ("default spawn!");
	return "0 0 300 1 0 0 0";
}

function GameConnection::Cheat (%client)
{
	%name = %client.getPlayerName ();
	%client.Cheat += 1;
	if (%client.Cheat > 10)
	{
		%client.schedule (10, delete, "");
	}
}

function findLocalClient ()
{
	%count = ClientGroup.getCount ();
	%i = 0;
	while (%i < %count)
	{
		%client = ClientGroup.getObject (%i);
		if (%client.isLocal ())
		{
			return %client;
		}
		%i += 1;
	}
	return 0;
}

function findClientByBL_ID (%bl_id)
{
	%count = ClientGroup.getCount ();
	%i = 0;
	while (%i < %count)
	{
		%client = ClientGroup.getObject (%i);
		if (%client.getBLID () == %bl_id)
		{
			return %client;
		}
		%i += 1;
	}
	return 0;
}

function GameConnection::resetVehicles (%client)
{
	if (!isObject (MissionCleanup))
	{
		if (getBuildString () !$= "Ship")
		{
			error ("ERROR: GameConnection::ResetVehicles() - MissionCleanUp group not found!");
		}
		return;
	}
	%ourBrickGroup = %client.brickGroup;
	%count = MissionCleanup.getCount ();
	%i = 0;
	while (%i < %count)
	{
		%obj = MissionCleanup.getObject (%i);
		if (!(%obj.getType () & ($TypeMasks::VehicleObjectType | $TypeMasks::PlayerObjectType)))
		{
			
		}
		else if (!isObject (%obj.spawnBrick))
		{
			
		}
		else if (%obj.spawnBrick.getGroup () != %ourBrickGroup)
		{
			
		}
		else 
		{
			%obj.spawnBrick.schedule (10, spawnVehicle);
		}
		%i += 1;
	}
}

function GameConnection::setLoadingIndicator (%client, %val)
{
	commandToClient (%client, 'setLoadingIndicator', %val);
}

function GameConnection::getSimpleName (%client)
{
	if (GameWindowExists () && !$Server::Dedicated)
	{
		return %client.getPlayerName ();
	}
	if (%client.simpleName !$= "")
	{
		return %client.simpleName;
	}
	%simpleName = %client.getPlayerName ();
	%simpleName = strreplace (%simpleName, "\xa1", "I");
	%simpleName = strreplace (%simpleName, "\xa2", "C");
	%simpleName = strreplace (%simpleName, "\xa3", "L");
	%simpleName = strreplace (%simpleName, "\xa4", "O");
	%simpleName = strreplace (%simpleName, "\xa5", "Y");
	%simpleName = strreplace (%simpleName, "\xa6", "I");
	%simpleName = strreplace (%simpleName, "\xa7", "S");
	%simpleName = strreplace (%simpleName, "\xa9", "C");
	%simpleName = strreplace (%simpleName, "\xaa", "A");
	%simpleName = strreplace (%simpleName, "\xab", "<");
	%simpleName = strreplace (%simpleName, "\xac", "-");
	%simpleName = strreplace (%simpleName, "\xae", "R");
	%simpleName = strreplace (%simpleName, "\xaf", "-");
	%simpleName = strreplace (%simpleName, "\xb0", "O");
	%simpleName = strreplace (%simpleName, "\xb1", "+");
	%simpleName = strreplace (%simpleName, "\xb2", 2);
	%simpleName = strreplace (%simpleName, "\xb3", 3);
	%simpleName = strreplace (%simpleName, "\xb5", "U");
	%simpleName = strreplace (%simpleName, "\xb6", "P");
	%simpleName = strreplace (%simpleName, "\xb7", "");
	%simpleName = strreplace (%simpleName, "\xb8", "");
	%simpleName = strreplace (%simpleName, "\xb9", 1);
	%simpleName = strreplace (%simpleName, "\xba", "O");
	%simpleName = strreplace (%simpleName, "\xbb", ">");
	%simpleName = strreplace (%simpleName, "\xbc", "1/4");
	%simpleName = strreplace (%simpleName, "\xbd", "1/2");
	%simpleName = strreplace (%simpleName, "\xbe", "3/4");
	%simpleName = strreplace (%simpleName, "\xde", "P");
	%simpleName = strreplace (%simpleName, "\xd7", "X");
	%simpleName = strreplace (%simpleName, "\xf7", "+");
	%simpleName = strreplace (%simpleName, "\xc0", "A");
	%simpleName = strreplace (%simpleName, "\xc1", "A");
	%simpleName = strreplace (%simpleName, "\xc2", "A");
	%simpleName = strreplace (%simpleName, "\xc3", "A");
	%simpleName = strreplace (%simpleName, "\xc4", "A");
	%simpleName = strreplace (%simpleName, "\xc5", "A");
	%simpleName = strreplace (%simpleName, "\xc6", "AE");
	%simpleName = strreplace (%simpleName, "\xc7", "C");
	%simpleName = strreplace (%simpleName, "\xc9", "E");
	%simpleName = strreplace (%simpleName, "\xca", "E");
	%simpleName = strreplace (%simpleName, "\xcb", "E");
	%simpleName = strreplace (%simpleName, "\xcc", "I");
	%simpleName = strreplace (%simpleName, "\xcd", "I");
	%simpleName = strreplace (%simpleName, "\xce", "I");
	%simpleName = strreplace (%simpleName, "\xcf", "I");
	%simpleName = strreplace (%simpleName, "\xd0", "D");
	%simpleName = strreplace (%simpleName, "\xd1", "N");
	%simpleName = strreplace (%simpleName, "\xf1", "N");
	%simpleName = strreplace (%simpleName, "\xd2", "O");
	%simpleName = strreplace (%simpleName, "\xd3", "O");
	%simpleName = strreplace (%simpleName, "\xd4", "O");
	%simpleName = strreplace (%simpleName, "\xd5", "O");
	%simpleName = strreplace (%simpleName, "\xd6", "O");
	%simpleName = strreplace (%simpleName, "\xd8", "O");
	%simpleName = strreplace (%simpleName, "\xf2", "O");
	%simpleName = strreplace (%simpleName, "\xf3", "O");
	%simpleName = strreplace (%simpleName, "\xf4", "O");
	%simpleName = strreplace (%simpleName, "\xf5", "O");
	%simpleName = strreplace (%simpleName, "\xf6", "O");
	%simpleName = strreplace (%simpleName, "\xf8", "O");
	%simpleName = strreplace (%simpleName, "\xf0", "O");
	%simpleName = strreplace (%simpleName, "\xd9", "U");
	%simpleName = strreplace (%simpleName, "\xda", "U");
	%simpleName = strreplace (%simpleName, "\xdb", "U");
	%simpleName = strreplace (%simpleName, "\xdc", "U");
	%simpleName = strreplace (%simpleName, "\xe0", "A");
	%simpleName = strreplace (%simpleName, "\xe1", "A");
	%simpleName = strreplace (%simpleName, "\xe2", "A");
	%simpleName = strreplace (%simpleName, "\xe3", "A");
	%simpleName = strreplace (%simpleName, "\xe4", "A");
	%simpleName = strreplace (%simpleName, "\xe5", "A");
	%simpleName = strreplace (%simpleName, "\xe6", "AE");
	%simpleName = strreplace (%simpleName, "\xe7", "C");
	%simpleName = strreplace (%simpleName, "\xe8", "E");
	%simpleName = strreplace (%simpleName, "\xe9", "E");
	%simpleName = strreplace (%simpleName, "\xea", "E");
	%simpleName = strreplace (%simpleName, "\xeb", "E");
	%simpleName = strreplace (%simpleName, "\xec", "I");
	%simpleName = strreplace (%simpleName, "\xed", "I");
	%simpleName = strreplace (%simpleName, "\xee", "I");
	%simpleName = strreplace (%simpleName, "\xef", "I");
	%simpleName = strreplace (%simpleName, "\xf9", "U");
	%simpleName = strreplace (%simpleName, "\xfa", "U");
	%simpleName = strreplace (%simpleName, "\xfb", "U");
	%simpleName = strreplace (%simpleName, "\xfc", "U");
	%simpleName = strreplace (%simpleName, "\xfd", "Y");
	%simpleName = strreplace (%simpleName, "\xdd", "Y");
	%simpleName = strreplace (%simpleName, "\xfe", "P");
	%simpleName = strreplace (%simpleName, "\xdf", "B");
	%simpleName = strreplace (%simpleName, "\x8c", "CE");
	%simpleName = strreplace (%simpleName, "\x9c", "CE");
	%client.simpleName = %simpleName;
	return %client.simpleName;
}

function GameConnection::onInfiniteLag (%client)
{
	%player = %client.Player;
	if (!isObject (%player))
	{
		return;
	}
	%controlObj = %player.getControlObject ();
	if (isObject (%controlObj))
	{
		if (%controlObj.getType () & $TypeMasks::PlayerObjectType)
		{
			%player = %controlObj;
		}
		else 
		{
			return;
		}
	}
	%currTime = getSimTime ();
	%pos = %player.getPosition ();
	%delta = VectorSub (%pos, %player.lastInfiniteLagPos);
	%deltaLen = mAbs (VectorLen (%delta));
	if (%currTime - mFloor (%player.lastInfiniteLagTime) > 1000 || %player.lastInfiniteLagTime $= "" || %player.lastInfiniteLagPos $= "")
	{
		%player.lastInfiniteLagPos = %pos;
		%pos = VectorAdd (%pos, "0 0 0.2");
	}
	else if (%delta < 0.1)
	{
		%pos = %player.lastInfiniteLagPos;
		%pos = VectorAdd (%pos, "0 0 0.2");
	}
	else if (%delta > 0.1 && %delta < 1)
	{
		
	}
	else 
	{
		%player.lastInfiniteLagPos = %pos;
		%pos = VectorAdd (%pos, "0 0 0.2");
	}
	%rot = getWords (%player.getTransform (), 3, 6);
	%player.setTransform (%pos SPC %rot);
	%player.lastInfiniteLagTime = %currTime;
}

function GameConnection::transmitMaxPlayers (%client)
{
	secureCommandToClient ("mod2maiegut^afoo", %client, 'SetMaxPlayersDisplay', $Pref::Server::MaxPlayers);
}

function GameConnection::transmitServerName (%client)
{
	secureCommandToClient ("mod2maiegut^afoo", %client, 'SetServerNameDisplay', $pref::Player::NetName, $Pref::Server::Name);
}

