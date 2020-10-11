using System;
using System.Collections.Generic;
using System.Linq;

namespace Elevator
{
    class Program
    {
        public class Passenger
        {
            public int FloorEnter { get; private set; }
            public int FloorExit { get; private set; }
            public bool IsUp { get; private set; }

            public Passenger(int currentFloor, int floors)
            {
                FloorEnter = currentFloor;
                (IsUp, FloorExit) = RandomFloor(currentFloor, floors);
            }

            public void SetRouteFloor(int currentFloor, int floors)
            {
                FloorEnter = FloorExit;
                (IsUp, FloorExit) = RandomFloor(currentFloor, floors);
            }

            public (bool, int) RandomFloor(int currentFloor, int floors)
            {
                bool isUp;
                Random random = new Random();

                if (currentFloor == 1)
                    isUp = true;
                else if (currentFloor == floors)
                    isUp = false;
                else
                    isUp = random.Next(100) < 50;

                return (isUp, random.Next(isUp ? currentFloor + 1 : 1, isUp ? floors + 1 : currentFloor));
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
            bool IsUp = true;
            int Floor = 1;
            int FreeCapacity;
            readonly List<Passenger> InElevator = new List<Passenger>();
            List<Passenger> OutElevator = new List<Passenger>();

            public void Start(int floors, List<Passenger> passengers)
            {
                while (passengers.Count() > 0)
                {
                    if (floors == Floor)
                        IsUp = false;
                    else if (Floor == 1)
                        IsUp = true;

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
                            && passengers.Where(i => i.FloorEnter == Floor && i.IsUp == !IsUp).Count() > passengers.Where(i => i.FloorEnter == Floor && i.IsUp == IsUp).Count())
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
                        if (IsUp && passengers.Where(i => i.FloorEnter > Floor).Count() == 0)
                            IsUp = false;
                        else if (!IsUp && passengers.Where(i => i.FloorEnter < Floor).Count() == 0)
                            IsUp = true;
                    }

                    if (IsUp)
                        Floor++;
                    else
                        Floor--;
                }

                void passengersToLift(bool reverse = false)
                {
                    if (reverse)
                        IsUp = !IsUp;

                    InElevator.AddRange(passengers.Where(i => i.FloorEnter == Floor && i.IsUp == IsUp).Take(FreeCapacity).ToList());
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

