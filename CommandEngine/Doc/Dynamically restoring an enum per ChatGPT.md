If you only have the enum name as a string and need to find the type first:
Let’s say:

Enum name: "MyEnum"
Value: "Value1"
csharp


string enumTypeName = "MyNamespace.MyEnum";
string enumValueName = "Value1";

Type enumType = Type.GetType(enumTypeName);
if (enumType != null && enumType.IsEnum)
{
    object enumValue = Enum.Parse(enumType, enumValueName);
    Console.WriteLine(enumValue); // prints "Value1"
}

===================================

In C#, I need to convert a string name of a Type and the 
value of the Type back to its original form. For example 
"System.String" and a value of "BChanged" should return 
the string with "BChanged".

Got it! You want to take a type name as a string, e.g. 
"System.String", and a value also as a string, e.g. 
"BChanged", and convert that to the actual typed value, 
like a string, int, DateTime, etc.

This is a common task when working with reflection, 
serialization, or config systems. Here's how to do it in C#:

✅ Solution using Type.GetType() and Convert.ChangeType()

using System;

class Program
{
static void Main()
{
string typeName = "System.String";
string rawValue = "BChanged";

        Type targetType = Type.GetType(typeName);

        if (targetType != null)
        {
            object typedValue = Convert.ChangeType(rawValue, targetType);
            Console.WriteLine($"Value: {typedValue}, Type: {typedValue.GetType()}");
        }
        else
        {
            Console.WriteLine("Could not find the type.");
        }
    }
}

✅ Works with common types:
"System.Int32" + "42" → int 42
"System.Boolean" + "true" → bool true
"System.DateTime" + "2024-01-01" → DateTime
"System.String" + "Hello" → string "Hello"

⚠️ Gotchas:
Type.GetType() only works automatically for 
types in mscorlib or System.Private.CoreLib. 
If you're dealing with custom types or types 
from another assembly, you'll need to include 
the fully qualified name, e.g.:

Type.GetType("MyNamespace.MyClass, MyAssembly")
For enum types, use Enum.Parse instead of Convert.ChangeType.

✅ Bonus: Handle Enums too
Here’s how to extend it to support enums:

object ConvertToType(string typeName, string value)
{
Type type = Type.GetType(typeName);

    if (type == null)
        throw new ArgumentException("Type not found");

    if (type.IsEnum)
        return Enum.Parse(type, value);

    return Convert.ChangeType(value, type);
}
