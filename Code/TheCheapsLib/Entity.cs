using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TheCheapsLib
{
    //MODEL
    //Contiene: posizione, inventario, percorso texture, eventuale rettangolo sorgente, layer z, fisica collisione

    class Entity
    {
        private Vector2 posxy;//posizione piedi
        private int z;
        private string texture;
        private Rectangle sourcerect;
        private int direction;

        private Inventory inventory;
        public Entity() { }
        public Entity(Vector2 posxy, int z, string texture, Rectangle sourcerect/*, Inventory inventory*/) 
        {
            this.posxy = posxy;
            this.z = z;
            this.texture = texture;
            this.sourcerect = sourcerect;
            //this.inventory = inventory;
        }
        public void binarywrite(BinaryWriter bw)
        {

        }
        public void binaryread(BinaryReader br)
        {

        }
    }
}
