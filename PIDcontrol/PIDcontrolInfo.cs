using System;
using System.Drawing;
using Grasshopper.Kernel;

namespace PIDcontrol
{
    public class PIDcontrolInfo : GH_AssemblyInfo
    {
        public override string Name
        {
            get
            {
                return "PIDcontrol";
            }
        }
        public override Bitmap Icon
        {
            get
            {
                //Return a 24x24 pixel bitmap to represent this GHA library.
                return null;
            }
        }
        public override string Description
        {
            get
            {
                //Return a short string describing the purpose of this GHA library.
                return "";
            }
        }
        public override Guid Id
        {
            get
            {
                return new Guid("e59bfc63-82b6-4320-b6ad-558f491daa5c");
            }
        }

        public override string AuthorName
        {
            get
            {
                //Return a string identifying you or your company.
                return "";
            }
        }
        public override string AuthorContact
        {
            get
            {
                //Return a string representing your preferred contact details.
                return "";
            }
        }
    }
}
