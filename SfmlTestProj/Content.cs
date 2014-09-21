using System;
using System.Collections.Generic;
using SFML.Graphics;
using SFML.Audio;

namespace Jaunt
{
	public class Content
	{
		private Dictionary<string, Texture> contentTexture;
		private Dictionary<string, SoundBuffer> contentSoundBuffer;
		private Dictionary<string, Font> contentFont;
		private string directoryPrefix;

		/// <summary>
		/// Initializes a new instance of the <see cref="Jaunt.Content"/> class.
		/// </summary>
		/// <param name="directoryPrefix">Directory prefix. Make sure it ends in /</param>
		public Content (string directoryPrefix)
		{
			this.directoryPrefix = directoryPrefix;

			contentTexture = new Dictionary<string, SFML.Graphics.Texture>();
			contentSoundBuffer = new Dictionary<string, SFML.Audio.SoundBuffer>();
			contentFont = new Dictionary<string, SFML.Graphics.Font>();
		}

		public Texture Texture(string filename)
		{
			//if key "fileName" isn't in "contentTexture" then load it
			if (!contentTexture.ContainsKey(filename))
			{
				string newTexturePath = directoryPrefix + filename;
				Texture newTexture = new Texture (newTexturePath);
				contentTexture[filename] = newTexture;
			}

			//return the Texture
			return contentTexture [filename];
		}

		public SoundBuffer SoundBuffer(string filename)
		{
			//if key "fileName" isn't in "contentSoundBuffer" then load it
			if (!contentSoundBuffer.ContainsKey(filename))
			{
				string newSoundBufferPath = directoryPrefix + filename;
				SoundBuffer newSoundBuffer = new SoundBuffer (newSoundBufferPath);
				contentSoundBuffer[filename] = newSoundBuffer;
			}

			//return the SoundBuffer
			return contentSoundBuffer[filename];
		}

		public Font Font(string filename)
		{			
			//if key "fileName" isn't in "contentFont" then load it
			if (!contentFont.ContainsKey(filename))
			{
				string newFontPath = directoryPrefix + filename;
				Font newFont = new Font (newFontPath);
				contentFont[filename] = newFont;
			}

			//return the Font
			return contentFont[filename];
		}
	}
}

