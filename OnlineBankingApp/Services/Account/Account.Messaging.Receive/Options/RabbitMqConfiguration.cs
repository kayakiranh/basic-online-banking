namespace Account.Messaging.Receive.Options
{
    //Options Pattern > Program.cs 69/70
    [Serializable]
    public record RabbitMqConfiguration
    {
        public required string Hostname { get; set; }

        public required string QueueName { get; set; }

        public required string UserName { get; set; }

        public required string Password { get; set; }
    }
}