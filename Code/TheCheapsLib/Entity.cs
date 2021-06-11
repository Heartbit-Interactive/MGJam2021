using Microsoft.Extensions.ObjectPool;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
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
        Texture2D _texture;
        [JsonIgnore]
        public Texture2D texture
        {
            get { return _texture; }
            set
            {
                _texture = value;
                if (sourcerect.Width == 0)
                    sourcerect = _texture.Bounds;
                hasShadow = sourcerect.Width <= 48;
            }
        }
        [JsonIgnore]
        public Vector2 origin;
        [JsonIgnore]
        public int uniqueId;
        /// <summary>
        /// Non in delta, depends if rect size <48 = true
        /// </summary>
        [JsonIgnore]
        public bool hasShadow;
        [JsonIgnore]
        public static Texture2D shadow;
        [JsonIgnore]
        public static Vector2 shadowOrigin;
        [JsonIgnore]
        internal static int UniqueCounter;
		

        internal float life_time = Settings.TimeOnTheFloor;
        internal bool removeable = false;
        /// PRIVATE: usa Entity.Create
        public Entity() { }

        internal void InitializeServer(float default_z)
        {
            if (z == 0)
                z = default_z;
            update_collision_rect();
            this.uniqueId = UniqueCounter++;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (GraphicSettings.ShowCollisions)
            {
                if (through)
                    spriteBatch.Draw(GraphicSettings.DebugSquare, collisionrect, null, GraphicSettings.NonCollidingColor, 0, Vector2.Zero, SpriteEffects.None, 0);
                else
                    spriteBatch.Draw(GraphicSettings.DebugSquare, collisionrect, null, GraphicSettings.CollisorColor, 0, Vector2.Zero, SpriteEffects.None, 0);
            }
            if (texture != null)
            {
                if (this.hasShadow)
                {
                    spriteBatch.Draw(shadow, this.posxy, null, Color.White, 0, shadowOrigin, 1, SpriteEffects.None, 0.1f);
                }
                var depth = z;
                if (collisionrect.Width!=0)
                    depth += ((posxy.Y+posz) / GraphicSettings.Bounds.Height) * 0.5f;

                spriteBatch.Draw(this.texture, this.posxy - this.posz * Vector2.UnitY, this.sourcerect, Color.White, 0, this.origin, 1, SpriteEffects.None, depth);
            }
        }

        public virtual void update_collision_rect()
        {
            var offx = (int)(posxy.X - collisionrect.Width / 2);
            var offy = (int)(posxy.Y - collisionrect.Height);
            collisionrect.X = offx;
            collisionrect.Y = offy;            
        }
        public void LoadTexture(ContentManager Content)
        {
            texture = Content.Load<Texture2D>(texture_path);
            if (sourcerect.Width != 0)
                origin = new Vector2(sourcerect.Width / 2, sourcerect.Height);
            else
                origin = new Vector2(texture.Width / 2, texture.Height);
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
            life_time = br.ReadSingle();
            removeable = br.ReadBoolean();
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
            bw.Write(life_time);
            bw.Write(removeable);
        }

        internal virtual void CopyDelta(Entity other)
        {
            this.posxy = other.posxy;
            this.z = other.z;
            this.direction = other.direction;
            this.through = other.through;
            this.speed = other.speed;
            this.tags = other.tags;
            this.posz = other.posz;
            this.removeable = other.removeable;
        }
        protected bool disposed;

        public virtual void Dispose()
        {
            if (this.disposed)
                return;
            Pool.Return(this);
            this.disposed = true;
        }
        private static ObjectPool<Entity> Pool = ObjectPool.Create<Entity>();
        public static Entity Create()
        {
            return Pool.Get();
        }

        internal Entity Clone()
        {
            var result = Create();
            result.texture_path = texture_path;
            result.name = name;
            result.posxy = posxy;
            result.z = z;
            result.sourcerect = sourcerect;
            result.direction = direction;
            result.through = through;
            result.speed = speed;
            result.tags = tags;
            result.collisionrect = collisionrect;
            result.origin = origin;
            result.posz = posz;
            result.removeable = removeable;
            result.InitializeServer(0.03f);
            return result;
        }
        public virtual void BinaryReadDelta(BinaryReader br)
        {
            uniqueId = br.ReadInt32();
            posxy = new Vector2(
            br.ReadSingle(),
            br.ReadSingle());

            z = br.ReadSingle();

            direction = new Vector2(
            br.ReadSingle(),
            br.ReadSingle());

            through = br.ReadBoolean();
            speed = br.ReadSingle();

            var count = br.ReadInt32();
            tags = new List<string>(count);
            for (int i = 0; i < count; i++)
                tags.Add(br.ReadString());

            posz = br.ReadSingle();
            removeable = br.ReadBoolean();
        }

        public virtual void BinaryWriteDelta(BinaryWriter bw)
        {
            bw.Write(uniqueId);
            bw.Write(posxy.X);
            bw.Write(posxy.Y);
            bw.Write(z);

            bw.Write(direction.X);
            bw.Write(direction.Y);

            bw.Write(through);
            bw.Write(speed);
            bw.Write(tags.Count);
            foreach (var tag in tags)
                bw.Write(tag);
            bw.Write(posz);
            bw.Write(removeable);
        }
    }
}
