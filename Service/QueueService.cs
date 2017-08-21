using Model;
using System;
using System.Messaging;

namespace Service
{
    public sealed class QueueService
    {
        private readonly MessageQueue messageQueue;
        private readonly MessageQueueTransaction transaction;

        public QueueService(string queuePath)
        {
            this.messageQueue = new MessageQueue(queuePath);

            this.transaction = new MessageQueueTransaction();
        }

        public void Send(Student student)
        {
            this.transaction.Begin();

            this.messageQueue.Send(student, this.transaction);

            this.transaction.Commit();
        }

        public Student Receive(TimeSpan timeout)
        {
            this.messageQueue.Formatter = new XmlMessageFormatter(new Type[] { typeof(Student), });

            this.transaction.Begin();

            var message = this.messageQueue.Receive(timeout, this.transaction);

            this.transaction.Commit();

            return (Student)message.Body;
        }

        public Student NonTransactionalReceive(TimeSpan timeout)
        {
            this.messageQueue.Formatter = new XmlMessageFormatter(new Type[] { typeof(Student), });

            var message = this.messageQueue.Receive(timeout);

            return (Student)message.Body;
        }

        public Student InternalReceive(TimeSpan timeout)
        {
            this.messageQueue.Formatter = new XmlMessageFormatter(new Type[] { typeof(Student), });

            var message = this.messageQueue.Receive(timeout, MessageQueueTransactionType.Single);

            return (Student)message.Body;
        }
    }
}