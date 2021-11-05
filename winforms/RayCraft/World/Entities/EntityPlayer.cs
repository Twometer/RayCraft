using Craft.Client.World.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Craft.Client.World.Entities
{
    public class EntityPlayer : Entity
    {
        public int Gamemode { get; set; }
        public bool Hardcore { get; set; }
    }
}
