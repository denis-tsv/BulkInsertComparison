using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using BulkWriter;
using Dapper.Contrib.Extensions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Calls.NetCore
{
    class Program
    {
        static void Main(string[] args)
        {
            var numbersCount = 10_000;
            var callsCount = 100_000;
            var numbersBatchSize = 1000;
            var callsBatchSize = 1000;
            
            var numbers = new List<PhoneNumber>(numbersCount);
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var context = new CallsDbContext();
            for (int i = 1; i <= numbersCount; i++)
            {
                var number = new PhoneNumber
                {
                    Number = Guid.NewGuid().ToString()
                };
                numbers.Add(number);
                context.PhoneNumbers.Add(number);
                //if (i % numbersBatchSize == 0)
                //{
                //    context.SaveChanges(); //3.4 sec
                //    context = new CallsDbContext();
                //}
            }
            context.SaveChanges(); //3.6 sec
            stopwatch.Stop();
            Console.WriteLine($"Add 10 000 numbers {stopwatch.Elapsed.TotalSeconds} sec");

            #region EF insert

            //stopwatch.Start();
            //var random = new Random();
            //context = new CallsDbContext();
            //for (int i = 1; i <= callsCount; i++)
            //{
            //    var from = numbers[random.Next() % numbersCount];
            //    var to = numbers[random.Next() % numbersCount];
            //    var call = new Call
            //    {
            //        CallerId = from.Id,
            //        ReceiverId = to.Id,
            //        //Caller = from,
            //        //Receiver = to,
            //        Duration = random.Next()
            //    };
            //    //context.Calls.Attach(call); 
            //    context.Calls.Add(call);// 30 sec without attach number
            //    if (i % callsBatchSize == 0)
            //    {
            //        context.SaveChanges(); // 90-100 sec if keep context
            //        context = new CallsDbContext(); //43 sec if recreate context
            //    }
            //}

            //stopwatch.Stop();
            //Console.WriteLine($"Add 100 000 calls with EF {stopwatch.Elapsed.TotalSeconds} sec");

            #endregion

            #region Dapper Extensions Insert

            //stopwatch.Start();
            //var random = new Random();
            //using (SqlConnection connection = new SqlConnection(@"Server=.;Database=Calls;Trusted_Connection=True;MultipleActiveResultSets=true"))
            //{
            //    connection.Open();
            //    var transaction = connection.BeginTransaction();
            //    for (int i = 1; i <= callsCount; i++)
            //    {
            //        var from = numbers[random.Next() % numbersCount];
            //        var to = numbers[random.Next() % numbersCount];
            //        var call = new Call
            //        {
            //            CallerId = from.Id,
            //            ReceiverId = to.Id,
            //            Duration = random.Next()
            //        };
            //        connection.Insert(call, transaction);
            //        if (i % callsBatchSize == 0)
            //        {
            //            transaction.Commit();//30 sec is the same time for EF core without attach of Number
            //            transaction = connection.BeginTransaction();
            //        }
            //    }

            //    connection.Close();
            //}

            //stopwatch.Stop();
            //Console.WriteLine($"Add 100 000 calls with Dapper {stopwatch.Elapsed.TotalSeconds} sec");

            #endregion

            #region BulkWriter

            var startId = 1;
            if (context.Calls.Any())
            {
                startId = context.Calls.Max(x => x.Id) + 1;
            }
            var random = new Random();
            var calls = new List<Call>(callsCount);
            for (int i = 1; i <= callsCount; i++)
            {
                var from = numbers[random.Next() % numbersCount];
                var to = numbers[random.Next() % numbersCount];
                var call = new Call
                {
                    Id = startId++,
                    CallerId = from.Id,
                    ReceiverId = to.Id,
                    Duration = random.Next()
                };

                calls.Add(call);
            }
            stopwatch.Start();
            using (var bulkWriter = new BulkWriter<Call>(@"Server=.;Database=Calls;Trusted_Connection=True;MultipleActiveResultSets=true"))
            {
                //1 billion rows - 15 sec, 100_000 rows 6 sec
                bulkWriter.WriteToDatabase(calls);
            }
            stopwatch.Stop();
            Console.WriteLine($"Add 100 000 calls with BulkWriter {stopwatch.Elapsed.TotalSeconds} sec");
            #endregion
        }
    }
}
