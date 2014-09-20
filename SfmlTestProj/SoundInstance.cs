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
    public class SoundInstance
    {
        public static float volume = 10f;
        public Sound sound;
        float basePitch;
        float pitchVariance;
        public bool started = false;

        public SoundInstance(SoundBuffer sound, float basePitch, float pitchVariance)
        {
            this.sound = new Sound(sound);
            this.basePitch = basePitch;
            this.pitchVariance = pitchVariance;
        }

        public void update()
        {
            float finalVariance =
               (float)(Game.r.Next((int)(pitchVariance * 100)) / 100f);

          
                sound.Pitch = finalVariance;
                sound.Volume = volume;
                sound.Play();
            

                Game.soundInstances.Remove(this);
            
        }

    }
}


