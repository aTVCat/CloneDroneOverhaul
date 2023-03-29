using LuaInterface;
using System.IO;

namespace CDOverhaul.LevelEditor
{
    /// <summary>
    /// Currently, it is not necessary to do this now
    /// </summary>
    public class LevelEditorLuaController : OverhaulController
    {
        public override void Initialize()
        {
        }

        protected override void OnDisposed()
        {
        }

        /// <summary>
        /// Open and execute Lua file that is in mod assets directory
        /// </summary>
        /// <param name="luaFile"></param>
        public void RunLua(string luaFile)
        {
            string path = GetPath(luaFile, out bool fileExists);
            if (!fileExists)
            {
                OverhaulDebugger.PrintError("Lua file named " + luaFile + " not found! (" + path + ")");
                return;
            }

            Lua lua = new Lua();
            _ = lua.DoFile(path);
        }

        /// <summary>
        /// Get path to Lua file under mod assets folder
        /// </summary>
        /// <param name="luaFilename"></param>
        /// <param name="exists"></param>
        /// <returns></returns>
        public string GetPath(string luaFilename, out bool exists)
        {
            string path = OverhaulMod.Core.ModDirectory + "Assets/Lua/" + luaFilename + ".lua";
            exists = File.Exists(path);
            return path;
        }

        public override string[] Commands()
        {
            throw new System.NotImplementedException();
        }
        public override string OnCommandRan(string[] command)
        {
            throw new System.NotImplementedException();
        }
    }
}