using System;
using Components;

namespace Hunter
{
    internal class Process1 : ProHub
    {
        private Process1(string[] args) : base(args)
        {
        }

        public static void Main(string[] args)
        {
            var runStatic = new Process1(args);
            if (!SistemSettings.EnableHunter) throw new Exception("Hunter is desibled!");
            var catchPairs = ProHub.GetCatchPairs();
            try
            {
                var hunter = new Hunter(catchPairs);
                hunter.RunHunter();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}