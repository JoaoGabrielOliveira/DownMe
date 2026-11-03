package message

import (
	"context"
	"log"

	rmq "github.com/rabbitmq/rabbitmq-amqp-go-client/pkg/rabbitmqamqp"
)

func CreatePublish(ctx context.Context, connection *rmq.AmqpConnection, exchangeName string, routingKey string) *rmq.Publisher {
	publisher, err := connection.NewPublisher(ctx, &rmq.ExchangeAddress{
		Exchange: exchangeName,
		Key:      routingKey,
	}, nil)

	if err != nil {
		log.Println("Error to connect with publisher")
		return nil
	}

	return publisher
}

func PublishMessage(publisher *rmq.Publisher, ctx context.Context, messageBody string) {
	message := rmq.NewMessage([]byte(messageBody))

	publishResult, err := publisher.Publish(ctx, message)
	if err != nil {
		log.Fatal("Error to send message", err)
	}
	switch publishResult.Outcome.(type) {
	case *rmq.StateAccepted:
		log.Println("Message was sent successfully!")
	case *rmq.StateReleased:
		// the broker could not route the message anywhere
		log.Fatal("The is no route for this message!")
	case *rmq.StateRejected:
		// at least one queue rejected the message
		stateType := publishResult.Outcome.(*rmq.StateRejected)
		if stateType.Error != nil {
			// in case there is error
		}
	default:
		log.Printf("Not supported status %s", publishResult.Outcome)
	}
	publisher.Close(ctx)
}
