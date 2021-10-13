using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;

namespace Waterdrop.Data
{
    /// <summary>
    /// Day item data model.
    /// </summary>
    public class Days : BaseNotifier
    {
        public Days(DateTime date, int goal)
        {
            Date = date;
            Amount = 0;
            Goal = goal;
        }

        public Days()
        {
            // Empty constructor
        }

        [AutoIncrement, PrimaryKey]
        public int Id { get; set; }

        private DateTime _date;
        public DateTime Date
        {
            get
            {
                return _date;
            }
            set
            {
                Set(ref _date, value);
            }
        }

        private int _amount;
        public int Amount
        {
            get
            {
                return _amount;
            }
            set
            {
                Set(ref _amount, value);
                OnPropertyChanged(nameof(ProgressStr));
                OnPropertyChanged(nameof(FullInfo));
            }
        }

        public int _goal;
        public int Goal
        {
            get
            {
                return _goal;
            }
            set
            {
                Set(ref _goal, value);
            }
        }

        public double Progress => 100 / (double)Goal * Amount;
        public string ProgressStr => Progress.ToString() + "%";
        public string FullInfo => Amount.ToString() + "ml/" + Goal.ToString() + "ml - " + Progress.ToString() + "%";

        public override string ToString() => Progress.ToString() + "%";
    }

    /// <summary>
    /// Creates a collection of groups and items with content read from a database.
    /// </summary>
    public class HydrationDataSource : BaseNotifier
    {
        private static SQLiteConnection dbConn;
        public ObservableCollection<Days> AllDays { get; set; }
            = new ObservableCollection<Days>();

        public HydrationDataSource()
        {
            dbConn = new SQLiteConnection(App.DB_PATH);
            dbConn.CreateTable<Days>();
            GetDays();
        }

        public void DrinkWater(int amount)
        {
            AllDays[0].Amount += amount;
            UpsertDay(AllDays[0]);
        }

        // Retrieve the specific day from the database.
        public Days ReadDay(int id)
        {
            var day = dbConn.Query<Days>("SELECT * FROM Days where Id = " + id).FirstOrDefault();
            return day;
        }

        // Retrieve the all day list from the database.
        public void GetDays()
        {
            IEnumerable<Days> days = dbConn.Table<Days>().ToList();

            if (days.Count() == 0)
            {
                Days day = new Days(DateTime.Now, 2000);
                UpsertDay(day);
                return;
            }

            foreach (Days day in days)
            {
                AllDays.Insert(0, day);
            }
        }

        // Upsert/update day.
        public void UpsertDay(Days day)
        {
            Days existingDay = dbConn.Query<Days>("SELECT * FROM Days where Id = " + day.Id).FirstOrDefault();

            if (existingDay != null)
            {
                existingDay.Amount = day.Amount;
                existingDay.Date = day.Date;
                existingDay.Goal = day.Goal;

                dbConn.RunInTransaction(() =>
                {
                    dbConn.Update(existingDay);
                });
            }
            else
            {
                dbConn.RunInTransaction(() =>
                {
                    dbConn.Insert(day);
                });

                AllDays.Insert(0, day);
            }
        }

        // Delete specific day.
        public void DeleteDay(int Id)
        {
            var existingDay = dbConn.Query<Days>("SELECT * FROM Days where Id = " + Id).FirstOrDefault();
            if (existingDay != null)
            {
                dbConn.RunInTransaction(() =>
                {
                    dbConn.Delete(existingDay);
                });
            }
        }

        // Delete Days table.
        public void DeleteAllDays()
        {
            //dbConn.RunInTransaction(() =>
            //   {
            dbConn.DropTable<Days>();
            dbConn.CreateTable<Days>();
            dbConn.Dispose();
            dbConn.Close();

            dbConn = new SQLiteConnection(App.DB_PATH);
            //});
        }
    }
}
