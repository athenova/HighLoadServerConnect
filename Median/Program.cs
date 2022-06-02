// See https://aka.ms/new-console-template for more information

using System.Net;
using Median;

var serverAddress = new IPEndPoint(IPAddress.Parse("88.212.241.115"), 2013);
IClient client = new Client
{
    SchedulerBuilder = RequestScheduleBuilder.CreateAdvancedRequestSchedulerBuilder(
        serverConnectionBuilder: new ServerConnectorBuilder
        {
            Address = serverAddress,
            ServerEncoding = new Koi8rWithUtf8PreambleEncoding(),
            ReadLineFeed = "\r\n",
            WriteLineFeed = "\n"
        },
        queueSize: 40,
        retryCount: 20,
        readTimeoutMs: 10_000,
        readTimeoutDelta: 10_000
    )
};

DateTime d1, d2;
string check;
float median, advancedMedian;

var task = client.GetTask().Result;
Console.WriteLine(task);

d1 = DateTime.Now;
median = client.GetMedian(Enumerable.Range(1, 2018)).Result;
d2 = DateTime.Now;
Console.WriteLine($"Time Elapsed: {(d2-d1).TotalMilliseconds} ms");
Console.WriteLine($"Median is {median}");
check = client.CheckAnswer(median).Result;
Console.WriteLine($"Answer is {check}");

var calcMedian = 4925680.5f;
check = client.CheckAnswer(calcMedian).Result;
Console.WriteLine($"Answer is {check}");

var key = client.GetKey();
Console.WriteLine($"Key is {key}");

d1 = DateTime.Now;
advancedMedian = client.GetAdvancedMedian(Enumerable.Range(1, 2018)).Result;
d2 = DateTime.Now;
Console.WriteLine($"Advanced median is {advancedMedian}");
Console.WriteLine($"Time Elapsed: {(d2-d1).TotalMilliseconds} ms");

advancedMedian = 4922031.5f;
check = client.CheckAdvancedAnswer(advancedMedian).Result;
Console.WriteLine($"Answer is {check}");