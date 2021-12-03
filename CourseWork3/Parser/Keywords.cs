using System.Linq;

namespace CourseWork3.Parser
{
    class Keywords
    {
        internal const string EOL = "EOL";
        internal const string EOF = "EOF";
        internal const string ParameterSeparator = ",";

        internal const string For = "For";

        internal const string Sprite = "sprite";
        internal const string Projectile = "projectile";
        internal const string Generator = "generator";
        internal const string Enemy = "enemy";
        internal const string Level = "level";
        internal const string End = "end";

        internal const string Runtime = "runtime";
        internal const string RepeatStart = "repeatstart";
        internal const string Pause = "pause";

        internal const string Set = "set";
        internal const string Increase = "inc";
        internal const string Clear = "clear";

        private readonly static string[] PropertiesMethods = { Set, Increase, Clear };
        internal static bool IsPropMethod(string method) => PropertiesMethods.Contains(method);

        internal readonly string VelocityToPoint = "velocitytopoint".ToLower();
        internal readonly string velocityToPlayer = "velocityToPlayer".ToLower();
        internal readonly string PointRotation = "pointRotation".ToLower();

        internal static string ProjParamName = "genTime".ToLower();

        internal static string[] controlledObjectProperties = { "positionx", "positiony", "velocity", "velocityAngle".ToLower(),
        "acceleration".ToLower(), "accelerationAngle".ToLower(),  };
        internal static bool isControlledObjectProperty(string property) 
            => controlledObjectProperties.Contains(property);

        internal static bool isProjectileProperty(string property) => throw new System.NotImplementedException();
        internal static bool isGeneratorProperty(string property) => throw new System.NotImplementedException();


    }
}
