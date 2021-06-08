using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TheCheapsLib
{
    //MODEL
    //Contiene: posizione, inventario, percorso texture, eventuale rettangolo sorgente, layer z, fisica collisione, direzione, se attraversabile
    //da aggiungere tag(elettronica o altro)

    public class Entity
    {
        public string texture_path="";
        public string name = "default";
        public Vector2 posxy;//posizione piedi
        public float z;
        public Rectangle sourcerect;
        public Vector2 direction;
        public bool through;
        public float speed;
        public List<string> tags;
        //for collision
        public Rectangle collisionrect;
      
        public Inventory inventory;
        public Texture2D texture;
        public Vector2 origin;

        public Entity() { }
        public Entity(Vector2 posxy, int z, string texture, Rectangle sourcerect) 
        {
            this.posxy = posxy;
            this.z = z;
            this.texture_path = texture;
            this.sourcerect = sourcerect;
        }
        public void binarywrite(BinaryWriter bw)
        {
            bw.Write(through);

            bw.Write(name??"");
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
            if (tags.Count > 0)
            {
                bw.Write(tags.Count);
                foreach (var tag in tags)
                    bw.Write(tag);
            }
        }

        public void binaryread(BinaryReader br)
        {
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
            for(int i =0; i< count; i++)
                tags.Add(br.ReadString());
        }
    }
}
