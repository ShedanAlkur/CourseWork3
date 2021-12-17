﻿using System.Linq;

namespace CourseWork3.Parser
{
    class Keywords
    {
        public static readonly string EOL = "EOL".ToLower();
        public static readonly string EOF = "EOF".ToLower();
        public static readonly string ParameterSeparator = ",";

        public static readonly string Sprite = "sprite";
        public static readonly string Projectile = "projectile";
        public static readonly string Generator = "generator";
        public static readonly string Enemy = "enemy";
        public static readonly string Level = "level";
        public static readonly string EndOfPattern = "end";

        public static readonly string Path = "path";
        public static readonly string SizeRelativeToHitbox = "size";
        public static readonly string Rows = "rows";
        public static readonly string Columns = "columns";

        public static readonly string Runtime = "runtime";
        public static readonly string RepeatStart = "repeatstart";
        public static readonly string Pause = "pause";
        public static readonly string Destroy = "destroy";

        public static readonly string Set = "set";
        public static readonly string Increase = "inc";
        public static readonly string Clear = "clear";

        private static readonly string[] PropertiesMethods = { Set, Increase, Clear };
        public static bool IsPropMethod(string method) => PropertiesMethods.Contains(method);


        public static readonly string PositionX = "positionX".ToLower();
        public static readonly string PositionY = "positionX".ToLower();
        public static readonly string VelocityScalar = "velocity";
        public static readonly string VelocityAngle = "velocityAngle".ToLower();
        public static readonly string AccelerationScalar = "acceleration";
        public static readonly string AccelerationAngle = "accelerationAngle".ToLower();
        public static readonly string Hitbox = "hitbox".ToLower();
        public static readonly string Size = "size".ToLower();

        public static readonly string Color = "color".ToLower();

        public static readonly string Angle = "angle".ToLower();
        public static readonly string Sector = "sector".ToLower();
        public static readonly string SpawnDelay = "spawnDelay".ToLower();
        public static readonly string SpawnCount = "spawnCount".ToLower();
        public static readonly string RotationSpeed = "RotationSpeed".ToLower();
        public static readonly string RotationAcceleration = "RotationAcceleration".ToLower();
        public static readonly string AimToPlayer = "aimToPlayer".ToLower();

        public static readonly string velocityToPlayer = "velocityToPlayer".ToLower();
        public static readonly string VelocityToPoint = "velocitytopoint".ToLower();
        public static readonly string PointRotation = "pointRotation".ToLower();
        public static readonly string PointCounterRotation = "pointCounterRotation".ToLower();
        public static readonly string RandomColor = "randomColor".ToLower();

        public static readonly string Life = "life";
        public static readonly string MoveTo = "MoveTo".ToLower();

        public static string ProjParamName = "genTime".ToLower();

        public static readonly string For = "for";
        public static readonly string From = "from";
        public static readonly string To = "to";
        public static readonly string Incrementor = "inc";

        public static readonly string Spawn = "spawn";

        public static readonly string[] SingleFloatParamCommands =
        {
            PositionX, PositionY, VelocityScalar, VelocityAngle, AccelerationScalar, AccelerationAngle,
            Angle, Sector, SpawnDelay, SpawnCount,
            Runtime, Pause,
        };
        public static bool IsSingleFloatParamCommands(string command) => SingleFloatParamCommands.Contains(command);

        public static readonly string[] SingleVec2ParamCommands =
        {
            VelocityToPoint, PointRotation,
        };
        public static bool IsSingleVec2ParamCommands(string command) => SingleVec2ParamCommands.Contains(command);

    }
}
