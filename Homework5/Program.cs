using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Homework5
{
    internal class Program
    {
        public static List<Thread> agentThreads = new List<Thread>();
        public static List<Agent> agents = new List<Agent>();
        public static Random random = new Random();

        static void Main(string[] args)
        {
            var secretBase = new Building("Area 51", numberOfFloors: 4);

            secretBase.AddFloors(new[] {
                new Floor(0, name: "G", securityLevelRequirement: SecurityLevel.Confidential, building: secretBase),
                new Floor(1, name: "S", securityLevelRequirement: SecurityLevel.Secret, building: secretBase),
                new Floor(2, name: "T1", securityLevelRequirement: SecurityLevel.TopSecret, building: secretBase),
                new Floor(3, name: "T2", securityLevelRequirement: SecurityLevel.TopSecret, building: secretBase)
            });

            //Console.WriteLine(SecurityLevel.TopSecret >= SecurityLevel.Secret);
            //Console.WriteLine(SecurityLevel.TopSecret >= SecurityLevel.Confidential);
            //Console.WriteLine(SecurityLevel.Secret >= SecurityLevel.Confidential);
            //Console.WriteLine(SecurityLevel.TopSecret >= SecurityLevel.TopSecret);
            //Console.WriteLine(SecurityLevel.TopSecret > SecurityLevel.TopSecret);
            //return;

            //Task.Run(secretBase.Elevator.StartWorking);
            //secretBase.Floors[2].ElevatorDoor.CallElevatorButton.CallElevator();

            //Thread.Sleep(1000);

            //Console.WriteLine(secretBase.Elevator.CurrentFloor.Position);
            //return;

            var agentsCount = random.Next(5, 5); // 20, 41 // 30, 50
            for (int i = 0; i < agentsCount; i++)
            {
                // 1, 2, 3       -> 1 - 1 -> SecurityLevel int 0 (3/11 chance)
                // 4, 5, 6, 7, 8 -> 2 - 1 -> SecurityLevel int 1 (5/11 chance)
                // 9, 10, 11     -> 3 - 1 -> SecurityLevel int 2 (3/11 chance)
                var securityLevel = (SecurityLevel)(int)(Math.Sqrt(random.Next(1, 12)) - 1);

                var agent = new Agent(secretBase, $"Agent {i:D2}", securityLevel);
                agents.Add(agent);
                agentThreads.Add(new Thread(agent.DoThingsAtWork));
            }

            var elevatorThread = new Thread(secretBase.Elevator.StartWorking);

            elevatorThread.Start();

            foreach (var thread in agentThreads)
            {
                thread.Start();
            }

            foreach (var thread in agentThreads)
            {
                thread.Join();
            }
            elevatorThread.Join();


            // TODO: Add agents threads, make floors, add floors, make elevator thread, start threads, test



            Console.WriteLine("Hello World!");
        }
    }

    public static class TODOClass
    {
        public static void TODO(string message = "")
        {
            throw new NotImplementedException(message);
        }
    }
}
