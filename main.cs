//-----------------------------------------------------------------------------
// Torque Game Engine 
// Copyright (C) GarageGames.com, Inc.
//-----------------------------------------------------------------------------

//-----------------------------------------------------------------------------
// Load up defaults console values.

exec("./defaults.cs");
//-----------------------------------------------------------------------------

function initCommon()
{
   // All mods need the random seed set
   setRandomSeed();

   // Very basic functions used by everyone
   exec("./client/canvas.cs");
   exec("./client/audio.cs");
}

function initBaseClient()
{
   // Base client functionality
   exec("./client/message.cs");
   exec("./client/mission.cs");
   exec("./client/missionDownload.cs");
   exec("./client/actionMap.cs");
   //exec("./editor/editor.cs");

   // There are also a number of support scripts loaded by the canvas
   // when it's first initialized.  Check out client/canvas.cs
}

function initBaseServer()
{
   // Base server functionality
   exec("./server/webCom.cs"); //badspot: master server communications
   exec("./server/audio.cs");
   exec("./server/server.cs");
   exec("./server/message.cs");
   exec("./server/commands.cs");
   exec("./server/missionInfo.cs");
   exec("./server/missionLoad.cs");
   exec("./server/missionDownload.cs");
   exec("./server/clientConnection.cs");
   exec("./server/kickban.cs");
   exec("./server/game.cs");
}   


