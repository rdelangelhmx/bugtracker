using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Web;

namespace BugTracker.Validations
{
    public class ValidateFileAttribute : RequiredAttribute 
    {
        public override bool IsValid(object value) {

            var file = value as HttpPostedFileBase;

            if (file == null)
                return false;

            if (file.ContentLength > 2 * 1024 * 1024)
                return false;

            List<string> TypesAllowed = new List<string>()
            {
                "image/jpeg", "application/pdf"
            };

            if (!TypesAllowed.Contains(file.ContentType))
                return false;


            return true;
        }
    }
}