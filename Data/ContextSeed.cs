using Microsoft.AspNetCore.Identity;
using TrainTickets.Enums;
using TrainTickets.Models;

namespace TrainTickets.Data
{
    public class ContextSeed
    {
        public static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            foreach (string role in Enum.GetNames(typeof(Role)))
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }

        public static async Task SeedAdminAsync(UserManager<ApplicationUser> userManager)
        {
            ApplicationUser defaultUser = new()
            {
                UserName = "Admin",
                Email = "admin@gmail.com",
                FirstName = "Yoana",
                MiddleName = "Kalinova",
                LastName = "Neycheva",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true
            };

            ApplicationUser foundUser = await userManager.FindByEmailAsync(defaultUser.Email);

            if (foundUser == null)
            {
                await userManager.CreateAsync(defaultUser, "P@ss123");

                await userManager.AddToRoleAsync(defaultUser, Role.Client.ToString());
                await userManager.AddToRoleAsync(defaultUser, Role.Admin.ToString());
            }
        }
        public static async Task SeedInfoAsync(ApplicationDbContext context)
        {
            if (!context.Stations.Any())
            {
                context.Stations.AddRange(
                    new Station { Name = "София Север", City = "София" },
                    new Station { Name = "Централна гара София", City = "София" },
                    new Station { Name = "Пловдив", City = "Пловдив" },
                    new Station { Name = "Варна", City = "Варна" },
                    new Station { Name = "Бургас", City = "Бургас" },
                    new Station { Name = "Русе", City = "Русе" },
                    new Station { Name = "Стара Загора", City = "Стара Загора" },
                    new Station { Name = "Велико Търново", City = "Велико Търново" },
                    new Station { Name = "Горна Оряховица", City = "Горна Оряховица" },
                    new Station { Name = "Мезда", City = "Мезда" },
                    new Station { Name = "Димитровград", City = "Димитровград" },
                    new Station { Name = "Сливен", City = "Сливен" },
                    new Station { Name = "Видин", City = "Видин" },
                    new Station { Name = "Кърджали", City = "Кърджали" },
                    new Station { Name = "Габрово", City = "Габрово" }
                );
            }

            if (!context.Trains.Any())
            {
                context.Trains.AddRange(
                    new Train
                    {
                        Name = "БВ 2611",
                        Type = TrainType.Fast,
                        Capacity = 120
                    },
                    new Train
                    {
                        Name = "БВ 7821",
                        Type = TrainType.Fast,
                        Capacity = 110
                    },
                    new Train
                    {
                        Name = "ПВ 34567",
                        Type = TrainType.Passenger,
                        Capacity = 140
                    },
                    new Train
                    {
                        Name = "ПВ 91011",
                        Type = TrainType.Passenger,
                        Capacity = 70
                    },
                    new Train
                    {
                        Name = "БВ 5678",
                        Type = TrainType.Fast,
                        Capacity = 50
                    },
                    new Train
                    {
                        Name = "БВ 3456",
                        Type = TrainType.Fast,
                        Capacity = 70
                    },
                    new Train
                    {
                        Name = "ПВ 17890",
                        Type = TrainType.Passenger,
                        Capacity = 60
                    },
                    new Train
                    {
                        Name = "ПВ 23456",
                        Type = TrainType.Passenger,
                        Capacity = 20
                    },
                    new Train
                    {
                        Name = "БВ 1234",
                        Type = TrainType.Fast,
                        Capacity = 10
                    },
                    new Train
                    {
                        Name = "ПВ 40123",
                        Type = TrainType.Passenger,
                        Capacity = 80
                    },
                    new Train
                    {
                        Name = "ПВ 12345",
                        Type = TrainType.Passenger,
                        Capacity = 100
                    },
                    new Train
                    {
                        Name = "ПВ 87654",
                        Type = TrainType.Passenger,
                        Capacity = 90
                    },
                    new Train
                    {
                        Name = "ПВ 43216",
                        Type = TrainType.Passenger,
                        Capacity = 110
                    },
                    new Train
                    {
                        Name = "ПВ 12346",
                        Type = TrainType.Passenger,
                        Capacity = 100
                    },
                    new Train
                    {
                        Name = "ПВ 56789",
                        Type = TrainType.Passenger,
                        Capacity = 90
                    }
                );
            }
            if (!context.Trips.Any())
            {
                context.Trips.AddRange(
                    new Trip
                    {
                        TrainId = 2,
                        EndStationId = 3,
                        StartStationId = 1,
                        DepartureTime = new DateTime(2026, 5, 10, 9, 00, 0),
                        ArrivalTime = new DateTime(2026, 5, 10, 11, 00, 0),
                        Price = 32.00
                    },
                    new Trip
                    {
                        TrainId = 1,
                        EndStationId = 3,
                        StartStationId = 2,
                        DepartureTime = new DateTime(2026, 6, 10, 8, 30, 0),
                        ArrivalTime = new DateTime(2026, 6, 10, 10, 30, 0),
                        Price = 28.00
                    },
                    new Trip
                    {
                        TrainId = 7,
                        EndStationId = 3,
                        StartStationId = 4,
                        DepartureTime = new DateTime(2026, 5, 21, 12, 20, 0),
                        ArrivalTime = new DateTime(2026, 5, 21, 15, 00, 0),
                        Price = 20.00
                    },
                    new Trip
                    {
                        TrainId = 8,
                        EndStationId = 5,
                        StartStationId = 9,
                        DepartureTime = new DateTime(2026, 5, 19, 12, 20, 0),
                        ArrivalTime = new DateTime(2026, 5, 19, 14, 10, 0),
                        Price = 23.50
                    },
                    new Trip
                    {
                        TrainId = 9,
                        EndStationId = 8,
                        StartStationId = 9,
                        DepartureTime = new DateTime(2026, 3, 28, 9, 45, 0),
                        ArrivalTime = new DateTime(2026, 3, 28, 13, 00, 0),
                        Price = 17.60
                    },
                    new Trip
                    {
                        TrainId = 3,
                        EndStationId = 8,
                        StartStationId = 10,
                        DepartureTime = new DateTime(2026, 6, 12, 10, 40, 0),
                        ArrivalTime = new DateTime(2026, 6, 12, 13, 20, 0),
                        Price = 30.50
                    },
                    new Trip
                    {
                        TrainId = 12,
                        EndStationId = 9,
                        StartStationId = 7,
                        DepartureTime = new DateTime(2026, 6, 17, 22, 45, 0),
                        ArrivalTime = new DateTime(2026, 6, 18, 1, 20, 0),
                        Price = 16.30
                    },
                    new Trip
                    {
                        TrainId = 14,
                        EndStationId = 11,
                        StartStationId = 12,
                        DepartureTime = new DateTime(2026, 6, 5, 8, 00, 0),
                        ArrivalTime = new DateTime(2026, 6, 5, 9, 00, 0),
                        Price = 12.80
                    },
                    new Trip
                    {
                        TrainId = 10,
                        EndStationId = 1,
                        StartStationId = 3,
                        DepartureTime = new DateTime(2026, 7, 6, 7, 10, 0),
                        ArrivalTime = new DateTime(2026, 7, 6, 8, 50, 0),
                        Price = 15.50
                    },
                    new Trip
                    {
                        TrainId = 7,
                        EndStationId = 5,
                        StartStationId = 12,
                        DepartureTime = new DateTime(2026, 6, 9, 10, 15, 0),
                        ArrivalTime = new DateTime(2026, 6, 9, 11, 20, 0),
                        Price = 18.20
                    },
                    new Trip
                    {
                        TrainId = 11,
                        EndStationId = 13,
                        StartStationId = 14,
                        DepartureTime = new DateTime(2026, 6, 20, 14, 30, 0),
                        ArrivalTime = new DateTime(2026, 6, 20, 16, 00, 0),
                        Price = 22.00
                    },
                    new Trip
                    {
                        TrainId = 15,
                        EndStationId = 15,
                        StartStationId = 14,
                        DepartureTime = new DateTime(2026, 5, 25, 9, 00, 0),
                        ArrivalTime = new DateTime(2026, 5, 25, 10, 30, 0),
                        Price = 19.50
                    },
                    new Trip
                    {
                        TrainId = 2,
                        EndStationId = 14,
                        StartStationId = 15,
                        DepartureTime = new DateTime(2026, 5, 30, 21, 00, 0),
                        ArrivalTime = new DateTime(2026, 5, 30, 22, 45, 0),
                        Price = 14.70
                    },
                    new Trip
                    {
                        TrainId = 4,
                        EndStationId = 6,
                        StartStationId = 13,
                        DepartureTime = new DateTime(2026, 6, 3, 11, 20, 0),
                        ArrivalTime = new DateTime(2026, 6, 3, 13, 50, 0),
                        Price = 25.00
                    },
                    new Trip
                    {
                        TrainId = 5,
                        EndStationId = 4,
                        StartStationId = 11,
                        DepartureTime = new DateTime(2026, 6, 15, 15, 40, 0),
                        ArrivalTime = new DateTime(2026, 6, 15, 19, 30, 0),
                        Price = 27.80
                    },
                    new Trip
                    {
                        TrainId = 6,
                        EndStationId = 2,
                        StartStationId = 5,
                        DepartureTime = new DateTime(2026, 6, 18, 17, 00, 0),
                        ArrivalTime = new DateTime(2026, 6, 18, 20, 00, 0),
                        Price = 24.50
                    },
                    new Trip
                    {
                        TrainId = 3,
                        EndStationId = 7,
                        StartStationId = 8,
                        DepartureTime = new DateTime(2026, 6, 22, 19, 30, 0),
                        ArrivalTime = new DateTime(2026, 6, 22, 21, 00, 0),
                        Price = 18.90
                    },
                    new Trip
                    {
                        TrainId = 4,
                        EndStationId = 10,
                        StartStationId = 9,
                        DepartureTime = new DateTime(2026, 5, 28, 6, 00, 0),
                        ArrivalTime = new DateTime(2026, 5, 28, 8, 30, 0),
                        Price = 21.40
                    },
                    new Trip
                    {
                        TrainId = 5,
                        EndStationId = 12,
                        StartStationId = 13,
                        DepartureTime = new DateTime(2026, 6, 1, 13, 15, 0),
                        ArrivalTime = new DateTime(2026, 6, 1, 15, 45, 0),
                        Price = 26.00
                    },
                    new Trip
                    {
                        TrainId = 5,
                        EndStationId = 14,
                        StartStationId = 15,
                        DepartureTime = new DateTime(2026, 6, 5, 18, 00, 0),
                        ArrivalTime = new DateTime(2026, 6, 5, 20, 30, 0),
                        Price = 29.50
                    },
                    new Trip
                    {
                        TrainId = 7,
                        EndStationId = 3,
                        StartStationId = 2,
                        DepartureTime = new DateTime(2026, 7, 10, 7, 45, 0),
                        ArrivalTime = new DateTime(2026, 7, 10, 9, 30, 0),
                        Price = 31.20
                    },
                    new Trip
                    {
                        TrainId = 7,
                        EndStationId = 4,
                        StartStationId = 1,
                        DepartureTime = new DateTime(2026, 6, 15, 22, 20, 0),
                        ArrivalTime = new DateTime(2026, 6, 16, 3, 10, 0),
                        Price = 27.80
                    },
                    new Trip
                    {
                        TrainId = 8,
                        EndStationId = 5,
                        StartStationId = 6,
                        DepartureTime = new DateTime(2026, 6, 20, 14, 00, 0),
                        ArrivalTime = new DateTime(2026, 6, 20, 16, 30, 0),
                        Price = 22.50
                    },
                    new Trip
                    {
                        TrainId = 8,
                        EndStationId = 7,
                        StartStationId = 8,
                        DepartureTime = new DateTime(2026, 6, 25, 9, 00, 0),
                        ArrivalTime = new DateTime(2026, 6, 25, 11, 30, 0),
                        Price = 19.80
                    },
                    new Trip
                    {
                        TrainId = 8,
                        EndStationId = 7,
                        StartStationId = 8,
                        DepartureTime = new DateTime(2026, 4, 30, 20, 00, 0),
                        ArrivalTime = new DateTime(2026, 5, 1, 0, 30, 0),
                        Price = 24.00
                    },
                    new Trip
                    {
                        TrainId = 15,
                        EndStationId = 9,
                        StartStationId = 10,
                        DepartureTime = new DateTime(2026, 5, 5, 6, 30, 0),
                        ArrivalTime = new DateTime(2026, 5, 5, 9, 00, 0),
                        Price = 18.50
                    },
                    new Trip
                    {
                        TrainId = 15,
                        EndStationId = 12,
                        StartStationId = 11,
                        DepartureTime = new DateTime(2026, 5, 15, 17, 00, 0),
                        ArrivalTime = new DateTime(2026, 5, 15, 19, 30, 0),
                        Price = 17.00
                    },
                    new Trip
                    {
                        TrainId = 15,
                        EndStationId = 14,
                        StartStationId = 13,
                        DepartureTime = new DateTime(2026, 4, 5, 12, 00, 0),
                        ArrivalTime = new DateTime(2026, 4, 5, 14, 30, 0),
                        Price = 16.50
                    },
                    new Trip
                    {
                        TrainId = 10,
                        EndStationId = 15,
                        StartStationId = 14,
                        DepartureTime = new DateTime(2026, 5, 20, 8, 00, 0),
                        ArrivalTime = new DateTime(2026, 5, 20, 10, 30, 0),
                        Price = 21.00
                    },
                    new Trip
                    {
                        TrainId = 10,
                        EndStationId = 15,
                        StartStationId = 14,
                        DepartureTime = new DateTime(2026, 6, 25, 8, 00, 0),
                        ArrivalTime = new DateTime(2026, 6, 25, 10, 30, 0),
                        Price = 20.00
                    },
                    new Trip
                    {
                        TrainId = 10,
                        EndStationId = 15,
                        StartStationId = 14,
                        DepartureTime = new DateTime(2026, 7, 25, 8, 00, 0),
                        ArrivalTime = new DateTime(2026, 7, 25, 10, 30, 0),
                        Price = 19.00
                    },
                    new Trip
                    {
                        TrainId = 7,
                        EndStationId = 15,
                        StartStationId = 14,
                        DepartureTime = new DateTime(2026, 8, 25, 8, 00, 0),
                        ArrivalTime = new DateTime(2026, 8, 25, 10, 30, 0),
                        Price = 18.00
                    },
                    new Trip
                    {
                        TrainId = 6,
                        EndStationId = 5,
                        StartStationId = 4,
                        DepartureTime = new DateTime(2026, 6, 7, 9, 15, 0),
                        ArrivalTime = new DateTime(2026, 6, 7, 10, 50, 0),
                        Price = 17.00
                    },
                    new Trip
                    {
                        TrainId = 6,
                        EndStationId = 5,
                        StartStationId = 4,
                        DepartureTime = new DateTime(2026, 4, 2, 9, 15, 0),
                        ArrivalTime = new DateTime(2026, 4, 2, 10, 50, 0),
                        Price = 16.00
                    },
                    new Trip
                    {
                        TrainId = 7,
                        EndStationId = 5,
                        StartStationId = 4,
                        DepartureTime = new DateTime(2026, 3, 29, 9, 15, 0),
                        ArrivalTime = new DateTime(2026, 3, 29, 10, 50, 0),
                        Price = 18.00
                    }
                );
            }

            context.SaveChanges();
        }
    }
}
