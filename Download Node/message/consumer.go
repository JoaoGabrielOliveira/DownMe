package message

import (
	"context"
	"encoding/json"
	"errors"
	"log"

	"github.com/Azure/go-amqp"
	"github.com/downloadNode/model"

	rmq "github.com/rabbitmq/rabbitmq-amqp-go-client/pkg/rabbitmqamqp"
)

func GetConsumer(ctx context.Context, connection *rmq.AmqpConnection, queueName string) *rmq.Consumer {
	consumerConnected, err := connection.NewConsumer(ctx, queueName, nil)
	if err != nil {
		log.Fatalf("Error to create consumer: %s", err)
	}

	return consumerConnected
}

func Consume(ctx context.Context, consumer *rmq.Consumer, callback func(model.DownloadRequest, rmq.IDeliveryContext)) {
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

		go callback(downloadRequest, delivery)
	}
}

func MessageToDownloadRequest(message amqp.Message) {

}
