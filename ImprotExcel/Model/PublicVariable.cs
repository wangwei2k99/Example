using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImprotExcel.Model
{
     class PublicVariable
    {
        private static string[] fileNames;

        public static string[] FileNames
        {
            get { return fileNames; }
            set { fileNames = value; }
        }

    }
}
