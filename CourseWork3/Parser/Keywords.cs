using System.Linq;

namespace CourseWork3.Parser
{
    class Keywords
    {
        internal static string EOL = "EOL";
        internal static string EOF = "EOF";

        internal static string SpriteBlockBegin = "sprite";
        internal static string ProjectileBlockBegin = "projectile";
        internal static string GeneratorBlockBegin = "generator";

        internal static string EndOfBlock = "end";

        internal static string Runtime;
        internal static string Repeat_start;
        internal static string Delay;


        internal static string Velocity_to_Point = "velocityToPoint".ToLower();
        internal static string Point_rotation = "pointRotation".ToLower();

        internal static string ProjParamName = "genTime".ToLower();

        internal static string[] controlledObjectProperties = { "position", "velocity", "velocityScalar".ToLower(),
        "velocityAngle".ToLower(), "accelerationScalar".ToLower(), "accelerationAngle".ToLower() };
        internal static bool isControlledObjectProperty(string property) 
            => controlledObjectProperties.Contains(property);

        internal static bool isProjectileProperty(string property) => throw new System.NotImplementedException();
        internal static bool isGeneratorProperty


        internal static string[] PropertiesMethods = {"set", "inc", "clear"};
        internal static bool IsPropMethod(string method) => PropertiesMethods.Contains(method);
    }
}
