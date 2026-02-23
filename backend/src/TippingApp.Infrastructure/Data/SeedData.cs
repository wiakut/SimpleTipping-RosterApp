using TippingApp.Domain.Entities;

namespace TippingApp.Infrastructure.Data;

public static class SeedData
{
    private static readonly Random Rng = new(42);

    public static void Initialize(AppDbContext context)
    {
        if (context.Employees.Any())
            return;

        var employees = CreateEmployees();
        context.Employees.AddRange(employees);
        context.SaveChanges();

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var currentMonday = today.AddDays(-(int)today.DayOfWeek + (int)DayOfWeek.Monday);
        if (today.DayOfWeek == DayOfWeek.Sunday)
            currentMonday = today.AddDays(-6);

        var shifts = GenerateShifts(employees, currentMonday, today);
        context.Shifts.AddRange(shifts);

        var tips = GenerateTips(currentMonday, today);
        context.TipEntries.AddRange(tips);

        context.SaveChanges();
    }

    private static List<Employee> CreateEmployees()
    {
        return
        [
            new() { Name = "Emma Richardson",  Role = "Senior Waitress" },
            new() { Name = "James O'Brien",    Role = "Head Bartender" },
            new() { Name = "Sofia Martinez",   Role = "Waitress" },
            new() { Name = "Liam Chen",        Role = "Waiter" },
            new() { Name = "Olivia Taylor",    Role = "Hostess" },
            new() { Name = "Noah Patel",       Role = "Barback" },
            new() { Name = "Ava Williams",     Role = "Waitress" },
            new() { Name = "Ethan Kowalski",   Role = "Bartender" },
            new() { Name = "Mia Thompson",     Role = "Junior Waitress" },
            new() { Name = "Lucas Andersson",  Role = "Floor Manager" },
        ];
    }

    private static List<Shift> GenerateShifts(List<Employee> employees, DateOnly currentMonday, DateOnly today)
    {
        var shifts = new List<Shift>();

        for (var weekOffset = 7; weekOffset >= 0; weekOffset--)
        {
            var monday = currentMonday.AddDays(-7 * weekOffset);

            foreach (var emp in employees)
            {
                var weekShifts = GenerateWeekShiftsForEmployee(emp, monday, today, weekOffset);
                shifts.AddRange(weekShifts);
            }
        }

        return shifts;
    }

    private static List<Shift> GenerateWeekShiftsForEmployee(
        Employee emp, DateOnly monday, DateOnly today, int weekOffset)
    {
        var shifts = new List<Shift>();
        var profile = GetScheduleProfile(emp.Name);

        for (var d = 0; d < 7; d++)
        {
            var date = monday.AddDays(d);
            var dow = date.DayOfWeek;

            if (weekOffset == 0 && date > today)
                continue;

            if (!ShouldWork(profile, dow, weekOffset))
                continue;

            var (start, end) = GetShiftTimes(profile, dow);
            shifts.Add(new Shift
            {
                Employee = emp,
                Date = date,
                StartTime = start,
                EndTime = end
            });
        }

        return shifts;
    }

    private static ScheduleProfile GetScheduleProfile(string name) => name switch
    {
        "Emma Richardson" => new(
            WorkDays: [DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday, DayOfWeek.Saturday],
            DayOff: DayOfWeek.Sunday,
            PreferredShift: ShiftType.Morning,
            IsFullTime: true,
            SkipChance: 0.08),

        "James O'Brien" => new(
            WorkDays: [DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday, DayOfWeek.Saturday],
            DayOff: DayOfWeek.Sunday,
            PreferredShift: ShiftType.Evening,
            IsFullTime: true,
            SkipChance: 0.05),

        "Sofia Martinez" => new(
            WorkDays: [DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday],
            DayOff: DayOfWeek.Saturday,
            PreferredShift: ShiftType.Morning,
            IsFullTime: true,
            SkipChance: 0.10),

        "Liam Chen" => new(
            WorkDays: [DayOfWeek.Thursday, DayOfWeek.Friday, DayOfWeek.Saturday, DayOfWeek.Sunday],
            DayOff: null,
            PreferredShift: ShiftType.Evening,
            IsFullTime: false,
            SkipChance: 0.12),

        "Olivia Taylor" => new(
            WorkDays: [DayOfWeek.Monday, DayOfWeek.Wednesday, DayOfWeek.Friday, DayOfWeek.Saturday, DayOfWeek.Sunday],
            DayOff: null,
            PreferredShift: ShiftType.Morning,
            IsFullTime: false,
            SkipChance: 0.10),

        "Noah Patel" => new(
            WorkDays: [DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday, DayOfWeek.Saturday],
            DayOff: null,
            PreferredShift: ShiftType.Evening,
            IsFullTime: false,
            SkipChance: 0.15),

        "Ava Williams" => new(
            WorkDays: [DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday, DayOfWeek.Saturday],
            DayOff: DayOfWeek.Sunday,
            PreferredShift: ShiftType.FullDay,
            IsFullTime: true,
            SkipChance: 0.08),

        "Ethan Kowalski" => new(
            WorkDays: [DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday, DayOfWeek.Saturday, DayOfWeek.Sunday],
            DayOff: null,
            PreferredShift: ShiftType.Evening,
            IsFullTime: false,
            SkipChance: 0.10),

        "Mia Thompson" => new(
            WorkDays: [DayOfWeek.Tuesday, DayOfWeek.Thursday, DayOfWeek.Saturday],
            DayOff: null,
            PreferredShift: ShiftType.Morning,
            IsFullTime: false,
            SkipChance: 0.18),

        "Lucas Andersson" => new(
            WorkDays: [DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday, DayOfWeek.Saturday, DayOfWeek.Sunday],
            DayOff: null,
            PreferredShift: ShiftType.FullDay,
            IsFullTime: true,
            SkipChance: 0.04),

        _ => throw new ArgumentException($"Unknown employee: {name}")
    };

    private static bool ShouldWork(ScheduleProfile profile, DayOfWeek dow, int weekOffset)
    {
        if (profile.DayOff == dow)
            return false;

        if (!profile.WorkDays.Contains(dow))
            return false;

        var hash = HashCode.Combine(dow, weekOffset, profile.GetHashCode());
        var chance = (uint)hash % 100;
        return chance >= (int)(profile.SkipChance * 100);
    }

    private static (TimeOnly start, TimeOnly end) GetShiftTimes(ScheduleProfile profile, DayOfWeek dow)
    {
        var minuteJitter = (Rng.Next(0, 4)) * 15;

        return profile.PreferredShift switch
        {
            ShiftType.Morning => (
                new TimeOnly(9, 0).AddMinutes(minuteJitter),
                new TimeOnly(15, 0).AddMinutes(Rng.Next(0, 3) * 30)),

            ShiftType.Evening => (
                new TimeOnly(15, 0).AddMinutes(minuteJitter),
                dow is DayOfWeek.Friday or DayOfWeek.Saturday
                    ? new TimeOnly(23, 0).AddMinutes(Rng.Next(0, 2) * 30)
                    : new TimeOnly(22, 0).AddMinutes(Rng.Next(0, 3) * 15)),

            ShiftType.FullDay => (
                new TimeOnly(10, 0).AddMinutes(minuteJitter),
                dow is DayOfWeek.Friday or DayOfWeek.Saturday
                    ? new TimeOnly(19, 0).AddMinutes(Rng.Next(0, 3) * 30)
                    : new TimeOnly(18, 0).AddMinutes(Rng.Next(0, 2) * 30)),

            _ => (new TimeOnly(10, 0), new TimeOnly(18, 0))
        };
    }

    private static List<TipEntry> GenerateTips(DateOnly currentMonday, DateOnly today)
    {
        var tips = new List<TipEntry>();

        for (var weekOffset = 7; weekOffset >= 0; weekOffset--)
        {
            var monday = currentMonday.AddDays(-7 * weekOffset);
            var weekMultiplier = GetWeekMultiplier(weekOffset);

            for (var d = 0; d < 7; d++)
            {
                var date = monday.AddDays(d);
                if (weekOffset == 0 && date > today)
                    continue;

                var dayTips = GenerateDayTips(date, weekMultiplier);
                tips.AddRange(dayTips);
            }
        }

        return tips;
    }

    private static double GetWeekMultiplier(int weekOffset) => weekOffset switch
    {
        7 => 0.85,
        6 => 0.92,
        5 => 1.15,
        4 => 1.0,
        3 => 0.95,
        2 => 1.25,
        1 => 1.08,
        0 => 1.0,
        _ => 1.0
    };

    private static List<TipEntry> GenerateDayTips(DateOnly date, double weekMultiplier)
    {
        var tips = new List<TipEntry>();
        var dow = date.DayOfWeek;

        var (baseMin, baseMax) = dow switch
        {
            DayOfWeek.Monday => (55.0, 130.0),
            DayOfWeek.Tuesday => (60.0, 140.0),
            DayOfWeek.Wednesday => (70.0, 155.0),
            DayOfWeek.Thursday => (80.0, 170.0),
            DayOfWeek.Friday => (140.0, 320.0),
            DayOfWeek.Saturday => (160.0, 380.0),
            DayOfWeek.Sunday => (90.0, 220.0),
            _ => (70.0, 150.0)
        };

        var cardAmount = Math.Round(
            (decimal)(baseMin + Rng.NextDouble() * (baseMax - baseMin)) * (decimal)weekMultiplier,
            2);
        var jitter = (decimal)(Rng.NextDouble() * 10 - 5);
        cardAmount = Math.Max(20m, cardAmount + jitter);
        var cents = Rng.Next(0, 20) * 5;
        cardAmount = Math.Floor(cardAmount) + cents / 100m;

        tips.Add(new TipEntry { Date = date, Amount = cardAmount, Source = "Card tips" });

        var cashRatio = 0.3 + Rng.NextDouble() * 0.35;
        var cashAmount = Math.Round(cardAmount * (decimal)cashRatio, 2);
        var cashCents = Rng.Next(0, 10) * 10;
        cashAmount = Math.Floor(cashAmount) + cashCents / 100m;

        tips.Add(new TipEntry { Date = date, Amount = cashAmount, Source = "Cash tips" });

        if (dow == DayOfWeek.Sunday)
        {
            var brunchAmount = (decimal)(40 + Rng.NextDouble() * 80);
            brunchAmount = Math.Floor(brunchAmount) + Rng.Next(0, 20) * 5 / 100m;
            tips.Add(new TipEntry { Date = date, Amount = Math.Round(brunchAmount, 2), Source = "Brunch tips" });
        }

        var privateEventHash = HashCode.Combine(date.Year, date.Month, date.Day, 0xCAFE);
        if (((uint)privateEventHash % 100) < 4)
        {
            var eventAmount = (decimal)(250 + Rng.NextDouble() * 300);
            eventAmount = Math.Floor(eventAmount) + Rng.Next(0, 4) * 25 / 100m;
            tips.Add(new TipEntry { Date = date, Amount = Math.Round(eventAmount, 2), Source = "Private event gratuity" });
        }

        return tips;
    }

    private record ScheduleProfile(
        DayOfWeek[] WorkDays,
        DayOfWeek? DayOff,
        ShiftType PreferredShift,
        bool IsFullTime,
        double SkipChance);

    private enum ShiftType { Morning, Evening, FullDay }
}
