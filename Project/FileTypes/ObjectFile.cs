using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMHEditor.Project.FileTypes
{
    enum ObjectAttributeType
    {
        INT, FLOAT, STRINGID, STRING
    }
    class ObjectAttribute
    {
        string name;
        public ObjectAttributeType type;
        object value;
    }
    class ObjectFile
    {
        List<ObjectAttribute> attributes;
    }
}
