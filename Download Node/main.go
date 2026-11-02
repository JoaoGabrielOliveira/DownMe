package main

import (
	"context"
	"log"

	rmq "github.com/rabbitmq/rabbitmq-amqp-go-client/pkg/rabbitmqamqp"

	"github.com/downloadNode/consumer"
)

const QueueName = "download_orders"

func main() {
	ctx := context.Background()
	env := rmq.NewEnvironment("amqp://go-consumer:go-consumer@localhost:5672/%2Fmain", nil)
	defer func() {
		_ = env.CloseConnections(context.Background())
	}()

	connection, err := env.NewConnection(ctx)
	if err != nil {
		log.Panicf("Error to create connection: %s", err)
	}

	consumer.ConnectQueue(ctx, connection, QueueName)

	log.Printf(" [*] Waiting for messages. To exit press CTRL+C")

	consumer.Consume(ctx)
}
