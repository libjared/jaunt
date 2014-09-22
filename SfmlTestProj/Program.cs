﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML;
using SFML.Graphics;
using SFML.Window;
using SFML.Audio;
using Lidgren.Network;


namespace Jaunt
{
    static class Game
    {
        public static RenderWindow window;
        public static Vector2f windowSize;
        public static Random r = new Random();
        public static List<SoundInstance> soundInstances = new List<SoundInstance>();
		public static Content content;

        public static float ping;

        public static NetClient client;
        public static List<Player> connectedPlayers = new List<Player>();
        public static List<String> chatMessages = new List<String>();
        public static Player clientPlayer;

        public static int timer = 0;

        public static Image map;

        static void Main(string[] args)
        {
			LoadContentInitialize();

            while (window.IsOpen())
            {
                UpdateDraw();
            }
        }

		private static void SetupClient ()
		{
			NetPeerConfiguration config = new NetPeerConfiguration ("jaunt");
			config.EnableMessageType (NetIncomingMessageType.ConnectionLatencyUpdated);
			string ip = "giga.krash.net"; //Jared's IP
			int port = 12345;
			client = new NetClient (config);
			client.Start ();
			client.Connect (ip, port);
		}

		private static void LoadContentInitialize()
        {
            window = new RenderWindow(
                new VideoMode(800, 600), "Project Title");

            windowSize = new Vector2f(800, 600);
            window.SetFramerateLimit(60);
            window.Closed += (object sender, EventArgs e) =>
			{
				window.Close();
			};

            window.TextEntered += (object sender, TextEventArgs e) =>
            {
                if (Keyboard.IsKeyPressed(Keyboard.Key.Back))
                {
                    if (clientPlayer.textCapture.Length > 0) {
                        clientPlayer.textCapture = clientPlayer.textCapture.Substring(0, clientPlayer.textCapture.Length - 1);
					}
                }
                else if (Keyboard.IsKeyPressed(Keyboard.Key.Return))
                {
                }
                else if (Keyboard.IsKeyPressed(Keyboard.Key.LControl))
                {
                }
                else if (Keyboard.IsKeyPressed(Keyboard.Key.Tab))
                {
                }
                else
                {
                    clientPlayer.textCapture += e.Unicode; //TODO: take out of player
                }
            };

            window.Closed += (object sender, EventArgs e) =>
            {
                client.Disconnect("Exit");
                System.Threading.Thread.Sleep(100);

            };
			
			content = new Content("Content/");
        }

        private static void UpdateDraw()
        {
			/*
			 * update
			 */
			window.DispatchEvents();
			UpdateSounds();
			foreach (Player ply in connectedPlayers)
			{
				ply.connectColor = Color.White;
			}
			HandleMessages();
			updatePlayers();

			/*
			 * draw
			 */
			window.Clear(new Color(0, 203, 255));

			//set the camera
			View cameraView = new View (window.DefaultView);
			cameraView.Center = clientPlayer.position.Floor();
			cameraView.Zoom(0.5f);
			window.SetView(cameraView);

			//world stuff
            window.Draw(content.Texture("background.png"));
			drawPlayers();

			//hud stuff
			window.SetView(window.DefaultView);
			var allFont = content.Font ("font1.ttf");
			Vector2f defaultScale = new Vector2f(0.4f, 0.4f);

			Text _chatCompose = new Text () {
				DisplayedString = clientPlayer.textCapture,
				Font = allFont,
				Scale = defaultScale,
				Position = new Vector2f(-300, 200),
			};
			window.Draw(_chatCompose);

			Text _textConnected = new Text () {
				DisplayedString = Enum.GetName(typeof(NetConnectionStatus), client.ConnectionStatus),
				Font = content.Font ("font1.ttf"),
				Scale = defaultScale,
				Position = new Vector2f(-300, -230),
			};
			window.Draw(_textConnected);

            Text _playersConnectedText = new Text()
			{
				DisplayedString = connectedPlayers.Count + " Players Connected",
				Font = content.Font("font1.ttf"),
				Scale = defaultScale,
				Position = new Vector2f(-300, -220),
			};
			window.Draw(_playersConnectedText);
           	
            Color pingColor = Color.White;

            if (ping == 0)
            {
                pingColor = Color.Red;
            }
            if (ping > 100)
            {
                pingColor = Color.Yellow;
            }
			
            Render.drawString(font, ping + " ms", new Vector2f(-300, -240), pingColor, .4f, false);

            for (int i = 0; i < chatMessages.Count; i++)
            {
				Font font = content.Font ("font1.ttf");
                Text chatMessage = new Text(chatMessages[i], font);
                chatMessage.Scale = new Vector2f(chatScale, chatScale);
                chatMessage.Position = new Vector2f(-300, -200 + (i * 10));

                window.Draw(chatMessage);
            }

            window.Display();
        }

        private static void UpdateSounds()
        {
            for (int i = 0; i < soundInstances.Count; i++)
            {
                if (!soundInstances[i].started)
                {
                    soundInstances[i].sound.Play();
                    soundInstances[i].started = true;
                }

				if (soundInstances [i].sound.Status.Equals(SoundStatus.Stopped)) {
					soundInstances.RemoveAt(i);
				}
            }
        }

        public static void drawPlayers()
        {

            //if (clientPlayer.ohmDecay > 0)
            //Game.spriteBatch.DrawString(Game.font1, clientPlayer.overheadMessage, new Vector2f(clientPlayer.position.X + playerSprite.TextureRect.Width / 2 - (font1.MeasureString(clientPlayer.overheadMessage).X / 2), clientPlayer.position.Y - 20), Color.White);

            clientPlayer.drawClient();



            for (int i = 0; i < connectedPlayers.Count; i++)
            {
                connectedPlayers[i].drawConnectedPlayer();
                connectedPlayers[i].updateConnectedPlayer();
            }
        }

        public static void updatePlayers()
        {
            clientPlayer.updateClient();
            clientPlayer.checkControls();
        }

        public static void HandleMessages()
        {
            NetIncomingMessage msg;
            while ((msg = Game.client.ReadMessage()) != null)
            {

                switch (msg.MessageType)
                {
                    case NetIncomingMessageType.VerboseDebugMessage:
                    case NetIncomingMessageType.DebugMessage:
                    case NetIncomingMessageType.WarningMessage:
                    case NetIncomingMessageType.ErrorMessage:
                        //Console.WriteLine(msg.ReadString());
                        string messageTypeError = msg.ReadString();
                        Console.WriteLine(messageTypeError);
                        Console.WriteLine(msg.LengthBytes);
                        connected = false;
                        break;
                    case NetIncomingMessageType.ConnectionLatencyUpdated:
                        ping = msg.ReadFloat() * 1000;
                        connected = true;
                        break;
                    case NetIncomingMessageType.Data:
                        //read the incoming string
                        string messageType = msg.ReadString();

                        
                        switch (messageType)
                        {
                            case "LIFE":
                                long UID_LIFE = msg.ReadInt64();
                                bool aliveStatus = msg.ReadBoolean();
                                for (int i = 0; i < connectedPlayers.Count; i++)
                                {
                                    if (connectedPlayers[i].UID == UID_LIFE)
                                    {
                                        connectedPlayers[i].alive = aliveStatus;
                                        if (!aliveStatus) //they're dead jimmy
                                        {
                                            Game.soundInstances.Add(new SoundInstance(Game.fart, 0f, 0f));
                                            // SoundPlayer.playSound(Game.SaD);
                                        }
                                    }
                                }
                                break;

                            case "NAME":
                                long UID_NAME = msg.ReadInt64();
                                string name = msg.ReadString();
                                for (int i = 0; i < connectedPlayers.Count; i++)
                                {
                                    if (connectedPlayers[i].UID == UID_NAME)
                                    {
                                        connectedPlayers[i].username = name;
                                    }
                                }
                                break;
                            case "POS": //Update a player's position

                                long UID = msg.ReadInt64();
                                float x = msg.ReadFloat();
                                float y = msg.ReadFloat();

                                for (int i = 0; i < connectedPlayers.Count; i++)
                                {
                                    if (connectedPlayers[i].UID == UID)
                                    {
                                        connectedPlayers[i].position = new Vector2f(connectedPlayers[i].position.X, connectedPlayers[i].position.Y);
                                        connectedPlayers[i].position.X = x;
                                        connectedPlayers[i].position.Y = y;
                                        connectedPlayers[i].connectColor = Color.Green;
                                    }
                                }

                                break;

                            case "JOIN": //Add a player
                                long UID_JOIN = msg.ReadInt64();
                                connectedPlayers.Add(new Player(100, 100, UID_JOIN));
                                chatMessages.Add("[Server]: " + Math.Abs(UID_JOIN).ToString().Substring(0, 4) + " has connected");
                                Game.soundInstances.Add(new SoundInstance(Game.click, 0f, 0f));
                                break;

                            case "CHAT": //Add chat
                                long UID_CHAT = msg.ReadInt64();
                                String message = msg.ReadString();
                                String messageOwner = "";

                                if (UID_CHAT == 0)
                                    messageOwner = "CONSOLE";
                                else
                                    for (int i = 0; i < connectedPlayers.Count; i++)
                                    {
                                        if (connectedPlayers[i].UID == UID_CHAT)
                                        {
                                            messageOwner = connectedPlayers[i].username;
                                        }
                                    }



                                if (messageOwner.Trim().Equals(""))
                                    chatMessages.Add(Math.Abs(UID_CHAT).ToString().Substring(0, 4) + ": " + message);
                                else
                                    chatMessages.Add(messageOwner + ": " + message);


                                Game.soundInstances.Add(new SoundInstance(Game.click, 0f, 0f));
                                if (Game.chatMessages.Count > 15)
                                    Game.chatMessages.RemoveAt(0);


                                for (int i = 0; i < connectedPlayers.Count; i++)
                                {
                                    if (connectedPlayers[i].UID == UID_CHAT)
                                    {
                                        if (!(message.IndexOf("/") == 0))
                                        {
                                            connectedPlayers[i].overheadMessage = message;
                                            connectedPlayers[i].ohmDecay = (60 * 5);
                                        }
                                    }
                                }

                                if (message.IndexOf("/setname") == 0)
                                {
                                    for (int i = 0; i < connectedPlayers.Count; i++)
                                    {
                                        if (connectedPlayers[i].UID == UID_CHAT)
                                        {
                                            connectedPlayers[i].username = message.Substring(9).Trim();
                                        }
                                    }
                                }
                                if (message.IndexOf("/scream") == 0)
                                    Game.soundInstances.Add(new SoundInstance(Game.SaD, 0f, 0f));
                                if (message.IndexOf("/fart") == 0)
                                    Game.soundInstances.Add(new SoundInstance(Game.fart, 0f, 0f));


                                break;

                            case "PART": //Remove a player
                                long UID_PART = msg.ReadInt64();
                                if (Math.Abs(UID_PART).ToString().Length > 4)
                                    chatMessages.Add("[Server]: " + Math.Abs(UID_PART).ToString().Substring(0, 4) + " has disconnected");
                                //SaD.Play(.1f, 0, 0);
                                Game.soundInstances.Add(new SoundInstance(Game.click, 0f, 0f));
                                for (int i = 0; i < connectedPlayers.Count; i++)
                                {
                                    if (connectedPlayers[i].UID == UID_PART)
                                        connectedPlayers.RemoveAt(i);
                                }
                                break;
                        }

                        break;
                    default:
                        Console.WriteLine("Unrecognized Message Recieved:" + msg.ToString());
                        break;
                }
                Game.client.Recycle(msg);
            }

        }
    }
}



