using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chalkable.Common.Exceptions
{
    public class UploadToCrocodocFailedException : ChalkableException
    {
        public UploadToCrocodocFailedException() : base("File upload to crocodoc failed")
        { }

        public UploadToCrocodocFailedException(string msg) : base(msg)
        { }
    }
}
