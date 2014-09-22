using System;
using System.Collections.Generic;
using Lidgren.Network;
using SFML.Audio;
using SFML.Graphics;
using SFML.Window;

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

        public int facingX = 1;
        public int facingY = 1;

        public bool onGround = false;
        public int groundTimer = 0;

        public int speed = 1;

        public long UID;

        public int moveFrames = 2;
        public int idleFrames = 3;
        public int currentMaxFrames = 0;


        public string textCapture = "";

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

            sprite = Game.tempIdleS;
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
            if (Keyboard.IsKeyPressed(Keyboard.Key.LShift))
            {
                if (Keyboard.IsKeyPressed(Keyboard.Key.Left))
                {
                    position.X -= 2;
                }
                if (Keyboard.IsKeyPressed(Keyboard.Key.Right))
                {
                    position.X += 2;
                }
                if (Keyboard.IsKeyPressed(Keyboard.Key.Up))
                {
                    position.Y -= 2;
                }
                if (Keyboard.IsKeyPressed(Keyboard.Key.Down))
                {
                    position.Y += 2;
                }
            }
            //bool moveX = true;
            //bool moveY = true;
            RectangleShape COL = new RectangleShape(new Vector2f(15, 6));
            COL.Position = position + new Vector2f(-Game.tempIdleS.Texture.Size.X / 2, Game.tempIdleS.Texture.Size.Y - COL.Size.Y);
            COL.FillColor = Color.Red;

            //Game.window.Draw(COL); //Draw Collision Box on Screen
            //left side
            if (Keyboard.IsKeyPressed(Keyboard.Key.Left))
            {
                bool ml = true; //"move left"
                for (int i = 0; i < COL.Size.Y - 1; i++)
                {
                    if (Game.map.GetPixel((uint)(COL.Position.X  - (speed + 1)), (uint)(COL.Position.Y + i)).Equals(Color.Black))
                    {
                        ml = false;
                        break;
                    }
                }
                if (ml)
                {
                    position.X -= speed;
                }
            }

            //right side
            if (Keyboard.IsKeyPressed(Keyboard.Key.Right))
            {
                bool mr = true;
                for (int i = 0; i < COL.Size.Y; i++)
                {
                    if (Game.map.GetPixel((uint)(COL.Position.X + COL.Size.X  + (speed)), (uint)(COL.Position.Y + i)).Equals(Color.Black))
                    {
                        mr = false;
                        break;
                    }
                }
                if (mr)
                {
                    position.X += speed;
                }
            }

            //top side
            if (Keyboard.IsKeyPressed(Keyboard.Key.Up))
            {
                bool mu = true;
                for (int i = 0; i < COL.Size.X - 1; i++)
                {
                    if (Game.map.GetPixel((uint)(COL.Position.X + i), (uint)(COL.Position.Y - (speed))).Equals(Color.Black))
                    {
                        mu = false;
                        break;
                    }
                }
                if (mu)
                {
                    position.Y -= speed;
                }
            }

            //bottom side
            if (Keyboard.IsKeyPressed(Keyboard.Key.Down))
            {
                bool mb = true;
                for (int i = 0; i < COL.Size.X; i++)
                {
                    if (Game.map.GetPixel((uint)(COL.Position.X  + i), (uint)(COL.Position.Y + COL.Size.Y + (speed))).Equals(Color.Black))
                    {
                        mb = false;
                        break;
                    }
                }
                if (mb)
                {
                    position.Y += speed;
                }
            }

            //RectangleShape rect = new RectangleShape();
            //rect.FillColor = Color.Red;
            //rect.Position = position;
            //rect.Size = new Vector2f(1, 1);
            //Game.window.Draw(rect);

            //if (moveX)
            //{
            //    currentMaxFrames = moveFrames;
            //    position.X += velocity.X;
            //}
            //if (moveY)
            //{
            //    currentMaxFrames = moveFrames;
            //    position.Y += velocity.Y;
            //}

             //velocity = new Vector2f(0, 0); //Has the effect of just moving by "speed" time you move


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
                    velocity.X = -speed;
                }
                if (Keyboard.IsKeyPressed(Keyboard.Key.Right))
                {
                    velocity.X = speed;
                }
                if (Keyboard.IsKeyPressed(Keyboard.Key.Up))
                {
                    velocity.Y = -speed;
                }
                if (Keyboard.IsKeyPressed(Keyboard.Key.Down))
                {
                    velocity.Y = speed;
                }
            }
            else
            {

            }



        }

        public void drawClient()
        {
            if (oldPosition.Y > position.Y)
            {
                facingY = 1;
                sprite = Game.tempIdleN;
            }
            else if (oldPosition.Y < position.Y)
            {
                facingY = -1;
                sprite = Game.tempIdleS;
            }
            else if (oldPosition.X > position.X)
            {   
                facingX = 1;
                sprite = Game.tempIdleEW;
            }
            else if (oldPosition.X < position.X)
            {
                facingX = -1;
                sprite = Game.tempIdleEW;
            }
            

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
                Render.draw(sprite, this.position, Color.White, new Vector2f((int)(Game.tempIdleS.Texture.Size.X / 2), 0), facingX);
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

            if (oldPosition.Y > position.Y)
            {
                facingY = 1;
                sprite = Game.tempIdleN;
            }
            else if (oldPosition.Y < position.Y)
            {
                facingY = -1;
                sprite = Game.tempIdleS;
            }
            else if (oldPosition.X > position.X)
            {
                facingX = 1;
                sprite = Game.tempIdleEW;
            }
            else if (oldPosition.X < position.X)
            {
                facingX = -1;
                sprite = Game.tempIdleEW;
            }


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
