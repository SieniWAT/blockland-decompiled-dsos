function clientCmdMissionStart (%null)
{
	
}

function clientCmdMissionEnd (%null)
{
	alxStopAll ();
	$lightingMission = 0;
	$sceneLighting::terminateLighting = 1;
}

