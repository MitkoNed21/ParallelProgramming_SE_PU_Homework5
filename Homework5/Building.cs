using System;
using System.Collections.Generic;
using static Homework5.TODOClass;

namespace Homework5
{
    public class Building
    {
        public string Name { get; set; }
        public Floor[] Floors { get; }
        public Elevator Elevator { get; private set; }
        public List<Agent> WorkingAgents { get; } = new List<Agent>();

        public Building(string name, int numberOfFloors)
        {
            if (numberOfFloors < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(numberOfFloors), "A building has to have at least 1 floor!");
            }

            this.Name = name;
            this.Floors = new Floor[numberOfFloors];
            this.Elevator = new Elevator(building: this);
        }

        public void AddFloors(Floor[] floors)
        {
            if (floors.Length != this.Floors.Length)
            {
                throw new ArgumentException(nameof(floors), $"Floors count must be {this.Floors.Length} " +
                    $"but count of floors to add is {floors.Length}!");
            }

            for (int i = 0; i < this.Floors.Length; i++)
            {
                this.Floors[i] = floors[i];
            }

        }

        public void RegisterAgentStartedWorking(Agent agent)
        {
            if (this.WorkingAgents.Contains(agent))
            {
                Console.WriteLine($"ERROR ON AGENT {agent.Name}.");
                throw new ArgumentException($"{agent.Name} is already registered as working!", nameof(agent));
            }

            this.WorkingAgents.Add(agent);
        }

        public void RegisterAgentLeftWork(Agent agent)
        {
            if (!this.WorkingAgents.Contains(agent))
            {
                throw new ArgumentException($"{agent.Name} is already registered as left work!", nameof(agent));
            }

            this.WorkingAgents.Remove(agent);
        }
    }
}