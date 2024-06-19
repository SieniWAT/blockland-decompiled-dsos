addMessageCallback ('MsgClientJoin', handleClientJoin);
addMessageCallback ('MsgClientDrop', handleClientDrop);
addMessageCallback ('MsgClientScoreChanged', handleClientScoreChanged);
function handleClientJoin (%null, %null, %clientName, %clientId, %null, %score, %isAI, %isAdmin, %isSuperAdmin)
{
	PlayerListGui.update (%clientId, detag (%clientName), %isSuperAdmin, %isAdmin, %isAI, %score);
	%name = StripMLControlChars (detag (%clientName));
	if (lstAdminPlayerList.getRowNumById (%clientId) == -1)
	{
		lstAdminPlayerList.addRow (%clientId, %name);
	}
	else 
	{
		lstAdminPlayerList.setRowById (%clientId, %name);
	}
}

function handleClientDrop (%null, %null, %clientName, %clientId)
{
	PlayerListGui.remove (%clientId);
	lstAdminPlayerList.removeRowById (%clientId);
}

function handleClientScoreChanged (%null, %null, %score, %clientId)
{
	PlayerListGui.updateScore (%clientId, %score);
}

addMessageCallback ('InitTeams', handleInitTeams);
addMessageCallback ('AddTeam', handleAddTeam);
addMessageCallback ('RemoveTeam', handleRemoveTeam);
addMessageCallback ('SetTeamName', handleSetTeamName);
function handleInitTeams (%null, %null)
{
	InitClientTeamManager ();
}

function handleAddTeam (%null, %null, %teamID, %teamName)
{
	ClientTeamManager.addTeam (%teamID, %teamName);
}

function handleRemoveTeam (%null, %null, %teamID)
{
	ClientTeamManager.removeTeam (%teamID);
}

function handleSetTeamName (%null, %null, %teamID, %teamName)
{
	ClientTeamManager.setTeamName (%teamID, %teamName);
}

addMessageCallback ('AddClientToTeam', handleAddClientToTeam);
addMessageCallback ('RemoveClientFromTeam', handleRemoveClientFromTeam);
addMessageCallback ('SetTeamCaptain', handleSetTeamCaptain);
function handleAddClientToTeam (%null, %null, %clientId, %clientName, %teamID)
{
	%teamObj = ClientTeamManager.findTeamByID (%teamID);
	if (%teamObj == 0)
	{
		error ("ERROR: handleAddClientToTeam - Team ID " @ %teamID @ " not found in manager");
		return 0;
	}
	%teamObj.addMember (%clientId, %clientName);
}

function handleRemoveClientFromTeam (%null, %null, %clientId, %teamID)
{
	%teamObj = ClientTeamManager.findTeamByID (%teamID);
	if (%teamObj == 0)
	{
		error ("ERROR: handleRemoveClientFromTeam - Team ID " @ %teamID @ " not found in manager");
		return 0;
	}
	%teamObj.removeMember (%clientId);
}

function handleSetTeamCaptain (%null, %null, %clientId, %teamID)
{
	%teamObj = ClientTeamManager.findTeamByID (%teamID);
	if (%teamObj == 0)
	{
		error ("ERROR: handleSetTeamCaptain - Team ID " @ %teamID @ " not found in manager");
		return 0;
	}
	%teamObj.captain = %clientId;
}

function InitClientTeamManager ()
{
	if (isObject (ClientTeamManager))
	{
		%i = 0;
		while (%i < ClientTeamManager.teamCount)
		{
			if (isObject (ClientTeamManager.team[%i]))
			{
				ClientTeamManager.team[%i].delete ();
			}
			%i += 1;
		}
		ClientTeamManager.delete ();
	}
	new ScriptObject (ClientTeamManager)
	{
		class = SO_ClientTeamManager;
		teamCount = 0;
	};
}

function SO_ClientTeamManager::addTeam (%this, %teamID, %teamName)
{
	if (%this.findTeamByID (%teamID) != 0)
	{
		error ("ERROR: SO_ClientTeamManager::addTeam - Team ID " @ %teamID @ " is already in use");
		return 0;
	}
	%newTeam = new ScriptObject ("")
	{
		class = SO_ClientTeam;
		memberCount = 0;
		serverID = %teamID;
		name = %teamName;
	};
	%this.team[%this.teamCount] = %newTeam;
	%this.teamCount += 1;
}

function SO_ClientTeamManager::removeTeam (%this, %teamID)
{
	%i = 0;
	while (%i < %this.teamCount)
	{
		%currTeam = %this.team[%i];
		if (%currTeam.serverID == %teamID)
		{
			%currTeam.delete ();
			%j = %i;
			while (%j < %this.teamCount - 1)
			{
				%this.team[%j] = %this.team[%j + 1];
				%j += 1;
			}
			%this.team[%this.teamCount] = "";
			%this.teamCount -= 1;
			return 1;
		}
		%i += 1;
	}
	error ("ERROR: SO_ClientTeamManager::removeTeam - Team ID " @ %teamID @ " not found in manager");
	return 0;
}

function SO_ClientTeamManager::setTeamName (%this, %teamID, %teamName)
{
	%teamObj = %this.findTeamByID (%teamID);
	if (%teamObj != 0)
	{
		%teamObj.name = %teamName;
	}
	else 
	{
		error ("ERROR: SO_ClientTeamManager::setTeamName - Team ID " @ %teamID @ " not found in manager");
	}
}

function SO_ClientTeamManager::findTeamByID (%this, %teamID)
{
	%i = 0;
	while (%i < %this.teamCount)
	{
		%currTeam = %this.team[%i];
		if (%currTeam.serverID == %teamID)
		{
			return %currTeam;
		}
		%i += 1;
	}
	return 0;
}

function SO_ClientTeamManager::dumpTeams (%this)
{
	echo ("===============");
	echo ("CLIENT Team Manager ID = ", %this);
	echo ("Number of teams = ", %this.teamCount);
	%i = 0;
	while (%i < %this.teamCount)
	{
		%currTeam = %this.team[%i];
		echo ("   Team " @ %i @ " = " @ %currTeam @ "(server:" @ %currTeam.serverID @ ") : " @ %currTeam.name @ " : " @ %currTeam.memberCount @ " members");
		%j = 0;
		while (%j < %currTeam.memberCount)
		{
			%client = %currTeam.memberID[%j];
			%clientName = StripMLControlChars (getTaggedString (%currTeam.memberName[%j]));
			if (%currTeam.captain == %client)
			{
				echo ("      " @ %client @ " : " @ %clientName @ " <CAPT>");
			}
			else 
			{
				echo ("      " @ %client @ " : " @ %clientName);
			}
			%j += 1;
		}
		%i += 1;
	}
	echo ("===============");
}

function SO_ClientTeam::addMember (%this, %clientId, %name)
{
	%this.memberID[%this.memberCount] = %clientId;
	%this.memberName[%this.memberCount] = %name;
	%this.memberCount += 1;
}

function SO_ClientTeam::removeMember (%this, %clientId)
{
	%i = 0;
	while (%i < %this.memberCount)
	{
		if (%this.memberID[%i] == %clientId)
		{
			%j = %i;
			while (%j < %this.memberCount - 1)
			{
				%this.memberID[%j] = %this.memberID[%j + 1];
				%this.memberName[%j] = %this.memberName[%j + 1];
				%j += 1;
			}
			%this.memberID[%this.memberCount] = "";
			%this.memberName[%this.memberCount] = "";
			%this.memberCount -= 1;
			return 1;
		}
		%i += 1;
	}
	error ("ERROR: SO_ClientTeam::removeMember - Client ID " @ %clientId @ " not found in team " @ %this @ "(server:" @ %this.serverID @ ")");
}

function SO_ClientTeam::setCaptain (%this, %clientId)
{
	%this.captain = %client;
}

