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
        float speed = 2;

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

            sprite = Game.playerSprite;

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
            double secsBetweenIntervals = .1 * 4 / 30;
            TimeSpan difference = (DateTime.Now - previousTime);
            if (difference.TotalSeconds >= secsBetweenIntervals)
            {
                previousTime += TimeSpan.FromSeconds(secsBetweenIntervals);
                frame++;
            }
            if (frame > 30)
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

            double secsBetweenIntervals = .1 * 4 / 30;
            TimeSpan difference = (DateTime.Now - previousTime);
            if (difference.TotalSeconds >= secsBetweenIntervals)
            {
                previousTime += TimeSpan.FromSeconds(secsBetweenIntervals);
                frame++;

                if (frame % 2 == 0)
                    sendPos();
            }
            if (frame > 30)
                frame = 0;




        }

        public void moveClient() //Client Player Only
        {
            onGround = false;
            bool moveX = true;
            bool moveY = true;

            if (position.X < 0 || (position.X + Game.playerSprite.TextureRect.Width > Game.background.Texture.Size.X) ||
                position.Y < 0 || (position.Y + Game.playerSprite.TextureRect.Height > Game.background.Texture.Size.Y))
            {
                position = new Vector2f(100, 25); //Out of bounds, reset player
                velocity = new Vector2f(0, 0);
                fallDistance = 0;
            }
            try
            {
                if (Game.map.GetPixel(
                    (uint)position.X + (uint)velocity.X,
                    (uint)position.Y).Equals(Color.Black))
                    moveX = false;

                if (Game.map.GetPixel(
                    (uint)position.X + (uint)velocity.X,
                    (uint)position.Y + (uint)Game.playerSprite.TextureRect.Height).Equals(Color.Black)) //bottom left
                    moveX = false;

                if (Game.map.GetPixel((uint)position.X + (uint)Game.playerSprite.TextureRect.Width + (uint)velocity.X, (uint)position.Y + (uint)Game.playerSprite.TextureRect.Height).Equals(Color.Black)) //bottom right
                    moveX = false;
                if (Game.map.GetPixel((uint)position.X + (uint)Game.playerSprite.TextureRect.Width + (uint)velocity.X, (uint)position.Y).Equals(Color.Black)) //top right
                    moveX = false;

                if (Game.map.GetPixel((uint)position.X, (uint)position.Y + (uint)velocity.Y).Equals(Color.Black)) //top left
                {
                    moveY = false;
                }
                if (Game.map.GetPixel((uint)position.X, (uint)position.Y + (uint)Game.playerSprite.TextureRect.Height + (uint)velocity.Y).Equals(Color.Black)) //bottom left
                    moveY = false;
                if (Game.map.GetPixel((uint)position.X + (uint)Game.playerSprite.TextureRect.Width, (uint)position.Y + (uint)velocity.Y + (uint)Game.playerSprite.TextureRect.Height).Equals(Color.Black)) //bottom right
                    moveY = false;
                if (Game.map.GetPixel((uint)position.X + (uint)Game.playerSprite.TextureRect.Width, (uint)position.Y + (uint)velocity.Y).Equals(Color.Black)) //top right
                    moveY = false;
                if (Game.map.GetPixel((uint)position.X + (uint)Game.playerSprite.TextureRect.Width / 2, (uint)position.Y + (uint)Game.playerSprite.TextureRect.Height + 2).Equals(Color.White))
                {
                    velocity.Y = 0;
                    if (Keyboard.IsKeyPressed(Keyboard.Key.Up))
                        velocity.Y = -3f;
                    if (Keyboard.IsKeyPressed(Keyboard.Key.Down))
                        velocity.Y = 3f;
                    fallDistance = 0;
                }

                if (moveX)
                    this.position.X += velocity.X;
                else //against a wall
                {
                }
                if (moveY)
                {
                    groundTimer = 0;
                    this.position.Y += velocity.Y;

                    if (velocity.Y > 10)
                        fallDistance++;
                    else
                        fallDistance = 0;
                    if (fallDistance == maxFall)
                    {
                        //SoundPlayer.playSound(Game.SaD);
                    }
                }
                else //on the ground or ceiling
                {
                    onGround = true;
                    groundTimer++;
                    if (fallDistance > maxFall)
                    {
                        alive = false;
                        Game.soundInstances.Add(new SoundInstance(Game.crunch, 0f, 0f));

                    }
                    fallDistance = 0;
                    velocity.Y = 0;
                }


            }
            catch (Exception)
            {
                position = new Vector2f(100, 25); //Out of bounds, reset player
                velocity = new Vector2f(0, 0);
            }
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
                velocity.X = 0;
                velocity.Y = 0;

                if (Keyboard.IsKeyPressed(Keyboard.Key.Right))
                    if (velocity.X < 4)
                        velocity.X += 4;
                if (Keyboard.IsKeyPressed(Keyboard.Key.Left))
                    if (velocity.X > -4)
                        velocity.X += -4;
                if (Keyboard.IsKeyPressed(Keyboard.Key.Up))
                    if (velocity.Y > -4)
                        velocity.Y += -4;
                if (Keyboard.IsKeyPressed(Keyboard.Key.Down))
                    if (velocity.Y < 4)
                        velocity.Y += 4;


            }
            else
            {
                velocity.X = 0;
                velocity.Y = 0;
            }

            //if (velocity.Y < 18)
            //    velocity.Y += 1f;

            if (velocity.X > 0)
                velocity.X--;
            if (velocity.X < 0)
                velocity.X++;

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

            if (Math.Abs(velocity.X) > .5f)
                animType = "move";
            else
                animType = "idle";

            if (alive)
            {
                if (groundTimer > 3)
                {

                    if (animType == "move")
                        Render.drawAnimation(Game.run2, position - new Vector2f(0, -2), Color.Black, new Vector2f(Game.run2.Texture.Size.X / 2 / 30, 0), facing, 30, 1, frame, 1);
                    if (animType == "idle")
                        Render.draw(Game.playerSprite, this.position - new Vector2f(0, -2), Color.Black, new Vector2f(Game.playerSprite.TextureRect.Width / 2, 0), facing);
                }
                else
                {
                    Render.draw(Game.jump, this.position - new Vector2f(0, -2), Color.Black, new Vector2f(Game.jump.TextureRect.Width / 2, 0), facing);

                }
            }
            else
            {
                Render.draw(Game.playerSpriteDead, this.position - new Vector2f(12, -2), Color.Black, new Vector2f(Game.playerSpriteDead.TextureRect.Width / 2, 0), facing);

            }

            if (Game.clientPlayer.username.Equals(""))
            {
                Render.drawString(Game.font, "CLIENT", position - new Vector2f(0, 15), Color.Black, .4f, true);
            }
            else
            {
                Render.drawString(Game.font, Game.clientPlayer.username, position - new Vector2f(0, 15), Color.Black, .4f, true);
            }

            //if (animType.Equals("move"))
            //  Animation.drawAnimatedSpriteNonRef(Main.moving, 4, 3, position, facing, Color.White, 1f, frame, 0f, 0);
            //if (animType.Equals("idle"))
            //  Main.spriteBatch.Draw(Main.standing, position, null, Color.White, 0f, Vector2.Zero, 1f, facing == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0); //Draw the client's player

            //if (Main.clientPlayer.username.Equals(""))
            //  Main.spriteBatch.DrawString(Main.font1, "CLIENT", Main.clientPlayer.position + new Vector2(20, 0), Color.White);
            //else
            //  Main.spriteBatch.DrawString(Main.font1, Main.clientPlayer.username, Main.clientPlayer.position + new Vector2(20, 0), Color.White);


            oldPosition = position;
            previousAlive = alive;
        }

        public void drawConnectedPlayer()
        {

            onGround = false;
            if (position.X < 0 || (position.X + Game.playerSprite.TextureRect.Width > Game.background.Texture.Size.X) ||
               position.Y < 0 || (position.Y + Game.playerSprite.TextureRect.Height > Game.background.Texture.Size.Y))
            {
            }
            else
            {

                if (Game.map.GetPixel((uint)(position.X + Game.playerSprite.TextureRect.Width / 2), (uint)(position.Y + Game.playerSprite.TextureRect.Height + velocity.Y)).Equals(Color.Black))
                {
                    onGround = true;
                }


            }


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
                if (animType == "move")
                    Render.drawAnimation(Game.run2, position - new Vector2f(0, -2), connectColor, new Vector2f(Game.run2.Texture.Size.X / 2 / 30, 0), facing, 30, 1, frame, 1);
                if (animType == "idle")
                    Render.draw(Game.playerSprite, this.position - new Vector2f(0, -2), connectColor, new Vector2f(Game.playerSprite.TextureRect.Width / 2, 0), facing);
            }
            else
            {
                Render.draw(Game.playerSpriteDead, this.position - new Vector2f(12, -2), connectColor, new Vector2f(Game.playerSpriteDead.TextureRect.Width / 2, 0), facing);

            }

            if (animType.Equals("move"))
            {
                //Animation.drawAnimatedSpriteNonRef(Main.moving, 4, 3, position, facing, Color.White, 1f, frame, 0f, 0);
            }
            if (animType.Equals("idle"))
            {
                //Main.spriteBatch.Draw(Main.standing, position, null, Color.White, 0f, new Vector2f(0,0), 1f, facing == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0); //Draw the client's player
            }

            oldPosition = position;
        }
    }
}
