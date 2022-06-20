namespace Messenger
{
    // test
    class Program
    {
        //static TickManager.TickManager tick_manager = new TickManager.TickManager();
        //static Messenger messenger = new Messenger();

        static void Main(string[] args)
        {
            //messenger.Init();

            //tick_manager.Frequency = 200; // For 200Hz
            //tick_manager.OnTick += Tick_manager_OnTick;

            while (true)
            {
                //messenger.Handle();
                //tick_manager.Tick();
                //Thread.Sleep(tick_manager.TimeRemaining()); // don't use this, Thread.Sleep takes longer than TimeRemaining() return value
            }
        }

        private static void Tick_manager_OnTick()
        {
            /*Console.Write("MSPT: ");
            Console.WriteLine(tick_manager.MSPT);

            Console.Write("TPS: ");
            Console.WriteLine(tick_manager.TPS);*/

            //messenger.Handle();

            // here we will test stuff and things
        }
    }
}