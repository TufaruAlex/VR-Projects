using System;


namespace rt
{
    public class Ellipsoid : Geometry
    {
        private Vector Center { get; }
        private Vector SemiAxesLength { get; }
        private double Radius { get; }
        
        
        public Ellipsoid(Vector center, Vector semiAxesLength, double radius, Material material, Color color) : base(material, color)
        {
            Center = center;
            SemiAxesLength = semiAxesLength;
            Radius = radius;
        }

        public Ellipsoid(Vector center, Vector semiAxesLength, double radius, Color color) : base(color)
        {
            Center = center;
            SemiAxesLength = semiAxesLength;
            Radius = radius;
        }
        
        private Vector Normal(Vector center, double A, double B, double C, Vector point)
        {
            return new Vector(
                2 * (point.X - center.X) / (A * A),
                2 * (point.Y - center.Y) / (B * B),
                2 * (point.Z - center.Z) / (C * C)
            ).Normalize();
        }

        public override Intersection GetIntersection(Line line, double minDist, double maxDist)
        {
            var a = line.Dx.X;
            var b = line.X0.X;
            var c = line.Dx.Y;
            var d = line.X0.Y;
            var e = line.Dx.Z;
            var f = line.X0.Z;

            var A = SemiAxesLength.X * SemiAxesLength.X;
            var B = SemiAxesLength.Y * SemiAxesLength.Y;
            var C = SemiAxesLength.Z * SemiAxesLength.Z;

            var ap = a * a / A + c * c / B + e * e / C;
            var bp = 2 * (a * (b - Center.X) / A + c * (d - Center.Y) / B + e * (f - Center.Z) / C);
            var cp = (b - Center.X) * (b - Center.X) / A + (d - Center.Y) * (d - Center.Y) / B +
                (f - Center.Z) * (f - Center.Z) / C - Radius * Radius;
            
            var delta = bp * bp - 4 * ap * cp;
            
            if (delta < 0.0001)
            {
                return new Intersection();
            }
            
            var t1 = (-bp - Math.Sqrt(bp * bp - 4 * ap * cp)) / (2 * ap);
            var t2 = (-bp + Math.Sqrt(bp * bp + 4 * ap * cp)) / (2 * ap);

            var t1Valid = minDist <= t1 && t1 <= maxDist;
            var t2Valid = minDist <= t2 && t2 <= maxDist;

            if (!t1Valid && !t2Valid)
                return new Intersection();
            var tMin = Math.Min(t1, t2);
            var point = line.Dx * tMin + line.X0;
            var normal = Normal(Center, SemiAxesLength.X, SemiAxesLength.Y, SemiAxesLength.Z, point);
            if (t1Valid && !t2Valid)
            {
                point = line.Dx * t1 + line.X0;
                normal = Normal(Center, SemiAxesLength.X, SemiAxesLength.Y, SemiAxesLength.Z, point);
                return new Intersection(true, true, this, line, t1, normal, Material, Color);
            }
            if (!t1Valid)
            {
                point = line.Dx * t2 + line.X0;
                normal = Normal(Center, SemiAxesLength.X, SemiAxesLength.Y, SemiAxesLength.Z, point);
                return new Intersection(true, true, this, line, t2, normal, Material, Color);
            }
            return new Intersection(true, true, this, line, tMin, normal, Material, Color);
        }

        public Tuple<Double, Double> GetFirstAndLastIntersection(Line line)
        {
            var a = line.Dx.X;
            var b = line.X0.X;
            var c = line.Dx.Y;
            var d = line.X0.Y;
            var e = line.Dx.Z;
            var f = line.X0.Z;

            var A = SemiAxesLength.X * SemiAxesLength.X;
            var B = SemiAxesLength.Y * SemiAxesLength.Y;
            var C = SemiAxesLength.Z * SemiAxesLength.Z;

            var ap = a * a / A + c * c / B + e * e / C;
            var bp = 2 * (a * (b - Center.X) / A + c * (d - Center.Y) / B + e * (f - Center.Z) / C);
            var cp = (b - Center.X) * (b - Center.X) / A + (d - Center.Y) * (d - Center.Y) / B +
                (f - Center.Z) * (f - Center.Z) / C - Radius * Radius;
            
            var delta = bp * bp - 4 * ap * cp;
            
            if (delta < 0.0001)
            {
                return new Tuple<double, double>(0, 0);
            }
            
            var t1 = (-bp - Math.Sqrt(bp * bp - 4 * ap * cp)) / (2 * ap);
            var t2 = (-bp + Math.Sqrt(bp * bp + 4 * ap * cp)) / (2 * ap);

            return new Tuple<double, double>(t1, t2);
        }
    }
}
