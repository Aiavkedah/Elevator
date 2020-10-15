using System;
using System.Collections.Generic;
using System.Linq;

namespace Elevator
{
    class Program
    {
        public enum Destination
        {
            Up,
            Down
        }

        public class Passenger
        {
            public int FloorEnter { get; private set; }
            public int FloorExit { get; private set; }
            public Destination Destination { get; private set; }

            public Passenger(int currentFloor, int floors)
            {
                FloorEnter = currentFloor;
                (Destination, FloorExit) = RandomFloor(currentFloor, floors);
            }

            public void SetRouteFloor(int currentFloor, int floors)
            {
                FloorEnter = FloorExit;
                (Destination, FloorExit) = RandomFloor(currentFloor, floors);
            }

            public (Destination, int) RandomFloor(int currentFloor, int floors)
            {
                Destination destination;
                Random random = new Random();

                if (currentFloor == 1)
                    destination = Destination.Up;
                else if (currentFloor == floors)
                    destination = Destination.Down;
                else
                    destination = (Destination)random.Next(Enum.GetNames(typeof(Destination)).Length);

                return (destination, random.Next(destination == Destination.Up ? currentFloor + 1 : 1, destination == Destination.Up ? floors + 1 : currentFloor));
            }
        }

        public class Building
        {
            readonly Random random = new Random();
            readonly List<Passenger> Passengers = new List<Passenger>();
            readonly Elevator Elevator = new Elevator();

            public int Floors { get; private set; }

            public Building()
            {
                Floors = random.Next(5, 21);
            }

            public void InitPassengers()
            {
                for (int floorEnter = 1; Floors >= floorEnter; floorEnter++)
                {
                    int k = random.Next(0, 11);

                    for (int i = 1; k >= i; i++)
                    {
                        Passengers.Add(new Passenger(floorEnter, Floors));
                    }
                }
            }

            public void StartElevator()
            {
                Elevator.Start(Floors, Passengers);
            }
        }

        public class Elevator
        {
            const int Capacity = 5;
            Destination Destination = Destination.Up;
            int Floor = 1;
            int FreeCapacity;
            readonly List<Passenger> InElevator = new List<Passenger>();
            List<Passenger> OutElevator = new List<Passenger>();

            public void Start(int floors, List<Passenger> passengers)
            {
                while (passengers.Count() > 0)
                {
                    if (floors == Floor)
                        Destination = Destination.Down;
                    else if (Floor == 1)
                        Destination = Destination.Up;

                    //check if passengers out
                    if (InElevator.Where(i => i.FloorExit == Floor).Count() > 0)
                    {
                        OutElevator = InElevator.Where(i => i.FloorExit == Floor).ToList();
                        InElevator.RemoveAll(i => OutElevator.Any(k => k.GetHashCode() == i.GetHashCode()));
                    }

                    FreeCapacity = Capacity - InElevator.Count();
                    //is capacity free for new passengers
                    if (FreeCapacity > 0)
                    {
                        if (InElevator.Count() == 0 && passengers.Where(i => i.FloorEnter == Floor).Count() > 0
                            && passengers.Where(i => i.FloorEnter == Floor && i.Destination == (Destination == Destination.Up ? Destination.Down : Destination.Up)).Count() > 
                            passengers.Where(i => i.FloorEnter == Floor && i.Destination == Destination).Count())
                            passengersToLift(true);
                        else
                            passengersToLift();
                    }

                    //set new floor for out passengers
                    if (OutElevator.Count() > 0)
                    {
                        passengers.Where(i => OutElevator.Any(k => k.GetHashCode() == i.GetHashCode())).ToList().ForEach(i => i.SetRouteFloor(Floor, floors));
                        OutElevator.Clear();
                    }

                    //route reverse
                    if (floors != Floor && Floor != 1 && InElevator.Count() == 0)
                    {
                        if (Destination == Destination.Up && passengers.Where(i => i.FloorEnter > Floor).Count() == 0)
                            Destination = Destination.Down;
                        else if (Destination == Destination.Down && passengers.Where(i => i.FloorEnter < Floor).Count() == 0)
                            Destination = Destination.Up;
                    }

                    if (Destination == Destination.Up)
                        Floor++;
                    else
                        Floor--;
                }

                void passengersToLift(bool reverse = false)
                {
                    if (reverse)
                        Destination = Destination == Destination.Up ? Destination.Down : Destination.Up;

                    InElevator.AddRange(passengers.Where(i => i.FloorEnter == Floor && i.Destination == Destination).Take(FreeCapacity).ToList());
                }
            }
        }

        static void Main()
        {
            Building building = new Building();

            building.InitPassengers();
            building.StartElevator();
        }
    }
}

