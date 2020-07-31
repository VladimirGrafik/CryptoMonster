using System;
using Components;

namespace Thinker
{
    internal class Process2 : ProHub
    {
        private Process2(string[] args) : base(args)
        {
        }

        public static void Main(string[] args)
        {
            var runStatic = new Process2(args);
            if (!SistemSettings.EnableThinker) throw new Exception("Thinker is desibled!");
            try
            {
                var thinker = new Thinker();
                thinker.RunThinker();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}