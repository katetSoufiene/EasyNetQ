using EasyNetQ;
using RabbitMQMessages;
using System;
using System.Threading;

namespace EasyNetQSendMessages
{
    class Program
    {
        static void Main(string[] args)
        {
            //Subscribe();
            //Publish();

            //Recive();
            //Send();

            Response();
            Request();
        }


        // Subscribe();  Publish();

        private static void Publish()
        {
            var rand = new Random();
            var bus = RabbitHutch.CreateBus("host=localhost");

            var randon = new Random();

            while (true)
            {
                var randomId = randon.Next(1, 1000);

                var message = new MyMessage { Object = $"Message object {randomId}", Body = $"Message body {randomId}" };

                bus.Publish(message);

                Console.WriteLine($"message: {message.Object}");

                Thread.Sleep(1000);
            }
        }

        private static void Subscribe()
        {
            var bus = RabbitHutch.CreateBus("host=localhost");
            bus.Subscribe<MyMessage>("subscribe_id", x =>
            {
                Console.WriteLine($"Recived Message : {x.Object} , {x.Body} ");
            });
        }


        //Recive(); Send();
        private static void Send()
        {
            var rand = new Random();
            var bus = RabbitHutch.CreateBus("host=localhost");

            var randon = new Random();

            while (true)
            {

                var message = new MyMessage { Object = $"Message object {randon.Next(1, 1000)}", Body = $"Message body {randon.Next(1, 1000)}" };

                //Same Gueue MyGueue
                bus.Send<MyMessage>("MyGueue", message);

                var otherMessage = new OtherMessage { OtherObject = $"Message OtherBbject {randon.Next(1, 1000)}", OtherBody = $"Message OtherBody {randon.Next(1, 1000)}" };
                //Same Gueue MyGueue
                bus.Send<OtherMessage>("MyGueue", otherMessage);

                Console.WriteLine($"Send MyMessage: {message.Object}");
                Console.WriteLine($"send OtherMessage: {otherMessage.OtherObject}");

                Thread.Sleep(1000);
            }
        }

        private static void Recive()
        {
            var bus = RabbitHutch.CreateBus("host=localhost");
            bus.Receive("MyGueue", x => x
            .Add<MyMessage>(m =>
            {
                Console.WriteLine($"Recived MyMessage : {m.Object} , {m.Body} ");
            })
            .Add<OtherMessage>(om =>
            {
                Console.WriteLine($"Recived OtherMessage : {om.OtherObject} , {om.OtherBody} ");
            }));
        }



        //Response(); Request();

        private static void Request()
        {
            var rand = new Random();
            var bus = RabbitHutch.CreateBus("host=localhost");

            var randon = new Random();

            while (true)
            {
                var randomId = randon.Next(1, 1000);

                var message = new MyMessage { Object = $"Message object {randomId}", Body = $"Message body {randomId}" };

                Console.WriteLine($"Send Message : {message.Object}");
                var response = bus.Request<MyMessage, MyResponse>(message);

                Console.WriteLine($"response: {response.Message}");

                Thread.Sleep(1000);
            }
        }

        private static void Response()
        {
            var bus = RabbitHutch.CreateBus("host=localhost");
            bus.Respond<MyMessage, MyResponse>(x =>
             {
                 return new MyResponse()
                 {
                     Message = $"Recived Message : {x.Object} , {x.Body} "
                 };
             });
        }
    }
}
