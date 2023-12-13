namespace Denobrium.Json.Tests.Helpers.ComplexTypes
{
    public class SubClass2 : BaseClass
    {
        public SubClass2()
        { }

        public SubClass2(string name, string code, string description)
        {
            Name = name;
            Code = code;
            Description = description;
        }

        public string Description { get; set; } = null!;
    }
}
