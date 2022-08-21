using System;
using System.Windows.Markup;

namespace Fasetto.Word
{
    public class EnumBindingSourceExtension:MarkupExtension

    {
        public Type EnumType { get; private set; }
        public EnumBindingSourceExtension(Type enumType)
        {
            if (enumType == null||!enumType.IsEnum)
                throw    new Exception ("EnumType may not be null AND must by of type Enum");
            EnumType = enumType;
        }
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return Enum.GetValues(EnumType);
        }
    }
}
