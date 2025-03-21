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
