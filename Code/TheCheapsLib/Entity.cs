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
        public Vector2 posxy;
        public float z;
        public Rectangle sourcerect;
        public Vector2 direction;
        public bool through;
        public float speed;
        public List<string> tags = new List<string>();
        //for collision
        public Rectangle collisionrect;
        public float posz;
        public int frame_index = 0;
        [JsonIgnore]
        Texture2D _texture;
        [JsonIgnore]
        public Texture2D texture
        {
            get { return _texture; }
            set
            {
                _texture = value;
                if (_texture == null)
                    sourcerect.Width = 0;
                else
                {
                    if (sourcerect.Width == 0)
                        sourcerect = _texture.Bounds;
                }
                hasShadow = sourcerect.Width <= 48;
                destinationRectangle.Height = sourcerect.Height;
                destinationRectangle.Width = sourcerect.Width;
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
        [JsonIgnore]
        internal static float ShakeCounter;
        [JsonIgnore]
        internal static float ShakeIntensity;
        [JsonIgnore]
        internal static float ShakeX;

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
        Rectangle destinationRectangle;
        public void Update()
        { 
        
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            if (texture != null)
            {
                destinationRectangle.X = (int)(this.posxy.X+shakeX);
                    destinationRectangle.Y = (int)(this.posxy.Y - this.posz);
                if (this.hasShadow)
                {
                    spriteBatch.Draw(shadow, this.posxy, null, Color.White, 0, shadowOrigin, 1, SpriteEffects.None, 0.1f);
                }
                var depth = z;
                if (collisionrect.Width != 0)
                    depth += ((posxy.Y + posz) / (GraphicSettings.Bounds.Height + 128)) * 0.5f;

                var effects = direction.X < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
                spriteBatch.Draw(this.texture, destinationRectangle, this.sourcerect, Color.White, 0, this.origin,  effects, depth);
            }
            if (GraphicSettings.ShowCollisions)
            {
                if (through)
                    spriteBatch.Draw(GraphicSettings.DebugSquare, collisionrect, null, GraphicSettings.NonCollidingColor, 0, Vector2.Zero, SpriteEffects.None, 1);
                else
                    spriteBatch.Draw(GraphicSettings.DebugSquare, collisionrect, null, GraphicSettings.CollisorColor, 0, Vector2.Zero, SpriteEffects.None, 1);
            }
        }

        public virtual void update_collision_rect()
        {
            var offx = (int)(posxy.X - collisionrect.Width / 2);
            var offy = (int)(posxy.Y - collisionrect.Height);
            collisionrect.X = offx;
            collisionrect.Y = offy;
        }
        public Rectangle get_displaced_collision_rect(Vector2 deltaxy)
        {
            var X = (int)(posxy.X + deltaxy.X - collisionrect.Width / 2);
            var Y = (int)(posxy.Y + deltaxy.Y - collisionrect.Height);
            return new Rectangle(X, Y, collisionrect.Width, collisionrect.Height);
        }
        public virtual void LoadTexture(ContentManager Content)
        {
            if (string.IsNullOrEmpty(texture_path))
                return;
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
            frame_index = br.ReadInt32();
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
            bw.Write(frame_index);
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
            if (this.frame_index != other.frame_index)
            {
                this.frame_index = other.frame_index;
                this.sourcerect.X = this.sourcerect.Width * this.frame_index;
                if (texture != null)
                {
                    this.sourcerect.Y = this.sourcerect.X / this.texture.Width * this.sourcerect.Height;
                    this.sourcerect.X = this.sourcerect.X % this.texture.Width;
                }
            }
        }
        protected bool disposed;

        public virtual void Dispose()
        {
            if (this.disposed)
                return;
            texture = null;
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
            result.tags = new List<string>(tags);
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
            frame_index = br.ReadInt32();
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
            bw.Write(frame_index);
        }
    }
}
