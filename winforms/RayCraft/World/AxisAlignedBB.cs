using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Craft.Client.World
{
    public class AxisAlignedBB
    {
        private double epsilon;
        public double x0;
        public double y0;
        public double z0;
        public double x1;
        public double y1;
        public double z1;

        public AxisAlignedBB(double x0, double y0, double z0, double x1, double y1, double z1)
        {
            this.epsilon = 0.0f;
            this.x0 = x0;
            this.y0 = y0;
            this.z0 = z0;
            this.x1 = x1;
            this.y1 = y1;
            this.z1 = z1;
        }

        public AxisAlignedBB expand(double xa, double ya, double za)
        {
            double _x0 = this.x0;
            double _y0 = this.y0;
            double _z0 = this.z0;
            double _x2 = this.x1;
            double _y2 = this.y1;
            double _z2 = this.z1;
            if (xa < 0.0f)
            {
                _x0 += xa;
            }
            if (xa > 0.0f)
            {
                _x2 += xa;
            }
            if (ya < 0.0f)
            {
                _y0 += ya;
            }
            if (ya > 0.0f)
            {
                _y2 += ya;
            }
            if (za < 0.0f)
            {
                _z0 += za;
            }
            if (za > 0.0f)
            {
                _z2 += za;
            }
            return new AxisAlignedBB(_x0, _y0, _z0, _x2, _y2, _z2);
        }

        public AxisAlignedBB grow(double xa, double ya, double za)
        {
            double _x0 = this.x0 - xa;
            double _y0 = this.y0 - ya;
            double _z0 = this.z0 - za;
            double _x2 = this.x1 + xa;
            double _y2 = this.y1 + ya;
            double _z2 = this.z1 + za;
            return new AxisAlignedBB(_x0, _y0, _z0, _x2, _y2, _z2);
        }

        public double clipXCollide(AxisAlignedBB c, double xa)
        {
            if (c.y1 <= this.y0 || c.y0 >= this.y1)
            {
                return xa;
            }
            if (c.z1 <= this.z0 || c.z0 >= this.z1)
            {
                return xa;
            }
            if (xa > 0.0f && c.x1 <= this.x0)
            {
                double max = this.x0 - c.x1 - this.epsilon;
                if (max < xa)
                {
                    xa = max;
                }
            }
            if (xa < 0.0f && c.x0 >= this.x1)
            {
                double max = this.x1 - c.x0 + this.epsilon;
                if (max > xa)
                {
                    xa = max;
                }
            }
            return xa;
        }

        public double clipYCollide(AxisAlignedBB c, double ya)
        {
            if (c.x1 <= this.x0 || c.x0 >= this.x1)
            {
                return ya;
            }
            if (c.z1 <= this.z0 || c.z0 >= this.z1)
            {
                return ya;
            }
            if (ya > 0.0f && c.y1 <= this.y0)
            {
                double max = this.y0 - c.y1 - this.epsilon;
                if (max < ya)
                {
                    ya = max;
                }
            }
            if (ya < 0.0f && c.y0 >= this.y1)
            {
                double max = this.y1 - c.y0 + this.epsilon;
                if (max > ya)
                {
                    ya = max;
                }
            }
            return ya;
        }

        public double clipZCollide(AxisAlignedBB c, double za)
        {
            if (c.x1 <= this.x0 || c.x0 >= this.x1)
            {
                return za;
            }
            if (c.y1 <= this.y0 || c.y0 >= this.y1)
            {
                return za;
            }
            if (za > 0.0f && c.z1 <= this.z0)
            {
                double max = this.z0 - c.z1 - this.epsilon;
                if (max < za)
                {
                    za = max;
                }
            }
            if (za < 0.0f && c.z0 >= this.z1)
            {
                double max = this.z1 - c.z0 + this.epsilon;
                if (max > za)
                {
                    za = max;
                }
            }
            return za;
        }

        public bool intersects(AxisAlignedBB c)
        {
            return c.x1 > this.x0 && c.x0 < this.x1 && c.y1 > this.y0 && c.y0 < this.y1 && c.z1 > this.z0 && c.z0 < this.z1;
        }

        public void move(double xa, double ya, double za)
        {
            this.x0 += xa;
            this.y0 += ya;
            this.z0 += za;
            this.x1 += xa;
            this.y1 += ya;
            this.z1 += za;
        }
    }
}
