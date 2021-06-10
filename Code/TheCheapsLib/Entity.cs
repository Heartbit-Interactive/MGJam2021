﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TheCheapsLib
{
    //MODEL
    //Contiene: posizione, inventario, percorso texture, eventuale rettangolo sorgente, layer z, fisica collisione, direzione, se attraversabile
    //da aggiungere tag(elettronica o altro)

    public class Entity: IBinarizable
    {
        public string texture_path="";
        public string name = "default";
        public Vector2 posxy;//posizione piedi
        public float z;
        public Rectangle sourcerect;
        public Vector2 direction;
        public bool through;
        public float speed;
        public List<string> tags = new List<string>();
        //for collision
        public Rectangle collisionrect;
        internal float posz;
        [JsonIgnore]
        public Texture2D texture;
        [JsonIgnore]
        public Vector2 origin;
        [JsonIgnore]
        public int uniqueId;
        internal static int UniqueCounter;
        public Entity() { }
        public Entity(string texture_path, string name, Vector2 posxy, float z, Rectangle sourcerect, Vector2 direction, bool through, float speed, List<string> tags, Rectangle collisionrect, Texture2D texture, Vector2 origin, float posz) 
        {
            this.texture_path = texture_path;
            this.name = name;
            this.posxy = posxy;
            this.z = z;
            this.sourcerect = sourcerect;
            this.direction = direction;
            this.through = through;
            this.speed = speed;
            this.tags = tags;
            this.collisionrect = collisionrect;
            this.texture = texture;
            this.origin = origin;
            this.posz = posz;
            InitializeServer();
        }

        internal void InitializeServer()
        {
            this.uniqueId = UniqueCounter++;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (this.sourcerect.Width == 0)
            {
                spriteBatch.Draw(this.texture, this.posxy, null, Color.White, 0, this.origin, 1, SpriteEffects.None, this.z);

                spriteBatch.Draw(this.texture, this.posxy - this.posz * Vector2.UnitY, null, Color.White, 0, this.origin, 1, SpriteEffects.None, this.z);
            }
            else
                spriteBatch.Draw(this.texture, this.posxy - this.posz * Vector2.UnitY, this.sourcerect, Color.White, 0, this.origin, 1, SpriteEffects.None, this.z);
        }

        public virtual void BinaryRead(BinaryReader br)
        {
            uniqueId = br.ReadInt32();
            through = br.ReadBoolean();

            name = br.ReadString();
            texture_path = br.ReadString();

            z = br.ReadSingle();
            speed = br.ReadSingle();

            posxy = new Vector2(
            br.ReadSingle(),
            br.ReadSingle());

            direction = new Vector2(
            br.ReadSingle(),
            br.ReadSingle());

            sourcerect = new Rectangle(
                br.ReadInt32(),
                br.ReadInt32(),
                br.ReadInt32(),
                br.ReadInt32());

            collisionrect = new Rectangle(
                br.ReadInt32(),
                br.ReadInt32(),
                br.ReadInt32(),
                br.ReadInt32());

            var count = br.ReadInt32();
            tags = new List<string>(count);
            for (int i = 0; i < count; i++)
                tags.Add(br.ReadString());

            posz = br.ReadSingle();
        }

        public virtual void BinaryWrite(BinaryWriter bw)
        {
            bw.Write(uniqueId);
            bw.Write(through);

            bw.Write(name ?? "");
            bw.Write(texture_path);

            bw.Write(z);
            bw.Write(speed);

            bw.Write(posxy.X);
            bw.Write(posxy.Y);

            bw.Write(direction.X);
            bw.Write(direction.Y);

            bw.Write(sourcerect.X);
            bw.Write(sourcerect.Y);
            bw.Write(sourcerect.Width);
            bw.Write(sourcerect.Height);

            bw.Write(collisionrect.X);
            bw.Write(collisionrect.Y);
            bw.Write(collisionrect.Width);
            bw.Write(collisionrect.Height);
            bw.Write(tags.Count);
            foreach (var tag in tags)
                bw.Write(tag);
            bw.Write(posz);
        }
    }
}
