using Rhino;
using Rhino.Commands;
using Rhino.Geometry;
using Rhino.Input;
using Rhino.Input.Custom;
using System;

namespace ZooPlugin
{
    public class CreateWoodPillarCommand : Command
    {
        public CreateWoodPillarCommand()
        {
            Instance = this;
        }

        ///<summary>The only instance of this command.</summary>
        public static CreateWoodPillarCommand Instance { get; private set; }

        ///<returns>The command name as it appears on the Rhino command line.</returns>
        public override string EnglishName => "CreateWoodPillar";

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            RhinoApp.WriteLine("The {0} command will create a wooden pillar right now.", EnglishName);

            // Step 1: Get the base center point of the pillar
            Point3d baseCenter;
            using (GetPoint getPointAction = new GetPoint())
            {
                getPointAction.SetCommandPrompt("Please select the base center point of the pillar");
                if (getPointAction.Get() != GetResult.Point)
                {
                    RhinoApp.WriteLine("No base center point was selected.");
                    return getPointAction.CommandResult();
                }
                baseCenter = getPointAction.Point();
            }

            // Step 2: Get the width and depth of the pillar
            double width;
            using (GetNumber getNumberAction = new GetNumber())
            {
                getNumberAction.SetCommandPrompt("Please enter the width of the pillar");
                getNumberAction.SetDefaultString("0.5"); // default width
                if (getNumberAction.Get() != GetResult.Number)
                {
                    RhinoApp.WriteLine("No width was specified.");
                    return getNumberAction.CommandResult();
                }
                width = getNumberAction.Number();
                if (width <= 0)
                {
                    RhinoApp.WriteLine("The width must be a positive number.");
                    return Result.Failure;
                }
            }

            double depth;
            using (GetNumber getNumberAction = new GetNumber())
            {
                getNumberAction.SetCommandPrompt("Please enter the depth of the pillar");
                getNumberAction.SetDefaultString("0.5"); // default depth
                if (getNumberAction.Get() != GetResult.Number)
                {
                    RhinoApp.WriteLine("No depth was specified.");
                    return getNumberAction.CommandResult();
                }
                depth = getNumberAction.Number();
                if (depth <= 0)
                {
                    RhinoApp.WriteLine("The depth must be a positive number.");
                    return Result.Failure;
                }
            }

            // Step 3: Get the height of the pillar
            double height;
            using (GetNumber getNumberAction = new GetNumber())
            {
                getNumberAction.SetCommandPrompt("Please enter the height of the pillar");
                getNumberAction.SetDefaultString("2.0"); // default height
                if (getNumberAction.Get() != GetResult.Number)
                {
                    RhinoApp.WriteLine("No height was specified.");
                    return getNumberAction.CommandResult();
                }
                height = getNumberAction.Number();
                if (height <= 0)
                {
                    RhinoApp.WriteLine("The height must be a positive number.");
                    return Result.Failure;
                }
            }

            // Step 4: Create the rectangular surface
            Point3d p1 = new Point3d(baseCenter.X - width / 2, baseCenter.Y - depth / 2, baseCenter.Z);
            Point3d p2 = new Point3d(baseCenter.X + width / 2, baseCenter.Y - depth / 2, baseCenter.Z);
            Point3d p3 = new Point3d(baseCenter.X + width / 2, baseCenter.Y + depth / 2, baseCenter.Z);
            Point3d p4 = new Point3d(baseCenter.X - width / 2, baseCenter.Y + depth / 2, baseCenter.Z);

            // Create a surface from the four corner points
            Curve[] rectangleCurves = new Curve[]
            {
                new Line(p1, p2).ToNurbsCurve(),
                new Line(p2, p3).ToNurbsCurve(),
                new Line(p3, p4).ToNurbsCurve(),
                new Line(p4, p1).ToNurbsCurve()
            };

            // Create the surface using the rectangle curves
            var boundary = new Polyline(new[] { p1, p2, p3, p4, p1 });
            var surface = Surface.CreateExtrusion(boundary.ToNurbsCurve(), new Vector3d(0, 0, height));

            // Step 5: Add the surface to the document
            if (surface != null && surface.IsValid)
            {
                doc.Objects.AddSurface(surface);
                doc.Views.Redraw();
                RhinoApp.WriteLine("The {0} command added one wooden pillar to the document.", EnglishName);
            }
            else
            {
                RhinoApp.WriteLine("The pillar surface is not valid.");
                return Result.Failure;
            }

            return Result.Success;
        }
    }
}
