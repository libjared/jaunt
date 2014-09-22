using System;
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
    class Game
    {
        public static RenderWindow window;
        static DateTime startTime;
        static Vector2f windowSize;
        public static Random r = new Random();
        public static View camera2D;
        public static Font font;
        public static List<SoundInstance> soundInstances = new List<SoundInstance>();

        public static float ping;

        public static NetClient client;
        public static List<Player> connectedPlayers = new List<Player>(); //List for the connected players
        public static List<String> chatMessages = new List<String>();
        public static Player clientPlayer;// = new Player(100, 100, -1); //Create the client's player
        public static Sprite tempIdleN, tempIdleS, tempIdleEW, background, basicLevelDec;
        public static Texture backgroundTexture, basicLevelCol;

        public static SoundBuffer click, SaD, fart, crunch;
        public static bool connected = false;

        public static int timer = 0;
        public static DateTime previousTime = new DateTime();
        public static Vector2f cameraPos = new Vector2f(0, 0);

        public static Image map;

        static void Main(string[] args)
        {
            PreRun();


            while (window.IsOpen())
            {
                UpdateDraw(window);
            }

        }

        private static void PreRun()
        {
            LoadContentInitialize();
            startTime = DateTime.Now;
            r = new Random(100);
            Initialize();
        }

        private static void Initialize()
        {
            NetPeerConfiguration config = new NetPeerConfiguration("jaunt");
            config.EnableMessageType(NetIncomingMessageType.ConnectionLatencyUpdated);
            string ip = "giga.krash.net"; //Jared's IP
            int port = 12345;
            client = new NetClient(config);
            client.Start();
            client.Connect(ip, port);


            clientPlayer = new Player(100, 100, -1); //Create the client's player
        }

        private static void LoadContentInitialize()
        {
            window = new RenderWindow(
                new VideoMode(800, 600), "Project Title");

            windowSize = new Vector2f(800, 600);
            window.SetFramerateLimit(60);
            window.Closed += (a, b) => { window.Close(); };

            camera2D = new View(cameraPos, new Vector2f(640, 480));



            tempIdleN = new Sprite(new Texture("Content/tempIdleN.png"));
            tempIdleS = new Sprite(new Texture("Content/tempIdleS.png"));
            tempIdleEW = new Sprite(new Texture("Content/tempIdleEW.png"));
            background = new Sprite(new Texture("Content/background.png"));
            basicLevelDec = new Sprite(new Texture("Content/basicLevel_decorMap.png"));


            basicLevelCol = new Texture("Content/basicLevel_collisionMap.png");
            backgroundTexture = basicLevelCol;
            map = backgroundTexture.CopyToImage();

            font = new Font("Content/Font1.ttf");

            click = new SoundBuffer("Content/click.wav");
            SaD = new SoundBuffer("Content/SaD.wav");
            fart = new SoundBuffer("Content/fart.wav");
            crunch = new SoundBuffer("Content/crunch.wav");

            window.TextEntered += (object sender, TextEventArgs e) =>
            {

                if (Keyboard.IsKeyPressed(Keyboard.Key.Back))
                {
                    if (clientPlayer.textCapture.Length > 0)
                        clientPlayer.textCapture = clientPlayer.textCapture.Substring(0, clientPlayer.textCapture.Length - 1);
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
                    clientPlayer.textCapture += e.Unicode;
                }

            };

            window.Closed += (object sender, EventArgs e) =>
            {
                client.Disconnect("Exit");
                System.Threading.Thread.Sleep(100);

            };

        }

        private static void UpdateDraw(RenderWindow window)
        {
            cameraPos = new Vector2f((int)clientPlayer.position.X, (int)clientPlayer.position.Y);
            camera2D = new View(cameraPos, new Vector2f(640, 480));
            camera2D.Zoom(.5f);
            View noCamera = new View(new Vector2f(0, 0), new Vector2f(640, 480));

            window.SetView(camera2D);

            UpdateSounds();

            window.DispatchEvents();
            window.Clear(Color.Black);

            for (int i = 0; i < connectedPlayers.Count; i++)
            {
                connectedPlayers[i].connectColor = Color.White;
            }

            //window.Draw(background);
            window.Draw(basicLevelDec);


            HandleMessages();

            Text _chatCompose = new Text(clientPlayer.textCapture, font);
            float chatScale = .4f;
            _chatCompose.Scale = new Vector2f(chatScale, chatScale);
            _chatCompose.Position = new Vector2f(-300, 200);// clientPlayer.position;

            Text _textConnected = new Text(connected ? "CONNECTED" : "DISCONNECTED", font);
            _textConnected.Scale = new Vector2f(chatScale, chatScale);
            _textConnected.Position = new Vector2f(-300, -230);// clientPlayer.position;



            Text _playersConnectedText = new Text(connectedPlayers.Count + " Player Connected", font);
            _playersConnectedText.Scale = new Vector2f(chatScale, chatScale);
            _playersConnectedText.Position = new Vector2f(-300, -220);// clientPlayer.position;



            drawPlayers();
            updatePlayers();


            window.SetView(noCamera);
            window.Draw(_chatCompose);
            window.Draw(_textConnected);
            window.Draw(_playersConnectedText);

            Color pingColor = Color.White;

            if (ping == 0)
            {
                //connected = false;
                pingColor = Color.Red;
            }
            if (ping > 100)
            {
                pingColor = Color.Yellow;
            }

            Render.drawString(font, ping + " ms", new Vector2f(-300, -240), pingColor, .4f, false);

            for (int i = 0; i < chatMessages.Count; i++)
            {
                Text chatMessage = new Text(chatMessages[i], font);
                chatMessage.Scale = new Vector2f(chatScale, chatScale);
                chatMessage.Position = new Vector2f(-300, -200 + (i * 10));// clientPlayer.position;

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

                if (soundInstances[i].sound.Status.Equals(SoundStatus.Stopped))
                    soundInstances.RemoveAt(i);
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
            //for (int i = 0; i < connectedPlayers.Count; i++)
            //{
            //    connectedPlayers[i].update();
            //    //Console.WriteLine(connectedPlayers[i].UID);
            //}
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



