using Rhino;
using Rhino.Commands;
using Rhino.Geometry;
using Rhino.Input;
using Rhino.Input.Custom;
using System;

namespace ZooPlugin
{
    public class ZooCommand : Command
    {
        public ZooCommand()
        {
            Instance = this;
        }

        ///<summary>The only instance of this command.</summary>
        public static ZooCommand Instance { get; private set; }

        ///<returns>The command name as it appears on the Rhino command line.</returns>
        public override string EnglishName => "ZooCommand";

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            // Start the command
            RhinoApp.WriteLine("The {0} command will create a sphere right now.", EnglishName);

            // Step 1: Get the center point of the sphere
            Point3d centerPoint;
            using (GetPoint getPointAction = new GetPoint())
            {
                getPointAction.SetCommandPrompt("Please select the center point of the sphere");
                if (getPointAction.Get() != GetResult.Point)
                {
                    RhinoApp.WriteLine("No center point was selected.");
                    return getPointAction.CommandResult();
                }
                centerPoint = getPointAction.Point();
            }

            // Step 2: Get the radius of the sphere
            double radius;
            using (GetNumber getNumberAction = new GetNumber())
            {
                getNumberAction.SetCommandPrompt("Please enter the radius of the sphere");
                getNumberAction.SetDefaultString("1.0"); // default radius
                if (getNumberAction.Get() != GetResult.Number)
                {
                    RhinoApp.WriteLine("No radius was specified.");
                    return getNumberAction.CommandResult();
                }
                radius = getNumberAction.Number();
            }

            // Step 3: Create the sphere and add it to the document
            Sphere sphere = new Sphere(centerPoint, radius);
            doc.Objects.AddSphere(sphere);
            doc.Views.Redraw();
            RhinoApp.WriteLine("The {0} command added one sphere to the document.", EnglishName);

            return Result.Success;
        }
    }
}
