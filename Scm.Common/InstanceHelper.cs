﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scm.Common
{
    public class InstanceHelper
    {
        public static Instance LoadInstance(string filename)
        {
            if (!File.Exists(filename))
            {
                return null;
            }

            var content = File.ReadAllLines(filename);
            var instance = new Instance();

            // Parse first line (type, no. of vehicles, no. of customers, no. if depots)
            var firstLineSplitted = content[0].Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);
            instance.Vehicles = int.Parse(firstLineSplitted[1]);
            instance.Customers = int.Parse(firstLineSplitted[2]);
            instance.Depots = int.Parse(firstLineSplitted[3]);

            var offset = 1; // after reading first line

            // per depot (vehicle load)
            var vehicleName = 1;
            for (var i = offset; i < offset + instance.Depots; i++)
            {
                var vehicleLineSplitted = content[i].Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);
                instance.VehicleList.Add(new Vehicle 
                    {
                        Name = (vehicleName++).ToString(), 
                        LoadCapacity = int.Parse(vehicleLineSplitted[1])
                    });
            }
            offset += instance.Depots;

            // per customer
            for (var i = offset; i < offset + instance.Customers; i++)
            {
                var customerLineSplitted = content[i].Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);
                instance.CustomerList.Add(new Customer
                    {
                        Name = (customerLineSplitted[0]),
                        Point = new Point(int.Parse(customerLineSplitted[1]), int.Parse(customerLineSplitted[2])),
                        Demand = int.Parse(customerLineSplitted[4])
                    });
            }
            offset += instance.Customers;

            // per depot
            for (var i = offset; i < offset + instance.Depots; i++)
            {
                var depotLineSplitted = content[i].Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                instance.DepotList.Add(new Depot
                {
                    Name = (depotLineSplitted[0]),
                    Point = new Point(int.Parse(depotLineSplitted[1]), int.Parse(depotLineSplitted[2]))
                });
            }

            return instance;
        }

        public static InstanceSolution LoadInstanceSolution(string filename)
        {
            if (!File.Exists(filename))
            {
                return null;
            }

            var content = File.ReadAllLines(filename);
            var instance = new InstanceSolution();

            for (var i = 0; i < content.Length; i++)
            {
                var trail = new Trail();
                var splittedLine = content[i].Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                if (i == 0)
                {
                    instance.TotalCosts = double.Parse(splittedLine[0]);
                    continue;
                }

                trail.DepotName = splittedLine[0];
                trail.VehicleNumber = splittedLine[1];
                trail.Costs = double.Parse(splittedLine[2]);
                trail.VehicleLoad = int.Parse(splittedLine[3]);

                var route = new List<string>();
                for (var j=4; j<splittedLine.Length; j++)
                {
                    route.Add(splittedLine[j]);
                }

                //for (var j = route.IndexOf("0"); j != -1; j = route.IndexOf("0"))
                //{
                //    route[j] = trail.DepotName;
                //}
                trail.Route = route;
                instance.Trails.Add(trail);
            }

            return instance;
        }
    }

    public class Instance
    {
        public int Vehicles { get; set; }
        public int Customers { get; set; }
        public int Depots { get; set; }

        public int MaxVehicleLoad { get; set; }

        public List<Vehicle> VehicleList { get; set; }
        public List<Customer> CustomerList { get; set; }
        public List<Depot> DepotList { get; set; }

        public Instance()
        {
            VehicleList = new List<Vehicle>();
            CustomerList = new List<Customer>();
            DepotList = new List<Depot>();
        }
    }

    public class Customer
    {
        public string Name { get; set; }
        public Point Point { get; set; }
        public int Demand { get; set; }
    }

    public class Vehicle
    {
        public string Name { get; set; }
        public int LoadCapacity { get; set; }
    }

    public class Depot
    {
        public string Name { get; set; }
        public Point Point {get;set;}
    }

    public class InstanceSolution
    {
        public List<Trail> Trails { get; set; }
        public double TotalCosts { get; set; }

        public InstanceSolution()
        {
            Trails = new List<Trail>();
        }
    }

    public class Trail
    {
        public string DepotName { get; set; }
        public string VehicleNumber { get; set; }
        public double Costs { get; set; }
        public int VehicleLoad { get; set; }
        public List<string> Route { get; set; }
    }
}
