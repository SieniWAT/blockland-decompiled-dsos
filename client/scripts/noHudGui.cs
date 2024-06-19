function noHudGui::onWake (%null)
{
	$enableDirectInput = 1;
	activateDirectInput ();
	Canvas.popDialog (MainChatHud);
	moveMap.push ();
	schedule (0, 0, "refreshCenterTextCtrl");
	schedule (0, 0, "refreshBottomTextCtrl");
}

function noHudGui::onSleep (%null)
{
	moveMap.pop ();
}

