﻿using System;

namespace rt
{
    class RayTracer
    {
        private Geometry[] geometries;
        private Light[] lights;

        public RayTracer(Geometry[] geometries, Light[] lights)
        {
            this.geometries = geometries;
            this.lights = lights;
        }

        private double ImageToViewPlane(int n, int imgSize, double viewPlaneSize)
        {
            return -n * viewPlaneSize / imgSize + viewPlaneSize / 2;
        }

        private Intersection FindFirstIntersection(Line ray, double minDist, double maxDist)
        {
            var intersection = Intersection.NONE;

            foreach (var geometry in geometries)
            {
                var intr = geometry.GetIntersection(ray, minDist, maxDist);

                if (!intr.Valid || !intr.Visible) continue;

                if (!intersection.Valid || !intersection.Visible)
                {
                    intersection = intr;
                }
                else if (intr.T < intersection.T)
                {
                    intersection = intr;
                }
            }

            return intersection;
        }

        private bool IsLit(Vector point, Light light)
        {
            var ray = new Line(light.Position, point);
            foreach (var geometry in geometries)
            {
                if (geometry.GetType() == typeof(RawCtMask)) continue;
                var intersection = geometry.GetIntersection(ray, 0, (light.Position - point).Length() - 0.001);
                if (intersection.Visible) return false;
            }

            return true;
        }

        public void Render(Camera camera, int width, int height, string filename)
        {
            var background = new Color(0.2, 0.2, 0.2, 1.0);

            var image = new Image(width, height);
            
            var viewParallel = (camera.Up ^ camera.Direction).Normalize();
            var vecW = camera.Direction * camera.ViewPlaneDistance;

            for (var i = 0; i < width; i++)
            {
                for (var j = 0; j < height; j++)
                {
                    var pointOnViewPlane = camera.Position + vecW + 
                                           camera.Up * ImageToViewPlane(j, height, camera.ViewPlaneHeight) + 
                                           viewParallel * ImageToViewPlane(i, width, camera.ViewPlaneWidth);
                    var ray = new Line(camera.Position, pointOnViewPlane);
                    var intersection = FindFirstIntersection(ray, camera.FrontPlaneDistance, camera.BackPlaneDistance);
                    if (intersection.Valid && intersection.Visible)
                    {
                        var material = intersection.Material;
                        var color = new Color();
                        foreach (var light in lights)
                        {
                            var lightColor = new Color();
                            lightColor += material.Ambient * light.Ambient;
                            if (IsLit(intersection.Position, light))
                            {
                                var t = (light.Position - intersection.Position).Normalize();
                                var n = intersection.Normal;
                                var e = (camera.Position - intersection.Position).Normalize();
                                var r = (n * (n * t) * 2 - t).Normalize();
                                if (n * t > 0)
                                {
                                    lightColor += material.Diffuse * light.Diffuse * (n * t);
                                }

                                if (e * r > 0)
                                {
                                    lightColor += material.Specular * light.Specular *
                                                  Math.Pow(e * r, material.Shininess);
                                }

                                lightColor *= light.Intensity;
                            }
                            color += lightColor;
                        }
                        image.SetPixel(i, j, color);
                    }
                    else
                    {
                        image.SetPixel(i, j, background);
                    }
                }
            }

            image.Store(filename);
        }
    }
}