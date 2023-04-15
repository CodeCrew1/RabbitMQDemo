using RabbitMQ.Client;
using System.Text; 

internal class Program
{
    private static void Main(string[] args)
    {
        ConnectionFactory factory = new ConnectionFactory();
        factory.Uri = new Uri("amqp://guest:guest@localhost:5672");
        factory.ClientProvidedName = "Rabbit sender app";
        IConnection cnn = factory.CreateConnection();
        IModel channel = cnn.CreateModel();

        string exchangeName = "DemoExchange";
        string routingKey = "demo-routing-key";
        string queueName = "DemoQueue";

        channel.ExchangeDeclare(exchangeName, ExchangeType.Direct);
        channel.QueueDeclare(queueName, false, false, false, null);
        channel.QueueBind(queueName, exchangeName, routingKey, null);

        bool quit = false;
        while (!quit)
        {
            Console.WriteLine("please enter a message or type q to quit - ");
            string? cmd = Console.ReadLine();
            if (cmd != "q")
            {
                Task.Delay(TimeSpan.FromSeconds(1)).Wait();
                byte[] messageBodyByte = Encoding.UTF8.GetBytes($"{cmd}");
                channel.BasicPublish(exchangeName, routingKey, null, messageBodyByte);
            }
            else
            {
                break;
            }
        }
        channel.Close();
        cnn.Close();
    }
}