using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorldServer.Services.World;
using WorldServer.World.Objects;
using WorldServer.World.Positions;

namespace WorldServer
{
    public static class WorldUtils
    {
        public static Point3D GetForward(Player plr, int unit_length)
        {
            float angle = Math.Min(1.0f, plr._Value.WorldO / 4100.0f) * 360.0f;
            Point2D offset = WorldUtils.RotateVector(0, unit_length, angle);
            return new Point3D(offset, ZoneService.OcclusionProvider.GetTerrainZ(plr.Zone.ZoneId, plr._Value.WorldX + offset.X - (plr.Zone.Info.OffX << 12), plr._Value.WorldY + offset.Y - (plr.Zone.Info.OffY << 12)));
        }

        public static Point2D RotateVector(int x_origin, int y_origin, float angle)
        {
            int x_rotated = (int)(x_origin * Math.Cos(angle * Math.PI / 180.0f) - y_origin * Math.Sin(angle * Math.PI / 180.0f));
            int y_rotated = (int)(y_origin * Math.Cos(angle * Math.PI / 180.0f) + x_origin * Math.Sin(angle * Math.PI / 180.0f));
            return new Point2D(x_rotated, y_rotated);
        }

        public static Point2D CalculatePoint(Random random, int radius, int originX, int originY)
        {
            double angle = random.NextDouble() * Math.PI * 2.0d;
            double pointRadius = random.NextDouble() * (double)radius;
            double x = originX + pointRadius * Math.Cos(angle);
            double y = originY + pointRadius * Math.Sin(angle);
            // Console.WriteLine($"Angle: {angle}, radius: {pointRadius}");
            return new Point2D((int)x, (int)y);
        }
    }
}