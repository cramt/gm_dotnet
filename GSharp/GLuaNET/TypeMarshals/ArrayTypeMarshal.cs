namespace GSharp.GLuaNET.TypeMarshals
{
    public class ArrayTypeMarshal<T> : ILuaTypeMarshal where T : class
    {
        public object Get(GLua GLua, int stackPos = -1)
        {
            if (stackPos != -1)
                throw new System.Exception("stackpos not implemented on array marshal");

            if (GLua.IsType(stackPos, LuaType.Table))
            {
                return GLua.GetArray<T>();
            }
            return null;
        }

        public void Push(GLua GLua, object obj)
        {
            if (obj is T[] val)
            {
                GLua.SetArray(val);
            }
        }
    }
}
