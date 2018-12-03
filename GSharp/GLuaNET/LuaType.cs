using System;
using System.Collections.Generic;
using System.Text;

namespace GSharp.GLuaNET
{
    public class LuaType
    {
        // Lua Types
        public static readonly LuaType None = new LuaType(-1, "None");
        public static readonly LuaType Nil = new LuaType(0, "Nil");
        public static readonly LuaType Bool = new LuaType(1, "Bool");
        public static readonly LuaType LightUserData = new LuaType(2, "LightUserData");
        public static readonly LuaType Number = new LuaType(3, "Number");
        public static readonly LuaType String = new LuaType(4, "String");
        public static readonly LuaType Table = new LuaType(5, "Table");
        public static readonly LuaType Function = new LuaType(6, "Function");
        public static readonly LuaType UserData = new LuaType(7, "UserData");
        public static readonly LuaType Thread = new LuaType(8, "Thread");
        // GMod Types
        public static readonly LuaType Entity = new LuaType(9, "Entity");
        public static readonly LuaType Vector = new LuaType(10, "Vector");
        public static readonly LuaType Angle = new LuaType(11, "Angle");
        public static readonly LuaType PhysObj = new LuaType(12, "PhysObj");
        public static readonly LuaType Save = new LuaType(13, "Save");
        public static readonly LuaType Restore = new LuaType(14, "Restore");
        public static readonly LuaType DamageInfo = new LuaType(15, "DamageInfo");
        public static readonly LuaType EffectData = new LuaType(16, "EffectData");
        public static readonly LuaType MoveData = new LuaType(17, "MoveData");
        public static readonly LuaType RecipientFilter = new LuaType(18, "RecipientFilter");
        public static readonly LuaType UserCmd = new LuaType(19, "UserCmd");
        public static readonly LuaType ScriptedVehicle = new LuaType(20, "ScriptedVehicle");
        // Client Only
        public static readonly LuaType Material = new LuaType(21, "Material");
        public static readonly LuaType Panel = new LuaType(22, "Panel");
        public static readonly LuaType Particle = new LuaType(23, "Particle");
        public static readonly LuaType ParticleEmitter = new LuaType(24, "ParticleEmitter");
        public static readonly LuaType Texture = new LuaType(25, "Texture");
        public static readonly LuaType UserMsg = new LuaType(26, "UserMsg");

        public static readonly LuaType ConVar = new LuaType(27, "ConVar");
        public static readonly LuaType IMesh = new LuaType(28, "IMesh");
        public static readonly LuaType Matrix = new LuaType(29, "Matrix");
        public static readonly LuaType Sound = new LuaType(30, "Sound");
        public static readonly LuaType PixelVisHandle = new LuaType(31, "PixelVisHandle");
        public static readonly LuaType DLight = new LuaType(32, "DLight");
        public static readonly LuaType Video = new LuaType(33, "Video");
        public static readonly LuaType File = new LuaType(34, "File");
        public static readonly LuaType Locomotion = new LuaType(35, "Locomotion");
        public static readonly LuaType Path = new LuaType(36, "Path");
        public static readonly LuaType NavArea = new LuaType(37, "NavArea");
        public static readonly LuaType SoundHandle = new LuaType(38, "SoundHandle");
        public static readonly LuaType NavLadder = new LuaType(39, "NavLadder");
        public static readonly LuaType ParticleSystem = new LuaType(40, "ParticleSystem");
        public static readonly LuaType ProjectedTexture = new LuaType(41, "ProjectedTexture");
        public static readonly LuaType PhysCollide = new LuaType(42, "PhysCollide");
        public static readonly LuaType Count = new LuaType(43, "Count");

        readonly int type;
        readonly string name;
        private LuaType(int type, string name = "")
        {
            this.type = type;
            this.name = name;
        }

        public static implicit operator int(LuaType value)
        {
            return value.type;
        }

        public static implicit operator LuaType(int value)
        {
            return new LuaType(value);
        }
    }
}
