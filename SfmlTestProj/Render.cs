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
    public class Render
    {
        public static void draw(Sprite sprite, Vector2f position, Color color, Vector2f origin, int facing)
        {
            Sprite newSprite;
            newSprite = sprite;
            newSprite.Texture.Smooth = false;
            sprite.Scale = new Vector2f(-facing, 1);
            sprite.Origin = origin;
            sprite.Position = position;
            sprite.Color = color;
            
            Game.window.Draw(sprite);
        }

        public static void drawString(Font font, String message, Vector2f position, Color color, float scale, bool centered)
        {
            Text text = new Text(message, font);
            text.Scale = new Vector2f(scale, scale);
            text.Position = position;// clientPlayer.position;
            text.Color = color;
            if (centered)
                text.Position = new Vector2f(text.Position.X - ((text.GetLocalBounds().Width * scale) / 2), text.Position.Y);

            Game.window.Draw(text);
        }

        public static void drawAnimation(Sprite sprite, Vector2f position, Color color, Vector2f origin, int facing, int frames, int rows, int currentFrame, int frameRow)
        {
            Sprite newSprite;
            newSprite = sprite;
            sprite.Scale = new Vector2f(-facing, 1);
            sprite.Origin = origin;
            sprite.Position = position;
            sprite.Color = color;
            sprite.TextureRect = runAnimation(sprite, (int)(sprite.Texture.Size.X / frames), (int)(sprite.Texture.Size.Y), ref currentFrame, frameRow, rows);
            Game.window.Draw(sprite);
        }


        private static IntRect runAnimation(Sprite texture, int widthOfFrame, int heightOfFrame, ref int CurrFrame, int currentRow, int totalRows)
        {
            IntRect source = new IntRect(0, 0, 0, 0);

            if (CurrFrame >= (texture.Texture.Size.X / widthOfFrame))
                CurrFrame = 0;

            source.Left = (CurrFrame * widthOfFrame);
            source.Width = widthOfFrame;
            heightOfFrame = (int)texture.Texture.Size.Y / totalRows;
            source.Top = (int)((heightOfFrame) * (currentRow - 1));
            source.Height = heightOfFrame;

            return source;
        }
    }
}
