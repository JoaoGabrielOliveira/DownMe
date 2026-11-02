package consumer

import (
	"context"
	"encoding/json"
	"errors"
	"log"

	"github.com/downloadNode/model"

	rmq "github.com/rabbitmq/rabbitmq-amqp-go-client/pkg/rabbitmqamqp"
)

var consumer *rmq.Consumer

func ConnectQueue(ctx context.Context, connection *rmq.AmqpConnection, queueName string) {
	consumerConnected, err := connection.NewConsumer(ctx, queueName, nil)
	if err != nil {
		log.Panicf("Error to create consumer: %s", err)
	}

	consumer = consumerConnected
}

func Consume(ctx context.Context) {
	defer func() { _ = consumer.Close(context.Background()) }()

	for {
		delivery, err := consumer.Receive(ctx)
		if err != nil {
			if errors.Is(err, context.Canceled) {
				return
			}
			log.Panicf("Failed to receive a message: %v", err)
		}
		var downloadRequest model.DownloadRequest
		var body string
		msg := delivery.Message()

		if len(msg.Data) > 0 {
			body = string(msg.Data[0])
			err = json.Unmarshal([]byte(body), &downloadRequest)
		}
		log.Printf("Received a message: %s", body)
		err = delivery.Accept(ctx)
		if err != nil {
			log.Panicf("Failed to accept message: %v", err)
		}
	}
}
