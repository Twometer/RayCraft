using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayCraft.Renderer
{
    public class HitResult
    {
        public byte BlockType { get; private set; }
        public EnumFace Face { get; private set; }
        public bool HitEntity { get; private set; }

        public HitResult(byte blockType, EnumFace face, bool hitEntity)
        {
            BlockType = blockType;
            Face = face;
            HitEntity = hitEntity;
        }
    }
}
