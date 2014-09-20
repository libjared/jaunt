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
    public class Player
    {
        public Vector2f oldPosition = new Vector2f(0, 0);
        public Vector2f position = new Vector2f(0, 0);
        public Vector2f velocity = new Vector2f(0, 0);

        public bool chatEnabled = true;

        public float fallDistance = 0;

        public bool alive = true;
        public bool previousAlive = true;

        public int maxFall = 40;

        public string animType = ""; //move, idle, air

        public int facing = 1;
        public bool onGround = false;
        public int groundTimer = 0;

        public long UID;

        public int moveFrames = 2;
        public int idleFrames = 3;
        public int currentMaxFrames = 0;


        public string textCapture = "";

        //public TextWriter textCapture = new TextWriter();
        public string username = "";

        public string overheadMessage = "";
        public int ohmDecay = 0;//Message is erased when < 0 (Stands for Overhead Message)

        public Color color = Color.White;
        public int frame = 0;

        public Sprite sprite = new Sprite();

        public DateTime previousTime = new DateTime();

        public Color connectColor = Color.White;

        public Player(float x, float y, long UID)
        {
            this.UID = UID;
            this.position.X = x;
            this.position.Y = y;
            color = new Color((byte)Game.r.Next(255), (byte)Game.r.Next(255), (byte)Game.r.Next(255));
            previousTime = DateTime.Now;

            sprite = Game.playerWalk;

        }

        public void sendPos()
        {

            NetOutgoingMessage outGoingMessage = Game.client.CreateMessage();
            outGoingMessage.Write("POS");
            outGoingMessage.Write(position.X);
            outGoingMessage.Write(position.Y);

            Game.client.SendMessage(outGoingMessage, NetDeliveryMethod.Unreliable);
        }

        public void sendAliveStatus()
        {

            NetOutgoingMessage outGoingMessage = Game.client.CreateMessage();
            outGoingMessage.Write("LIFE");
            outGoingMessage.Write(alive);

            Game.client.SendMessage(outGoingMessage, NetDeliveryMethod.UnreliableSequenced);
        }


        public void sendChat()
        {
            NetOutgoingMessage outGoingMessage = Game.client.CreateMessage();
            outGoingMessage.Write("CHAT");
            outGoingMessage.Write(textCapture);
            Game.chatMessages.Add("You: " + textCapture);
            //SoundPlayer.playSound(Game.click);
            Game.soundInstances.Add(new SoundInstance(Game.click, 0f, 0f));
            if (Game.chatMessages.Count > 15)
                Game.chatMessages.RemoveAt(0);

            if (!(textCapture.IndexOf("/") == 0))
                overheadMessage = textCapture;
            ohmDecay = (60 * 5);

            if (textCapture.IndexOf("/setname") == 0)
                if (textCapture.Substring(0, 8).Equals("/setname"))
                {
                    username = textCapture.Substring(8).Trim();
                }

            if (textCapture.IndexOf("/scream") == 0)
                Game.soundInstances.Add(new SoundInstance(Game.SaD, 0f, 0f));

            if (textCapture.IndexOf("/fart") == 0)
                Game.soundInstances.Add(new SoundInstance(Game.fart, 0f, 0f));
            if (textCapture.IndexOf("/clear") == 0)
                Game.chatMessages.Clear();
            if (textCapture.IndexOf("/kill") == 0)
            {
                alive = false;
                sendAliveStatus();
                Game.soundInstances.Add(new SoundInstance(Game.fart, 0f, 0f));
                Game.soundInstances.Add(new SoundInstance(Game.SaD, 0f, 0f));
            }


            textCapture = "";
            Game.client.SendMessage(outGoingMessage, NetDeliveryMethod.ReliableOrdered);
        }

        public void updateConnectedPlayer()
        {
            double secsBetweenIntervals = .1;
            TimeSpan difference = (DateTime.Now - previousTime);
            if (difference.TotalSeconds >= secsBetweenIntervals)
            {
                previousTime += TimeSpan.FromSeconds(secsBetweenIntervals);
                frame++;
            }
            if (frame > 3)
                frame = 0;
        }

        public void updateClient() //Client Player Only
        {

            sprite.Position = position;

            ohmDecay--;
            if (ohmDecay <= 0)
                overheadMessage = "";



            moveClient();

            if (alive != previousAlive)
            {
                sendAliveStatus();
            }

            if (Keyboard.IsKeyPressed(Keyboard.Key.Return))//Main.ks.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Enter) && Main.oks.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Enter))
            {
                if (textCapture.Trim() != "")
                {
                    textCapture.Trim();
                    sendChat();
                }
            }



            double secsBetweenIntervals = .2;
            TimeSpan difference = (DateTime.Now - previousTime);
            if (difference.TotalSeconds >= secsBetweenIntervals)
            {
                previousTime += TimeSpan.FromSeconds(secsBetweenIntervals);
                frame++;

                if (frame % 2 == 0)
                    sendPos();
            }
            if (frame >= currentMaxFrames)
                frame = 0;




        }

        public void moveClient() //Client Player Only
        {
            bool moveX = true;
            bool moveY = true;

            Vector2f xCheck = new Vector2f(position.X + velocity.X, position.Y + Game.playerWalk.Texture.Size.Y);
            Vector2f yCheck = new Vector2f(position.X, position.Y + Game.playerWalk.Texture.Size.Y + velocity.Y);

            if (Game.map.GetPixel((uint)xCheck.X, (uint)xCheck.Y).Equals(Color.Black))
            {
                moveX = false;
            }
            if (Game.map.GetPixel((uint)yCheck.X, (uint)yCheck.Y).Equals(Color.Black))
            {
                moveY = false;
            }

            //RectangleShape rect = new RectangleShape(new Vector2f(1, 1));
            //rect.FillColor = Color.Red;
            //rect.Position = new Vector2f(xCheck.X, yCheck.Y);
            //    Game.window.Draw(rect);

            if (moveX)
            {
                currentMaxFrames = moveFrames;
                position.X += velocity.X;
            }
            if (moveY)
            {
                currentMaxFrames = moveFrames;
                position.Y += velocity.Y;
            }
            velocity = new Vector2f(0, 0);


        }

        public void checkControls()
        {
            if (Keyboard.IsKeyPressed(Keyboard.Key.Tab)) //Respawn
            {
                this.position = new Vector2f(100, 420);
                fallDistance = 0;
                if (!alive)
                {
                    alive = true;
                    sendAliveStatus();
                }
            }

            if (alive)
            {
                if (Keyboard.IsKeyPressed(Keyboard.Key.Left))
                {
                    velocity.X = -2f;
                }
                if (Keyboard.IsKeyPressed(Keyboard.Key.Right))
                {
                    velocity.X = 2f;
                }
                if (Keyboard.IsKeyPressed(Keyboard.Key.Up))
                {
                    velocity.Y = -2f;
                }
                if (Keyboard.IsKeyPressed(Keyboard.Key.Down))
                {
                    velocity.Y = 2f;
                }
            }
            else
            {

            }



        }

        public void drawClient()
        {


            if (oldPosition.X > position.X)
                facing = 1;
            if (oldPosition.X < position.X)
                facing = -1;

            //Main.spriteBatch.Draw(Main.playerSprite, position, null, color, 0f, Vector2.Zero, 1f, facing == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0); //Draw the client's player
            //Animation.drawAnimatedSpriteNonRef(Main.moving, 4, 10, position, facing, Color.White, 1f, frame, 0f, 1);
            //Main.spriteBatch.Draw(Main.moving, position, Color.White);

            ohmDecay--;
            if (ohmDecay > 0)
            {
                Render.drawString(Game.font, overheadMessage, new Vector2f(position.X, position.Y - 30), Color.Black, .4f, true);
                //Main.spriteBatch.DrawString(Main.font1, overheadMessage, new Vector2f(position.X + Main.playerSprite.Width / 2 - (Main.font1.MeasureString(overheadMessage).X / 2), position.Y - 20), Color.White);
            }



            if (alive)
            {
                Render.drawAnimation(Game.playerWalk, this.position, Color.White, new Vector2f((int)(Game.playerWalk.Texture.Size.X / moveFrames / 2), 0), facing, moveFrames, 1, frame, 1);
            }
            else
            {

            }

            if (Game.clientPlayer.username.Equals(""))
            {
                Render.drawString(Game.font, "CLIENT", position - new Vector2f(0, 15), Color.Black, .4f, true);
            }
            else
            {
                Render.drawString(Game.font, Game.clientPlayer.username, position - new Vector2f(0, 15), Color.Black, .4f, true);
            }



            oldPosition = position;
            previousAlive = alive;
        }

        public void drawConnectedPlayer()
        {

            onGround = false;



            ohmDecay--;
            if (ohmDecay > 0)
            {
                Render.drawString(Game.font, overheadMessage, new Vector2f(position.X, position.Y - 30), Color.Black, .4f, true);
                //Main.spriteBatch.DrawString(Main.font1, overheadMessage, new Vector2f(position.X + Main.playerSprite.Width / 2 - (Main.font1.MeasureString(overheadMessage).X / 2), position.Y - 20), Color.White);
            }

            Vector2f indexPlayerPos = position;

            if (username.Equals(""))
            {
                Render.drawString(Game.font, (Math.Abs((UID)) + "").Substring(0, 4), position - new Vector2f(0, 15), Color.Black, .4f, true);
            }
            else
            {
                Render.drawString(Game.font, username, position - new Vector2f(0, 15), Color.Black, .4f, true);
            }

            if (Keyboard.IsKeyPressed(Keyboard.Key.O))
            {
                Render.drawString(Game.font, position.ToString(), position - new Vector2f(0, 25), Color.Black, .4f, true);
            }


            if (!(oldPosition.Equals(position)))
            {
                animType = "move";

            }
            else
            {

                animType = "idle";
            }

            if (oldPosition.X > position.X)
                facing = 1;
            if (oldPosition.X < position.X)
                facing = -1;


            if (alive)
            {
            }
            else
            {

            }

            oldPosition = position;
        }
    }
}
